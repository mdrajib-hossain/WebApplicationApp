using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.Reporting.NETCore;
using System.ComponentModel.DataAnnotations;
//using System.Data.SqlClient;
using System.IO;
using System.Reflection.Metadata;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using WebApplicationApp.Date;
using WebApplicationApp.Date.Entities;
using WebApplicationApp.Models;
using WebApplicationApp.ReturnResponse;



namespace WebApplicationApp.Repositories.SelesManService
{
    public class SalesManServices : ISalesManServices
    {
        private readonly SqlDbContext _context;


        public SalesManServices(SqlDbContext context)
        {
            _context = context;
        }



        #region SalesManInfo

        public async Task<List<SalesManInfoModel>> GetAllSalesManInfoAsync()
        {
            var result = await _context.SalesMans
                .Select(salesMan => new SalesManInfoModel
                {
                    Id = salesMan.Id,
                    Name = salesMan.Name,
                    SmCode = salesMan.SmCode,
                    Phone = salesMan.Phone,
                    IsActive = salesMan.IsActive
                })
                .ToListAsync();

            return result;
        }



        public async Task<List<SalesMan>> GetAllSalesMansAsync()
        {
            return await _context.SalesMans.Where(sm => sm.IsActive != false).ToListAsync() ?? new List<SalesMan>();
        }

        public async Task<SalesManInfoModel> GetAllSalesManInfoAsyncById(int Id)
        {
            //var salesManInfo = await _context.SalesMans.FindAsync(Id);
            var salesManInfo = await _context.SalesMans.FindAsync(Id);

            return new SalesManInfoModel
            {
                Id = salesManInfo.Id,
                Name = salesManInfo.Name,
                SmCode = salesManInfo.SmCode,
                Phone = salesManInfo.Phone,
                IsActive = salesManInfo.IsActive
            };



        }

