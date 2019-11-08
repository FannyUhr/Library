using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookID { get; set; }
        public string ISBNNumber { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int PurchaseYear { get; set; }        
        public int Price { get; set; }
        public int Condition { get; set; }
        public bool IsBorrowed { get; set; } 

        public int ShelfID { get; set; }
        public Shelf Shelf { get; set; }

        
        public ICollection<Borrow> Borrows { get; set; }
        
        public ICollection<Customer> Customers { get; set; }
        
        public ICollection<Discard> Discards { get; set; }
    }
}
