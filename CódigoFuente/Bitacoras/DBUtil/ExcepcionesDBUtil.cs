using Clases.Models;
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
    public class ExcepcionesBDUtil
    {
        ///<summary>
        ///Guarda en la base de datos el contenido de una excepción
        ///</summary>
        public static void RegistraExcepcion(EntradaRegistroEvento entrada)
        {

            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_registra_evento_excepcion", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.VarChar).Value = entrada.Usuario;
                        cmd.Parameters.Add("@TipoEvento", SqlDbType.VarChar).Value = entrada.ObtenerTipoCodigo();
                        cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = entrada.FechaEvento;
                        cmd.Parameters.Add("@Origen", SqlDbType.VarChar).Value = entrada.Origen;
                        cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar).Value = entrada.Descripcion;
                        cmd.Parameters.Add("@Gravedad", SqlDbType.TinyInt).Value = entrada.Gravedad;
                        cmd.Parameters.Add("@NumeroError", SqlDbType.Int).Value = entrada.NumeroError;

                        int filas = cmd.ExecuteNonQuery();


                        conn.Close();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("No se puede realizar la conexion a la base de datos por: " + e.Message);
                }

            }
        }
    }
}
