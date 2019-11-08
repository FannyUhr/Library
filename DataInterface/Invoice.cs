using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceID { get; set; }
        public decimal InvoiceSum { get; set; }

        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        public int BorrowID { get; set; }
        public Borrow Borrow { get; set; }
    }
}
