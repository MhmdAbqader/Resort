using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Resort.Domain.Models;

namespace Resort.Web.ViewModels
{
    public class SendEmailsVM
    {
        [ValidateNever]
        public List<ApplicationUser> CustomerEmailsSelected { get; set; }
        public string Body { get; set; } = "Hello  our Respected Guests from <span style='color:red;'> Resort </span> Offers Department " +
                " follow us to see exclusive offers this summer, our offers are unlimited :)" +
                " <br> here you are <br/> ";
        public string Subject { get; set; }
    }
}
