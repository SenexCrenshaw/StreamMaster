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
        /// The type of OAuth token that was returned, typically bearer
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        private int _expiresInSeconds;
        /// <summary>
        /// Gets the amount of seconds after creation of this OAuth token after which it expires
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds
        {
            get { return _expiresInSeconds; }
            set { _expiresInSeconds = value; ExpiresAt = DateTime.Now.AddSeconds(value); }
        }

        /// <summary>
        /// Gets a DateTime with when this token expires
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// The OAuth Refresh Token that can be used to get a new OAuth Access Token after it expires
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Scope to which this OAuth token grants access, typically client
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Seconds since January 1, 1970 at indicating when this OAuth Token was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public int CreatedAtTicks { get; set; }

        /// <summary>
        /// Date and time at which this OAuth Token was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return new DateTime(1970, 1, 1).AddSeconds(CreatedAtTicks); }
        }
    }
}