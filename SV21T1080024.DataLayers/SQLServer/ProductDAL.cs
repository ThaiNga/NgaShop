using Dapper;
using SV21T1080024.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080024.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                            VALUES(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                            SELECT @@IDENTITY";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            VALUES(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            SELECT @@IDENTITY";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder 
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            SELECT @@IDENTITY";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	COUNT(*)
                            FROM	Products
                            WHERE	(@searchValue = N'' OR ProductName like @searchValue)
                                AND (@categoryID = 0 OR CategoryID = @categoryID)
                                AND (@supplierID = 0 OR SupplierID = @supplierID)
                                AND (Price >= @minPrice)
                                AND (@maxPrice <= 0 OR Price <= @maxPrice)";
                var parameters = new
                {
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice,
                    searchValue = $"%{searchValue}%"
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Products WHERE ProductId = @ProductId";
                var parameters = new
                {
                    ProductId = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductAttributes WHERE AttributeId = @AttributeId";
                var parameters = new
                {
                    AttributeId = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductPhotos WHERE PhotoId = @PhotoId";
                var parameters = new
                {
                    PhotoId = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Products WHERE ProductId = @ProductId";
                var parameters = new { ProductId = productID };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE AttributeId = @AttributeId";
                var parameters = new { AttributeId = attributeID };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE PhotoId = @PhotoId";
                var parameters = new { PhotoId = photoID };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID)
	                            SELECT 1
                            ELSE 
	                            SELECT 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM	(
			                        SELECT	*,
					                        ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber
			                        FROM	Products
			                        WHERE	(@searchValue = N'' OR ProductName like @searchValue)
                                        AND (@categoryID = 0 OR CategoryID = @categoryID)
                                        AND (@supplierID = 0 OR SupplierID = @supplierID)
                                        AND (Price >= @minPrice)
                                        AND (@maxPrice <= 0 OR Price <= @maxPrice)
		                            ) AS t
                             WHERE	(@pageSize = 0)
	                            OR	(RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                             ORDER BY RowNumber;";
                var parameters = new
                {
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice,
                    page = page,
                    pageSize = pageSize,
                    searchValue = $"%{searchValue}%"
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE ProductId = @ProductId ORDER BY DisplayOrder";
                var parameters = new { ProductId = productID };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE ProductId = @ProductId ORDER BY DisplayOrder";
                var parameters = new { ProductId = productID };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Products
                            SET ProductName = @ProductName, 
                                ProductDescription = @ProductDescription,
                                SupplierID = @SupplierID,
                                CategoryID = @CategoryID,
                                Unit = @Unit,
                                Price = @Price,
                                Photo = @Photo, 
                                IsSelling = @IsSelling
                            WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductAttributes
                            SET ProductID = @ProductID,
                                AttributeName = @AttributeName,
                                AttributeValue = @AttributeValue,
                                DisplayOrder = @DisplayOrder
                            WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductPhotos
                            SET ProductID = @ProductID,
                                Photo = @Photo,
                                Description = @Description,
                                DisplayOrder = @DisplayOrder,
                                IsHidden = @IsHidden
                            WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
