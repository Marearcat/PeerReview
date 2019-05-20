using System.Collections.Generic;

namespace PeerReview.Core.DTOs
{
    public class RegistrationPost
    {
        public string Key { get; set; }
        public string FullName { get; set; }
        public string Nick { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}