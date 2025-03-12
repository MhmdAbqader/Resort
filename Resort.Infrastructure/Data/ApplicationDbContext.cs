using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Resort.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Infrastructure.Data
{
    public class ApplicationDbContext :  IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }   
        public DbSet<Villa> Villas {  get; set; }
        public DbSet<VillaNumber>  VillaNumbers {  get; set; }
        public DbSet<Amenity> Amenities {  get; set; }
        public DbSet<ApplicationUser> ApplicationUser {  get; set; }
        public DbSet<Booking> Bookings {  get; set; }
        public DbSet<Owner>  Owners {  get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<VillaNumber>()
           .HasOne(o => o.Villa) // Order has one Customer
           .WithMany() // Customer can have many Orders (if you have a reverse navigation)
           .HasForeignKey(o => o.VillaId);

            modelBuilder.Entity<Villa>()
                .HasOne(c => c.Owner) // Customer has one Address
                .WithMany() // Customer can have many Orders (if you have a reverse navigation)
                .HasForeignKey(o => o.OwnerId); // Define the foreign key


            modelBuilder.Entity<Owner>().HasData(
               new Owner
               {
                   Id = 1,
                   OwnerName = "mhmd"
               },
               new Owner
               {
                   Id = 2,
                   OwnerName = "bkr"
               },
                new Owner
                {
                    Id = 3,
                    OwnerName = "ahmed"
                }
               );


            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Normal Villa",
                    Description = "No more prons for that Villa",
                    Price = 1000,
                    Occupancy = 4,
                    SquareFeet = 500,
                    ImgURL= "https://placehold.co/600X400",
                    OwnerId = 1,                   
                },
                new Villa
                {
                    Id = 2,
                    Name = "Royal Villa",
                    Description = " more advantages for that Villa",
                    Price = 2000,
                    Occupancy = 6,
                    SquareFeet = 600,
                    ImgURL = "https://placehold.co/600X401",
                    OwnerId = 1,
                },
                new Villa
                {
                    Id = 3,
                    Name = "Luxury Villa",
                    Description = "The Best Villa at all",
                    Price = 3000,
                    Occupancy = 4,
                    SquareFeet = 900,
                    ImgURL = "https://placehold.co/600X402",
                    OwnerId = 2,
                }
            );

            modelBuilder.Entity<VillaNumber>().HasData(
                new VillaNumber { Villa_Number =101,VillaId=1},
                new VillaNumber { Villa_Number =102,VillaId=1},
                new VillaNumber { Villa_Number =103,VillaId=1},
                new VillaNumber { Villa_Number =201,VillaId=2},
                new VillaNumber { Villa_Number =202,VillaId=2},
                new VillaNumber { Villa_Number =203,VillaId=2},
                new VillaNumber { Villa_Number =301,VillaId=3},
                new VillaNumber { Villa_Number =302,VillaId=3},
                new VillaNumber { Villa_Number =303,VillaId=3} 
                );
            modelBuilder.Entity<Amenity>().HasData(
                    new Amenity
                    {
                        Id = 1,
                        VillaId = 1,
                        Name = "Private Pool"
                    }, new Amenity
                    {
                        Id = 2,
                        VillaId = 1,
                        Name = "Microwave"
                    }, new Amenity
                    {
                        Id = 3,
                        VillaId = 1,
                        Name = "Private Balcony"
                    }, new Amenity
                    {
                        Id = 4,
                        VillaId = 1,
                        Name = "1 king bed and 1 sofa bed"
                    },

                    new Amenity
                    {
                        Id = 5,
                        VillaId = 2,
                        Name = "Private Plunge Pool"
                    }, new Amenity
                    {
                        Id = 6,
                        VillaId = 2,
                        Name = "Microwave and Mini Refrigerator"
                    }, new Amenity
                    {
                        Id = 7,
                        VillaId = 2,
                        Name = "Private Balcony"
                    }, new Amenity
                    {
                        Id = 8,
                        VillaId = 2,
                        Name = "king bed or 2 double beds"
                    },

                    new Amenity
                    {
                        Id = 9,
                        VillaId = 3,
                        Name = "Private Pool"
                    }, new Amenity
                    {
                        Id = 10,
                        VillaId = 3,
                        Name = "Jacuzzi"
                    }, new Amenity
                    {
                        Id = 11,
                        VillaId = 3,
                        Name = "Private Balcony"
                    });
            /////// owners

        


        }
    }
}