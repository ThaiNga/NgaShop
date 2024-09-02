using SV21T1080024.DomainModels;

namespace SV21T1080024.Web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
