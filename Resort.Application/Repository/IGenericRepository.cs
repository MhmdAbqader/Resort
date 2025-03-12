using Microsoft.EntityFrameworkCore.Query;
using Resort.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Repository
{
    public interface IGenericRepository<T> where T : class 
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includes = null, bool track = false);
        T GetById(Expression<Func<T, bool>> filter, string? includes = null, bool track = false);
        void Add(T entity);       
        void Remove(T entity);

        // using include && ThenInclude 
  
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
    }
}
