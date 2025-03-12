using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
    public class GenericRepository<T> : IGenericRepository<T> where T :class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includes = null, bool track = false)
        {

            // i can use _context.Set<T>(); with all operation better than _dbset 
            // and i have no need to do that =>  private readonly DbSet<T> _dbSet and give it value inside ctor 

            IQueryable<T> query; //= _context.Set<T>();
            // IQueryable<T> query = _dbSet ;
            if (track)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }



            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includes != null)
            {
                foreach (var inc in includes.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(inc.Trim());
                }
            }
            return query.ToList();
        }

        public T GetById(Expression<Func<T, bool>> filter, string? includes = null, bool track = false)
        {

            
            IQueryable<T> query; //= _context.Set<T>();
            // IQueryable<T> query = _dbSet ;
            if (track)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includes != null)
            {
                foreach (var inc in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                  query = query.Include(inc.Trim()) ;
                }
            }
            return query.FirstOrDefault();
        }
        public void Add(T entity)
        {
     
          //    _context.Set<T>().Add(entity);
           _dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            // i can use _context.Set<T>(); with all operation better than _dbset 
            // and i have no need to do that =>  private readonly DbSet<T> _dbSet and give it value inside ctor 

         //    _context.Set<T>().Remove(entity); 
           _dbSet.Remove(entity);
        }


        // GetAll method with Include and ThenInclude support

        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> include_thenInclude = null)
        {
            IQueryable<T> query = _dbSet;

            if (include_thenInclude != null)
            {
                query = include_thenInclude(query);
            }

            return await query.ToListAsync();
        }
    }
}
