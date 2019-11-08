using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataInterface
{
    public class Aisle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AisleID { get; set; }
        public int AisleNumber { get; set; }
        
        public ICollection<Shelf> Shelfs { get; set; }
    }
}
