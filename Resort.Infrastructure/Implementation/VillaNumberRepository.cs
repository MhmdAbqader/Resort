using Resort.Application.Repository;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Infrastructure.Implementation
{
    public class VillaNumberRepository : GenericRepository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _context;

      
        public VillaNumberRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

      

        public void Update(VillaNumber villaNumber)
        {
            _context.VillaNumbers.Update(villaNumber);
        }
    }
}
