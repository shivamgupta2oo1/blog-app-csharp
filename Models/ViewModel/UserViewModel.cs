
namespace Bloggie.Web.Models.ViewModel
{
    public class UserViewModel
    {
        public List<User> Users { set; get; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AdminRoleCheckBox { get; set; }


    }
}