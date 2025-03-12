using Resort.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Repository
{
    public interface IVillaRepository : IGenericRepository<Villa> 
    {
        // move those methods to generic repository
        //IEnumerable<Villa> GetAll(Expression<Func<Villa,bool>>? filter = null , string? include=null);
        //Villa GetById(Expression<Func<Villa,bool>>  filter   , string? include=null);
        //void Add (Villa villa);   
        //void Remove(Villa villa);
        void Update (Villa villa);     
 

    }
}
