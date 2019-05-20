namespace PeerReview.Core.DTOs
{
    public class LoginPost
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}