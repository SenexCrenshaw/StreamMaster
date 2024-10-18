using StreamMaster.Ring.API.Entities;

using System.Collections.Specialized;
using System.Text.Json;

namespace StreamMaster.Ring.API
{
    public class Session
    {
        #region Properties

        /// <summary>
        /// Username to use to connect to the Ring API. Set by providing it in the constructor.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password to use to connect to the Ring API. Set by providing it in the constructor.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Uri on which OAuth tokens can be requested from Ring
        /// </summary>
        public Uri RingApiOAuthUrl => new("https://oauth.ring.com/oauth/token");

        /// <summary>
        /// Base Uri with which all Ring API requests start
        /// </summary>
        public Uri RingApiBaseUrl => new("https://api.ring.com/clients_api/");

        /// <summary>
        /// Boolean indicating if the current session is authenticated
        /// </summary>
        public bool IsAuthenticated => OAuthToken != null;

        /// <summary>
        /// Authentication Token that will be used to communicate with the Ring API
        /// </summary>
        public string AuthenticationToken => OAuthToken?.AccessToken;

        /// <summary>
        /// OAuth Token for communicating with the Ring API
        /// </summary>
        public OAutToken OAuthToken { get; private set; }

        #endregion Properties

        #region Fields

        /// <summary>
        /// HttpUtility instance to make HTTP requests
        /// </summary>
        private static readonly HttpUtility _httpUtility = new();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initiates a new session to the Ring API
        /// </summary>
        public Session(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Initiates a new session without username/password. Only to be used with the static method to create a session based on a RefreshToken.
        /// </summary>
        private Session()
        {
        }

        #endregion Constructors

        #region Static methods

        /// <summary>
        /// Creates a new session to the Ring API using a RefreshToken received from a previous session
        /// </summary>
        /// <param name="refreshToken">RefreshToken received from the prior authentication</param>
        /// <returns>Authenticated session based on the RefreshToken or NULL if the session could not be authenticated</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public static async Task<Session> GetSessionByRefreshToken(string refreshToken)
        {
            Session session = new();
            await session.RefreshSession(refreshToken);
            return session;
        }

        #endregion Static methods

        #region Methods

        /// <summary>
        /// Authenticates to the Ring API
        /// </summary>
        /// <param name="operatingSystem">Operating system from which this API is accessed. Defaults to 'windows'. Required field.</param>
        /// <param name="hardwareId">Hardware identifier of the device for which this API is accessed. Defaults to 'unspecified'. Required field.</param>
        /// <param name="appBrand">Device brand for which this API is accessed. Defaults to 'ring'. Optional field.</param>
        /// <param name="deviceModel">Device model for which this API is accessed. Defaults to 'unspecified'. Optional field.</param>
        /// <param name="deviceName">Name of the device from which this API is being used. Defaults to 'unspecified'. Optional field.</param>
        /// <param name="resolution">Screen resolution on which this API is being used. Defaults to '800x600'. Optional field.</param>
        /// <param name="appVersion">Version of the app from which this API is being used. Defaults to '1.3.810'. Optional field.</param>
        /// <param name="appInstallationDate">Date and time at which the app was installed from which this API is being used. By default not specified. Optional field.</param>
        /// <param name="manufacturer">Name of the manufacturer of the product for which this API is being accessed. Defaults to 'unspecified'. Optional field.</param>
        /// <param name="deviceType">Type of device from which this API is being used. Defaults to 'tablet'. Optional field.</param>
        /// <param name="architecture">Architecture of the system from which this API is being used. Defaults to 'x64'. Optional field.</param>
        /// <param name="language">Language of the app from which this API is being used. Defaults to 'en'. Optional field.</param>
        /// <param name="twoFactorAuthCode">The two factor authentication code retrieved through a text message to authenticate to two factor authentication enabled accounts. Leave this NULL at first to retrieve the text message. Then use this method again specifying the proper number received in the text message to finalize authentication.</param>
        /// <returns>Session object if the authentication was successful</returns>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Entities.Session> Authenticate(string operatingSystem = "windows",
                                                            string hardwareId = "unspecified",
                                                            string appBrand = "ring",
                                                            string deviceModel = "unspecified",
                                                            string deviceName = "unspecified",
                                                            string resolution = "800x600",
                                                            string appVersion = "2.1.8",
                                                            DateTime? appInstallationDate = null,
                                                            string manufacturer = "unspecified",
                                                            string deviceType = "tablet",
                                                            string architecture = "x64",
                                                            string language = "en",
                                                            string twoFactorAuthCode = null)
        {
            // Check for mandatory parameters
            if (string.IsNullOrEmpty(operatingSystem))
            {
                throw new ArgumentNullException("operatingSystem", "Operating system is mandatory");
            }

            if (string.IsNullOrEmpty(hardwareId))
            {
                throw new ArgumentNullException("hardwareId", "HardwareId system is mandatory");
            }

            // Construct the Form POST fields to send along with the authentication request
            Dictionary<string, string> oAuthformFields = new()
            {
                { "grant_type", "password" },
                { "username", Username },
                { "password", Password},
                { "client_id", "ring_official_android" },
                { "scope", "client" }
            };

            // If a two factor auth code has been provided, add the code through the HTTP POST header
            NameValueCollection headerFields = new(capacity: 3)
            {
                { "hardware_id", hardwareId }
            };
            if (twoFactorAuthCode != null)
            {
                headerFields.Add("2fa-support", "true");
                headerFields.Add("2fa-code", twoFactorAuthCode);
            }

            // Make the Form POST request to request an OAuth Token
            string oAuthResponse = await _httpUtility.FormPost(RingApiOAuthUrl,
                                                            oAuthformFields,
                                                            headerFields);

            // Deserialize the JSON result into a typed object
            OAuthToken = JsonSerializer.Deserialize<OAutToken>(oAuthResponse);

            // Construct the Form POST fields to send along with the session request
            Dictionary<string, string> sessionFormFields = new()
            {
                { "device[os]", System.Net.WebUtility.UrlEncode(operatingSystem) },
                { "device[hardware_id]", System.Net.WebUtility.UrlEncode(hardwareId) }
            };

            // Add optional fields if they have been provided
            if (!string.IsNullOrEmpty(appBrand))
            {
                sessionFormFields.Add("device[app_brand]", System.Net.WebUtility.UrlEncode(appBrand));
            }

            if (!string.IsNullOrEmpty(deviceModel))
            {
                sessionFormFields.Add("device[metadata][device_model]", System.Net.WebUtility.UrlEncode(deviceModel));
            }

            if (!string.IsNullOrEmpty(deviceName))
            {
                sessionFormFields.Add("device[metadata][device_name]", System.Net.WebUtility.UrlEncode(deviceName));
            }

            if (!string.IsNullOrEmpty(resolution))
            {
                sessionFormFields.Add("device[metadata][resolution]", System.Net.WebUtility.UrlEncode(resolution));
            }

            if (!string.IsNullOrEmpty(appVersion))
            {
                sessionFormFields.Add("device[metadata][app_version]", System.Net.WebUtility.UrlEncode(appVersion));
            }

            if (appInstallationDate.HasValue)
            {
                sessionFormFields.Add("device[metadata][app_instalation_date]", string.Format("{0:yyyy-MM-dd}+{0:HH}%3A{0:mm}%3A{0:ss}Z", appInstallationDate.Value));
            }

            if (!string.IsNullOrEmpty(manufacturer))
            {
                sessionFormFields.Add("device[metadata][manufacturer]", System.Net.WebUtility.UrlEncode(manufacturer));
            }

            if (!string.IsNullOrEmpty(deviceType))
            {
                sessionFormFields.Add("device[metadata][device_type]", System.Net.WebUtility.UrlEncode(deviceType));
            }

            if (!string.IsNullOrEmpty(architecture))
            {
                sessionFormFields.Add("device[metadata][architecture]", System.Net.WebUtility.UrlEncode(architecture));
            }

            if (!string.IsNullOrEmpty(language))
            {
                sessionFormFields.Add("device[metadata][language]", System.Net.WebUtility.UrlEncode(language));
            }

            // Make the Form POST request to authenticate
            string sessionResponse = await _httpUtility.FormPost(new Uri(RingApiBaseUrl, "session"),
                                                                sessionFormFields,
                                                                new NameValueCollection
                                                                {
                                                                    { "Accept-Encoding", "gzip, deflate" },
                                                                    { "X-API-LANG", "en" },
                                                                    { "Authorization", $"Bearer {OAuthToken.AccessToken}" }
                                                                });

            // Deserialize the JSON result into a typed object
            Entities.Session? session = JsonSerializer.Deserialize<Entities.Session>(sessionResponse);

            return session;
        }

