using System;

namespace CloudService.Web.ViewModels.Auth
{
    public class TokenViewModel
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
