using System;
using System.ComponentModel.DataAnnotations;

namespace PeerReview.Core.Models
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string ArticleId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
