using System.Text.Json.Serialization;
using System;

namespace StreamMaster.Ring.API.Entities
{
    /// <summary>
    /// Represents an OAuth Token received from the Ring API that can be used to communicate with the Ring Services
    /// </summary>
    public class OAutToken
    {
        /// <summary>
        /// The OAuth access token that can be used as a Bearer token to communicate with the Ring API
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets a DateTime with when this token expires
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// The OAuth Refresh Token that can be used to get a new OAuth Access Token after it expires
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}