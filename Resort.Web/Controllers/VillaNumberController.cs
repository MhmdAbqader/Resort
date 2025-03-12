using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Services.Implementation;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using Stripe.Climate;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Resort.Web.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class VillaNumberController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IVillaNumberService _villaNumberService ;
        private readonly IVillaService _villaService;

        public VillaNumberController(/*IUnitOfWork unitOfWork,*/ IVillaNumberService  villaNumberService,IVillaService villaService)
        {    
            //_unitOfWork = unitOfWork;
            _villaNumberService =  villaNumberService;
            _villaService = villaService;

        }

        public IActionResult CheckVilla_NumberExist(int Villa_Number) //the param name must match the prop name  
        {
            var villa_NumberExistID = _villaNumberService.GetAllVillasNumber()
                                                         .SingleOrDefault(a => a.Villa_Number == Villa_Number);
            if (villa_NumberExistID is null)
                return Json(true);

            return Json(false);
        }

        public IActionResult Index()
        {
            //  var VillaNumberList = _context.VillaNumbers.Include(a => a.Villa).ToList();
            //var VillaNumberList = _unitOfWork.VillaNumberRepository.GetAll(null,"Villa");
            var VillaNumberList = _villaNumberService.GetAllVillasNumber();

            return View(VillaNumberList);
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
        public IActionResult Create(VillaNumber  villaNumber)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                   .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            // to skip validation there are 3 ways to do that 
            // 1- add attr called [ValidateNever]
            // 2- add null operator to property 
            // 3- modify ModelState like that ModelState.Remove("Villa");
            ModelState.Remove("Villa");

            // bool isRoomNumberIDExist = _context.VillaNumbers.Any(v => v.Villa_Number == villaNumber.Villa_Number);
            //bool isRoomNumberIDExist = _unitOfWork.VillaNumberRepository.GetAll().Any(v => v.Villa_Number == villaNumber.Villa_Number);
            bool isRoomNumberIDExist = _villaNumberService.GetAllVillasNumber().Any(v => v.Villa_Number == villaNumber.Villa_Number);
            if (isRoomNumberIDExist)
            {         
                @TempData["error"] = "The Villa Number is already exists! :)";
                return View(villaNumber);
            }
            else if (ModelState.IsValid)
            {
                //_unitOfWork.VillaNumberRepository.Add(villaNumber);
                //_unitOfWork.SaveChanges();
                _villaNumberService.CreateVillaNumber(villaNumber);
                @TempData["success"] = "The Villa Number has been created successfully! :)";
                return RedirectToAction("Index", "VillaNumber");
            }
            else
            {             
                return View(villaNumber);
            }
        }

        [HttpGet]
        public IActionResult Update(int villaNumberId)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                    .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            var villaNumberDb = _villaNumberService.GetVillaNumberById(villaNumberId);
            if (villaNumberDb == null) 
            {
                // return NotFound();
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberDb);


        }
        [HttpPost]
        public IActionResult Update(VillaNumber villaNumber)
        {
            ModelState.Remove("Villa");
            if (ModelState.IsValid && villaNumber.Villa_Number > 0)
            {
                _villaNumberService.UpdateVillaNumber(villaNumber);
                @TempData["success"] = "The Villa villaNumber has been updated successfully! :)";
                return RedirectToAction("Index", "VillaNumber");
            }

            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
               .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            return View(villaNumber);  
        }

        [HttpGet]
        public IActionResult Delete(int villaNumberId)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
              .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            //  var villaNumberDb =  _context.VillaNumbers.Find(villaNumberId);
            var villaNumberDb = _villaNumberService.GetVillaNumberById(villaNumberId);
            if (villaNumberDb is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberDb);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumber villaNumber)
        {
            IEnumerable<SelectListItem> list = _villaService.GetAllVillas()
                   .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            ViewData["VillaList"] = list;

            var villaNumberDb = _villaNumberService.GetVillaNumberById(villaNumber.Villa_Number);
            if (villaNumberDb is null)
            {
                @TempData["error"] = "The villa NumberDb could not be deleted! ):";
                return View();
            }
            else 
            {
                _villaNumberService.DeleteVillaNumber(villaNumber.Villa_Number);
                @TempData["success"] = "The Villa Number has been deleted successfully! :)";
                return RedirectToAction("Index", "VillaNumber");
            }

        }

        // test include and ThenInclude on villaNumber that contains Villa property and Villa contains Amenity
        public async Task<IActionResult> Include_ThenInclude() 
        {
            var result = await _villaNumberService.Include_ThenIncludeVillaNumber();
      
            return Json(result);
        }
   
    }
}
