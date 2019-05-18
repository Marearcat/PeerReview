using Microsoft.AspNetCore.Identity;

namespace PeerReview.Core.Account
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Nick { get; set; }
    }
}
