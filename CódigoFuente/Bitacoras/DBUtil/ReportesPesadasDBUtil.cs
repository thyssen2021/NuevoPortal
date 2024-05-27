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
        public static List<ReportePesada> ObtieneReporteSilao(string cliente, DateTime fecha_inicio, DateTime fecha_final)
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


                        cmd.Parameters.Add("@cliente", SqlDbType.VarChar).Value = client;
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
                                    //Diferencia_Peso_Neto = diferenciaPesoNeto,
                                    Thickness = thickness,
                                    Width = width,
                                    Advance = advance,
                                    peso_teorico_acero = pesoAcero,
                                    Peso_Bruto_SAP = pesoBrutoSap,
                                    Peso_Bruto__mean_ = pesoBrutoMean,
                                    //Diferencia_SAP_Mean = DiferenciaBruto,
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
        public static List<ReportePesada> ObtieneReportePuebla(string cliente, DateTime fecha_inicio, DateTime fecha_final, int? planta, string material, int muestra)
        {

            List<ReportePesada> listado = new List<ReportePesada>();
            string client = cliente;

            if (String.IsNullOrEmpty(client))
                client = "";


            string cadenaConexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_reporte_pesadas", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 180; // 3 minutos

                        cmd.Parameters.Add("@cliente", SqlDbType.VarChar).Value = client;
                        cmd.Parameters.Add("@fecha_inicio", SqlDbType.DateTime).Value = fecha_inicio;
                        cmd.Parameters.Add("@fecha_fin", SqlDbType.DateTime).Value = fecha_final;
                        cmd.Parameters.Add("@id_planta", SqlDbType.Int).Value = planta.HasValue ? planta.Value : 0;
                        cmd.Parameters.Add("@material", SqlDbType.VarChar).Value = !string.IsNullOrEmpty(material) ? material : string.Empty;
                        cmd.Parameters.Add("@muestra", SqlDbType.Int).Value = muestra;


                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {

                            while (dr.Read())
                            {

                                ////convierte de forma segura
                                double? pesoNetoSap;
                                if (DBNull.Value.Equals(dr["peso_neto_sap"]))
                                    pesoNetoSap = null;
                                else
                                    pesoNetoSap = Convert.ToDouble(dr["peso_neto_sap"]);

                                double? pesoNetoMean;
                                if (DBNull.Value.Equals(dr["peso_neto_mean"]))
                                    pesoNetoMean = null;
                                else
                                    pesoNetoMean = Convert.ToDouble(dr["peso_neto_mean"]);

                                double? pesoNetoStdev;
                                if (DBNull.Value.Equals(dr["peso_neto_stdev"]))
                                    pesoNetoStdev = null;
                                else
                                    pesoNetoStdev = Convert.ToDouble(dr["peso_neto_stdev"]);

                                int? count;
                                if (DBNull.Value.Equals(dr["count"]))
                                    count = null;
                                else
                                    count = Convert.ToInt32(dr["count"]);

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
                                if (DBNull.Value.Equals(dr["peso_bruto_sap"]))
                                    pesoBrutoSap = null;
                                else
                                    pesoBrutoSap = Convert.ToDouble(dr["peso_bruto_sap"]);

                                double? pesoBrutoMean;
                                if (DBNull.Value.Equals(dr["peso_bruto_mean"]))
                                    pesoBrutoMean = null;
                                else
                                    pesoBrutoMean = Convert.ToDouble(dr["peso_bruto_mean"]);

                                double? piezas;
                                if (DBNull.Value.Equals(dr["total_piezas"]))
                                    piezas = null;
                                else
                                    piezas = Convert.ToDouble(dr["total_piezas"]);

                                double? pesoEtiqueta;
                                if (DBNull.Value.Equals(dr["peso_etiqueta"]))
                                    pesoEtiqueta = null;
                                else
                                    pesoEtiqueta = Convert.ToDouble(dr["peso_etiqueta"]);

                                int? count_muestra;
                                if (DBNull.Value.Equals(dr["tamano_muestra"]))
                                    count_muestra = null;
                                else
                                    count_muestra = Convert.ToInt32(dr["tamano_muestra"]);

                                double? porcentaje_tolerancia;
                                if (DBNull.Value.Equals(dr["porcentaje_tolerancia"]))
                                    porcentaje_tolerancia = null;
                                else
                                    porcentaje_tolerancia = Convert.ToDouble(dr["porcentaje_tolerancia"]);

                                double? porcentaje_diferencia_total;
                                if (DBNull.Value.Equals(dr["diferencia_porcentaje_total_rollos"]))
                                    porcentaje_diferencia_total = null;
                                else
                                    porcentaje_diferencia_total = Convert.ToDouble(dr["diferencia_porcentaje_total_rollos"]);

                                double? porcentaje_tolerancia_muestra;
                                if (DBNull.Value.Equals(dr["diferencia_porcentaje_muestra_rollo"]))
                                    porcentaje_tolerancia_muestra = null;
                                else
                                    porcentaje_tolerancia_muestra = Convert.ToDouble(dr["diferencia_porcentaje_muestra_rollo"]);

                                double? peso_neto_mean_muestra;
                                if (DBNull.Value.Equals(dr["peso_neto_mean_muestra"]))
                                    peso_neto_mean_muestra = null;
                                else
                                    peso_neto_mean_muestra = Convert.ToDouble(dr["peso_neto_mean_muestra"]);

                                bool? muestra_dentro_rango;
                                if (DBNull.Value.Equals(dr["muestra_dentro_del_rango"]))
                                    muestra_dentro_rango = null;
                                else
                                    muestra_dentro_rango = Convert.ToBoolean(dr["muestra_dentro_del_rango"]);

                                double? piezas_muestra;
                                if (DBNull.Value.Equals(dr["total_piezas_muestra"]))
                                    piezas_muestra = null;
                                else
                                    piezas_muestra = Convert.ToDouble(dr["total_piezas_muestra"]);


                                listado.Add(new ReportePesada
                                {
                                    id = Convert.ToInt32(dr["id"]),
                                    Cliente = Convert.ToString(dr["invoiced_to"]),
                                    SAP_Platina = Convert.ToString(dr["sap_platina"]),
                                    SAP_Rollo = Convert.ToString(dr["sap_rollo"]),
                                    Type_of_Metal = Convert.ToString(dr["tipo_metal"]),
                                    Peso_Neto_SAP = pesoNetoSap,
                                    Peso_Neto__mean_ = pesoNetoMean,
                                    Peso_Neto_stdev = pesoNetoStdev,
                                    count = count,
                                    //Diferencia_Peso_Neto = diferenciaPesoNeto,
                                    Thickness = thickness,
                                    Width = width,
                                    Advance = advance,
                                    peso_teorico_acero = pesoAcero,
                                    Peso_Bruto_SAP = pesoBrutoSap,
                                    Peso_Bruto__mean_ = pesoBrutoMean,
                                    //Diferencia_SAP_Mean = DiferenciaBruto,
                                    //Diferencia_Bruto_Sap_teorico = DiferenciaBrutoSAPTeorico, 
                                    Total_de_piezas = piezas,
                                    Peso_Etiqueta = pesoEtiqueta,
                                    count_muestra = count_muestra,
                                    tamano_pieza = Convert.ToString(dr["tamano_pieza"]),
                                    porcentaje_tolerancia = porcentaje_tolerancia,
                                    porcentaje_diferencia_muestra = porcentaje_tolerancia_muestra,
                                    porcentaje_diferencia_total = porcentaje_diferencia_total,
                                    peso_neto_mean_muestra = peso_neto_mean_muestra,
                                    muestra_dentro_rango = muestra_dentro_rango,
                                    fecha_inicio_validez_peso_bom = Convert.ToDateTime(dr["fecha_inicio_validez_peso_bom"]),
                                    fecha_fin_validez_peso_bom = Convert.ToDateTime(dr["fecha_fin_validez_peso_bom"]),
                                    total_piezas_muestra = piezas_muestra

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
                        command.CommandText = @"select distinct [Name 1] from [cube_tkmm].[dbo].[view_detalle_pesada_union_silao] WHERE [Name 1]<>''";
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
                        command.CommandText = @"select distinct [Name 1] from [cube_tkmm].[dbo].[view_detalle_pesada_union_puebla] WHERE [Name 1]<>''";
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
        [Display(Name = "Peso Neto Mean (total)")]
        public Nullable<double> Peso_Neto__mean_ { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Neto Stdev")]
        public Nullable<double> Peso_Neto_stdev { get; set; }

        [Display(Name = "Count (total)")]
        public Nullable<int> count { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Neto")]
        public Nullable<double> Diferencia_Peso_Neto
        {
            get
            {
                return Peso_Neto_SAP - Peso_Neto__mean_;
            }
        }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Thickness { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Width { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Advance { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Teórico")]
        public Nullable<double> peso_teorico_acero { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Bruto SAP")]
        public Nullable<double> Peso_Bruto_SAP { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Bruto Mean (total)")]
        public Nullable<double> Peso_Bruto__mean_ { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Bruto Mean (total)")]
        public Nullable<double> diferencia_peso_bruto_mean
        {
            get
            {
                return Peso_Bruto_SAP - Peso_Bruto__mean_;
            }
        }


        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Bruto SAP-teorico (total)")]
        public Nullable<double> diferencia_peso_bruto_sap_teorico
        {
            get
            {
                return Peso_Bruto_SAP - peso_teorico_acero;
            }
        }


        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Total Piezas (total)")]
        public Nullable<double> Total_de_piezas { get; set; }

        [Display(Name = "Peso Etiqueta")]
        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        public Nullable<double> Peso_Etiqueta { get; set; }

        ///////////// valores para muestra

        [Display(Name = "Rollos (muestra)")]
        public Nullable<int> count_muestra { get; set; }

        [Display(Name = "Tamaño Pieza")]
        public string tamano_pieza { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "Porcentaje Tolerancia")]
        public Nullable<double> porcentaje_tolerancia { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "Porcentaje Diferencia Peso Neto (muestra)")]
        public Nullable<double> porcentaje_diferencia_muestra { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "Porcentaje Diferencia Peso Neto (total)")]
        public Nullable<double> porcentaje_diferencia_total { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Peso Neto Mean (muestra)")]
        public Nullable<double> peso_neto_mean_muestra { get; set; }

        [Display(Name = "Resultado")]
        public Nullable<bool> muestra_dentro_rango { get; set; }

        [Display(Name = "Inicio Validez BOM")]
        public DateTime fecha_inicio_validez_peso_bom { get; set; }

        [Display(Name = "Fin Validez BOM")]
        public DateTime fecha_fin_validez_peso_bom { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Total Piezas (muestra)")]
        public Nullable<double> total_piezas_muestra { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Diferencia Peso Neto BOM-Mean (muestra)")]
        public Nullable<double> diferencia_peso_neto_bom_mean_muestra
        {
            get
            {
                return Peso_Neto_SAP - peso_neto_mean_muestra;
            }
        }

        [DisplayFormat(DataFormatString = "{0:##0.###}")]
        [Display(Name = "Pzas x diferencia (muestra)")]
        public Nullable<double> piezas_x_diferencia_muestra
        {
            get
            {
                return total_piezas_muestra * diferencia_peso_neto_bom_mean_muestra;
            }
        }


    }


}
