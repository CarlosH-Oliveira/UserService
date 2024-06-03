using System.Net;
using System.Net.Mail;

namespace UserService.Services
{
    public class EmailService
    {
        public bool Validate(string email)
        {
            try
            {
                MailAddress validateEmail = new MailAddress(email);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SendEmail(string email, string subject, string body)
        {
            try
            {
                MailAddress from = new MailAddress("carlosholiveiradevtest@outlook.com");

                MailAddress to = new MailAddress(email);

                string fromPassword = "devtest32300508";

                var smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(from.Address, fromPassword)
                };

                using (var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body,
                })
                {
                    smtp.Send(message);
                }
            }
            catch
            {
                throw;
            }
            
            

        }
    }
}