        public async Task<ResponseModel> CreateSalesManInfo(SalesManInfoModel salesManInfoModel)
        {

            if (salesManInfoModel == null)
            {
                return new ResponseModel { Id = 0, Message = "Invalid input" };
            }

            try
            {
                if (salesManInfoModel.Id > 0)
                {
                    // UPDATE PROCESS
                    // 1. Retrieve the existing entity from the database

                    var existingSalesMan = await _context.SalesMans.FindAsync(salesManInfoModel.Id);

                    if (existingSalesMan == null)
                    {
                        return new ResponseModel { Id = 0, Message = "SalesMan not found." };
                    }

                    // 2. Map the new values to the existing entity
                    existingSalesMan.Name = salesManInfoModel.Name;
                    existingSalesMan.SmCode = salesManInfoModel.SmCode;
                    existingSalesMan.Phone = salesManInfoModel.Phone;
                    existingSalesMan.IsActive = salesManInfoModel.IsActive;



                    await _context.SaveChangesAsync();

                    return new ResponseModel
                    {
                        Id = existingSalesMan.Id,
                        Message = "SalesMan updated successfully!"
                    };
                }
                else
                {
                    // INSERT PROCESS
                    var newSalesMan = new SalesMan
                    {
                        Name = salesManInfoModel.Name,
                        SmCode = salesManInfoModel.SmCode,
                        Phone = salesManInfoModel.Phone,
                        IsActive = salesManInfoModel.IsActive
                    };

                    await _context.SalesMans.AddAsync(newSalesMan);
                    await _context.SaveChangesAsync();

                    return new ResponseModel
                    {
                        Id = newSalesMan.Id,
                        Message = "SalesMan saved successfully!"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logger configured (e.g., _logger.LogError(ex, ex.Message))
                return new ResponseModel
                {
                    Id = 0,
                    Message = "An error occurred while saving: " + ex.Message
                };
            }
        }



        public async Task<ResponseModel> DeleteSalesMan(int id)
        {
            try
            {

                var salesMan = await _context.SalesMans.FindAsync(id);

                if (salesMan == null)
                {
                    return new ResponseModel { Id = 0, Message = "SalesMan not found." };
                }

                _context.SalesMans.Remove(salesMan);

                await _context.SaveChangesAsync();

                return new ResponseModel
                {
                    Id = id,
                    Message = "SalesMan deleted successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Id = 0,
                    Message = "An error occurred while deleting: " + ex.Message
                };
            }
        }




        #endregion


        #region SalesMasterInfo

        public async Task<List<SalesMaster>> GetAllSalesMastersAsync()
        {

            return await _context.SalesMasters.ToListAsync() ?? new List<SalesMaster>();

        }
        #endregion


        #region SalesDetailInfo

        public async Task<List<SalesDetail>> GetAllSalesDetailsAsync()
        {

            return await _context.SalesDetails.ToListAsync() ?? new List<SalesDetail>();

        }
        #endregion


        #region SaleEntry
        public async Task<ResponseModel> CreateSalesEntryInfo(SalesEntryViewModel salesEntryPostModel)
        {
            try
            {
                if (salesEntryPostModel == null)
                {
                    return new ResponseModel { Id = 0, Message = "Invalid input" };
                }

                // ==========================================
                // UPDATE EXISTING SALES ENTRY
                // ==========================================
                if (salesEntryPostModel.Id > 0)
                {
                    var existingMaster = await _context.SalesMasters.FindAsync(salesEntryPostModel.Id);
                    if (existingMaster == null)
                    {
                        return new ResponseModel { Id = 0, Message = "Sales entry not found for update." };
                    }

                    existingMaster.EntryDate = salesEntryPostModel.EntryDate;
                    existingMaster.Months = salesEntryPostModel.Months;
                    existingMaster.Remarks = salesEntryPostModel.Remarks;
                    _context.SalesMasters.Update(existingMaster);

                  
                    var existingDetails = await _context.SalesDetails
                        .Where(x => x.MstId == salesEntryPostModel.Id)
                        .ToListAsync();

                    if (existingDetails.Any())
                    {
                        _context.SalesDetails.RemoveRange(existingDetails);
                    }

                
                    if (salesEntryPostModel.salesDetailsList != null && salesEntryPostModel.salesDetailsList.Count > 0)
                    {
                        foreach (var detail in salesEntryPostModel.salesDetailsList)
                        {
                            var saleDtl = new SalesDetail
                            {
                                MstId = salesEntryPostModel.Id,
                                SmId = detail.SmId,
                                FromDate = detail.FromDate,
                                ToDate = detail.ToDate,
                                SalesQuantity = detail.SalesQuantity,
                                SalesValue = detail.SalesValue
                            };
                            await _context.SalesDetails.AddAsync(saleDtl);
                        }
                    }

                    await _context.SaveChangesAsync();

                    return new ResponseModel
                    {
                        Id = existingMaster.Id,
                        Message = "SalesEntry updated successfully!"
                    };
                }
                // ==========================================
                // CREATE NEW SALES ENTRY
                // ==========================================
                else
                {
                    var salesMaster = new SalesMaster
                    {
                        EntryDate = salesEntryPostModel.EntryDate,
                        Months = salesEntryPostModel.Months,
                        Remarks = salesEntryPostModel.Remarks
                    };

                    await _context.SalesMasters.AddAsync(salesMaster);
                    await _context.SaveChangesAsync(); 

                    var mstId = salesMaster.Id;

                    if (salesEntryPostModel.salesDetailsList != null && salesEntryPostModel.salesDetailsList.Count > 0)
                    {
                        foreach (var detail in salesEntryPostModel.salesDetailsList)
                        {
                            var saleDtl = new SalesDetail
                            {
                                MstId = mstId,
                                SmId = detail.SmId,
                                FromDate = detail.FromDate,
                                ToDate = detail.ToDate,
                                SalesQuantity = detail.SalesQuantity,
                                SalesValue = detail.SalesValue
                            };

                            await _context.SalesDetails.AddAsync(saleDtl);
                        }

                        await _context.SaveChangesAsync(); 
                    }

                    return new ResponseModel
                    {
                        Id = mstId,
                        Message = "SalesEntry info created successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Id = 0,
                    Message = "An error occurred while processing the request: " + ex.Message
                };
            }
        }


        public async Task<SalesEntryViewModel> GetSalesEntryByIdAsync(int MstId)
        {
            // 1. Fetch the master record
            var salesMaster = await _context.SalesMasters.FindAsync(MstId) ?? new SalesMaster();

            // 2. Fetch details belonging to this specific master ID
            var salesDetails = await _context.SalesDetails
                .Where(sd => sd.MstId == MstId)
                .ToListAsync();

            // 3. FIX: Fetch ALL salesmen (active and inactive) so historical names never show as "Unknown"
            var salesMansList = await _context.SalesMans.ToListAsync();

            // 4. Fetch all master records for the bottom table
            var salesMastersList = await GetAllSalesMastersAsync();

            // 5. Map everything to the ViewModel
            var salesEntryViewModel = new SalesEntryViewModel
            {
                Id = salesMaster.Id,
                EntryDate = salesMaster.EntryDate,
                Months = salesMaster.Months,
                Remarks = salesMaster.Remarks,
                salesMansList = salesMansList,
                salesMastersList = salesMastersList,
                salesDetailsList = salesDetails
            };

            return salesEntryViewModel;
        }


        public async Task<ResponseModel> DeleteSalesEntry(int MstId)
        {
           

            try
            {
                var salesMaster = await _context.SalesMasters.FindAsync(MstId);
                if (salesMaster == null)
                {
                    return new ResponseModel { Id = 0, Message = "Sales entry not found." };
                }

                var salesDetails = await _context.SalesDetails
                    .Where(x => x.MstId == MstId)
                    .ToListAsync();

                if (salesDetails.Any())
                {
                    _context.SalesDetails.RemoveRange(salesDetails);
                }

                _context.SalesMasters.Remove(salesMaster);

                
                await _context.SaveChangesAsync();

                return new ResponseModel
                {
                    Id = MstId,
                    Message = "Sales Mast & Dtl deleted successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Id = 0,
                    Message = "An error occurred while deleting: " + ex.Message
                };
            }
        }
        #endregion

        #region  SalesReports


    

        public async Task<List<SalesReportDto>> GetReportDataAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@SmId", smId.HasValue && smId.Value > 0 ? smId.Value : DBNull.Value),
                new SqlParameter("@Month", string.IsNullOrEmpty(month) ? DBNull.Value : month),
                new SqlParameter("@FromDate", fromDate.HasValue ? fromDate.Value : DBNull.Value),
                new SqlParameter("@ToDate", toDate.HasValue ? toDate.Value : DBNull.Value)
            };
                     
            // 2. Execute the Stored Procedure using FromSqlRaw
            try
            {

                var result = await _context.Set<SalesReportDto>()
        .FromSqlRaw("EXEC sp_GetSalesReportData @SmId, @Month, @FromDate, @ToDate", parameters).ToListAsync();

                return result;

            }
            catch (Exception e)
            {        

                var result = await _context.Set<SalesReportDto>()
        .FromSqlRaw("EXEC sp_GetSalesReportData @SmId, @Month, @FromDate, @ToDate", parameters)
        .ToListAsync();

                return result;
            }

            //return result;
        }



        public async Task<byte[]> GenerateSalesPdfReportAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            // 1. Fetch data using the existing service method
            var reportData = await GetReportDataAsync(smId, month, fromDate, toDate);

            if (reportData == null || !reportData.Any())
            {
                return null;
            }

            byte[] pdfBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                iTextSharp.text.Font titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font subtitleFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headerFont = iTextSharp.text.FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font rowFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);

