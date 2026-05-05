using IdentityEmail.Dtos;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace IdentityEmail.Controllers
{
    public class EmailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(MailRequestDto mailRequestDto)
        {
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin Identity", "berdan0227@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo=new MailboxAddress("User", mailRequestDto.ReceiverEmail);
            mimeMessage.To.Add(mailboxAddressTo);

            mimeMessage.Subject = mailRequestDto.Subject;

            var bodybuilder = new BodyBuilder();
            bodybuilder.TextBody = mailRequestDto.MessageDetail;
            mimeMessage.Body = bodybuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("berdan0227@gmail.com", "your code");
            client.Send(mimeMessage);
            client.Disconnect(true);

            return RedirectToAction("Sendbox");
        }
    }
}
