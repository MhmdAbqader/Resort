using Resort.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Repository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        void Update(Booking booking);     
    }
}
