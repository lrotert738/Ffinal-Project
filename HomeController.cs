using FinalProject.UI.MVC.Models;
using System.Net;       // Needed for Network Credentials
using System.Net.Mail;  // Needed for MailMessage and SmtpClien
using System.Web.Mvc;

namespace FinalProject.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel contact)
        {
            // Create the body of the email
            string body =
                string.Format("Name: {0}<br/>Email: {1}<br/>Subject: {2}<br/>Message: {3}",
                contact.Name, contact.Email, contact.Subject, contact.Message);

            // Create and configure the Mail message
            MailMessage msg = new MailMessage(
                "no - reply@lrotertcode.com", //From address must be an email on your hosting acct
                "lrotert738@outlook.com",
                contact.Subject,
                body);

            // Additional mail message configuration
            //msg.CC.Add("secondary@domain.com")
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;


            // Create and configure the SMTP client
            SmtpClient client = new SmtpClient(" mail.lrotertcode.com");
            client.Credentials = new NetworkCredential("no-reply@lrotertcode.com", "032358Plr!!");

            if (ModelState.IsValid)
            {
                // Attempt to send the message
                using (client)
                {
                    // using in the controller indicates we want to 
                    // open a connection between our application and the
                    // mail server

                    try
                    {
                        client.Send(msg);
                    }
                    catch
                    {
                        ViewBag.ErrorMessage = "There was an error sending your message" +
                            " Please try again.";
                    }
                }
                return View("ContactConfirmation", contact);
            } // end if

            return View();
        } // end public ActionResult Contact(ContactViewModel contact)
    } // end class controller
}
