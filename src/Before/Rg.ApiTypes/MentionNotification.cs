namespace Rg.ApiTypes
{
    public class MentionNotification
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string MentionedIn { get; set; }
        public string MentionedBy { get; set; }
        public string Message { get; set; }
    }
}
