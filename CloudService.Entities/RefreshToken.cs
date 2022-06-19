using System;

namespace CloudService.Entities
{
    public class RefreshToken : Entity
    {
        public string Token { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
