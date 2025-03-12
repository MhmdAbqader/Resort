using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Resort.Domain.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        [NotMapped] // you tell ef core deosn't make it as column in Db
        public IFormFile? ImgIFormFile { get; set; }
        public string? ImgURL { get; set; }
        public double Price { get; set; }
        public int SquareFeet { get; set; }
        public int Occupancy { get; set; } // No of Guest villa can hold them  
               
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ValidateNever]
      //  [JsonIgnore]
        public IEnumerable<Amenity> VillaAmenities { get; set; }
        
        // to check villa is booked or not 
        [NotMapped]
        public bool IsAvailable { get; set; } = true;

        //Owner
        [ForeignKey(nameof(Owner))]
        public int? OwnerId { get; set; }
        [ValidateNever]
        public Owner Owner { get; set; }


    }
}
