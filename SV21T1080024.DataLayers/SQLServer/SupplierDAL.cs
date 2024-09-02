using Dapper;
using SV21T1080024.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080024.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Suppliers(SupplierName, ContactName, Province, Address, Phone, Email, Logo)
                            VALUES(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email, @Logo);
                            SELECT @@IDENTITY";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Logo = data.Logo ?? ""
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	COUNT(*)
                            FROM	Suppliers
                            WHERE	(SupplierName like @searchValue) OR (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = $"%{searchValue}%"
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Suppliers WHERE SupplierId = @SupplierId";
                var parameters = new
                {
                    SupplierId = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Suppliers WHERE SupplierId = @SupplierId";
                var parameters = new { SupplierId = id };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Products WHERE SupplierID = @SupplierId)
	                            SELECT 1
                            ELSE 
	                            SELECT 0";
                var parameters = new
                {
                    SupplierId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM	(
			                        SELECT	*,
					                        ROW_NUMBER() over (ORDER BY SupplierName) AS RowNumber
			                        FROM	Suppliers
			                        WHERE	(SupplierName like @searchValue) OR (ContactName like @searchValue)
		                            ) AS t
                             WHERE	(@pageSize = 0)
	                            OR	(RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                             ORDER BY RowNumber;";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = $"%{searchValue}%"
                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Suppliers
                            SET SupplierName = @SupplierName,
                                ContactName = @ContactName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                Logo = @Logo
                            WHERE SupplierId = @SupplierId";
                var parameters = new
                {
                    SupplierId = data.SupplierID,
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Logo = data.Logo ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
