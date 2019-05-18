using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Unite
{
    public class BlackList
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ArticleId { get; set; }
    }
}
