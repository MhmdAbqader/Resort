using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Resort.Web.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IVillaService _villaService;

        public AmenityController(IAmenityService amenityService,IVillaService villaService)
        {
            _amenityService = amenityService;
            _villaService = villaService;
        }

  
        public IActionResult Index()
        {
            //  var VillaNumberList = _context.VillaNumbers.Include(a => a.Villa).ToList();
            var amenitiesList = _amenityService.GetAllAmenity();

            return View(amenitiesList);
        }
        [HttpGet]
        public IActionResult Create() 
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                .Select(v => new SelectListItem {Value= v.Id.ToString(), Text=v.Name });
            ViewData["VillaList"] = list;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Amenity amenity)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            // to skip validation there are 3 ways to do that 
            // 1- add attr called [ValidateNever]
            // 2- add null operator to property 
            // 3- modify ModelState like that ModelState.Remove("Villa");
            //   ModelState.Remove("Villa");

            // bool isRoomNumberIDExist = _context.VillaNumbers.Any(v => v.Villa_Number == villaNumber.Villa_Number);
            bool isAmenityIDExist = _amenityService.GetAllAmenity().Any(v => v.Id == amenity.Id);
            if (isAmenityIDExist)
            {         
                @TempData["error"] = "The Amenity is already exists! :)";
                return View(amenity);
            }
            else if (ModelState.IsValid)
            {
                _amenityService.CreateAmenity(amenity);
                @TempData["success"] = "The Amenity has been created successfully! :)";
                return RedirectToAction("Index", "Amenity");
            }
            else
            {             
                return View(amenity);
            }
        }

        [HttpGet]
        public IActionResult Update(int amenityId)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            var amenityExistDb = _amenityService.GetAmenityById(amenityId);
            if (amenityExistDb == null) 
            {
                // return NotFound();
                return RedirectToAction("Error", "Home");
            }
            return View(amenityExistDb);


        }
        [HttpPost]
        public IActionResult Update(Amenity amenityModel)
        {
            //  ModelState.Remove("Villa");
            //Test tracking and NoTracking to avoid any exception through this simle condition 
            //var amenityFirstUsingTrack = _amenityService.GetAmenityById(amenityId);
            if (ModelState.IsValid && amenityModel.Id > 0)
            {
                //if (amenityFirstUsingTrack.Name.ToLower().Contains("special"))
                //{
                //    amenityModel.Description = amenityFirstUsingTrack.Description;
                //} 
                //else
                //{
                //    _unitOfWork.AmenityRepository.Update(amenityModel);        
                //}
                //_unitOfWork.SaveChanges();

                _amenityService.UpdateAmenity(amenityModel);
                @TempData["success"] = "The amenity has been updated successfully! :)";
                return RedirectToAction("Index", "Amenity");
            }

            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            return View(amenityModel);  
        }

        [HttpGet]
        public IActionResult Delete(int amenityId)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
               .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;
                         
            var amenityExistDb = _amenityService.GetAmenityById(amenityId); 
            if (amenityExistDb is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityExistDb);
        }

        [HttpPost]
        public IActionResult Delete(Amenity amenity )
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
               .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

           
            bool result = _amenityService.DeleteAmenity(amenity.Id);
            if (result)
            {
                @TempData["success"] = "The Amenity has been deleted successfully! :)";
                return RedirectToAction("Index", "Amenity");
         
            }
            else 
            {
                @TempData["error"] = "The villa NumberDb could not be deleted! ):";
                return View(amenity);
            }

        }
    }
}
