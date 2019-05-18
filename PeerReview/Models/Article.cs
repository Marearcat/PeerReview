using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Models
{
    public class Article
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
