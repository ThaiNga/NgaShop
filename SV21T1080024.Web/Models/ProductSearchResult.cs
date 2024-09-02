using SV21T1080024.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1080024.Web.Models
{
    public class ProductSearchResult : PaginationSearchResult
    {
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public List<Product> Data { get; set; } = new List<Product>();

    }
}
