using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration _configuration)
            : base(options)
        {
            configuration = _configuration;
        }

        public IConfiguration configuration { get; }

        public DbSet<Config> Config { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<SelectListData> SelectListData { get; set; }
        public DbSet<StaffDetail> StaffDetail { get; set; }
        public DbSet<StaffMember> StaffMember { get; set; }
        public DbSet<StaffMember> StaffRequest { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Needed to add composite key
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Config>()
                .HasKey(c => new { c.AcademicYear });
            modelBuilder.Entity<EmailSettings>()
                .HasKey(c => new { c.Username });
            modelBuilder.Entity<SelectListData>()
               .HasKey(d => new { d.Code });
            modelBuilder.Entity<StaffMember>()
                .HasKey(c => new { c.StaffRef });

            modelBuilder.Entity<SystemSettings>()
                .HasNoKey();

            //Prevent creating table in EF Migration
            modelBuilder.Entity<Config>(entity => {
                entity.ToView("Config", "dbo");
            });
            modelBuilder.Entity<EmailSettings>(entity => {
                entity.ToView("EmailSettings", "dbo");
            });
            modelBuilder.Entity<News>(entity => {
                entity.ToView("News", "dbo");
            });
            modelBuilder.Entity<SelectListData>(entity => {
                entity.ToView("SelectListData", "dbo");
            });
            modelBuilder.Entity<StaffDetail>(entity => {
                entity.ToView("StaffDetail", "dbo");
            });
            modelBuilder.Entity<StaffMember>(entity => {
                entity.ToView("StaffMember", "dbo");
            });
            modelBuilder.Entity<StaffRequest>(entity => {
                entity.ToView("StaffRequest", "dbo");
            });
            modelBuilder.Entity<SystemSettings>(entity => {
                entity.ToView("SystemSettings", "dbo");
            });
        }

        //Rename migration history table
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsHistoryTable("__STP_EFMigrationsHistory", "dbo"));

        //Rename migration history table
        public DbSet<StaffPortal.Models.StaffRequest> StaffRequest_1 { get; set; }
    }
}
