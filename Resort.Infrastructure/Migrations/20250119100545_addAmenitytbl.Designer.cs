﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Resort.Infrastructure.Data;

#nullable disable

namespace Resort.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250119100545_addAmenitytbl")]
    partial class addAmenitytbl
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Resort.Domain.Models.Amenity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VillaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VillaId");

                    b.ToTable("Amenities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Private Pool",
                            VillaId = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "Microwave",
                            VillaId = 1
                        },
                        new
                        {
                            Id = 3,
                            Name = "Private Balcony",
                            VillaId = 1
                        },
                        new
                        {
                            Id = 4,
                            Name = "1 king bed and 1 sofa bed",
                            VillaId = 1
                        },
                        new
                        {
                            Id = 5,
                            Name = "Private Plunge Pool",
                            VillaId = 2
                        },
                        new
                        {
                            Id = 6,
                            Name = "Microwave and Mini Refrigerator",
                            VillaId = 2
                        },
                        new
                        {
                            Id = 7,
                            Name = "Private Balcony",
                            VillaId = 2
                        },
                        new
                        {
                            Id = 8,
                            Name = "king bed or 2 double beds",
                            VillaId = 2
                        },
                        new
                        {
                            Id = 9,
                            Name = "Private Pool",
                            VillaId = 3
                        },
                        new
                        {
                            Id = 10,
                            Name = "Jacuzzi",
                            VillaId = 3
                        },
                        new
                        {
                            Id = 11,
                            Name = "Private Balcony",
                            VillaId = 3
                        });
                });

            modelBuilder.Entity("Resort.Domain.Models.Villa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Occupancy")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("SquareFeet")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Villas");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "No more prons for that Villa",
                            ImgURL = "https://placehold.co/600X400",
                            Name = "Normal Villa",
                            Occupancy = 4,
                            Price = 1000.0,
                            SquareFeet = 500
                        },
                        new
                        {
                            Id = 2,
                            Description = " more advantages for that Villa",
                            ImgURL = "https://placehold.co/600X401",
                            Name = "Royal Villa",
                            Occupancy = 6,
                            Price = 2000.0,
                            SquareFeet = 600
                        },
                        new
                        {
                            Id = 3,
                            Description = "The Best Villa at all",
                            ImgURL = "https://placehold.co/600X402",
                            Name = "Luxury Villa",
                            Occupancy = 4,
                            Price = 3000.0,
                            SquareFeet = 900
                        });
                });

            modelBuilder.Entity("Resort.Domain.Models.VillaNumber", b =>
                {
                    b.Property<int>("Villa_Number")
                        .HasColumnType("int");

                    b.Property<string>("SpecialDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VillaId")
                        .HasColumnType("int");

                    b.HasKey("Villa_Number");

                    b.HasIndex("VillaId");

                    b.ToTable("VillaNumbers");

                    b.HasData(
                        new
                        {
                            Villa_Number = 101,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 102,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 103,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 201,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 202,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 203,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 301,
                            VillaId = 3
                        },
                        new
                        {
                            Villa_Number = 302,
                            VillaId = 3
                        },
                        new
                        {
                            Villa_Number = 303,
                            VillaId = 3
                        });
                });

            modelBuilder.Entity("Resort.Domain.Models.Amenity", b =>
                {
                    b.HasOne("Resort.Domain.Models.Villa", "Villa")
                        .WithMany()
                        .HasForeignKey("VillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Villa");
                });

            modelBuilder.Entity("Resort.Domain.Models.VillaNumber", b =>
                {
                    b.HasOne("Resort.Domain.Models.Villa", "Villa")
                        .WithMany()
                        .HasForeignKey("VillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Villa");
                });
#pragma warning restore 612, 618
        }
    }
}
