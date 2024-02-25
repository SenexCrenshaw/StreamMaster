using StreamMaster.Domain.Extensions;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirectAPIService
{
    public static string Token { get; private set; }
    public static DateTime TokenTimestamp = DateTime.MinValue;
    public static bool GoodToken;

    /// <summary>
    /// Retrieves a session token from Schedules Direct
    /// </summary>
    /// <returns>true if successful</returns>
    public async Task<bool> GetToken()
    {

        return settings.SDSettings.SDEnabled && await GetToken(settings.SDSettings.SDUserName, settings.SDSettings.SDPassword);
    }

    public void ClearToken()
    {
        GoodToken = false;
        TokenTimestamp = DateTime.MinValue;
        //_ = _httpClient.DefaultRequestHeaders.Remove("token");
    }

    public async Task ResetToken()
    {
        ClearToken();
        await GetToken();
    }

    public void SetToken(TokenResponse tokenResponse)
    {
        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
        {
            _httpClient.DefaultRequestHeaders.Remove("token");
            _httpClient.DefaultRequestHeaders.Add("token", tokenResponse.Token);
            Token = tokenResponse.Token;
            GoodToken = true;
            TokenTimestamp = tokenResponse.Datetime;
            //_ = _timer.Change(60000, 60000); // timer event every 60 seconds
            logger.LogInformation($"Refreshed Schedules Direct API token. Token={Token[..5]}...");
        }
        else
        {
            //GoodToken = false;
            //TokenTimestamp = DateTime.MinValue;
            ClearToken();
            logger.LogError("Did not receive a response from Schedules Direct for a token request.");
        }
    }

    public bool CheckToken(bool forceReset = false)
    {
        return (!forceReset && SMDT.UtcNow - TokenTimestamp <= TimeSpan.FromHours(23)) || GetToken().Result;
    }

    private async Task<bool> GetToken(string? username = null, string? password = null, bool requestNew = false, CancellationToken cancellationToken = default)
    {
        if (!requestNew && SMDT.UtcNow - TokenTimestamp < TimeSpan.FromMinutes(1))
        {
            return true;
        }

        if (username == null || password == null)
        {
            return false;
        }

        ClearToken();

        TokenResponse? ret = await GetHttpResponse<TokenResponse>(HttpMethod.Post, "token", new TokenRequest { Username = username, PasswordHash = password }, cancellationToken: cancellationToken);

        return (ret?.Code ?? -1) == 0;

    }

}