        /// <summary>
        /// Authenticates to the Ring API using the refresh token in the current session
        /// </summary>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task RefreshSession()
        {
            await RefreshSession(OAuthToken.RefreshToken);
        }

        /// <summary>
        /// Authenticates to the Ring API using the provided refresh token
        /// </summary>
        /// <param name="refreshToken">RefreshToken to set up a new authenticated session</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task RefreshSession(string refreshToken)
        {
            // Check for mandatory parameters
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken), "refreshToken is mandatory");
            }

            // Construct the Form POST fields to send along with the authentication request
            Dictionary<string, string> oAuthformFields = new()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            NameValueCollection headerFields = new()
            {
                { "hardware_id", "unspecified" }
            };
            // Make the Form POST request to request an OAuth Token
            try
            {
                string oAuthResponse = await _httpUtility.FormPost(RingApiOAuthUrl,
                                                                oAuthformFields,
                                                                headerFields);

                // Deserialize the JSON result into a typed object
                OAuthToken = JsonSerializer.Deserialize<OAutToken>(oAuthResponse);
            }
            catch (System.Net.WebException e)
            {
                // If a WebException gets thrown with Unauthorized it means that the refresh token was not valid, throw a custom exception to indicate this
                if (e.Message.Contains("Unauthorized"))
                {
                    throw new Exceptions.AuthenticationFailedException(e);
                }

                throw;
            }
        }

        /// <summary>
        /// Ensure that the current session is authenticated and check if the access token is still valid. If not, it will try to renew the session using the refresh token.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the refresh token is null or empty.</exception>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        ///
        public async Task EnsureSessionValid()
        {
            // Ensure the session is authenticated
            if (!IsAuthenticated)
            {
                // Session is not authenticated
                throw new Exceptions.SessionNotAuthenticatedException();
            }

            // Ensure the access token in the session is still valid
            if (OAuthToken.ExpiresAt < DateTime.Now)
            {
                // Access token is no longer valid, check if we have a refresh token available to refresh the session
                if (string.IsNullOrEmpty(OAuthToken.RefreshToken))
                {
                    // No refresh token available so can't renew the session
                    throw new Exceptions.SessionNotAuthenticatedException();
                }

                // Refresh token available, try refreshing the session
                await RefreshSession();
            }

            // All good
        }

        /// <summary>
        /// Returns all devices registered with Ring under the current account being used
        /// </summary>
        /// <returns>Devices registered with Ring under the current account</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Devices> GetRingDevices()
        {
            await EnsureSessionValid();

            string response = await _httpUtility.GetContents(new Uri(RingApiBaseUrl, $"ring_devices"), AuthenticationToken);

            Devices? devices = JsonSerializer.Deserialize<Devices>(response);
            return devices;
        }

        /// <summary>
        /// Returns all events registered for the doorbots
        /// </summary>
        /// <param name="doorbotId">Id of the doorbot to retrieve the history for. Provide NULL to retrieve the history for all available doorbots.</param>
        /// <param name="limit">Amount of history items to retrieve. If you don't provide this value, Ring will default to returning only the most recent 20 items.</param>
        /// <returns>All events triggered by registered doorbots under the current account</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="Exceptions.DeviceUnknownException">Thrown when the web server indicates the requested Ring device was not found (HTTP 404).</exception>
        public async Task<List<DoorbotHistoryEvent>> GetDoorbotsHistory(int? doorbotId, int? limit = null)
        {
            await EnsureSessionValid();

            // Receive the first batch
            string response = await _httpUtility.GetContents(new Uri(RingApiBaseUrl, $"doorbots/{(doorbotId.HasValue ? $"{doorbotId.Value}/" : "")}history{(limit.HasValue ? $"?limit={limit}" : "")}"), AuthenticationToken);

            // Parse the result
            List<DoorbotHistoryEvent>? doorbotHistory = JsonSerializer.Deserialize<List<DoorbotHistoryEvent>>(response);

            // If no limit has been specified or the amount of items requested have been returned already, just return whatever has been returned by the API
            if (!limit.HasValue || doorbotHistory.Count >= limit.Value)
            {
                return doorbotHistory;
            }

            // Calculate how many items we still need to retrieve after this first batch
            int remainingItems = limit.Value - doorbotHistory.Count;

            // Create a list to hold all the results
            List<DoorbotHistoryEvent> allHistory = new();

            // Add the first batch to the list with all the results
            allHistory.AddRange(doorbotHistory);

            do
            {
                // Retrieve the next batch
                response = await _httpUtility.GetContents(new Uri(RingApiBaseUrl, $"doorbots/{(doorbotId.HasValue ? $"{doorbotId.Value}/" : "")}history?limit={remainingItems}&older_than={allHistory.Last().Id}"), AuthenticationToken);

                // Parse the result
                doorbotHistory = JsonSerializer.Deserialize<List<DoorbotHistoryEvent>>(response);

                // Add this next batch to the list with all the results
                allHistory.AddRange(doorbotHistory);

                // Calculate how many items we still need to retrieve after this next batch
                remainingItems = limit.Value - allHistory.Count;
            }
            // Keep retrieving next batches until nothing is being returned anymore or we have retrieved the amount of items that were requested through the limit
            while (doorbotHistory.Count > 0 && remainingItems > 0);

            return allHistory;
        }

        /// <summary>
        /// Returns all events registered for all the available doorbots
        /// </summary>
        /// <param name="limit">Amount of history items to retrieve. If you don't provide this value, Ring will default to returning only the most recent 20 items.</param>
        /// <returns>All events triggered by registered doorbots under the current account</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<List<DoorbotHistoryEvent>> GetDoorbotsHistory(int? limit = null)
        {
            return await GetDoorbotsHistory(null, limit);
        }

        /// <summary>
        /// Returns all events registered for the doorbots that happened between the provided dates. Notice: Ring does not provide an API which allows for retrieving items between two specific dates. This means that this code will just keep retriving historical items until it has all items that occurred in the provided date span. This is not super efficient, but unfortunately the only way.
        /// </summary>
        /// <param name="startDate">Date and time in the past from where to start collecting history</param>
        /// <param name="endDate">Date and time in the past until where to start collecting history. Provide NULL to get everything up till now.</param>
        /// <param name="doorbotId">Id of the doorbot to retrieve the history for. Provide NULL to retrieve the history for all available doorbots.</param>
        /// <returns>All events triggered by registered doorbots under the current account between the provided dates</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="Exceptions.DeviceUnknownException">Thrown when the web server indicates the requested Ring device was not found (HTTP 404).</exception>
        public async Task<List<DoorbotHistoryEvent>> GetDoorbotsHistory(DateTime startDate, DateTime? endDate, int? doorbotId = null)
        {
            await EnsureSessionValid();

            // Amount of items to retrieve in each request
            const short batchWithItems = 200;

            // Create a list to hold all the results
            List<DoorbotHistoryEvent> allHistory = new();
            List<DoorbotHistoryEvent>? doorbotHistory = new();
            DateTime? lastItemDateTime = null;

            do
            {
                // Retrieve a batch with historical items
                string response = await _httpUtility.GetContents(new Uri(RingApiBaseUrl, $"doorbots/{(doorbotId.HasValue ? $"{doorbotId.Value}/" : "")}history?limit={batchWithItems}{(doorbotHistory.Count == 0 ? "" : "&older_than=" + doorbotHistory.Last().Id)}"), AuthenticationToken);

                // Parse the result
                doorbotHistory = JsonSerializer.Deserialize<List<DoorbotHistoryEvent>>(response);

                // Add this next batch to the list with all the results which fit within the provided date span
                allHistory.AddRange(doorbotHistory.Where(h => h.CreatedAtDateTime.HasValue && h.CreatedAtDateTime.Value >= startDate && (!endDate.HasValue || h.CreatedAtDateTime.Value <= endDate.Value)));

                if (doorbotHistory.Count > 0)
                {
                    lastItemDateTime = doorbotHistory[^1]?.CreatedAtDateTime ?? DateTime.MinValue;
                }
            }
            // Keep retrieving next batches until the last item in the retrieved batch does not fit within the request date span anymore
            while (doorbotHistory.Count > 0 && lastItemDateTime.HasValue && lastItemDateTime.Value > startDate);

            return allHistory;
        }

        /// <summary>
        /// Returns a stream with the recording of the provided Ding Id of a doorbot
        /// </summary>
        /// <param name="doorbotHistoryEvent">The doorbot history event to retrieve the recording for</param>
        /// <returns>Stream containing contents of the recording</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="ArgumentNullException">Thrown when no historyEvent provided or one provided without an Id</exception>
        public async Task<Stream> GetDoorbotHistoryRecording(DoorbotHistoryEvent doorbotHistoryEvent)
        {
            return doorbotHistoryEvent == null || !doorbotHistoryEvent.Id.HasValue
                ? throw new ArgumentNullException(nameof(doorbotHistoryEvent))
                : await GetDoorbotHistoryRecording(doorbotHistoryEvent.Id.Value.ToString());
        }

        /// <summary>
        /// Returns a stream with the recording of the provided Ding Id of a doorbot
        /// </summary>
        /// <param name="dingId">Id of the doorbot history event to retrieve the recording for</param>
        /// <returns>Stream containing contents of the recording</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Stream> GetDoorbotHistoryRecording(string dingId)
        {
            await EnsureSessionValid();

            // Construct the URL where to request downloading of a recording
            Uri downloadRequestUri = new(RingApiBaseUrl, $"dings/{dingId}/share/download?disable_redirect=true");

            DownloadRecording downloadResult = null;
            for (int downloadAttempt = 1; downloadAttempt < 60; downloadAttempt++)
            {
                // Request to download the recording
                string response = await _httpUtility.GetContents(downloadRequestUri, AuthenticationToken);

                // Parse the result
                downloadResult = JsonSerializer.Deserialize<DownloadRecording>(response);

                // If the Ring API returns an empty URL property, it means its still preparing the download on the server side. Just keep requesting the recording until it returns an URL.
                if (!string.IsNullOrWhiteSpace(downloadResult.Url))
                {
                    // URL returned is not empty, start the download from the returned URL
                    break;
                }

                // Wait one second before requesting the recording again
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            // Ensure we ended with a valid URL to download the recording from
            if (downloadResult == null || string.IsNullOrWhiteSpace(downloadResult.Url) || !Uri.TryCreate(downloadResult.Url, UriKind.Absolute, out Uri downloadUri))
            {
                throw new Exceptions.DownloadFailedException(downloadResult?.Url ?? "(no URL was created)");
            }

            // Request the file download from the returned URI
            Stream stream = await _httpUtility.DownloadFile(downloadUri);
            return stream;
        }

        /// <summary>
        /// Saves the recording of the provided Ding Id of a doorbot to the provided location
        /// </summary>
        /// <param name="doorbotHistoryEvent">The doorbot history event to retrieve the recording for</param>
        /// <param name="saveAs">Full path including the filename where to save the recording</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="ArgumentNullException">Thrown when no historyEvent provided or one provided without an Id</exception>
        public async Task GetDoorbotHistoryRecording(DoorbotHistoryEvent doorbotHistoryEvent, string saveAs)
        {
            if (doorbotHistoryEvent == null || !doorbotHistoryEvent.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(doorbotHistoryEvent));
            }

            await GetDoorbotHistoryRecording(doorbotHistoryEvent.Id.Value.ToString(), saveAs);
        }

        /// <summary>
        /// Saves the recording of the provided Ding Id of a doorbot to the provided location
        /// </summary>
        /// <param name="dingId">Id of the doorbot history event to retrieve the recording for</param>
        /// <param name="saveAs">Full path including the filename where to save the recording</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task GetDoorbotHistoryRecording(string dingId, string saveAs)
        {
            await EnsureSessionValid();

            using Stream stream = await GetDoorbotHistoryRecording(dingId);
            using FileStream fileStream = File.Create(saveAs);

            await stream.CopyToAsync(fileStream);
        }

        /// <summary>
        /// Shares the Ring recording with the provided identifier and returns the shared URL to it
        /// </summary>
        /// <param name="historyEvent">The doorbot history event to share the recording of</param>
        /// <returns>Uri to the shared recording</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.SharingFailedException">Thrown when a share URL could not be created.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="ArgumentNullException">Thrown when no historyEvent provided or one provided without an Id</exception>
        public async Task<Uri> ShareRecording(DoorbotHistoryEvent historyEvent)
        {
            return historyEvent == null || !historyEvent.Id.HasValue
                ? throw new ArgumentNullException(nameof(historyEvent))
                : await ShareRecording(historyEvent.Id.Value.ToString());
        }

        /// <summary>
        /// Shares the Ring recording with the provided identifier and returns the shared URL to it
        /// </summary>
        /// <param name="recordingId">Id of the recording</param>
        /// <returns>Uri to the shared recording</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.SharingFailedException">Thrown when a share URL could not be created.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Uri> ShareRecording(string recordingId)
        {
            await EnsureSessionValid();

            // Construct the URL where to request sharing of a recording
            Uri downloadRequestUri = new(RingApiBaseUrl, $"dings/{recordingId}/share/share?disable_redirect=true");

            SharedRecording shareResult = null;
            for (int downloadAttempt = 1; downloadAttempt < 60; downloadAttempt++)
            {
                // Request to share the recording
                string response = await _httpUtility.GetContents(downloadRequestUri, AuthenticationToken);

                // Parse the result
                shareResult = JsonSerializer.Deserialize<SharedRecording>(response);

                // If the Ring API returns an empty URL property, it means its still preparing the sharing on the server side. Just keep requesting the recording until it returns an URL.
                if (!string.IsNullOrWhiteSpace(shareResult.WrapperUrl))
                {
                    // URL returned is not empty, return the URL
                    break;
                }

                // Wait one second before requesting the sharing again
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            // Ensure we ended with a valid URL to the shared recording
            return shareResult == null || string.IsNullOrWhiteSpace(shareResult.WrapperUrl) || !Uri.TryCreate(shareResult.WrapperUrl, UriKind.Absolute, out Uri shareUri)
                ? throw new Exceptions.SharingFailedException(recordingId)
                : shareUri;
        }

        /// <summary>
        /// Saves the latest available snapshot from the provided doorbot to the provided location
        /// </summary>
        /// <param name="doorbot">The doorbot to retrieve the latest available snapshot from</param>
        /// <param name="saveAs">Full path including the filename where to save the snapshot</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task GetLatestSnapshot(Doorbot doorbot, string saveAs)
        {
            await GetLatestSnapshot(doorbot.Id, saveAs);
        }

        /// <summary>
        /// Saves the latest available snapshot from the provided doorbot to the provided location
        /// </summary>
        /// <param name="doorbotId">ID of the doorbot to retrieve the latest available snapshot from</param>
        /// <param name="saveAs">Full path including the filename where to save the snapshot</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task GetLatestSnapshot(int doorbotId, string saveAs)
        {
            using Stream stream = await GetLatestSnapshot(doorbotId);
            using FileStream fileStream = File.Create(saveAs);

            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
        }

        /// <summary>
        /// Returns the latest available snapshot from the provided doorbot
        /// </summary>
        /// <param name="doorbot">The doorbot to retrieve the latest available snapshot from</param>
        /// <returns>Stream with the latest snapshot from the doorbot</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Stream> GetLatestSnapshot(Doorbot doorbot)
        {
            return await GetLatestSnapshot(doorbot.Id);
        }

        /// <summary>
        /// Returns the latest available snapshot from the provided doorbot
        /// </summary>
        /// <param name="doorbotId">ID of the doorbot to retrieve the latest available snapshot from</param>
        /// <returns>Stream with the latest snapshot from the doorbot</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<Stream> GetLatestSnapshot(int doorbotId)
        {
            await EnsureSessionValid();

            // Construct the URL where to download the latest doorbot snapshot from
            Uri downloadSnapshotUri = new(RingApiBaseUrl, $"snapshots/image/{doorbotId}");

            // Request the snapshot
            Stream stream = await _httpUtility.DownloadFile(downloadSnapshotUri, AuthenticationToken);
            return stream;
        }

        /// <summary>
        /// Requests the Ring API to get a fresh snapshot from the provided doorbot
        /// </summary>
        /// <param name="doorbot">The doorbot to request a fresh snapshot from</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="Exceptions.UnexpectedOutcomeException">Thrown if the actual HTTP response is different from what was expected</exception>
        public async Task UpdateSnapshot(Doorbot doorbot)
        {
            await UpdateSnapshot(doorbot.Id);
        }

        /// <summary>
        /// Requests the Ring API to get a fresh snapshot from the provided doorbot
        /// </summary>
        /// <param name="doorbotId">ID of the doorbot to request a fresh snapshot from</param>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.DownloadFailedException">Thrown when a download URL could not be created.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        /// <exception cref="Exceptions.UnexpectedOutcomeException">Thrown if the actual HTTP response is different from what was expected</exception>
        public async Task UpdateSnapshot(int doorbotId)
        {
            await EnsureSessionValid();

            // Construct the URL which will trigger the Ring API to refresh the snapshots
            Uri updateSnapshotUri = new(RingApiBaseUrl, "snapshots/update_all");

            // Construct the body of the message
            string bodyContent = string.Concat(@"{ ""doorbot_ids"": [", doorbotId, @"], ""refresh"": true }");

            // Send the request
            await _httpUtility.SendRequestWithExpectedStatusOutcome(updateSnapshotUri, HttpMethod.Put, System.Net.HttpStatusCode.NoContent, bodyContent, AuthenticationToken);
        }

        /// <summary>
        /// Request the date and time when the last snapshot was taken from the provided doorbot
        /// </summary>
        /// <param name="doorbot">The doorbot to request when the last snapshot was taken from</param>
        /// <returns>Entity with information regarding the last taken snapshot</returns>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<DoorbotTimestamps> GetDoorbotSnapshotTimestamp(Doorbot doorbot)
        {
            return await GetDoorbotSnapshotTimestamp(doorbot.Id);
        }

        /// <summary>
        /// Request the date and time when the last snapshot was taken from the provided doorbot
        /// </summary>
        /// <param name="doorbotId">ID of the doorbot to request when the last snapshot was taken from</param>
        /// <returns>Entity with information regarding the last taken snapshot</returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">Thrown when the refresh token is invalid.</exception>
        /// <exception cref="Exceptions.SessionNotAuthenticatedException">Thrown when there's no OAuth token, or the OAuth token has expired and there is no valid refresh token.</exception>
        /// <exception cref="Exceptions.ThrottledException">Thrown when the web server indicates too many requests have been made (HTTP 429).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationIncorrectException">Thrown when the web server indicates the two-factor code was incorrect (HTTP 400).</exception>
        /// <exception cref="Exceptions.TwoFactorAuthenticationRequiredException">Thrown when the web server indicates two-factor authentication is required (HTTP 412).</exception>
        public async Task<DoorbotTimestamps> GetDoorbotSnapshotTimestamp(int doorbotId)
        {
            await EnsureSessionValid();

            // Construct the URL which will request the timestamps of the latest snapshots
            Uri updateSnapshotUri = new(RingApiBaseUrl, "snapshots/timestamps");

            // Construct the body of the message
            string bodyContent = string.Concat(@"{ ""doorbot_ids"": [", doorbotId, @"]}");

            // Send the request
            DoorbotTimestamps doorbotTimestamps = await _httpUtility.SendRequest<DoorbotTimestamps>(updateSnapshotUri, HttpMethod.Post, bodyContent, AuthenticationToken);
            return doorbotTimestamps;
        }

        #endregion Methods
    }
}