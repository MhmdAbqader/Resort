using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Web.ViewModels;
using Syncfusion.DocIO.DLS;
using Syncfusion.Presentation;

namespace Resort.Web.Controllers
{
    public class HomeController : Controller
    {
      

        //private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IVillaService _villaService;
        public HomeController(/*IUnitOfWork unitOfWork,*/IVillaService villaService, IWebHostEnvironment webHostEnvironment)
        {
            _villaService = villaService;
            //_unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeModel = new HomeVM()
            {
                VillaList = _villaService.GetAllVillas(), 
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                NoOfNights=1
            };
            return View(homeModel);
        }

        // it's working but i enhanced to call it by -- Ajax & PartialVeiw 
        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            var villaList = _villaService.GetAllVillas();

            foreach (var villa in villaList)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }

            homeVM.VillaList = villaList;

            return View(homeVM);
        }
        [HttpGet]
        public IActionResult CheckVillaAvailability(DateOnly checkInDate , int nights )
        {            
            var homeVM = new HomeVM()
            {
                VillaList = _villaService.GetAllVillas(),
                CheckInDate = checkInDate,
                NoOfNights = nights
            };

            return PartialView("_VillaList", homeVM);
        }

        public IActionResult GeneratePPTExport(int id)
        {
            var villa = _villaService.GetVillaById(id);
            if (villa == null)
                return RedirectToAction(nameof(Error));
            

            string basePath = _webHostEnvironment.WebRootPath;           
            //load temeplete pptx file that i will use it to bind data on it
            string filePath = basePath + @"/exports/ExportVillaDetails.pptx";
            using IPresentation presentation = Presentation.Open(filePath);

            ISlide slide = presentation.Slides[0];
            IShape? shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaName") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Name;
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaDescription") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Description;
            }


            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtOccupancy") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Max Occupancy : {0} adults", villa.Occupancy);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaSize") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Villa Size: {0} sqft", villa.SquareFeet);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtPricePerNight") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("USD {0}/night", villa.Price.ToString("C"));
            }


            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaAmenitiesHeading") as IShape;
            if (shape is not null)
            {
                List<string> listItems = villa.VillaAmenities.Select(x => x.Name).ToList();

                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textPart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = Syncfusion.Presentation.ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textPart.Font.FontName = "system-ui";
                    textPart.Font.FontSize = 18;
                    textPart.Font.Color = ColorObject.FromArgb(144, 148, 152);

                }

            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "imgVilla") as IShape;
            if (shape is not null)
            {
                byte[] imageData;
                string imageUrl;
                try
                {
                    imageUrl = string.Format("{0}{1}", basePath+"\\", villa.ImgURL);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch (Exception)
                {
                    imageUrl = string.Format("{0}{1}", basePath, "/images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                slide.Shapes.Remove(shape);
                using MemoryStream imageStream = new(imageData);
                IPicture newPicture = slide.Pictures.AddPicture(imageStream, 60, 120, 300, 200);

            }



            MemoryStream memoryStream = new MemoryStream();
            presentation.Save(memoryStream);
            memoryStream.Position = 0;
            return File(memoryStream,"application/pptx","PPTVilla.pptx");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();            
        }
    }
}
