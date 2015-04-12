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
    public class TBClienteDAO
    {
        public static List<TbClienteBE> SelectAllClientes(){
        string connectionString = ConfigurationManager.ConnectionStrings["Northwind"].ToString();
        string sql = "select CodCliente,Nombre,Ruc from TbCliente";
        
        using(SqlConnection conn=new SqlConnection(connectionString))
            {
                conn.Open();
                using(SqlCommand command=new SqlCommand(sql,conn)){
                    command.CommandType = CommandType.Text;
                    List<TbClienteBE> Clientes = new List<TbClienteBE>();
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            TbClienteBE objCliente = new TbClienteBE(Convert.ToString(reader.GetDecimal(0)),reader.GetString(1),reader.GetString(2));
                            Clientes.Add(objCliente);
                        }
                    }
                    return Clientes;
                }
            }
        
        }
        
    

    }
}
