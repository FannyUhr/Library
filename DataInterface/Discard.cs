using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Discard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscardID { get; set; }

        public int BookID { get; set; }
        public Book Book { get; set; }
        
        public int ShelfID { get; set; }
        public Shelf Shelf { get; set; }

    }
}
