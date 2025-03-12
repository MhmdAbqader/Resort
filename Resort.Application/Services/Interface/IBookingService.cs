using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Domain.Models;

namespace Resort.Application.Services.Interface
{
    public interface IBookingService
    {
        void CreateBooking(Booking booking);
        Booking GetBookingById(int bookingId);
        IEnumerable<Booking> GetAllBooking(string userId = "", string? statusFilterList="");
        void UpdateBookingStatus(int bookingId, string bookingStatus, int villaNumber);
        void UpdatePaymentStripeSessionId_PaymentIntendId(int bookingId, string sessionId, string paymentIntendId);
    }
}
