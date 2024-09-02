using SV21T1080024.DomainModels;

namespace SV21T1080024.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }
 
}
