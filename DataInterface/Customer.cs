using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }
        public int CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string BirthDate { get; set; }
        public string Address { get; set; }
        public decimal Debt { get; set; }
        public Customer Guardian { get; set; }
        public bool IsGuardian { get; set; }
       
        public ICollection<Borrow> Borrows { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
