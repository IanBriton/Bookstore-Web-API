using BookstoreAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            :base(options)
        {
                        
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Book>()
                .HasOne(b=>b.Genre)
                .WithMany(g=>g.Books)
                .HasForeignKey(b=>b.GenreId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
