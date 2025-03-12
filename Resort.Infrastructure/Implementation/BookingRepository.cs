using Resort.Application.Repository;
using Resort.Application.Utility;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Infrastructure.Implementation
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _context ;
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

     
    }
}
