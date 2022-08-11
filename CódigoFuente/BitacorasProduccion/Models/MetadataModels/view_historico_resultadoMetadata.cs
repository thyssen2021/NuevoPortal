using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class view_historico_resultadoMetadata
    {
        [Display(Name = "Operador")]
        public string Operador { get; set; }

        [Display(Name = "Supervisor")]
        public string Supervisor { get; set; }

        [Display(Name = "SAP Platina")]
        public string SAP_Platina { get; set; }

        [Display(Name = "Tipo de Material")]
        public string Tipo_de_Material { get; set; }

        [Display(Name = "Número Parte Cliente")]
        public string Número_de_Parte__de_cliente { get; set; }

        [Display(Name = "SAP Rollo")]
        public string SAP_Rollo { get; set; }

        [Display(Name = "Material")]
        public string Material { get; set; }

        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> Fecha { get; set; }

        [Display(Name = "Turno")]
        public string Turno { get; set; }

        [Display(Name = "Hora")]
        public Nullable<System.DateTime> Hora { get; set; }

        [Display(Name = "Orden SAP")]
        public string Orden_SAP { get; set; }

        [Display(Name = "Orden SAP 2")]
        public string Orden_en_SAP_2 { get; set; }

        [Display(Name = "Pieza por Golpe")]
        public Nullable<double> Pieza_por_Golpe { get; set; }

        [Display(Name = "Núm. Rollo")]
        public string N__de_Rollo { get; set; }

        [Display(Name = "Lote de Rollo")]
        public string Lote_de_rollo { get; set; }

        [Display(Name = "Peso Etiqueta (kg)")]
        public Nullable<double> Peso_Etiqueta__Kg_ { get; set; }

        [Display(Name = "Peso Regreso Rollo Real")]
        public Nullable<double> Peso_de_regreso_de_rollo_Real { get; set; }

        [Display(Name = "Peso Rollo Usado")]
        public Nullable<double> Peso_de_rollo_usado { get; set; }

        [Display(Name = "Peso Báscula")]
        public Nullable<double> Peso_Báscula_Kgs { get; set; }

        [Display(Name = "Piezas por paquete")]
        public Nullable<double> Piezas_por_paquete { get; set; }

        [Display(Name = "Total de Piezas")]
        public Nullable<double> Total_de_piezas { get; set; }

        [Display(Name = "Peso de Rollo Consumido")]
        public Nullable<double> Peso_de_rollo_consumido { get; set; }

        [Display(Name = "Núm. de Golpes")]
        public Nullable<double> Numero_de_golpes { get; set; }

        [Display(Name = "Kg restante de Rollo")]
        public Nullable<double> Kg_restante_de_rollo { get; set; }

        [Display(Name = "Peso Despunte Kgs")]
        public Nullable<double> Peso_despunte_kgs_ { get; set; }

        [Display(Name = "Peso Cola Kgs")]
        public Nullable<double> Peso_cola_Kgs_ { get; set; }

        [Display(Name = "% Punta y Colas")]
        public Nullable<double> Porcentaje_de_puntas_y_colas { get; set; }

        [Display(Name = "Total Piezas Ajuste")]
        public Nullable<double> Total_de_piezas_de_Ajustes { get; set; }

        [Display(Name = "Peso Bruto kgs (Piezas Ajuste - Platina 1)")]
        public Nullable<double> Peso_Bruto_Kgs { get; set; }

        [Display(Name = "Peso Real Pieza Bruto")]
        public Nullable<double> Peso_Real_Pieza_Bruto { get; set; }

        [Display(Name = "Peso Real Pieza Neto")]
        public Nullable<double> Peso_Real_Pieza_Neto { get; set; }

        [Display(Name = "Scrap Natural")]
        public Nullable<double> Scrap_Natural { get; set; }

        [Display(Name = "Peso Neto SAP")]
        public Nullable<double> Peso_neto_SAP { get; set; }

        [Display(Name = "Peso Bruto SAP")]
        public Nullable<double> Peso_Bruto_SAP { get; set; }

        [Display(Name = "Balance de Scrap")]
        public Nullable<double> Balance_de_Scrap { get; set; }

        [Display(Name = "Línea")]
        public string Linea { get; set; }

        [Display(Name = "Planta")]
        public string Planta { get; set; }

        [Display(Name = "Año")]
        public Nullable<int> Anio { get; set; }

        [Display(Name = "Id")]
        public Nullable<double> Column40 { get; set; }

        [Display(Name = "Órdenes por piezas")]
        public Nullable<double> Ordenes_por_pieza { get; set; }

        [Display(Name = "Peso de Rollo Usado Real")]
        public Nullable<double> Peso_de_rollo_usado_real__Kg { get; set; }

        [Display(Name = "Peso Bruto Total Piezas Kg")]
        public Nullable<double> Peso_bruto_Total_piezas_Kg { get; set; }

        [Display(Name = "Peso Neto Total Piezas")]
        public Nullable<double> Peso_NetoTotal_piezas_Kg { get; set; }

        [Display(Name = "Scrap de Ingieniería (buenas + ajuste) Total Piezas")]
        public Nullable<double> Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg { get; set; }

        [Display(Name = "Peso Neto Total Piezas de Ajuste Kgs")]
        public Nullable<double> Peso_Neto_total_piezas_de_ajuste_Kgs { get; set; }

        [Display(Name = "Peso Punta y Colas Reales Kgs")]
        public Nullable<double> Peso_puntas_y_colas_reales_Kg { get; set; }

        [Display(Name = "Balance de Scrap Real")]
        public Nullable<double> Balance_de_Scrap_Real { get; set; }
        public string comentario { get; set; }

        [Display(Name = "SAP Platina 2")]
        public string SAP_Platina_2 { get; set; }

        //nuevos campos
        [Display(Name = "Tipo de Material (platina 2)")]
        public string Tipo_de_Material_platina2 { get; set; }

        [Display(Name = "Número de Parte Cliente (platina 2)")]
        public string Número_de_Parte_de_Cliente_platina2 { get; set; }

        [Display(Name = "Material (platina 2)")]
        public string Material_platina2 { get; set; }

        [Display(Name = "Total Piezas Platina 1")]
        public Nullable<int> Total_de_piezas_platina1 { get; set; }

        [Display(Name = "Total Piezas Platina 2")]
        public Nullable<int> Total_de_piezas_platina2 { get; set; }

        [Display(Name = "Total Piezas Ajuste (Platina 1)")]
        public Nullable<int> Total_de_piezas_de_Ajustes_platina1 { get; set; }

        [Display(Name = "Total Piezas Ajuste (Platina 2)")]
        public Nullable<int> Total_de_piezas_de_Ajustes_platina2 { get; set; }

        [Display(Name = "Peso Bruto kgs (Piezas Ajuste - Platina 2)")]
        public Nullable<double> Peso_Bruto_Kgs_platina2 { get; set; }

        [Display(Name = "Peso Real Pieza Bruto (Platina 2)")]
        public Nullable<double> Peso_Real_Pieza_Bruto_platina2 { get; set; }

        [Display(Name = "Peso Real Pieza Neto (Platina 2)")]
        public Nullable<double> Peso_Real_Pieza_Neto_platina2 { get; set; }

        [Display(Name = "Scrap Natural (Platina 2)")]
        public Nullable<double> Scrap_Natural_platina2 { get; set; }

        [Display(Name = "Peso Neto SAP (Platina 2)")]
        public Nullable<double> Peso_neto_SAP_platina2 { get; set; }

        [Display(Name = "Peso Bruto SAP (Platina 2)")]
        public Nullable<double> Peso_Bruto_SAP_platina2 { get; set; }

        [Display(Name = "Balance de Scrap (Platina 2)")]
        public Nullable<double> Balance_de_Scrap_platina2 { get; set; }

        [Display(Name = "Peso de Rollo Usado Real (Platina 2)")]
        public Nullable<double> Peso_de_rollo_usado_real__Kg_platina2 { get; set; }

        [Display(Name = "Peso Bruto Total Piezas Kg (Platina 2)")]
        public Nullable<double> Peso_bruto_Total_piezas_Kg_platina2 { get; set; }

        [Display(Name = "Peso Neto Total Piezas (Platina 2)")]
        public Nullable<double> Peso_NetoTotal_piezas_Kg_platina2 { get; set; }

        [Display(Name = "Scrap de Ingieniería (buenas + ajuste) Total Piezas (Platina 2)")]
        public Nullable<double> Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2 { get; set; }

        [Display(Name = "Peso Neto Total Piezas de Ajuste Kgs (Platina 2)")]
        public Nullable<double> Peso_Neto_total_piezas_de_ajuste_Kgs_platina2 { get; set; }

        [Display(Name = "Peso Punta y Colas Reales Kgs (Platina 2)")]
        public Nullable<double> Peso_puntas_y_colas_reales_Kg_platina2 { get; set; }

        [Display(Name = "Balance de Scrap Real (Platina 2)")]
        public Nullable<double> Balance_de_Scrap_Real_platina2 { get; set; }

        [Display(Name = "Peso Bruto kgs (Piezas Ajuste - General)")]
        public Nullable<double> Peso_Bruto_Kgs_general { get; set; }

        [Display(Name = "Peso Real Pieza Bruto (General)")]
        public Nullable<double> Peso_Real_Pieza_Bruto_general { get; set; }

        [Display(Name = "Peso Real Pieza Neto (General)")]
        public Nullable<double> Peso_Real_Pieza_Neto_general { get; set; }

        [Display(Name = "Scrap Natural (General)")]
        public Nullable<double> Scrap_Natural_general { get; set; }

        [Display(Name = "Peso Neto SAP (General)")]
        public Nullable<double> Peso_neto_SAP_general { get; set; }

        [Display(Name = "Peso Bruto SAP (General)")]
        public Nullable<double> Peso_Bruto_SAP_general { get; set; }

        [Display(Name = "Balance de Scrap (General)")]
        public Nullable<double> Balance_de_Scrap_general { get; set; }

        [Display(Name = "Peso de Rollo Usado Real (General)")]
        public Nullable<double> Peso_de_rollo_usado_real__Kg_general { get; set; }

        [Display(Name = "Peso Bruto Total Piezas Kg (General)")]
        public Nullable<double> Peso_bruto_Total_piezas_Kg_general { get; set; }

        [Display(Name = "Peso Neto Total Piezas (General)")]
        public Nullable<double> Peso_NetoTotal_piezas_Kg_general { get; set; }

        [Display(Name = "Scrap de Ingieniería (buenas + ajuste) Total Piezas (General)")]
        public Nullable<double> Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_general { get; set; }

        [Display(Name = "Peso Neto Total Piezas de Ajuste Kgs (General)")]
        public Nullable<double> Peso_Neto_total_piezas_de_ajuste_Kgs_general { get; set; }

        [Display(Name = "Peso Punta y Colas Reales Kgs (General)")]
        public Nullable<double> Peso_puntas_y_colas_reales_Kg_general { get; set; }

        [Display(Name = "Balance de Scrap Real (General)")]
        public Nullable<double> Balance_de_Scrap_Real_general { get; set; }

    }

    [MetadataType(typeof(view_historico_resultadoMetadata))]
    public partial class view_historico_resultado
    {
        //retorna el IdRegistro
        [NotMapped]
        public int? IdRegistro
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.Column40.Value);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}