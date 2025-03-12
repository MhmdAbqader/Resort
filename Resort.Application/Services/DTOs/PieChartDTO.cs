using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Services.DTOs
{
    public class PieChartDTO
    {
        public string[] Labels { get; set; }
        public decimal[] Series { get; set; }
    }
}
