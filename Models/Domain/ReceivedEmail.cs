namespace Bloggie.Web.Models.Domain
{
    public class ReceivedEmails
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
    }
}
