using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Application.Repository;
using Resort.Application.Services.DTOs;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Domain.Models;

namespace Resort.Application.Services.Implementation
{
    public class DashboardService: IDashboardService
    {
        static int LastMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime PreviousMonthStartDate = new DateTime(DateTime.Now.Year, LastMonth, 1);
        readonly DateTime CurrentMonthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ApplicationUser> CustomerBooked()
        {
            List<ApplicationUser> customerSelected = new List<ApplicationUser>();
            List<string> userids = new();
            var allBookings = _unitOfWork.BookingRepository.GetAll(s=>s.Status == SD.Approved || s.Status == SD.Completed);

            foreach (var c in allBookings) 
            {
                var userid = _unitOfWork.ApplicationUserRepository.GetById(a => a.Id == c.UserId).Id;
                if(!userids.Contains(userid))
                    userids.Add(userid);
            }
            foreach (var id in userids)
                customerSelected.Add(_unitOfWork.ApplicationUserRepository.GetById(a => a.Id == id));
            
            return customerSelected;
        }
       public RadialBarChartDTO GetTotalBookingRadialChartData()
        {
            var allTotalBooking = _unitOfWork.BookingRepository.GetAll(s => s.Status != SD.Pending || s.Status != SD.Cancelled);

            var countByCurrentMonthBooking = allTotalBooking.Count(a => a.BookingDate >= CurrentMonthStartDate && a.BookingDate < DateTime.Now);
            var countByPreviousMonthBooking = allTotalBooking.Count(a => a.BookingDate >= PreviousMonthStartDate && a.BookingDate < CurrentMonthStartDate);


            return CalcReturnedResultInChart(allTotalBooking.Count(), countByCurrentMonthBooking, countByPreviousMonthBooking);
        } 
        public RadialBarChartDTO GetRegisteredUserChartData()
        {
            var TotalUser = _unitOfWork.ApplicationUserRepository.GetAll();

            var countByCurrentMonth = TotalUser.Count(u => u.CreatedAt >= CurrentMonthStartDate && u.CreatedAt < DateTime.Now);
            var countByPreviousMonth = TotalUser.Count(u => u.CreatedAt >= PreviousMonthStartDate && u.CreatedAt < CurrentMonthStartDate);

            return  CalcReturnedResultInChart(TotalUser.Count(), countByCurrentMonth, countByPreviousMonth);
        }

        public RadialBarChartDTO GetRevenueChartData()
        {
            var allTotalBooking = _unitOfWork.BookingRepository.GetAll(s => s.Status != SD.Pending || s.Status != SD.Cancelled);
            int totalRevenue = Convert.ToInt32(allTotalBooking.Sum(a => a.TotalCost));

            int countByCurrentMonthBooking = Convert.ToInt32(allTotalBooking.Where(a => a.BookingDate >= CurrentMonthStartDate && a.BookingDate < DateTime.Now).Sum(a => a.TotalCost));
            int countByPreviousMonthBooking = Convert.ToInt32(allTotalBooking.Where(a => a.BookingDate >= PreviousMonthStartDate && a.BookingDate < CurrentMonthStartDate).Sum(a => a.TotalCost));


            return CalcReturnedResultInChart(allTotalBooking.Count(), countByCurrentMonthBooking, countByPreviousMonthBooking);
        }
        public PieChartDTO GetBookingPieChartData()
        {
            var allTotalBooking = _unitOfWork.BookingRepository.GetAll(s => s.BookingDate >= DateTime.Now.AddDays(-30) && (s.Status != SD.Pending || s.Status != SD.Cancelled));

            var customersHaveOnlyOneBooking = allTotalBooking.GroupBy(a => a.UserId)
                                                             .Where(b => b.Count() == 1).Select(k => k.Key).ToList();
            // it's the first time to book on our hotel
            int bookingByNewCustomer = customersHaveOnlyOneBooking.Count();
            // the Customers that it's not  the first time to book on our hotel , have been come before 
            int bookingByOldCustomer = allTotalBooking.Count() - bookingByNewCustomer;

            PieChartDTO pieChartDTO = new PieChartDTO()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = [bookingByNewCustomer, bookingByOldCustomer]
            };

            return  (pieChartDTO);
        }


        private static RadialBarChartDTO CalcReturnedResultInChart(int totalcount, int countByCurrentMonth, int countByPreviousMonth)
        {
            int increaseDecreaseRatio = 100;
            if (countByPreviousMonth != 0)
                increaseDecreaseRatio = Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);

            RadialBarChartDTO radialBarChartDTO = new RadialBarChartDTO();
            radialBarChartDTO.TotalCount = totalcount;
            radialBarChartDTO.HasRatioIncreased = countByCurrentMonth > countByPreviousMonth;
            radialBarChartDTO.CountInCurrentMonth = countByCurrentMonth;
            radialBarChartDTO.Series = [increaseDecreaseRatio]; // the same is new int[] { increaseDecreaseRatio };

            return radialBarChartDTO;
        }

    }
}
