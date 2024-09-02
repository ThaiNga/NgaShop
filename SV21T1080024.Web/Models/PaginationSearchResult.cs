using SV21T1080024.DomainModels;

namespace SV21T1080024.Web.Models
{
    /// <summary>
    /// Lớp cơ sở cho các kết quả tìm kiếm, hiển thị dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchResult
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Giá trị tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
        /// <summary>
        /// Tổng số dữ liệu
        /// </summary>
        public int RowCount { get; set; } = 0;
        /// <summary>
        /// Tổng số trang phân trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int n = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    n += 1;
                return n;
            }
        }
    }
    /// <summary>
    /// Kết quả tìm kiếm khách hàng
    /// </summary>
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
    /*public class ProductSearchResult : PaginationSearchResult
    {
        public required List<Product> Data { get; set; }
    }*/

}
