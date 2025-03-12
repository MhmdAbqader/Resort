using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Application.Services.DTOs;
using Resort.Domain.Models;

namespace Resort.Application.Services.Interface
{
    public interface IDashboardService
    {
       IEnumerable<ApplicationUser> CustomerBooked();
       RadialBarChartDTO GetTotalBookingRadialChartData();
       RadialBarChartDTO  GetRegisteredUserChartData();
       RadialBarChartDTO GetRevenueChartData();
       PieChartDTO GetBookingPieChartData();
    }
}
