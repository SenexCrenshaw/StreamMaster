using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
namespace StreamMaster.SchedulesDirect.Services;

public class HttpService(ILogger<HttpService> logger, IOptionsMonitor<SDSettings> sdSettings, IOptionsMonitor<Setting> settings) : IHttpService
{
    private readonly HttpClient _httpClient = CreateHttpClient(settings);

    public string? Token { get; private set; }
    public DateTime TokenTimestamp { get; private set; }
    public bool GoodToken { get; private set; }
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

    public async Task<T?> SendRequestAsync<T>(APIMethod method, string endpoint, object? payload = null, CancellationToken cancellationToken = default)
    {
        if (!await ValidateTokenAsync(cancellationToken: cancellationToken))
        {
            throw new TokenValidationException("Token validation failed. Cannot proceed with the request.");
        }

        try
        {
            using HttpRequestMessage request = new(new HttpMethod(method.ToString()), endpoint)
            {
                Content = payload != null
                    ? new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                    : null
            };

            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                await HandleHttpResponseError(response, content);
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during HTTP request to {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<HttpResponseMessage> SendRawRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        await ValidateTokenAsync(cancellationToken: cancellationToken);

        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                await HandleHttpResponseError(response, content);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error sending raw HTTP request to {Uri}", request.RequestUri);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error sending raw HTTP request to {Uri}", request.RequestUri);
            throw;
        }
    }

    private async Task<HttpResponseMessage> HandleHttpResponseError(HttpResponseMessage response, string? content)
    {
        string? tokenUsed = null;

        if (response.RequestMessage?.Headers.Contains("token") == true)
        {
            tokenUsed = response.RequestMessage.Headers.GetValues("token")?.FirstOrDefault();
        }

        if (!string.IsNullOrEmpty(content))
        {
            BaseResponse? err = JsonSerializer.Deserialize<BaseResponse>(content);
            if (err != null)
            {
                SDHttpResponseCode sdCode = (SDHttpResponseCode)err.Code;
                if (sdCode == SDHttpResponseCode.TOKEN_INVALID)
                {
                    logger.LogError("SDToken is invalid {Token} {Length}", tokenUsed, tokenUsed?.Length ?? 0);
                }
                switch (sdCode)
                {
                    case SDHttpResponseCode.SERVICE_OFFLINE:
                        response.StatusCode = HttpStatusCode.ServiceUnavailable;
                        response.ReasonPhrase = "Service Unavailable";
                        break;

                    case SDHttpResponseCode.ACCOUNT_DISABLED:
                    case SDHttpResponseCode.ACCOUNT_EXPIRED:
                    case SDHttpResponseCode.APPLICATION_DISABLED:
                        response.StatusCode = HttpStatusCode.Forbidden;
                        response.ReasonPhrase = "Forbidden";
                        break;

                    case SDHttpResponseCode.ACCOUNT_LOCKOUT:
                        response.StatusCode = HttpStatusCode.Locked;
                        response.ReasonPhrase = "Locked";
                        break;

                    case SDHttpResponseCode.IMAGE_NOT_FOUND:
                    case SDHttpResponseCode.IMAGE_QUEUED:
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.ReasonPhrase = "Not Found";
                        break;

                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS:
                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS_TRIAL:
                        response.StatusCode = HttpStatusCode.TooManyRequests;
                        response.ReasonPhrase = "Too Many Requests";
                        break;

                    case SDHttpResponseCode.TOKEN_MISSING:
                    case SDHttpResponseCode.INVALID_USER:
                    case SDHttpResponseCode.TOKEN_INVALID:
                    case SDHttpResponseCode.TOKEN_EXPIRED:
                    case SDHttpResponseCode.TOKEN_DUPLICATED:
                    case SDHttpResponseCode.UNKNOWN_USER:
                        response.StatusCode = HttpStatusCode.Unauthorized;
                        response.ReasonPhrase = "Unauthorized";
                        await RefreshTokenAsync(CancellationToken.None);
                        break;
                }
            }
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.StatusCode = (HttpStatusCode)418;
            response.ReasonPhrase = "I'm a teapot";
        }

        if (response.StatusCode != HttpStatusCode.NotModified)
        {
            string tokenUsedShort = tokenUsed?.Length >= 5 ? tokenUsed[..5] : tokenUsed ?? string.Empty;
            logger.LogError(
                "{RequestPath}: {StatusCode} {ReasonPhrase} : Token={TokenUsed}...{Content}",
                response.RequestMessage?.RequestUri?.AbsolutePath.Replace("https://json.schedulesdirect.org/20141201/", "/"),
                (int)response.StatusCode,
                response.ReasonPhrase,
                tokenUsedShort,
                !string.IsNullOrEmpty(content) ? $"\n{content}" : ""
            );
        }

        return response;
    }

    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        await _tokenSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (SMDT.UtcNow - TokenTimestamp < TimeSpan.FromMinutes(1))
            {
                return true; // Token is still fresh
            }

            string username = sdSettings.CurrentValue.SDUserName;
            string password = sdSettings.CurrentValue.SDPassword;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                logger.LogWarning("Username or password is missing.");
                return false;
            }
            ClearToken();


            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "token",
                new { username, password },
                cancellationToken
            ).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to fetch token. Status code: {StatusCode}", response.StatusCode);
                throw new TokenRefreshException($"Failed to refresh token. Status code: {response.StatusCode}");
                //return false;
            }

            TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            if (tokenResponse?.Code == 0 && !string.IsNullOrEmpty(tokenResponse.Token))
            {
                Token = tokenResponse.Token;
                TokenTimestamp = tokenResponse.Datetime;
                GoodToken = true;

                _httpClient.DefaultRequestHeaders.Remove("token");
                _httpClient.DefaultRequestHeaders.Add("token", Token);

                logger.LogInformation("Token refreshed successfully. Token={Token[..5]}...", Token[..5]);
                return true;
            }

            logger.LogError("Failed to refresh token. Error code: {Code}", tokenResponse?.Code);
            return false;
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }



    public async Task<bool> ValidateTokenAsync(bool forceReset = false, CancellationToken cancellationToken = default)
    {
        if (forceReset || SMDT.UtcNow - TokenTimestamp > TimeSpan.FromHours(23))
        {
            bool refreshed = await RefreshTokenAsync(cancellationToken);
            if (!refreshed)
            {
                logger.LogError("Token validation failed. Unable to refresh token.");
                return false;
            }
        }

        return GoodToken;
    }


    public void ClearToken()
    {
        Token = null;
        GoodToken = false;
        TokenTimestamp = DateTime.MinValue;
        //_httpClient.DefaultRequestHeaders.Remove("token");
        logger.LogWarning("Token cleared.");
    }

    private static HttpClient CreateHttpClient(IOptionsMonitor<Setting> settings)
    {
        HttpClient httpClient = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        })
        {
            BaseAddress = new Uri("https://json.schedulesdirect.org/20141201/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.CurrentValue.ClientUserAgent);
        httpClient.DefaultRequestHeaders.ExpectContinue = true;

        return httpClient;
    }
}

