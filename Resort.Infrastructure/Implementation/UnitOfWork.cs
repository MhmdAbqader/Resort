using Resort.Application.Repository;
using Resort.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Infrastructure.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IVillaRepository VillaRepository {  get; set; }
        public IVillaNumberRepository VillaNumberRepository {  get; set; }

        public IAmenityRepository AmenityRepository { get; set; }

        public IBookingRepository BookingRepository {  get; set; }

        public IApplicationUserRepository ApplicationUserRepository { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            VillaRepository = new VillaRepository(_context);
            VillaNumberRepository = new VillaNumberRepository(_context);
            AmenityRepository = new AmenityRepository(_context);
            BookingRepository = new BookingRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context); 
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
