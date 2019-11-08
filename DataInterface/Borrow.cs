using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Borrow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowID { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        public int BookID { get; set; }
        public Book Book { get; set; }
        
        
        public ICollection<Invoice> Invoices { get; set; }
    }
}
