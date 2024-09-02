using Azure;
using Dapper;
using SV21T1080024.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080024.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                            VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                            SELECT @@IDENTITY";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Islocked = data.IsLocked
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
                            FROM	Customers
                            WHERE	(CustomerName like @searchValue) OR (ContactName like @searchValue)";
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
                var sql = @"DELETE FROM Customers WHERE CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Customers WHERE CustomerId = @CustomerId";
                var parameters = new { CustomerId = id };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Orders WHERE CustomerID = @CustomerId)
	                            SELECT 1
                            ELSE 
	                            SELECT 0";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM	(
			                        SELECT	*,
					                        ROW_NUMBER() over (ORDER BY CustomerName) AS RowNumber
			                        FROM	Customers
			                        WHERE	(CustomerName like @searchValue) OR (ContactName like @searchValue)
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
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers
                            SET CustomerName = @CustomerName,
                                ContactName = @ContactName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                IsLocked = @IsLocked
                            WHERE CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