                // 2. Add Report Title
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Sales Report Summary", titleFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER,
                    SpacingAfter = 2f
                };
                document.Add(title);

                // 3. Add Search Criteria Subtitle
                List<string> criteriaList = new List<string>();
                if (smId.HasValue && smId.Value > 0)
                {
                    string smName = _context.SalesMans.FirstOrDefault(s => s.Id == smId.Value)?.Name ?? "Unknown";
                    criteriaList.Add($"Salesman: {smName}");
                }
                if (!string.IsNullOrEmpty(month)) criteriaList.Add($"Month: {month}");
                if (fromDate.HasValue && toDate.HasValue) criteriaList.Add($"Date: {fromDate.Value:dd-MMM-yyyy} to {toDate.Value:dd-MMM-yyyy}");
                else if (fromDate.HasValue) criteriaList.Add($"Date: From {fromDate.Value:dd-MMM-yyyy}");
                else if (toDate.HasValue) criteriaList.Add($"Date: Up to {toDate.Value:dd-MMM-yyyy}");

                string criteriaText = criteriaList.Any() ? string.Join("  |  ", criteriaList) : "Showing All Records";

                iTextSharp.text.Paragraph criteriaParagraph = new iTextSharp.text.Paragraph(criteriaText, subtitleFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER,
                    SpacingAfter = 5f
                };
                document.Add(criteriaParagraph);

