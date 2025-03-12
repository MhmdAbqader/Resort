using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Domain.Models
{
    public class Owner
    { 
            public int Id { get; set; }            
            public required string OwnerName { get; set; }  // Navigation property
     
    }
}
