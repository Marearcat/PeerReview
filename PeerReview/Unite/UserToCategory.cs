using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Unite
{
    public class UserToSpec
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string SpecId { get; set; }
    }
}
