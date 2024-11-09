using DM7._1.Models;
using DM7._1.Services;
using Microsoft.AspNetCore.Mvc;

namespace DM7._1.Controllers
{
    public class MailController : Controller
    {
        private readonly EmailService _emailService;
        public MailController(EmailService emailService) { 
            _emailService = emailService;
        }
        public IActionResult SendMail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(EmailRequest emailRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _emailService.SendMailAsync(emailRequest);
                    ViewBag.Message = "Send mail successfully";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Opp!. Something went wrong" + ex.Message;
            }
            return View();
        }
       
    }
}
