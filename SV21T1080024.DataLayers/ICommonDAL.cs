using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080024.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu chung
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang (bằng 0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");

        /// <summary>
        /// Đếm số dòng dữ liệu tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        
        /// <summary>
        /// Lấy một bản ghi/dòng dữ liệu dựa trên mã (id)
        /// </summary>
        /// <param name="id">Mã của dữ liệu cần lấy</param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// Bổ sung dữ liệu vào bảng. Hàm trả về id (mã) của dữ liệu được bổ sung.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
       
        /// <summary>
        /// Xóa một dòng dữ liệu dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        
        /// <summary>
        /// Kiểm tra xem một dòng dữ liệu có mã là id hiện có dữ liệu liên quan 
        /// ở các bảng khác hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
