using WebApplicationApp.Date.Entities;
using WebApplicationApp.Models;
using WebApplicationApp.ReturnResponse;

namespace WebApplicationApp.Repositories.SelesManService
{
    public interface ISalesManServices
    {



        #region SalesMan

        Task<List<SalesManInfoModel>> GetAllSalesManInfoAsync();

        Task<List<SalesMan>> GetAllSalesMansAsync();
        Task<SalesManInfoModel> GetAllSalesManInfoAsyncById(int Id);
        Task<ResponseModel> CreateSalesManInfo(SalesManInfoModel salesManInfoModel);

        Task<ResponseModel> DeleteSalesMan(int id);

        #endregion

        #region SalesMaster 


        Task<List<SalesMaster>> GetAllSalesMastersAsync();


        #endregion

        #region SalesDetail 


        Task<List<SalesDetail>> GetAllSalesDetailsAsync();

        #endregion


        #region salesEntry info 

        Task<ResponseModel> CreateSalesEntryInfo(SalesEntryViewModel salesEntryPostModel);

        Task<SalesEntryViewModel> GetSalesEntryByIdAsync(int MstId);
        Task<ResponseModel> DeleteSalesEntry(int MstId);
        #endregion

        Task<List<SalesReportDto>> GetReportDataAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate);

        Task<byte[]> GenerateSalesPdfReportAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate);


        Task<byte[]> GenerateSalesRdlcReportAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate, string format);



        Task<SalesReportPrintModel> GetSalesReportHtmlAsync(int? smId, string month, DateTime? fromDate, DateTime? toDate);


    }
}
