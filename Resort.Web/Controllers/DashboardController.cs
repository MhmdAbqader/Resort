using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Resort.Application.Repository;
using Resort.Application.Services;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Web.ViewModels;
using Humanizer;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;


namespace Resort.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService  _dashboardService;   
        private readonly IEmailService _emailService;   
        public DashboardController(IEmailService emailService,IDashboardService dashboardService)
        {          
            _dashboardService = dashboardService;
            _emailService = emailService;
        }
        public IActionResult AdminDashboradIndex()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SendEmailToAllUserRegisteredAndBooked()
        {
            var customerBooked = _dashboardService.CustomerBooked().ToList();
            SendEmailsVM sendEmailsVM = new()
            {
                CustomerEmailsSelected = customerBooked,
                Subject = ""
            };

            return View(sendEmailsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailToAllUserRegisteredAndBooked(SendEmailsVM sendEmails , List<string> selectedEmails)
        {
            if (selectedEmails.Count() == 0)
            {
                TempData["error"] = "No Emails are sent Must select at least one customer!";
                return RedirectToAction("AdminDashboradIndex", "Dashboard");
            }

            string body = sendEmails.Body;
            string subject = sendEmails.Subject;
            bool isSent = await _emailService
                .SendEmailsAsync(selectedEmails, subject, body);
            if (isSent)
            {
                TempData["success"] = "Emails are sent successfully!";
                return RedirectToAction("AdminDashboradIndex", "Dashboard");
            }

            return Content("Please, contact the environment development that support <h2>Resort</h2> website,<br/> Thanks");
        }

        public IActionResult GetTotalBookingRadialChartData()
        {
            // return total booking 
            // return countByCurrentMonthBooking
            // return countByPreviousMonthBooking 
            // return increaseDecreaseRatio
            return Json(_dashboardService.GetTotalBookingRadialChartData());
        }

        public IActionResult GetRegisteredUserChartData()
        {        
            return Json(_dashboardService.GetRegisteredUserChartData());
        }

        public IActionResult GetRevenueChartData()
        {
            return Json(_dashboardService.GetRevenueChartData());
        } 
 
        public IActionResult GetBookingPieChartData() 
        {         
            return Json(_dashboardService.GetBookingPieChartData());
        }

    }
}
