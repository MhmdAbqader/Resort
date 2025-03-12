using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Resort.Domain.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [ForeignKey(nameof(Villa))]
        public int VillaId { get; set; }
        //   public Villa? Villa { get; set; } first way to skip validation

        [ValidateNever] //second way to skip validation
        public Villa Villa { get; set; }
    }
}