                // 4. Add Print Date
                iTextSharp.text.Paragraph printDate = new iTextSharp.text.Paragraph($"Print Date: {DateTime.Now:dd-MMM-yyyy}", subtitleFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_LEFT,
                    SpacingAfter = 10f
                };
                document.Add(printDate);

                // 5. Initialize PDF Table (4 Columns)
                iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(4)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 10f, 40f, 25f, 25f });

                // 6. Add Table Headers
                string[] headers = { "SL", "Sales Man Name", "Sales Quantity", "Sales Value" };
                foreach (var header in headers)
                {
                    iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont))
                    {
                        BackgroundColor = new iTextSharp.text.BaseColor(230, 230, 230),
                        HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                        Padding = 6f
                    };
                    table.AddCell(cell);
                }

                // 7. Add Data Rows and Calculate Totals
                int slNumber = 1;
                decimal totalQty = 0;
                decimal totalValue = 0;

                foreach (var item in reportData)
                {
                    table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(slNumber.ToString(), rowFont))
                    { HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, Padding = 5f });

                    table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.SalesManName ?? "Unknown", rowFont))
                    { Padding = 5f });

                    table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.SalesQuantity?.ToString("0.00") ?? "0.00", rowFont))
                    { HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, Padding = 5f });

                    table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.SalesValue?.ToString("0.00") ?? "0.00", rowFont))
                    { HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, Padding = 5f });

                    totalQty += (item.SalesQuantity ?? 0);
                    totalValue += (item.SalesValue ?? 0);
                    slNumber++;
                }

                // 8. Add Summary/Total Row (Fixed: Colspan = 2 merges SL and Sales Man Name columns seamlessly)
                iTextSharp.text.pdf.PdfPCell totalLabelCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total Summary", headerFont))
                {
                    Colspan = 2,
                    HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT,
                    Padding = 5f
                };
                table.AddCell(totalLabelCell);

                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalQty.ToString("0.00"), headerFont))
                {
                    HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT,
                    Padding = 5f
                });

                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalValue.ToString("0.00"), headerFont))
                {
                    HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT,
                    Padding = 5f
                });

                document.Add(table);

                // 9. Add "In Word" (English)
                string amountInWordsEng = NumberToWordsConverter.ConvertToWords(totalValue);
                iTextSharp.text.Paragraph inWordsEng = new iTextSharp.text.Paragraph($"\nIn Word: {amountInWordsEng}", rowFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_LEFT
                };
                document.Add(inWordsEng);

                // 10. Add "কথায়" (Bangla using Kalpurush font)
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "kalpurush.ttf");

                if (File.Exists(fontPath))
                {
                    iTextSharp.text.pdf.BaseFont bfKalpurush = iTextSharp.text.pdf.BaseFont.CreateFont(
                        fontPath,
                        iTextSharp.text.pdf.BaseFont.IDENTITY_H,
                        iTextSharp.text.pdf.BaseFont.EMBEDDED
                    );

                    iTextSharp.text.Font banglaFont = new iTextSharp.text.Font(bfKalpurush, 10, iTextSharp.text.Font.NORMAL);

                    // Convert number to Bangla words
                    string rawBanglaWords = NumberToWordsConverter.ConvertToBanglaWords(totalValue);

                    // Fix Bangla vowel mark ordering for iTextSharp renderer
                    string fixedBanglaWords = NumberToWordsConverter.FixForPdf(rawBanglaWords);
                    string fixedKathayLabel = NumberToWordsConverter.FixForPdf("কথায়");

                    iTextSharp.text.Paragraph inWordsBng = new iTextSharp.text.Paragraph($"{fixedKathayLabel}: {fixedBanglaWords}", banglaFont)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_LEFT,
                        SpacingBefore = 3f
                    };
                    document.Add(inWordsBng);
                }

                document.Close();
                writer.Close();

                pdfBytes = ms.ToArray();
            }

            return pdfBytes;
        }
      
        
        
        public async Task<byte[]> GenerateSalesRdlcReportAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate, string format)
        {
            var reportData = await GetReportDataAsync(smId, month, fromDate, toDate);

            if (reportData == null || !reportData.Any())
            {
                return null;
            }

            using var localReport = new LocalReport();

            var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "SalesReport.rdlc");
            localReport.ReportPath = reportPath;
            localReport.DataSources.Add(new ReportDataSource("SalesDataSet", reportData));

            List<string> criteriaList = new List<string>();

            if (smId.HasValue && smId.Value > 0)
            {
                string smName = _context.SalesMans.FirstOrDefault(s => s.Id == smId.Value)?.Name ?? "Unknown";
                criteriaList.Add($"Salesman: {smName}");
            }

            if (!string.IsNullOrEmpty(month))
            {
                criteriaList.Add($"Month: {month}");
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                criteriaList.Add($"Date: {fromDate.Value:dd-MMM-yyyy} to {toDate.Value:dd-MMM-yyyy}");
            }
            else if (fromDate.HasValue)
            {
                criteriaList.Add($"Date: From {fromDate.Value:dd-MMM-yyyy}");
            }
            else if (toDate.HasValue)
            {
                criteriaList.Add($"Date: Up to {toDate.Value:dd-MMM-yyyy}");
            }

            string criteriaText = criteriaList.Any()
                ? string.Join("  |  ", criteriaList)
                : "Showing All Records";

            var parameters = new[]
            {
        new ReportParameter("ReportTitlecriteriaText", criteriaText)
        
    };

            localReport.SetParameters(parameters);

            string renderFormat = format.ToUpper() == "EXCEL" ? "EXCELOPENXML" : "PDF";
            byte[] fileBytes = localReport.Render(renderFormat);

            return fileBytes;
        }



        public async Task<SalesReportPrintModel> GetSalesReportHtmlAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate)
        {
            var reportData = await GetReportDataAsync(smId, month, fromDate, toDate);

            if (reportData == null)
            {
                reportData = new List<SalesReportDto>();
            }

            // Build criteria text (same logic as the PDF/RDLC methods)
            List<string> criteriaList = new List<string>();

            if (smId.HasValue && smId.Value > 0)
            {
                string smName = _context.SalesMans.FirstOrDefault(s => s.Id == smId.Value)?.Name ?? "Unknown";
                criteriaList.Add($"Salesman: {smName}");
            }

            if (!string.IsNullOrEmpty(month))
            {
                criteriaList.Add($"Month: {month}");
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                criteriaList.Add($"Date: {fromDate.Value:dd-MMM-yyyy} to {toDate.Value:dd-MMM-yyyy}");
            }
            else if (fromDate.HasValue)
            {
                criteriaList.Add($"Date: From {fromDate.Value:dd-MMM-yyyy}");
            }
            else if (toDate.HasValue)
            {
                criteriaList.Add($"Date: Up to {toDate.Value:dd-MMM-yyyy}");
            }

            string criteriaText = criteriaList.Any() ? string.Join("  |  ", criteriaList) : "Showing All Records";

            // Totals
            decimal totalQty = reportData.Sum(x => x.SalesQuantity ?? 0);
            decimal totalValue = reportData.Sum(x => x.SalesValue ?? 0);

            // Words (English + Bangla) - reuse the same converter used in the PDF report
            string amountInWordsEng = NumberToWordsConverter.ConvertToWords(totalValue);
            string amountInWordsBng = NumberToWordsConverter.ConvertToBanglaWords(totalValue);

            var model = new SalesReportPrintModel
            {
                Items = reportData,
                CriteriaText = criteriaText,
                PrintDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                TotalQuantity = totalQty,
                TotalValue = totalValue,
                AmountInWordsEnglish = amountInWordsEng,
                AmountInWordsBangla = amountInWordsBng
            };

            return model;
        }


        #endregion

    }
}
