
namespace PeerReview.Core.DTOs
{
    public class CreateReview
    {
        public int Rating { get; set; }
        public bool Accept { get; set; }
        public string ArticleId { get; set; }
    }
}