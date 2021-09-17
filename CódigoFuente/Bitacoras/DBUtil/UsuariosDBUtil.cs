using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clases.DBUtil
{
    ///<summary>
    ///Obtiene el username de un usuario apartir de un email
    ///</summary>
    ///<return>
    ///Devuelve un string con el username del usuario, retorna String.empty si no hay coincidencias 
    ///</return>
    ///<param name="email">
    ///Email del usuario a buscar.
    ///</param>
    public class UsuariosDBUtil
    {
        public static string ObtieneUsername(string email)
        {

            string username = String.Empty;

            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = @"SELECT UserName FROM AspNetUsers WHERE Email=@EMAIL";
                        command.CommandType = CommandType.Text;
                        command.Connection = conn;
                        command.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = email;
                        conn.Open();

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                username = Convert.ToString(dr["UserName"]);
                            }
                        }

                        conn.Close();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener información de la Base de Datos.", ex);
                }
                catch (Exception e)
                {
                    throw new Exception("Error interno al obtener la información de la Base de Datos.", e);
                }

                return username;
            }
        }
    }
}
