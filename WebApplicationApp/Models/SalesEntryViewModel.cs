using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplicationApp.Date.Entities;

namespace WebApplicationApp.Models
{
    public class SalesEntryViewModel
    {

        public int Id { get; set; }


        /// -------------

        #region SalesMan Info
        public string Name { get; set; }
        public string SmCode { get; set; }

        public string Phone { get; set; }

        public bool? IsActive { get; set; }

        #endregion

        /// -------------
        /// 

        #region SalesMaster Info
        public DateTime? EntryDate { get; set; }
        public string Months { get; set; }
        public string Remarks { get; set; }

        #endregion

        /// -------------

        #region SalesDetail Info
        public int? MstId { get; set; }

        public int? SmId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? SalesQuantity { get; set; }
        public decimal? SalesValue { get; set; }

        #endregion

        /// -------------
        public List<SalesMan> salesMansList { get; set; }
        public List<SalesMaster> salesMastersList { get; set; }
        public List<SalesDetail> salesDetailsList { get; set; }



    }



    // DTO for Report

    public class SalesReportDto
    {
        public string SalesManName { get; set; }
        public decimal? SalesQuantity { get; set; }
        public decimal? SalesValue { get; set; }
    }



    public class SalesReportPrintModel
    {
        public List<SalesReportDto> Items { get; set; } = new List<SalesReportDto>();
        public string? CriteriaText { get; set; }
        public string? PrintDate { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? TotalValue { get; set; }
        public string? AmountInWordsEnglish { get; set; }
        public string? AmountInWordsBangla { get; set; }
    }


}