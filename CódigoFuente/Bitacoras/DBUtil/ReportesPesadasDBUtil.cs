using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.DBUtil
{
    public class ReportesPesadasDBUtil
    {

        ///<summary>
        ///Obtiene los registros del Reporte 
        ///</summary>
        public static List<ReportePesada> ObtieneReporteSilao(string cliente,  DateTime fecha_inicio, DateTime fecha_final)
        {

            List<ReportePesada> listado = new List<ReportePesada>();
            string client = cliente;

            if (String.IsNullOrEmpty(client))
                client = "";


            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["cube_tkmmConnection"].ConnectionString;

            using (var conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_reporte_pesadas_silao", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                       

                        cmd.Parameters.Add("@cliente", SqlDbType.VarChar).Value = client
                            ;
                        cmd.Parameters.Add("@fecha_inicio", SqlDbType.DateTime).Value = fecha_inicio;
                        cmd.Parameters.Add("@fecha_fin", SqlDbType.DateTime).Value = fecha_final;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {

                            while (dr.Read())
                            {

                                //convierte de forma segura
                                double? pesoNetoSap;
                                if (DBNull.Value.Equals(dr["Peso Neto SAP"]))
                                    pesoNetoSap = null;
                                else
                                    pesoNetoSap = Convert.ToDouble(dr["Peso Neto SAP"]);

                                double? pesoNetoMean;
                                if (DBNull.Value.Equals(dr["Peso Neto (mean)"]))
                                    pesoNetoMean = null;
                                else
                                    pesoNetoMean = Convert.ToDouble(dr["Peso Neto (mean)"]);

                                double? pesoNetoStdev;
                                if (DBNull.Value.Equals(dr["Peso Neto (sdtv)"]))
                                    pesoNetoStdev = null;
                                else
                                    pesoNetoStdev = Convert.ToDouble(dr["Peso Neto (sdtv)"]);

                                int? count;
                                if (DBNull.Value.Equals(dr["count"]))
                                    count = null;
                                else
                                    count = Convert.ToInt32(dr["count"]);

                                double? diferenciaPesoNeto;
                                if (DBNull.Value.Equals(dr["Diferencia Peso Neto"]))
                                    diferenciaPesoNeto = null;
                                else
                                    diferenciaPesoNeto = Convert.ToDouble(dr["Diferencia Peso Neto"]);

                                double? thickness;
                                if (DBNull.Value.Equals(dr["Thickness"]))
                                    thickness = null;
                                else
                                    thickness = Convert.ToDouble(dr["Thickness"]);

                                double? width;
                                if (DBNull.Value.Equals(dr["Width"]))
                                    width = null;
                                else
                                    width = Convert.ToDouble(dr["Width"]);

                                double? advance;
                                if (DBNull.Value.Equals(dr["Advance"]))
                                    advance = null;
                                else
                                    advance = Convert.ToDouble(dr["Advance"]);

                                double? pesoAcero;
                                if (DBNull.Value.Equals(dr["peso_teorico_acero"]))
                                    pesoAcero = null;
                                else
                                    pesoAcero = Convert.ToDouble(dr["peso_teorico_acero"]);

                                double? pesoBrutoSap;
                                if (DBNull.Value.Equals(dr["Peso Bruto SAP"]))
                                    pesoBrutoSap = null;
                                else
                                    pesoBrutoSap = Convert.ToDouble(dr["Peso Bruto SAP"]);

                                double? pesoBrutoMean;
                                if (DBNull.Value.Equals(dr["Peso Bruto (mean)"]))
                                    pesoBrutoMean = null;
                                else
                                    pesoBrutoMean = Convert.ToDouble(dr["Peso Bruto (mean)"]);


                                double? DiferenciaBruto;
                                if (DBNull.Value.Equals(dr["Diferencia SAP Mean"]))
                                    DiferenciaBruto = null;
                                else
                                    DiferenciaBruto = Convert.ToDouble(dr["Diferencia SAP Mean"]);

                                double? piezas;
                                if (DBNull.Value.Equals(dr["Total de piezas"]))
                                    piezas = null;
                                else
                                    piezas = Convert.ToDouble(dr["Total de piezas"]);

                                double? pesoEtiqueta;
                                if (DBNull.Value.Equals(dr["Peso Etiqueta"]))
                                    pesoEtiqueta = null;
                                else
                                    pesoEtiqueta = Convert.ToDouble(dr["Peso Etiqueta"]);

                                listado.Add(new ReportePesada
                                {
                                    id = Convert.ToInt32(dr["id"]),
                                    Cliente = Convert.ToString(dr["Name 1"]),
                                    SAP_Platina = Convert.ToString(dr["SAP Platina"]),
                                    SAP_Rollo = Convert.ToString(dr["SAP Rollo"]),
                                    Type_of_Metal = Convert.ToString(dr["Type of Metal"]),
                                    Peso_Neto_SAP = pesoNetoSap,
                                    Peso_Neto__mean_ = pesoNetoMean,
                                    Peso_Neto_stdev = pesoNetoStdev,
                                    count = count,
                                    Diferencia_Peso_Neto = diferenciaPesoNeto,
                                    Thickness = thickness,
                                    Width = width,
                                    Advance = advance,
                                    peso_teorico_acero = pesoAcero,
                                    Peso_Bruto_SAP = pesoBrutoSap,
                                    Peso_Bruto__mean_ = pesoBrutoMean,
                                    Diferencia_SAP_Mean = DiferenciaBruto,
                                    Total_de_piezas = piezas,
                                    Peso_Etiqueta = pesoEtiqueta
                                });
    
                               
                            }
                        }

                        conn.Close();

                    }
                }
                catch (Exception e)
                {
                    throw new Exception("No se puede realizar la conexion a la base de datos por: " + e.Message);
                }

            }
            return listado;
        }


        ///<summary>
        ///Obtiene los registros del Reporte 
        ///</summary>
        public static List<ReportePesada> ObtieneReportePuebla(string cliente, DateTime fecha_inicio, DateTime fecha_final)
        {

            List<ReportePesada> listado = new List<ReportePesada>();
            string client = cliente;

            if (String.IsNullOrEmpty(client))
                client = "";


            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["cube_tkmmConnection"].ConnectionString;

            using (var conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_reporte_pesadas_puebla", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("@cliente", SqlDbType.VarChar).Value = client
                            ;
                        cmd.Parameters.Add("@fecha_inicio", SqlDbType.DateTime).Value = fecha_inicio;
                        cmd.Parameters.Add("@fecha_fin", SqlDbType.DateTime).Value = fecha_final;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {

                            while (dr.Read())
                            {

                                //convierte de forma segura
                                double? pesoNetoSap;
                                if (DBNull.Value.Equals(dr["Peso Neto SAP"]))
                                    pesoNetoSap = null;
                                else
                                    pesoNetoSap = Convert.ToDouble(dr["Peso Neto SAP"]);

                                double? pesoNetoMean;
                                if (DBNull.Value.Equals(dr["Peso Neto (mean)"]))
                                    pesoNetoMean = null;
                                else
                                    pesoNetoMean = Convert.ToDouble(dr["Peso Neto (mean)"]);

                                double? pesoNetoStdev;
                                if (DBNull.Value.Equals(dr["Peso Neto (sdtv)"]))
                                    pesoNetoStdev = null;
                                else
                                    pesoNetoStdev = Convert.ToDouble(dr["Peso Neto (sdtv)"]);

                                int? count;
                                if (DBNull.Value.Equals(dr["count"]))
                                    count = null;
                                else
                                    count = Convert.ToInt32(dr["count"]);

                                double? diferenciaPesoNeto;
                                if (DBNull.Value.Equals(dr["Diferencia Peso Neto"]))
                                    diferenciaPesoNeto = null;
                                else
                                    diferenciaPesoNeto = Convert.ToDouble(dr["Diferencia Peso Neto"]);

                                double? thickness;
                                if (DBNull.Value.Equals(dr["Thickness"]))
                                    thickness = null;
                                else
                                    thickness = Convert.ToDouble(dr["Thickness"]);

                                double? width;
                                if (DBNull.Value.Equals(dr["Width"]))
                                    width = null;
                                else
                                    width = Convert.ToDouble(dr["Width"]);

                                double? advance;
                                if (DBNull.Value.Equals(dr["Advance"]))
                                    advance = null;
                                else
                                    advance = Convert.ToDouble(dr["Advance"]);

                                double? pesoAcero;
                                if (DBNull.Value.Equals(dr["peso_teorico"]))
                                    pesoAcero = null;
                                else
                                    pesoAcero = Convert.ToDouble(dr["peso_teorico"]);

                                double? pesoBrutoSap;
                                if (DBNull.Value.Equals(dr["Peso Bruto SAP"]))
                                    pesoBrutoSap = null;
                                else
                                    pesoBrutoSap = Convert.ToDouble(dr["Peso Bruto SAP"]);

                                double? pesoBrutoMean;
                                if (DBNull.Value.Equals(dr["Peso Bruto (mean)"]))
                                    pesoBrutoMean = null;
                                else
                                    pesoBrutoMean = Convert.ToDouble(dr["Peso Bruto (mean)"]);


                                double? DiferenciaBruto;
                                if (DBNull.Value.Equals(dr["Diferencia SAP Mean"]))
                                    DiferenciaBruto = null;
                                else
                                    DiferenciaBruto = Convert.ToDouble(dr["Diferencia SAP Mean"]);

                                double? DiferenciaBrutoSAPTeorico;
                                if (DBNull.Value.Equals(dr["Diferencia Peso Bruto SAP-teorico"]))
                                    DiferenciaBrutoSAPTeorico = null;
                                else
                                    DiferenciaBrutoSAPTeorico = Convert.ToDouble(dr["Diferencia Peso Bruto SAP-teorico"]);

                                double? piezas;
                                if (DBNull.Value.Equals(dr["Total de piezas"]))
                                    piezas = null;
                                else
                                    piezas = Convert.ToDouble(dr["Total de piezas"]);

                                double? pesoEtiqueta;
                                if (DBNull.Value.Equals(dr["Peso Etiqueta"]))
                                    pesoEtiqueta = null;
                                else
                                    pesoEtiqueta = Convert.ToDouble(dr["Peso Etiqueta"]);

                                listado.Add(new ReportePesada
                                {
                                    id = Convert.ToInt32(dr["id"]),
                                    Cliente = Convert.ToString(dr["Name 1"]),
                                    SAP_Platina = Convert.ToString(dr["SAP Platina "]),
                                    SAP_Rollo = Convert.ToString(dr["SAP Rollo"]),
                                    Type_of_Metal = Convert.ToString(dr["Type of Metal"]),
                                    Peso_Neto_SAP = pesoNetoSap,
                                    Peso_Neto__mean_ = pesoNetoMean,
                                    Peso_Neto_stdev = pesoNetoStdev,
                                    count = count,
                                    Diferencia_Peso_Neto = diferenciaPesoNeto,
                                    Thickness = thickness,
                                    Width = width,
                                    Advance = advance,
                                    peso_teorico_acero = pesoAcero,
                                    Peso_Bruto_SAP = pesoBrutoSap,
                                    Peso_Bruto__mean_ = pesoBrutoMean,
                                    Diferencia_SAP_Mean = DiferenciaBruto,
                                    Diferencia_Bruto_Sap_teorico = DiferenciaBrutoSAPTeorico, 
                                    Total_de_piezas = piezas,
                                    Peso_Etiqueta = pesoEtiqueta
                                });


                            }
                        }

                        conn.Close();

                    }
                }
                catch (Exception e)
                {
                    throw new Exception("No se puede realizar la conexion a la base de datos por: " + e.Message);
                }

            }
            return listado;
        }


        public static List<String> ObtieneClientesSilao()
        {
            List<String> listado = new List<string>(); 
            

            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["cube_tkmmConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = @"select distinct [Name 1] from [cube_tkmm].[dbo].[view_detalle_pesada_union_silao]";
                        command.CommandType = CommandType.Text;
                        command.Connection = conn;
                        conn.Open();

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listado.Add (Convert.ToString(dr["Name 1"]));
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

                return listado;
            }
        }

        public static List<String> ObtieneClientesPuebla()
        {
            List<String> listado = new List<string>();


            string cadenaConexion = cadenaConexion = ConfigurationManager.ConnectionStrings["cube_tkmmConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = @"select distinct [Name 1] from [cube_tkmm].[dbo].[view_detalle_pesada_union_puebla]";
                        command.CommandType = CommandType.Text;
                        command.Connection = conn;
                        conn.Open();

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listado.Add(Convert.ToString(dr["Name 1"]));
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

                return listado;
            }
        }
    }

    public class ReportePesada
    {
        public long id { get; set; }

        [Display(Name = "Invoice To")]
        public string Cliente { get; set; }

        [Display(Name = "SAP PLATINA")]
        public string SAP_Platina { get; set; }
        
        [Display(Name = "SAP ROLLO")]
        public string SAP_Rollo { get; set; }

        [Display(Name = "Type of Metal")]
        public string Type_of_Metal { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Neto SAP")]
        public Nullable<double> Peso_Neto_SAP { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Neto Mean")]
        public Nullable<double> Peso_Neto__mean_ { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Neto Stdev")]
        public Nullable<double> Peso_Neto_stdev { get; set; }

        [Display(Name = "Count")]
        public Nullable<int> count { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Neto")]
        public Nullable<double> Diferencia_Peso_Neto { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Thickness { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Width { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Advance { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Teórico Acero")]
        public Nullable<double> peso_teorico_acero { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Bruto SAP")]
        public Nullable<double> Peso_Bruto_SAP { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Bruto Mean")]
        public Nullable<double> Peso_Bruto__mean_ { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Bruto")]
        public Nullable<double> Diferencia_SAP_Mean { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Bruto SAP-teorico")]
        public Nullable<double> Diferencia_Bruto_Sap_teorico { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Total Piezas")]
        public Nullable<double> Total_de_piezas { get; set; }

        [Display(Name = "Peso Etiqueta")]
        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Peso_Etiqueta { get; set; }
    }

  
}
