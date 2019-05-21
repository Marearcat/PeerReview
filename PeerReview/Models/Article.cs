using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Models
{
    public class Article
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string SpecId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
