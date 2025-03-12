using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resort.Domain.Models;

namespace Resort.Application.Utility
{
    
        public static class SD //  sd => static details
        {

                public const string AdminRole = "Admin";
                //public const string EditorRole = "Editor";
                public const string CustomerRole = "Customer";

                // New Status for booking Process
                public const string Pending = "Pending";
                public const string Approved = "Approved";
                public const string CheckedIn = "CheckedIn";
                public const string Completed = "Completed";
                public const string Cancelled = "Cancelled";      
                public const string Refund = "Refund";


        // So Complex but i got it praise to allah
        public static int GetVillaRoomsAvailableCount(int villaId , List<VillaNumber> rooms , List<Booking> bookings,
        DateOnly dateSelected, int nights) 
        {
            var allRoomsVilla = rooms.Where(a => a.VillaId == villaId).Count();
            List<int> bookingInDates = new List<int>();
            int finalTotalAvailalbeRoomsForAllNight = int.MaxValue;

            for (int i = 0; i < nights; i++) 
            {
                var villasRoomsBooked = bookings.Where(a => a.CheckInDate <= dateSelected.AddDays(i) &&
                a.CheckOutDate > dateSelected.AddDays(i) && a.VillaId == villaId);

                foreach (var booking in villasRoomsBooked) 
                {
                    // to prevent adding duplicate ids 
                    if (!bookingInDates.Contains(booking.Id)) 
                        bookingInDates.Add(booking.Id);
                }
                int totalAvailalbeRooms = allRoomsVilla - bookingInDates.Count();

                if (totalAvailalbeRooms == 0)
                {
                    return 0;
                }
                else
                {
                    // so important calcs to get the lowest available rooms for a Villa  
                    if (finalTotalAvailalbeRoomsForAllNight > totalAvailalbeRooms)
                    { finalTotalAvailalbeRoomsForAllNight = totalAvailalbeRooms; }
                }

         
            }
         

            return finalTotalAvailalbeRoomsForAllNight;
        }



        }
    
}
