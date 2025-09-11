using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Bank_Configuration_Portal.Models.Api
{
  

    /// <summary>Access & refresh tokens.</summary>
    public class TokenResponseModel
    {
        /// <summary>Opaque bearer token for API calls.</summary>
        [Required, JsonProperty("access_token")] 
        public string AccessToken { get; set; }

        /// <summary>Minutes until the access token expires.</summary>
        [JsonProperty("expires_in")] 
        public int ExpiresIn { get; set; }

        /// <summary>Opaque refresh token.</summary>
        [Required, JsonProperty("refresh_token")] 
        public string RefreshToken { get; set; }

        /// <summary>Minutes until the refresh token expires.</summary>
        [JsonProperty("refresh_expires_in")] 
        public int RefreshExpiresIn { get; set; }
    }

   

}
