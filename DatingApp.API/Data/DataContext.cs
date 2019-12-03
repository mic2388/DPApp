using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DatingApp.API.Data
{
    public static class ModelBuilderExtensions 
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }
        }
    }
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
            //uncomment if you want to create database
            // this.Database.EnsureCreated();
        }

        public DbSet<Values> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           // builder.RemovePluralizingTableNameConvention();
            builder.Entity<Like>().HasKey(k=> new {k.LikerId, k.LikeeId});
                
            builder.Entity<Like>()
                .HasOne(u=> u.Likee)
                .WithMany(u=>u.Likers)
                .HasForeignKey(u=>u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u=> u.Liker)
                .WithMany(u=>u.Likees)
                .HasForeignKey(u=>u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.Entity<Message>()
                .HasOne(u=>u.Sender)
                .WithMany(m=>m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u=>u.Recipient)
                .WithMany(m=>m.MessagesRecieved)
                .OnDelete(DeleteBehavior.Restrict);
            }
    }
}