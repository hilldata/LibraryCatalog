﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XRD.LibCat.Models;

namespace XRD.LibCat.Migrations
{
    [DbContext(typeof(LibraryContext))]
    partial class LibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("XRD.LibCat.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CatId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(400);

                    b.Property<int>("OrdIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Role")
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("CatId", "FullName")
                        .IsUnique();

                    b.HasIndex("CatId", "OrdIndex");

                    b.ToTable("tblAuthors");
                });

            modelBuilder.Entity("XRD.LibCat.Models.BorrowingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CheckInDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("date");

                    b.Property<int>("PatronId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("PatronId");

                    b.HasIndex("BookNumber", "PatronId", "DueDate")
                        .IsUnique();

                    b.ToTable("tblBorrowingHx");
                });

            modelBuilder.Entity("XRD.LibCat.Models.CatalogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ec")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MaxAge")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MaxGrade")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MinAge")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MinGrade")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PageCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PubDate")
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<string>("Publisher")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<string>("ShelfLocation")
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<string>("Subtitle")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("Ts")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Ts");

                    b.ToTable("tblCatalog");
                });

            modelBuilder.Entity("XRD.LibCat.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CatId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("CatId", "Value")
                        .IsUnique();

                    b.ToTable("tblGenres");
                });

            modelBuilder.Entity("XRD.LibCat.Models.Identifier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CatId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("CatId", "Value")
                        .IsUnique();

                    b.ToTable("tblIdentifiers");
                });

            modelBuilder.Entity("XRD.LibCat.Models.OwnedBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CatId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("CatId");

                    b.HasIndex("IsDeleted");

                    b.ToTable("tblOwnedBooks");
                });

            modelBuilder.Entity("XRD.LibCat.Models.Patron", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ec")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<string>("First")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<long>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Last")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int?>("MaxAge")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MaxGrade")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Middle")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int?>("MinAge")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MinGrade")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Prefix")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Suffix")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int?>("TeacherId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Ts")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("TeacherId");

                    b.HasIndex("Ts");

                    b.ToTable("tblPatrons");
                });

            modelBuilder.Entity("XRD.LibCat.Models.StaffMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ec")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<string>("First")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<long>("GradesTaught")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Last")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Middle")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Prefix")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Room")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Subjects")
                        .HasColumnType("TEXT");

                    b.Property<string>("Suffix")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Ts")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Uid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Ts");

                    b.ToTable("tblStaffMembers");
                });

            modelBuilder.Entity("XRD.LibCat.Models.Author", b =>
                {
                    b.HasOne("XRD.LibCat.Models.CatalogEntry", "Book")
                        .WithMany("Authors")
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("XRD.LibCat.Models.BorrowingHistory", b =>
                {
                    b.HasOne("XRD.LibCat.Models.OwnedBook", "Book")
                        .WithMany("BorrowingHistories")
                        .HasForeignKey("BookNumber")
                        .HasPrincipalKey("BookNumber")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("XRD.LibCat.Models.Patron", "Patron")
                        .WithMany("BorrowingHistories")
                        .HasForeignKey("PatronId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("XRD.LibCat.Models.Genre", b =>
                {
                    b.HasOne("XRD.LibCat.Models.CatalogEntry", "Book")
                        .WithMany("Genres")
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XRD.LibCat.Models.Identifier", b =>
                {
                    b.HasOne("XRD.LibCat.Models.CatalogEntry", "Book")
                        .WithMany("Identifiers")
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XRD.LibCat.Models.OwnedBook", b =>
                {
                    b.HasOne("XRD.LibCat.Models.CatalogEntry", "Book")
                        .WithMany("OwnedBooks")
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("XRD.LibCat.Models.Patron", b =>
                {
                    b.HasOne("XRD.LibCat.Models.StaffMember", "Teacher")
                        .WithMany("Students")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
