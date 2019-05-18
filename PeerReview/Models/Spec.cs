using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Models
{
    public class Spec
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
