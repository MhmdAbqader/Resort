using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Infrastructure.Implementation
{
    public class VillaRepository : GenericRepository<Villa>, IVillaRepository 
    {
        private readonly ApplicationDbContext _context;

        public VillaRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }
        public void Update(Villa villa)
        {                     
            _context.Update(villa);
        }
 

        

        #region move these methods to generic repository


        // move these methods to generic repository
        //public IEnumerable<Villa> GetAll(Expression<Func<Villa, bool>>? filter = null, string? includes = null)
        //{
        //    IQueryable<Villa> query = _context.Set<Villa>();
        //    if (filter != null) 
        //    {
        //        query = query.Where(filter);
        //    }
        //    if (includes != null)
        //    {
        //        foreach (var inc in includes.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries)) 
        //        {
        //            query.Include(inc);
        //        }
        //    }
        //    return query.ToList();
        //}

        //public Villa GetById(Expression<Func<Villa, bool>> filter, string? includes = null)
        //{
        //    IQueryable<Villa> query = _context.Set<Villa>();
        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }
        //    if (includes != null)
        //    {
        //        foreach (var inc in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query.Include(inc);
        //        }
        //    }
        //    return query.FirstOrDefault();
        //}
        //public void Add(Villa villa)
        //{
        //    _context.Add(villa);
        //}
        //public void Remove(Villa villa)
        //{
        //    _context.Remove(villa);
        //}
        #endregion

    }
}
