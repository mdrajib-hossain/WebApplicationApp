using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Rotativa.AspNetCore;
using WebApplicationApp.Date.Entities;
using WebApplicationApp.Models;
using WebApplicationApp.Repositories.SelesManService;
using WebApplicationApp.ReturnResponse;

namespace WebApplicationApp.Controllers
{

    [Authorize]

    public class SalesMansController : Controller
    {
        private readonly ISalesManServices _selesservices;


        public SalesMansController( ISalesManServices salesManServices)
        {
            _selesservices = salesManServices;

            
        }
       

        #region SalesMan 


        // GET: SalesMans/SalesManEntry
        [HttpGet]
        public async Task<IActionResult> SalesManEntry(int Id = 0)
        {
            // 1. Always fetch all salesmen for the table list section
            var allSalesMen = await _selesservices.GetAllSalesManInfoAsync();

            SalesManInfoModel viewModel;

            // 2. Fetch single record if editing, otherwise initialize empty form
            if (Id == 0)
            {
                viewModel = new SalesManInfoModel();
            }
            else
            {
                viewModel = await _selesservices.GetAllSalesManInfoAsyncById(Id);
                if (viewModel == null)
                {
                    viewModel = new SalesManInfoModel();
                }
            }

            // 3. Attach the list to the view model so the table can render it
            viewModel.salesManInfoModel = allSalesMen;

            return View(viewModel);
        }

        // POST: SalesMans/CreateSalesMan
        [HttpPost]
        public async Task<IActionResult> CreateSalesMan(SalesManInfoModel model)
        {
            var result = await _selesservices.CreateSalesManInfo(model);
            return Json(result);
        }

        // GET: SalesMans/DeleteSalesMan
        [HttpGet]
        public async Task<IActionResult> DeleteSalesMan(int Id)
        {
            var response = await _selesservices.DeleteSalesMan(Id);
            TempData["DeleteMessage"] = response.Message;
            return RedirectToAction(nameof(SalesManEntry));
        }


        #endregion



        #region SelesInfo
           
        
        [HttpGet]
        public async Task<IActionResult> SelesEntry(int Id = 0)
        {
            var modeldate = new SalesEntryViewModel();

            if (Id == 0)
            {
                modeldate.salesMansList = await _selesservices.GetAllSalesMansAsync(); ;
                modeldate.salesMastersList = await _selesservices.GetAllSalesMastersAsync();
                modeldate.salesDetailsList = new List<SalesDetail>();

            }
            if (Id > 0)
            {
                modeldate = await _selesservices.GetSalesEntryByIdAsync(Id);
            }


            return View(modeldate);
        }



        [HttpPost]
        public async Task<IActionResult> CreateSalesEntry(SalesEntryViewModel model)
        {



            var result = await _selesservices.CreateSalesEntryInfo(model) ;
            return Json(result);
        }


        // GET: SalesMans/DeleteSalesMstDtl
        [HttpGet]
        public async Task<IActionResult> DeleteSalesMstDtl(int Id)
        {
            var response = await _selesservices.DeleteSalesEntry(Id);
            TempData["DeleteMessage"] = response.Message;
            return RedirectToAction(nameof(SelesEntry));
        }

        #endregion

        #region SelesReports


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> SelesReports(int Id = 0)
        {
            var modeldate = new SalesEntryViewModel();

            if (Id == 0)
            {
                modeldate.salesMansList = await _selesservices.GetAllSalesMansAsync(); ;
                modeldate.salesMastersList = await _selesservices.GetAllSalesMastersAsync();
                modeldate.salesDetailsList = new List<SalesDetail>();

            }
           

            return View(modeldate);
        }


        [HttpGet]
        public async Task<IActionResult> GetSalesReportData(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            var reportData = await _selesservices.GetReportDataAsync(smId, month, fromDate, toDate);
            return Json(reportData);
        }



        [HttpGet]
        public async Task<IActionResult> GenerateITextReport(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            byte[] pdfBytes = await _selesservices.GenerateSalesPdfReportAsync(smId, month, fromDate, toDate);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return BadRequest("Failed to generate report or no data found for the selected criteria.");
            }

            // Return as a downloadable/viewable PDF file
            return File(pdfBytes, "application/pdf", "SalesReport.pdf");
        }



        //[HttpGet]
        //public async Task<IActionResult> GenerateRdlcReport(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        //{
        //    // Generate the PDF byte array using the RDLC engine
        //    byte[] pdfBytes = await _selesservices.GenerateSalesRdlcReportAsync(smId, month, fromDate, toDate);

        //    if (pdfBytes == null || pdfBytes.Length == 0)
        //    {
        //        return BadRequest("Failed to generate report or no data found for the selected criteria.");
        //    }

        //    // Return as an inline viewable PDF
        //    return File(pdfBytes, "application/pdf", "SalesRdlcReport.pdf");
        //}


        //[HttpGet]
        //public async Task<IActionResult> DownloadRdlcReport(int? smId, string month, DateTime? fromDate, DateTime? toDate, string format)
        //{
        //    byte[] fileBytes = await _selesservices.GenerateSalesRdlcReportAsync(smId, month, fromDate, toDate, format);

        //    if (fileBytes == null || fileBytes.Length == 0)
        //    {
        //        return BadRequest("No data found.");
        //    }

        //    if (format.ToUpper() == "EXCEL")
        //    {
        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
        //    }
        //    else
        //    {
        //        return File(fileBytes, "application/pdf");
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> DownloadRdlcReport(int? smId, string month, DateTime? fromDate, DateTime? toDate, string format, bool isDownload = false)
        {
            byte[] fileBytes = await _selesservices.GenerateSalesRdlcReportAsync(smId, month, fromDate, toDate, format);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return BadRequest("No data found.");
            }

            if (format.ToUpper() == "EXCEL")
            {
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
            }
            else
            {
                if (isDownload)
                {
                    // Forces the browser to download the file (used for the download button)
                    return File(fileBytes, "application/pdf", "SalesReport.pdf");
                }
                else
                {
                    // Displays the file inline (used for the iframe preview)
                    return File(fileBytes, "application/pdf");
                }
            }
        }





        //[HttpGet]
        //public async Task<IActionResult> GenerateHtmlReport(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        //{
        //    var model = await _selesservices.GetSalesReportHtmlAsync(smId, month, fromDate, toDate);

        //    if (model == null || model.Items == null || !model.Items.Any())
        //    {
        //        return Content("<h4 style='text-align:center;margin-top:40px;font-family:sans-serif;'>No data found for the selected criteria.</h4>", "text/html");
        //    }

        //    return View("SalesReportHtml", model);


        //}



        [HttpGet]
        public async Task<IActionResult> GenerateHtmlReport(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            var model = await _selesservices.GetSalesReportHtmlAsync(smId, month, fromDate, toDate);

            if (model == null || model.Items == null || !model.Items.Any())
            {
                return Content("<h4 style='text-align:center;margin-top:40px;font-family:sans-serif;'>No data found for the selected criteria.</h4>", "text/html");
            }

            return new ViewAsPdf("SalesReportHtml", model)
            {
                FileName = "SalesReportHtml.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(15, 15, 15, 15)
            };
        }

        #endregion

    }
}