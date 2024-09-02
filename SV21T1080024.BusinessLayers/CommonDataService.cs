using SV21T1080024.DataLayers;
using SV21T1080024.DomainModels;

namespace SV21T1080024.BusinessLayers
{
    public static class CommonDataService
    {
        static readonly ICommonDAL<Province> proviceDB;
        static readonly ICommonDAL<Customer> customerDB;
        static readonly ICommonDAL<Supplier> supplierDB;
        static readonly ICommonDAL<Shipper> shipperDB;
        static readonly ICommonDAL<Employee> employeeDB;
        static readonly ICommonDAL<Category> categoryDB;
        static CommonDataService()
        {
            proviceDB = new DataLayers.SQLServer.ProvinceDAL(Configuration.ConnectionString);
            customerDB = new DataLayers.SQLServer.CustomerDAL(Configuration.ConnectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(Configuration.ConnectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(Configuration.ConnectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(Configuration.ConnectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Lấy danh sách toàn bộ tỉnh/thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        { 
            return proviceDB.List().ToList();
        }

        /// <summary>
        /// Danh sách khách hàng (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValues = "")
        { 
            rowCount = customerDB.Count(searchValues);
            return customerDB.List(page, pageSize, searchValues).ToList();

        }
        /// <summary>
        /// Danh sách khách hàng (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(string searchValues)
        {
            return customerDB.List(1, 0, searchValues).ToList();
        }
        /// <summary>
        /// Lấy thông tin của 1 khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            if(id <= 0 )
                return null;
            return customerDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 khách hàng mới. Hàm trả về id của khách hàng được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        { 
            return customerDB.Update(data); 
        }
        /// <summary>
        /// Xóa 1 khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra khách hàng có mã id hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        /// <summary>
        /// Danh sách Nhà cung cấp (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValues = "")
        {
            rowCount = supplierDB.Count(searchValues);
            return supplierDB.List(page, pageSize, searchValues).ToList() ;
        }
        /// <summary>
        /// Danh sách Nhà cung cấp (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(string searchValues)
        {
            return supplierDB.List(1, 0, searchValues).ToList();
        }
        /// <summary>
        /// Lấy thông tin của 1 Nhà cung cấp dựa vào mã Nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            if (id <= 0)
                return null;
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một Nhà cung cấp mới. Hàm trả về id của Nhà cung cấp được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin Nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 Nhà cung cấp dựa vào mã Nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id) 
        { 
            return supplierDB.Delete(id); 
        }
        /// <summary>
        /// Kiểm tra Nhà cung cấp có mã id hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        /// <summary>
        /// Danh sách người giao hàng (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValues = "")
        {
            rowCount = shipperDB.Count(searchValues);
            return shipperDB.List(page, pageSize, searchValues).ToList();
        }
        /// <summary>
        /// Danh sách người giao hàng (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(string searchValues)
        {
            return shipperDB.List(1, 0, searchValues).ToList();
        }
        /// <summary>
        /// Lấy thông tin của 1 người giao hàng dựa vào mã người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            if (id <= 0)
                return null;
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một người giao hàng mới. Hàm trả về id của người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// Xóa một người giao hàng dựa vào mã người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra người giao hàng có mã id hiện có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        /// <summary>
        /// danh sách loại hàng (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValues = "")
        {
            rowCount = categoryDB.Count(searchValues);
            return categoryDB.List(page, pageSize, searchValues).ToList() ;
        }
        /// <summary>
        /// Danh sách loại hàng (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(string searchValues)
        {
            return categoryDB.List(1, 0, searchValues).ToList();
        }
        /// <summary>
        /// Lấy thông tin của 1 loại hàng dựa vào mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            if (id <= 0)
                return null;
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một loại hàng mới. Hàm trả về id của loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        { 
            return categoryDB.Add(data); 
        }
        /// <summary>
        /// Cập nhật thông tin loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa một loại hàng dựa vào mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra loại hàng có mã id hiện có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        /// <summary>
        /// danh sách nhân viên (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValues = "")
        {
            rowCount = employeeDB.Count(searchValues);
            return employeeDB.List(page, pageSize, searchValues).ToList();
        }
        /// <summary>
        /// Danh sách nhân viên (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="searchValues"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(string searchValues)
        {
            return employeeDB.List(1, 0, searchValues).ToList();
        }
        /// <summary>
        /// Lấy thông tin của 1 nhân viên dựa vào mã nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            if (id <= 0)
                return null;
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một nhân viên mới. Hàm trả về id của nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// Xóa một nhân viên dựa vào mã nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra nhân viên có mã id hiện có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
    }
}
//Lớp static là gì?
//Constructor trong lớp static có đặc điểm gì? Được gọi khi nào?
