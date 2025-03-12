using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Services.Interface;
using Resort.Application.Utility;
using Resort.Domain.Models;

namespace Resort.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateBooking(Booking booking)
        {
            _unitOfWork.BookingRepository.Add(booking);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Booking> GetAllBooking(string userId = "", string? statusFilterList = null)
        {
            IEnumerable<string> statusList = statusFilterList.ToLower().Split(",");
            if (!string.IsNullOrEmpty(statusFilterList) && !string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.BookingRepository.GetAll(u => statusList.Contains(u.Status.ToLower()) &&
                u.UserId == userId, includes: "User,Villa");
            }
            else
            {
                if (!string.IsNullOrEmpty(statusFilterList))
                {
                    return _unitOfWork.BookingRepository.GetAll(u => statusList.Contains(u.Status.ToLower()), includes: "User,Villa");
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    return _unitOfWork.BookingRepository.GetAll(u => u.UserId == userId, includes: "User,Villa");
                }
            }
            return _unitOfWork.BookingRepository.GetAll(includes: "User, Villa");
        }

        public Booking GetBookingById(int bookingId)
        {
            return _unitOfWork.BookingRepository.GetById(u => u.Id == bookingId, includes: "User, Villa");
        }
        public void UpdateBookingStatus(int bookingId, string bookingStatus, int villaNumber)
        {
            var bookingDb = _unitOfWork.BookingRepository.GetById(a => a.Id == bookingId,track:true);
            if (bookingDb != null)
            {

                bookingDb.Status = bookingStatus;

                if (bookingStatus == SD.CheckedIn)
                {
                    bookingDb.VillaNumber = villaNumber;
                    bookingDb.ActualCheckInDate = DateTime.Now;
                }

                if (bookingStatus == SD.Completed)
                {
                    bookingDb.ActualCheckOutDate = DateTime.Now;
                }
               
            }
            _unitOfWork.SaveChanges();
        }



        public void UpdatePaymentStripeSessionId_PaymentIntendId(int bookingId, string stripeSessionId, string stripePaymentIntendId)
        {
            var bookingDb = _unitOfWork.BookingRepository.GetById(a => a.Id == bookingId, track:true);
            if (bookingDb is not null)
            {
                if (!string.IsNullOrEmpty(stripeSessionId))
                {
                    bookingDb.StripeSessionId = stripeSessionId;
                }
                if (!string.IsNullOrEmpty(stripePaymentIntendId))
                {
                    bookingDb.StripePaymentIntentId = stripePaymentIntendId;
                    bookingDb.PaymentDate = DateTime.Now;
                    bookingDb.IsPaymentSuccessful = true;
                }
                
            }
            _unitOfWork.SaveChanges();
        }

    }
}
