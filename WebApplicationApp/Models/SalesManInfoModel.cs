namespace WebApplicationApp.Models
{
    public class SalesManInfoModel
    {

        public int Id { get; set; }


        public string Name { get; set; } = null!;

        public string? SmCode { get; set; }

        public string? Phone{ get; set; }
        public Boolean? IsActive{ get; set; }

       public List<SalesManInfoModel> salesManInfoModel { get; set; }
    }
}
