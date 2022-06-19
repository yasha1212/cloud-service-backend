using System;

namespace CloudService.Security.Token
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expiration { get; set; }
    }
}
