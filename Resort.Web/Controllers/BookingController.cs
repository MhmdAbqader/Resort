using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Resort.Application.Repository;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Web.ViewModels;
using Stripe;
using Stripe.Checkout;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Security.Claims;
using Syncfusion.Drawing;
using Resort.Application.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace Resort.Web.Controllers
{
    public class BookingController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingService;        
        private readonly IVillaService _villaService;
        private readonly IVillaNumberService _villaNumberService;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public BookingController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager,
            IVillaService villaService,
            IVillaNumberService villaNumberService)
        {
           // _unitOfWork = unitOfWork;
            _bookingService = bookingService;
            _userManager = userManager;
            _villaService = villaService;
            _villaNumberService = villaNumberService;
            _webHostEnvironment = webHostEnvironment;
        }


        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        #region API Calls
        [HttpGet]
        //[Authorize]
        public IActionResult GetAll(string? status = "")       
        {
            IEnumerable<Booking> objBookings;
            string userId = "";       

            if (!User.IsInRole(SD.AdminRole))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objBookings = _bookingService.GetAllBooking(userId,status);
            }
            else
            {
                objBookings = _bookingService.GetAllBooking(userId,status);
            }
            // perfect Code *****************88**88**88**88**88**88**88
            //if (!User.IsInRole(SD.AdminRole))
            //{
            //    var claimsIdentity = (ClaimsIdentity)User.Identity;
            //    userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //}
            //objBookings = _bookingService.GetAllBooking(userId, status);


            if (status != null || status != "")
            {
                objBookings = objBookings.Where(a => a.Status.ToLower().Equals(status.ToLower()));
            }
            //  objBookings = _bookingService.GetAllBookings(userId, status);

            return Json(new { data = objBookings });
        }
        #endregion

      



        [Authorize]
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser? user = _userManager.FindByIdAsync(currentUserId).GetAwaiter().GetResult();

            Booking booking  = new Booking()
            {
                VillaId = villaId,
                CheckInDate = checkInDate, 
                CheckOutDate =  checkInDate.AddDays(nights),
                Nights = nights,
                Villa = _villaService.GetVillaById(villaId),
                UserId = currentUserId,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Email = user.Email
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }
        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _villaService.GetVillaById(booking.VillaId);                       
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.Pending;
            booking.BookingDate = DateTime.Now;
                  
            if (!_villaService.isVillaAvailableByDate(villa.Id,booking.Nights,booking.CheckInDate)) 
            {
                TempData["error"] = " room has been reserved ):";
                return RedirectToAction(nameof(FinalizeBooking),new 
                {
                    villaId = booking.VillaId ,
                    checkInDate = booking.CheckInDate ,
                    nights = booking.Nights
                });
            }



            //_unitOfWork.BookingRepository.Add(booking);
            //_unitOfWork.SaveChanges();
            _bookingService.CreateBooking(booking);

            string currentDomain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                // LineItems = {
                //new SessionLineItemOptions
                //{
                //  PriceData = new SessionLineItemPriceDataOptions
                //  {
                //    UnitAmount = 2000,
                //    Currency = "usd",
                //    ProductData = new SessionLineItemPriceDataProductDataOptions
                //    {
                //      Name = "T-shirt",
                //    },
                //  },
                //  Quantity = 1,
                //},
                //},


                // After editing code 
                LineItems = new List<SessionLineItemOptions>(),    
                Mode = "payment",
                SuccessUrl = currentDomain + $"Booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = currentDomain + $"Booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
            };

            // new  our Lineitems with our data
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100), // because unit is treate as cent currency 
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        Description = "No Details",
                        //Images =new List<string> { currentDomain + villa.ImgURL }
                    }
                },
                Quantity = 1,
            });
           

            var service = new SessionService();
            Session session = service.Create(options);


            //_unitOfWork.BookingRepository
            //    .UpdatePaymentStripeSessionId_PaymentIntendId(bookingId: booking.Id, sessionId: session.Id, paymentIntendId: session.PaymentIntentId);
            //_unitOfWork.SaveChanges();

            _bookingService.UpdatePaymentStripeSessionId_PaymentIntendId(booking.Id, session.Id, session.PaymentIntentId);

            Response.Headers.Add("Location", session.Url);       
            return new StatusCodeResult(303);
            //return RedirectToAction("BookingConfirmation", new { bookingId = booking.Id});
        }



        public IActionResult BookingConfirmation(int bookingId) 
        {
          //  var bookingDb = _unitOfWork.BookingRepository.GetById(a => a.Id == bookingId, includes: "User,Villa");
            var bookingDb = _bookingService.GetBookingById(bookingId);

            if (bookingDb?.Status == SD.Pending) 
            {
                var service = new SessionService();
                Session session = service.Get(bookingDb.StripeSessionId);

                if (session.PaymentStatus == "paid") 
                {
                    _bookingService.UpdateBookingStatus(bookingId, SD.Approved, 0);
                    _bookingService.UpdatePaymentStripeSessionId_PaymentIntendId(bookingId, session.Id, session.PaymentIntentId);
                }
            }
            else
                return RedirectToAction("Error", "Home");

            return View(bookingId);
        }


        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            var bookingDB = _bookingService.GetBookingById(bookingId);
            if (bookingDB.VillaNumber == 0 && bookingDB.Status == SD.Approved)
            {
                List<int> availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingDB.VillaId);
                bookingDB.VillaNumbers = _villaNumberService.GetAllVillasNumber()
                    .Where(a => a.VillaId == bookingDB.VillaId && availableVillaNumber.Any(x => x == a.Villa_Number))
                    .ToList();
            }
            return View(bookingDB);
        }
        private List<int> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = _villaNumberService.GetAllVillasNumber().Where(a=>a.VillaId == villaId);           

            var checkedInVilla = _bookingService.GetAllBooking().Where(a => a.VillaId == villaId && a.Status == SD.CheckedIn).Select(a => a.VillaNumber);

            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;
        }
        [HttpPost]
        [Authorize(Roles =SD.AdminRole)]
        public IActionResult CheckIn(Booking booking) 
        { 
                _bookingService.UpdateBookingStatus(booking.Id, SD.CheckedIn, booking.VillaNumber);                 
                TempData["success"] = "Booking updated successfully.!";
                return RedirectToAction(nameof(BookingDetails), new { bookingId  = booking.Id } );
        }
        [HttpPost]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult CheckOut(Booking booking) // this means  that the admin can accept the order booking  
        {
            _bookingService.UpdateBookingStatus(booking.Id, SD.Completed,booking.VillaNumber);            
            TempData["success"] = " Booking Completed successfully.!";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingService.UpdateBookingStatus(booking.Id, SD.Cancelled, 0);             
            TempData["success"] = "Booking Cancelled successfully.!";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize]
        public IActionResult GenerateInvoice(int id, string downloadType) 
        {
            string basePath = _webHostEnvironment.WebRootPath;
            WordDocument document = new();

            //load temeplete file that i will use it to bind data on it
            string dataPath = basePath + @"/exports/BookingDetails.docx";
            using (FileStream fileStream = new FileStream(dataPath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite)) 
            {
                // auto that means FormatType can know the document type from stream 
                document.Open(fileStream, FormatType.Automatic);  
            }

         
            Booking bookingFromDb = _bookingService.GetBookingById(id);
            //update templete by data from database
            TextSelection textSelection = document.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Name;
            textSelection = document.Find("xx_customer_phone", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Phone;

            textSelection = document.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Email;

            textSelection = document.Find("XX_BOOKING_NUMBER", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING ID - " + bookingFromDb.Id;
            textSelection = document.Find("XX_BOOKING_DATE", false, true);
            textRange = textSelection.GetAsOneRange();     
            textRange.Text = "BOOKING DATE - " + bookingFromDb.BookingDate.ToShortDateString();


            textSelection = document.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.PaymentDate.ToShortDateString();
            textSelection = document.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckInDate.ToShortDateString();
            textSelection = document.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckOutDate.ToShortDateString(); ;
            textSelection = document.Find("xx_booking_total", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.TotalCost.ToString("c");

            WTable table = new(document);

            table.TableFormat.Borders.LineWidth = 1f;
            table.TableFormat.Borders.Color = Color.Black;
            table.TableFormat.Paddings.Top = 7f;
            table.TableFormat.Paddings.Bottom = 7f;
            table.TableFormat.Borders.Horizontal.LineWidth = 1f;

            int rows = bookingFromDb.VillaNumber > 0 ? 3 : 2;
            table.ResetCells(rows, 4);

            WTableRow row0 = table.Rows[0];

            row0.Cells[0].AddParagraph().AppendText("NIGHTS");
            row0.Cells[0].Width = 80;
            row0.Cells[1].AddParagraph().AppendText("VILLA");
            row0.Cells[1].Width = 220;
            row0.Cells[2].AddParagraph().AppendText("PRICE PER NIGHT");
            row0.Cells[3].AddParagraph().AppendText("TOTAL");
            row0.Cells[3].Width = 80;

            WTableRow row1 = table.Rows[1];

            row1.Cells[0].AddParagraph().AppendText(bookingFromDb.Nights.ToString());
            row1.Cells[0].Width = 80;
            row1.Cells[1].AddParagraph().AppendText(bookingFromDb.Villa.Name);
            row1.Cells[1].Width = 220;
            row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost / bookingFromDb.Nights).ToString("c"));
            row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("c"));
            row1.Cells[3].Width = 80;

            if (bookingFromDb.VillaNumber > 0)
            {
                WTableRow row2 = table.Rows[2];

                row2.Cells[0].Width = 80;
                row2.Cells[1].AddParagraph().AppendText("Villa Number - " + bookingFromDb.VillaNumber.ToString());
                row2.Cells[1].Width = 220;
                row2.Cells[3].Width = 80;
            }

            WTableStyle tableStyle = document.AddTableStyle("CustomStyle") as WTableStyle;
            tableStyle.TableProperties.RowStripe = 1;
            tableStyle.TableProperties.ColumnStripe = 2;
            tableStyle.TableProperties.Paddings.Top = 2;
            tableStyle.TableProperties.Paddings.Bottom = 1;
            tableStyle.TableProperties.Paddings.Left = 5.4f;
            tableStyle.TableProperties.Paddings.Right = 5.4f;

            ConditionalFormattingStyle firstRowStyle = tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
            firstRowStyle.CharacterFormat.Bold = true;
            firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
            firstRowStyle.CellProperties.BackColor = Color.Black;

            table.ApplyStyle("CustomStyle");

            TextBodyPart bodyPart = new(document);
            bodyPart.BodyItems.Add(table);

            document.Replace("<ADDTABLEHERE>", bodyPart, false, false);





            // rendering and create file as docx or pdf as u like 
            using DocIORenderer renderer = new();
            MemoryStream stream = new();
            if (downloadType == "word")
            {
                document.Save(stream, FormatType.Docx);
                stream.Position = 0;

                return File(stream, "application/docx", $"BookingDetails{DateTime.Now.ToShortTimeString()}.docx");
            }
            else
            {
                PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                pdfDocument.Save(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", $"BookingDetails{DateTime.Now.ToShortTimeString()}.pdf");
            }
        }

    
  
    }
}
