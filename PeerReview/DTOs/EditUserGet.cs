using System.Collections.Generic;

namespace PeerReview.Core.DTOs
{
    public class EditUser
    {
        public string Nick { get; set; }
        public string FullName { get; set; }
        public IEnumerable<string> Specs { get; set; }
    }
}