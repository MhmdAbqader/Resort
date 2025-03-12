using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Domain.Models;

namespace Resort.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void CreateAmenity(Amenity amenity)
        {
            _unitOfWork.AmenityRepository.Add(amenity);
            _unitOfWork.SaveChanges();
        }

        public bool DeleteAmenity(int id)
        {
            try
            {
               Amenity? objFromDb = _unitOfWork.AmenityRepository.GetById(u => u.Id == id);
                if (objFromDb is not null)
                {
                    _unitOfWork.AmenityRepository.Remove(objFromDb);
                    _unitOfWork.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex) { return false; }
        }

        public IEnumerable<Amenity> GetAllAmenity()
        {
            return
          _unitOfWork.AmenityRepository.GetAll(includes: "Villa");
        }

        public Amenity GetAmenityById(int id)
        {
            return
          _unitOfWork.AmenityRepository.GetById(i => i.Id == id, includes: "Villa"); ;
        }

        public void UpdateAmenity(Amenity amenity)
        {

            _unitOfWork.AmenityRepository.Update(amenity);
            _unitOfWork.SaveChanges();
        }
    }
}
