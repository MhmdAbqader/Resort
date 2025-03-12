using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        public IVillaRepository VillaRepository   { get;  }
        public IVillaNumberRepository VillaNumberRepository   { get;  }
        public IAmenityRepository AmenityRepository  { get;  }
        public IBookingRepository BookingRepository  { get;  }
        public IApplicationUserRepository  ApplicationUserRepository  { get;  }
     




         void SaveChanges();
    }
}
