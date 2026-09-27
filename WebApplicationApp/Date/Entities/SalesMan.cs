using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationApp.Date.Entities
{
    [Table("SalesMans")]
    public class SalesMan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string SmCode { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public bool? IsActive { get; set; }

    }
}