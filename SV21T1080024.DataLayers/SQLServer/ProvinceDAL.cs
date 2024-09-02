using SV21T1080024.DomainModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV21T1080024.DataLayers.SQLServer
{
    public class ProvinceDAL : _BaseDAL, ICommonDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Province data)
        {
            throw new NotImplementedException();
        }

        public int Count(string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Province? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Province> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Province> data = new List<Province>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Provinces";
                data = connection.Query<Province>(sql: sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Province data)
        {
            throw new NotImplementedException();
        }
    }
}
