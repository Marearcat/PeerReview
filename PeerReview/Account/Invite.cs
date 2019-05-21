using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Account
{
    public class Invite
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string InviterId { get; set; }
        public bool Confirmed { get; set; }
    }
}
