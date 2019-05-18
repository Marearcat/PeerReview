using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PeerReview.Core.Models
{
    public class Review
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public int Mark { get; set; }
        public string ArticleId { get; set; }
    }
}
