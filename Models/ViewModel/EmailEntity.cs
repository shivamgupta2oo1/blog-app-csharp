using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bloggie.Web.Models.ViewModel
{
    public class EmailEntity
    {
        [ValidateNever]
        public string FromEmailAddress { get; set; }
        public string ToEmailAddress { get; set; }
        public string Subject { get; set; }
        public string EmailBodyMessage { get; set;}
    }
}