using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationApp.Date.Entities
{
    [Table("SalesDetails")]
    public class SalesDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SalesMaster")]
        public int? MstId { get; set; }



        [ForeignKey("SalesMan")]
        public int? SmId { get; set; }



        [Column(TypeName = "Date")]
        public DateTime? FromDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ToDate { get; set; }

        public int? SalesQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalesValue { get; set; }

 

    }
}