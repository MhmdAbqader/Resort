using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Domain.Models;

namespace Resort.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;      

        public VillaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }

        void IVillaNumberService.CreateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumberRepository.Add(villaNumber);
            _unitOfWork.SaveChanges();
        }

        bool IVillaNumberService.DeleteVillaNumber(int id)
        {
            try 
            {
                VillaNumber? objFromDb = _unitOfWork.VillaNumberRepository.GetById(u => u.Villa_Number == id);
                if (objFromDb is not null)
                {
                    _unitOfWork.VillaNumberRepository.Remove(objFromDb);
                    _unitOfWork.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex) {return false;}
        }

        IEnumerable<VillaNumber> IVillaNumberService.GetAllVillasNumber()
        {
            return
            _unitOfWork.VillaNumberRepository.GetAll(includes:"Villa");
        }

        VillaNumber IVillaNumberService.GetVillaNumberById(int id)
        {
            return
           _unitOfWork.VillaNumberRepository.GetById(i => i.Villa_Number == id,includes:"Villa");
        }

        void IVillaNumberService.UpdateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumberRepository.Update(villaNumber);
            _unitOfWork.SaveChanges();
        }
        public async Task<IEnumerable<VillaNumber>> Include_ThenIncludeVillaNumber()
        {
            var result = await _unitOfWork.VillaNumberRepository.
                GetAllAsync(include: query =>
                query.Include(p => p.Villa)
                     .ThenInclude(q => q.VillaAmenities)
                     .Include(o => o.Villa) // Include Villa
                     .ThenInclude(c => c.Owner) // ThenInclude Owner (nested navigation)
        );
            return result;
        }
    }
}
