using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityEmail.Context
{
    public class EmailContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;initial catalog=IdentityEmailDb;integrated security=true; trust server certificate=true;");
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMessageBox> UserMessageBoxes { get; set; }
        public DbSet<MessageCategory> MessageCategory { get; set; }
        public DbSet<SpamSender> SpamSenders { get; set; }
        public DbSet<UserContactCategory> UserContactCategories { get; set; }
    }
}
