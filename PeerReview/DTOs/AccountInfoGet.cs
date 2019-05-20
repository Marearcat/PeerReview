using System.Collections.Generic;

namespace PeerReview.Core.DTOs
{
    public class AccountInfoGet
    {
        public string Mail { get; set; }
        public string FullName { get; set; }
        public string Nick { get; set; }
        public string Inviter { get; set; }
        public IEnumerable<string> Specs { get; set; }
    }
}