using System.Text.Json;
using Atletika_Denik_API.Data.ViewModels;
using Atletika_Denik_API.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Atletika_Denik_API.Data.Models
{
    public class TrainingContext : DbContext
    {
        Errors error = new Errors();
        
        public DbSet<ViewModels.Training> Training { get; set; }
        public DbSet<ViewModels.Trenink> Trenink { get; set; }
        public DbSet<ViewModels.Trenink_user_response> Trenink_user_response { get; set; }
        public DbSet<ViewModels.Training_data> Training_data { get; set; }
        public DbSet<ViewModels.User> User { get; set; }
        public DbSet<ViewModels.Users> Users { get; set; }
        public DbSet<ViewModels.Asociace_Trener_Uzivatel> Asociace_Trener_Uzivatel { get; set; }
        public DbSet<ViewModels.Asociace_Treninku> Asociace_Treninku { get; set; }



        public DbSet<ViewModels.Training_Association> Training_Association { get; set; }
        public DbSet<ViewModels.Training_Definition> Training_Definition { get; set; }
        public DbSet<ViewModels.Training_User_Response> Training_User_Response { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString1"]);
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
            modelBuilder.Entity<ViewModels.Training_Definition>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Training_User_Response>().HasKey(k => k.id);
        }
    }
}