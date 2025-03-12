using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Domain.Models;
using Resort.Application.Utility;
using static System.Net.Mime.MediaTypeNames;



namespace Resort.Application.Services.Implementation
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _webHostEnvironment;

        public VillaService(IUnitOfWork unitOfWork, IHostingEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
         
        public IEnumerable<Villa> GetAllVillas()
        {
            IEnumerable<Villa> villaList = _unitOfWork.VillaRepository.GetAll(includes: "VillaAmenities");
            return villaList;
        }

        public Villa GetVillaById(int id)
        {          
            var villaDb = _unitOfWork.VillaRepository.GetById(a => a.Id == id, includes: "VillaAmenities");           
            
            return villaDb;                      
        }

        public void CreateVilla(Villa villa)
        {
            if (villa.ImgIFormFile is not null)
            {
                //string fileName = productImage.FileName;     
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(villa.ImgIFormFile.FileName);

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                //string fullPathUploaded = Path.Combine(wwwRootPath, @"\Images\Products"); // don't work this code
                string fullPathUploaded = wwwRootPath + @"\images\VillaImage";

                using (var fileStream = new FileStream(Path.Combine(fullPathUploaded, fileName + extension), FileMode.Create))
                {
                    villa.ImgIFormFile.CopyTo(fileStream);
                }
                villa.ImgURL = @"images\VillaImage\" + fileName + extension;
            }
            else
            {
                villa.ImgURL = "https://placehold.co/600X402";
            }
            _unitOfWork.VillaRepository.Add(villa);
            _unitOfWork.SaveChanges();

        }
        public void UpdateVilla(Villa villa)
        {
            if (villa.ImgIFormFile is not null)
            {
                      
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(villa.ImgIFormFile.FileName);

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fullPathUploaded = wwwRootPath + @"\images\VillaImage";

                string[] files = System.IO.Directory.GetFiles(fullPathUploaded);

                 
                int lastIndex = villa.ImgURL.LastIndexOf("\\");

                if (lastIndex != -1)
                {
                    if (files.Any(a => a.Substring(a.LastIndexOf("\\")).Equals(villa.ImgURL.Substring(villa.ImgURL.LastIndexOf("\\")))))
                    {
                        System.IO.File.Delete(wwwRootPath + "\\" + villa.ImgURL);
                    }
                }
                
                using (var fileStream = new FileStream(Path.Combine(fullPathUploaded, fileName + extension), FileMode.Create))
                {
                    villa.ImgIFormFile.CopyTo(fileStream);
                }
                villa.ImgURL = @"images\VillaImage\" + fileName + extension;
            }
            _unitOfWork.VillaRepository.Update(villa);
            _unitOfWork.SaveChanges();
        }      
  

        public bool DeleteVilla(int id)
        {
            var villaDb = _unitOfWork.VillaRepository.GetById(a => a.Id == id);
            try
            {
                if (villaDb is not null) 
                {
                    if (!string.IsNullOrEmpty(villaDb.ImgURL))
                    {
                        // my custome code in details with another idea
                        //string wwwRootPath = _webHostEnvironment.WebRootPath;
                        //string fullPathUploaded = wwwRootPath + @"\images\VillaImage";

                        //string[] files = System.IO.Directory.GetFiles(fullPathUploaded);

                        //if (files.Any(a => a.Substring(a.LastIndexOf("\\")).Equals(villa.ImgURL.Substring(villa.ImgURL.LastIndexOf("\\")))))
                        //{
                        //    System.IO.File.Delete(wwwRootPath + "\\" + villa.ImgURL);
                        //}

                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villaDb.ImgURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _unitOfWork.VillaRepository.Remove(villaDb);
                    _unitOfWork.SaveChanges();
                    return true;
                }
                return false;
            } 
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<Villa> GetVillasAvailabilityByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _unitOfWork.VillaRepository.GetAll(includes: "VillaAmenity").ToList();
            var villaNumbersList = _unitOfWork.VillaNumberRepository.GetAll().ToList();
            var bookedVillas = _unitOfWork.BookingRepository.GetAll(u => u.Status == SD.Approved ||
            u.Status == SD.CheckedIn).ToList();


            foreach (var villa in villaList)
            {
                int roomAvailable = SD.GetVillaRoomsAvailableCount
                    (villa.Id, villaNumbersList, bookedVillas, checkInDate, nights );

                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            return villaList;
        }

        public bool isVillaAvailableByDate(int villaId, int nights, DateOnly dateselected)
        {
            var villaList = _unitOfWork.VillaRepository.GetAll(null, "VillaAmenities");
            var villaNumbers = _unitOfWork.VillaNumberRepository.GetAll().ToList();
            var bookings = _unitOfWork.BookingRepository.GetAll(a => a.Status == SD.Approved || a.Status == SD.CheckedIn).ToList();


            int roomsAvailable = SD.GetVillaRoomsAvailableCount(villaId, villaNumbers, bookings, dateselected, nights);

            return roomsAvailable > 0;
        }
    }
}
