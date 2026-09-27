using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationApp.Date.Entities
{
    [Table("SalesMasters")]
    public class SalesMaster
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? EntryDate { get; set; }

        [StringLength(30)]
        public string Months { get; set; }

        [StringLength(150)]
        public string? Remarks { get; set; }

        
    }
}