﻿// <auto-generated />
using System;
using ChemicalProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChemicalProject.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240722081609_AddName")]
    partial class AddName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("CC_Schema")
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChemicalProject.Models.ActualRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Badge")
                        .HasColumnType("int");

                    b.Property<int>("ChemicalId")
                        .HasColumnType("int");

                    b.Property<int>("CurrentStock")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChemicalId");

                    b.ToTable("ActualRecords", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Areas", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.Chemical_FALab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ApprovalDateESH")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ApprovalDateManager")
                        .HasColumnType("datetime2");

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<int>("Badge")
                        .HasColumnType("int");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChemicalName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CostCentre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Justify")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MinimumStock")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Packaging")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("RemarkESH")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RemarkManager")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("StatusESH")
                        .HasColumnType("bit");

                    b.Property<bool?>("StatusManager")
                        .HasColumnType("bit");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Chemicals", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.Records_FALab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Badge")
                        .HasColumnType("int");

                    b.Property<int>("ChemicalId")
                        .HasColumnType("int");

                    b.Property<int>("Consumption")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Justify")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReceivedQuantity")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RecordDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("WasteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChemicalId");

                    b.HasIndex("WasteId");

                    b.ToTable("Records", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserAdmin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("UserAdmins", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserArea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("EmailManager")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("UserAreas", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("UserManagers", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserSuperVisor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("UserSuperVisors", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.Waste_FALab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Badge")
                        .HasColumnType("int");

                    b.Property<DateTime?>("WasteDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("WasteQuantity")
                        .HasColumnType("int");

                    b.Property<string>("WasteType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Wastes", "CC_Schema");
                });

            modelBuilder.Entity("ChemicalProject.Models.ActualRecord", b =>
                {
                    b.HasOne("ChemicalProject.Models.Chemical_FALab", "Chemical_FALab")
                        .WithMany("ActualRecords")
                        .HasForeignKey("ChemicalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chemical_FALab");
                });

            modelBuilder.Entity("ChemicalProject.Models.Chemical_FALab", b =>
                {
                    b.HasOne("ChemicalProject.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ChemicalProject.Models.Records_FALab", b =>
                {
                    b.HasOne("ChemicalProject.Models.Chemical_FALab", "Chemical_FALab")
                        .WithMany()
                        .HasForeignKey("ChemicalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChemicalProject.Models.Waste_FALab", "Waste")
                        .WithMany()
                        .HasForeignKey("WasteId");

                    b.Navigation("Chemical_FALab");

                    b.Navigation("Waste");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserAdmin", b =>
                {
                    b.HasOne("ChemicalProject.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId");

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserArea", b =>
                {
                    b.HasOne("ChemicalProject.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserManager", b =>
                {
                    b.HasOne("ChemicalProject.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ChemicalProject.Models.UserSuperVisor", b =>
                {
                    b.HasOne("ChemicalProject.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ChemicalProject.Models.Chemical_FALab", b =>
                {
                    b.Navigation("ActualRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
