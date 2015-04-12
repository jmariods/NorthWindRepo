using NorthWind.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.DAO
{
    public class TBProductoDAO
    {

        public static List<TbProductoBE> SelectAllProductos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Northwind"].ToString();
            string sql = "SELECT [ProductID],[ProductName],[UnitPrice] FROM [Products]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    List<TbProductoBE> Productos = new List<TbProductoBE>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TbProductoBE objCliente = new TbProductoBE(Convert.ToString(reader.GetInt32(0)), reader.GetString(1), Convert.ToString(reader.GetDecimal(2)));
                            Productos.Add(objCliente);
                        }
                    }
                    return Productos;
                }
            }

        }
    }
}
