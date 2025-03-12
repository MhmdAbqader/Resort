using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using Resort.Infrastructure.Implementation;

namespace Resort.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        // private readonly IVillaRepository _villaRepository;
        //private readonly IUnitOfWork _unitOfWork;
       // private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IVillaService _villaService ;
       

        public VillaController(/*IVillaRepository villaRepository IUnitOfWork unitOfWork*/ 
            IWebHostEnvironment webHostEnvironment,IVillaService villaService)
        {
            //   _villaRepository =  villaRepository;
            //    _unitOfWork = unitOfWork;
          //  _webHostEnvironment = webHostEnvironment;
            _villaService = villaService;
        }
        public IActionResult Index()
        {          
            IEnumerable<Villa> villaList = _villaService.GetAllVillas();                       
            return View(villaList.ToList());
        }

        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa)
        {
            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("name", "The description cannot exactly match the Name.");
            }
            if (ModelState.IsValid) 
            {
                _villaService.CreateVilla(villa);
                @TempData["success"] = "The Villa has been created successfully! :)";
                return RedirectToAction("Index","Villa");
            }
            return View(villa);
        }
        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult Update(int villaId)
        {
           
        //    var villaDb = _context.Villas.Find(villaId);
            var villaDb = _villaService.GetVillaById(villaId);
            if (villaDb == null) 
            {
                // return NotFound();
                return RedirectToAction("Error", "Home");
            }
            return View(villaDb);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                _villaService.UpdateVilla(villa);
                @TempData["success"] = "The Villa has been updated successfully! :)";
                return RedirectToAction("Index", "Villa");
            }
            return View(villa);  
        }

        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult Delete(int villaId)
        {
            //  Villa? villaDb =  _context.Villas.Find(villaId);
            var villaDb = _villaService.GetVillaById(villaId);
            if (villaDb is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaDb);
        }

        [HttpPost]
        public IActionResult Delete(Villa villa)
        {

            //Villa? villaDb = _context.Villas.Find(villa.Id);
            var villaDb = _villaService.GetVillaById(villa.Id);
            if (villaDb is null)
            {
                @TempData["error"] = "The Villa could not be deleted! ):";
                return View();
            }
            else
            {
                _villaService.DeleteVilla(villa.Id);
                @TempData["success"] = "The Villa has been deleted successfully! :)";
                return RedirectToAction("Index", "Villa");
            }

        }
    }
}
