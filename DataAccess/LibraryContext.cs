using DataInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class LibraryContext : DbContext 
    {
        private const string connectionString = "Server=DESKTOP-AIMLK4Q;Database=Library;Trusted_Connection=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            
            var key = modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetForeignKeys().Where(
                    fk => fk.DeclaringEntityType.ClrType.Name == "Borrow" &&
                        fk.DependentToPrincipal.ClrType.Name == "Book")).First();

            key.DeleteBehavior = DeleteBehavior.SetNull;
        }

        

        public DbSet<Aisle> Aisles { get; set; }
        public DbSet<Shelf> Shelfs { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<HistoryBorrowList> HistoryBorrowLists { get; set; }        
        public DbSet<Discard> Discards { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
    }
}
