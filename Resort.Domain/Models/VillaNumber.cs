using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Resort.Domain.Models
{
    public class VillaNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Villa Number")]
        [Remote(action:"CheckVilla_NumberExist",controller:"VillaNumber"
            ,ErrorMessage = "already, Villa_NumberId is in use!")]
        public int Villa_Number { get; set; }
        public string? SpecialDetails {  get; set; } 

        [ForeignKey(nameof(Villa))]
        public int VillaId { get; set; }
        //   public Villa? Villa { get; set; } first way to skip validation

        //  [ValidateNever] second way to skip validation
        public Villa  Villa { get; set; } 

    }
}
