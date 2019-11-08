using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class HistoryBorrowList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryBorrowListID { get; set; }
        public int CustomerNumber { get; set; }
        public string BookTitle { get; set; }
        public DateTime BorrowDate { get; set; }
    }
}
