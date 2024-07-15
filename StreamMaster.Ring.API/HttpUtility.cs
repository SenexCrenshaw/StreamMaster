using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.Ring.API
{
    /// <summary>
    /// Internal utility class for Http communication with the Ring API
    /// </summary>
    internal class HttpUtility
    {
        #region Fields

        /// <summary>
        /// Keep one reusable instance of a HttpClient to avoid port exhaustion
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Cookiecontainer to keep the cookies needed for the requests
        /// </summary>
        private readonly CookieContainer _cookieContainer;

        /// <summary>
        /// HttpClientHandler to use for the HttpClient requests
        /// </summary>
        private readonly HttpClientHandler _httpClientHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new HttpUtility helper. Cookies will be shared among all requests done in this instance.
        /// </summary>
        /// <param name="timeout">Default timeout in milliseconds to apply to the HTTP requests</param>
        public HttpUtility(int timeout = 60000)
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler { CookieContainer = _cookieContainer };

            _httpClient = new(_httpClientHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(timeout)
            };
        }

        #endregion

        #region Descructors

        /// <summary>
        /// Clean up resources
        /// </summary>
        ~HttpUtility()
        {
            _httpClientHandler?.Dispose();
            _httpClient?.Dispose();
        }

        #endregion

        /// <summary>
        /// Performs a GET request to the provided url to return the contents
        /// </summary>
        /// <param name="url">Url of the request to make</param>
        /// <param name="bearerToken">Bearer token to authenticate the request with. Leave out to not authenticate the session.</param>
        /// <returns>Contents of the result returned by the webserver</returns>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        public async Task<string> GetContents(Uri url, string? bearerToken = null)
        {
            // Construct the request
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = url
            };

            // Check if the OAuth Bearer Authorization token should be added to the request
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Add(nameof(HttpRequestHeader.Authorization), $"Bearer {bearerToken}");
            }

            // Send the request to the webserver
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.TooManyRequests:
                    throw new Exceptions.ThrottledException();

                case HttpStatusCode.NotFound:
                    throw new Exceptions.DeviceUnknownException();
            }

            // Return the response from the server
            string responseFromServer = await response.Content.ReadAsStringAsync();
            return responseFromServer;
        }

        /// <summary>
        /// Sends a POST request using the url encoded form method
        /// </summary>
        /// <param name="url">Url to POST to</param>
        /// <param name="formFields">Dictonary with key/value pairs containing the forms data to POST to the webserver</param>
        /// <param name="headerFields">NameValueCollection with the fields to add to the header sent to the server with the request</param>
        /// <returns>The website contents returned by the webserver after posting the data</returns>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<string?> FormPost(Uri url, Dictionary<string, string> formFields, NameValueCollection headerFields)
        {
            // Construct the POST request which performs the login
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = url
            };

            if (headerFields != null)
            {
                foreach (string headerField in headerFields)
                {
                    request.Headers.Add(headerField, headerFields[headerField]);
                }
            }

            // Set the content for the HTTP request
            request.Content = new FormUrlEncodedContent(formFields);

            // Receive the response from the webserver
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            // Make sure the webserver has sent a response
            if (response == null)
            {
                return null;
            }

            // Get the response body
            string responseText = await response.Content.ReadAsStringAsync();

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    // Check if the response is HTTP 429 Too Many Requests throttling
                    if (responseText.Contains("Too many requests", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exceptions.ThrottledException();
                    }

                    // Check if the two factor authentication token was incorrect or has expired. HTTP 400 Bad Request.
                    if (responseText.Contains("Verification Code is invalid or expired", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exceptions.TwoFactorAuthenticationIncorrectException();
                    }
                    break;

                case HttpStatusCode.PreconditionFailed:
                    // Multi factor authentication failed
                    throw new Exceptions.TwoFactorAuthenticationRequiredException();

                case HttpStatusCode.Unauthorized:
                    throw new Exceptions.AuthenticationFailedException();
            }

            // Make sure the response content is available
            return responseText == null ? null : responseText;
        }

        /// <summary>
        /// Downloads the file from the provided Url
        /// </summary>
        /// <param name="url">Url to download the file from</param>
        /// <param name="bearerToken">Bearer token to authenticate the request with. Leave out to not authenticate the session.</param>
        /// <returns>Stream with the file download</returns>
        public async Task<Stream> DownloadFile(Uri url, string? bearerToken = null)
        {
            // Construct the request
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = url,
                Headers =
                {
                    { HttpRequestHeader.Accept.ToString(), "*/*" },
                    //{ HttpRequestHeader.Range.ToString(), "bytes 0" }
                }
            };

            request.Headers.Range = new RangeHeaderValue(0, null);

            // Check if the OAuth Bearer Authorization token should be added to the request
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {bearerToken}");
            }

            // Receive the response from the webserver
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Performs a HTTP request expecting a certain status code to be returned by the server
        /// </summary>
        /// <param name="url">Url of the request to make</param>
        /// <param name="httpMethod">The HTTP method to use to call the provided Url</param>
        /// <param name="expectedStatusCode">The expected HTTP status code to be replied by the Ring API. An exception will be thrown if the expectation was wrong. Leave NULL to not set an expectation.</param>
        /// <param name="bodyContent">Content to send along with the request in the body. Leave NULL to not send along any content.</param>
        /// <param name="bearerToken">Bearer token to authenticate the request with. Leave out to not authenticate the session.</param>
        /// <exception cref="Exceptions.UnexpectedOutcomeException">Thrown if the actual HTTP response is different from what was expected</exception>
        public async Task SendRequestWithExpectedStatusOutcome(Uri url, HttpMethod httpMethod, HttpStatusCode? expectedStatusCode, string? bodyContent = null, string? bearerToken = null)
        {
            using HttpRequestMessage request = new(httpMethod, url);

            // Check if the OAuth Bearer Authorization token should be added to the request
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {bearerToken}");
            }

            if (bodyContent != null)
            {
                request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            }

            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            // Validate the resulting HTTP status against the expected status
            if (expectedStatusCode.HasValue && response.StatusCode != expectedStatusCode.Value)
            {
                throw new Exceptions.UnexpectedOutcomeException(response.StatusCode, expectedStatusCode.Value);
            }
        }

        /// <summary>
        /// Sends a HttpRequest to the Ring API server
        /// </summary>
        /// <typeparam name="T">Type of entity to try to parse the result from the Ring API in</typeparam>
        /// <param name="url">Url of the request to make</param>
        /// <param name="httpMethod">The HTTP method to use to call the provided Url</param>
        /// <param name="bodyContent">Content to send along with the request in the body. Leave NULL to not send along any content.</param>
        /// <param name="bearerToken">Bearer token to authenticate the request with. Leave out to not authenticate the session.</param>
        /// <returns>Contents of the result returned by the Ring API parsed in the type T provided</returns>
        public async Task<T> SendRequest<T>(Uri url, HttpMethod httpMethod, string bodyContent, string? bearerToken = null)
        {
            // Make the request and get the body contents of the response
            string response = await SendRequest(url, httpMethod, bodyContent, bearerToken);

            // Try parsing the response to the type provided with this method
            T? responseEntity = JsonSerializer.Deserialize<T>(response);
            return responseEntity;
        }

        /// <summary>
        /// Sends a HttpRequest to the Ring API server
        /// </summary>
        /// <param name="url">Url of the request to make</param>
        /// <param name="httpMethod">The HTTP method to use to call the provided Url</param>
        /// <param name="bodyContent">Content to send along with the request in the body. Leave NULL to not send along any content.</param>
        /// <param name="bearerToken">Bearer token to authenticate the request with. Leave out to not authenticate the session.</param>
        /// <returns>Contents of the result returned by the Ring API</returns>
        public async Task<string> SendRequest(Uri url, HttpMethod httpMethod, string bodyContent, string? bearerToken = null)
        {
            using HttpRequestMessage request = new(httpMethod, url);

            // Check if the OAuth Bearer Authorization token should be added to the request
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {bearerToken}");
            }

            if (bodyContent != null)
            {
                request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            }

            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            // Get the response body and return it
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
