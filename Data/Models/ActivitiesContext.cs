using System.Text.Json;
using Atletika_Denik_API.Data.ViewModels;
using Atletika_Denik_API.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Atletika_Denik_API.Data.Models
{
    public class ActivitiesContext : DbContext
    {
        Errors error = new Errors();
        
        public DbSet<ViewModels.Tag> Tag { get; set; }
        public DbSet<ViewModels.Tag_Association> Tag_Association { get; set; }

        public DbSet<ViewModels.Tag_Details> Tag_Details { get; set; }

        public DbSet<ViewModels.Tag_User_Settings> Tag_User_Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString3"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewModels.Training>().HasNoKey(); // HasKey(c => new { c.User_Id, c.Date});
            modelBuilder.Entity<ViewModels.Trenink>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_data>().HasNoKey();
            modelBuilder.Entity<ViewModels.User>().HasNoKey();
            modelBuilder.Entity<ViewModels.Asociace_Treninku>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Trenink_user_response>().HasKey(k => k.id);

            modelBuilder.Entity<ViewModels.Training_Association>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_Association>().Property(p => p.id).ValueGeneratedNever();
            modelBuilder.Entity<ViewModels.Training_Definition>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_Definition>().Property(p => p.id).ValueGeneratedNever();
            modelBuilder.Entity<ViewModels.Training_Definition>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_Definition>().Property(p => p.id).ValueGeneratedNever();
            modelBuilder.Entity<ViewModels.Training_User_Response>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_User_Response>().Property(p => p.id).ValueGeneratedNever();
        }
    }
}