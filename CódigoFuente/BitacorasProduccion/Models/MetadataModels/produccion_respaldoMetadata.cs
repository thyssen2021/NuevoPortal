﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_respaldoMetadata
    {
        [Display(Name = "Planta")]
        public string planta { get; set; }
        [Display(Name = "Línea")]
        public string linea { get; set; }
        [Display(Name = "Operador")]
        public string operador { get; set; }
        [Display(Name = "Supervisor")]
        public string supervisor { get; set; }
        [Display(Name = "SAP Platina")]
        public string sap_platina { get; set; }
        [Display(Name = "Tipo Material")]
        public string tipo_material { get; set; }
        [Display(Name = "Núm. Parte Cliente")]
        public string numero_parte_cliente { get; set; }
        [Display(Name = "SAP Rollo")]
        public string sap_rollo { get; set; }
        [Display(Name = "Material")]
        public string material { get; set; }

        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> fecha { get; set; }
        [Display(Name = "Turno")]
        public string turno { get; set; }
        [Display(Name = "Hora")]
        public Nullable<System.DateTime> hora { get; set; }
        [Display(Name = "Orden SAP")]
        public string orden_sap { get; set; }
        [Display(Name = "Orden SAP 2")]
        public string orden_sap_2 { get; set; }
        [Display(Name = "Piezas por Golpe")]
        public Nullable<double> pieza_por_golpe { get; set; }
        [Display(Name = "Número Rollo")]
        public string numero_rollo { get; set; }
        [Display(Name = "Lote Rollo")]
        public string lote_rollo { get; set; }

        [Display(Name = "Peso Etiqueta")]
        public Nullable<double> peso_etiqueta { get; set; }
        [Display(Name = "Peso Regreso Rollo Real")]
        public Nullable<double> peso_regreso_rollo_real { get; set; }

        [Display(Name = "Peso Rollo Usado")]
        public Nullable<double> peso_rollo_usado { get; set; }
        [Display(Name = "Peso Báscula Kgs")]
        public Nullable<double> peso_bascula_kgs { get; set; }
        [Display(Name = "Piezas por Paquete")]
        public Nullable<double> piezas_por_paquete { get; set; }
        [Display(Name = "Total piezas")]
        public Nullable<double> total_piezas { get; set; }
        [Display(Name = "Peso Rollo Consumido")]
        public Nullable<double> peso_rollo_consumido { get; set; }
        [Display(Name = "Número de Golpes")]
        public Nullable<double> numero_golpes { get; set; }
        [Display(Name = "Peso Despunte Kgs")]
        public Nullable<double> peso_despunte_kgs { get; set; }
        [Display(Name = "Peso Cola Kgs")]
        public Nullable<double> peso_cola_kgs { get; set; }
        [Display(Name = "Porcentaje Punta y Colas")]
        public Nullable<double> porcentaje_punta_y_colas { get; set; }
        [Display(Name = "Total Piezas Ajuste")]
        public Nullable<double> total_piezas_ajuste { get; set; }
        [Display(Name = "Peso Bruto Kgs")]
        public Nullable<double> peso_bruto_kgs { get; set; }
        [Display(Name = "Peso Real Pieza Bruto")]
        public Nullable<double> peso_real_pieza_bruto { get; set; }
        [Display(Name = "Peso Real Pieza Neto")]
        public Nullable<double> peso_real_pieza_neto { get; set; }
        [Display(Name = "Scrap Natural")]
        public Nullable<double> scrap_natural { get; set; }
        [Display(Name = "Peso Neto SAP")]
        public Nullable<double> peso_neto_sap { get; set; }
        [Display(Name = "Peso Bruto SAP")]
        public Nullable<double> peso_bruto_sap { get; set; }
        [Display(Name = "Balance Scrap")]
        public Nullable<double> balance_scrap { get; set; }
        [Display(Name = "Ordenes por Pieza")]
        public Nullable<double> ordenes_por_pieza { get; set; }
        [Display(Name = "Peso de rollo usado real Kg")]
        public Nullable<double> peso_rollo_usado_real_kgs { get; set; }
        [Display(Name = "Peso Bruto Total Piezas Kgs")]
        public Nullable<double> peso_bruto_total_piezas_kgs { get; set; }
        [Display(Name = "Peso Neto Total Piezas Kgs")]
        public Nullable<double> peso_neto_total_piezas_kgs { get; set; }
        [Display(Name = "Scrap de ingeniería (buenas + Ajuste) Total Piezas Kg")]
        public Nullable<double> scrap_ingenieria_buenas_mas_ajuste { get; set; }
        [Display(Name = "Peso Neto total piezas de ajuste Kgs")]
        public Nullable<double> peso_neto_total_piezas_ajuste { get; set; }
        [Display(Name = "Peso puntas y colas reales Kg")]
        public Nullable<double> peso_punta_y_colas_reales { get; set; }
        [Display(Name = "Balance de Scrap Real")]
        public Nullable<double> balance_scrap_real { get; set; }
    }

    [MetadataType(typeof(produccion_respaldoMetadata))]
    public partial class produccion_respaldo : IEquatable<produccion_respaldo>
    {
        //para realizar la comparacion   
        public bool Equals(produccion_respaldo other)
        {
            if (other is null)
                return false;

            return
                    this.linea == other.linea
                    && this.planta == other.planta
                    && this.hora == other.hora
                    && this.operador == other.operador
                    && this.supervisor == other.supervisor
                    && this.sap_platina == other.sap_platina
                    && this.tipo_material == other.tipo_material
                    && this.numero_parte_cliente == other.numero_parte_cliente
                    && this.sap_rollo == other.sap_rollo
                    && this.material == other.material
                    && this.fecha == other.fecha
                    && this.turno == other.turno
                    && this.orden_sap == other.orden_sap
                    && this.orden_sap_2 == other.orden_sap_2
                    && this.pieza_por_golpe == other.pieza_por_golpe
                    && this.numero_rollo == other.numero_rollo
                    && this.lote_rollo == other.lote_rollo
                    && this.peso_etiqueta == other.peso_etiqueta
                    && this.peso_regreso_rollo_real == other.peso_regreso_rollo_real
                    && this.peso_rollo_usado == other.peso_rollo_usado
                    && this.peso_bascula_kgs == other.peso_bascula_kgs
                    && this.piezas_por_paquete == other.piezas_por_paquete
                    && this.total_piezas == other.total_piezas
                    && this.peso_rollo_consumido == other.peso_rollo_consumido
                    && this.numero_golpes == other.numero_golpes
                    && this.peso_despunte_kgs == other.peso_despunte_kgs
                    && this.peso_cola_kgs == other.peso_cola_kgs
                    && this.porcentaje_punta_y_colas == other.porcentaje_punta_y_colas
                    && this.total_piezas_ajuste == other.total_piezas_ajuste
                    && this.peso_bruto_kgs == other.peso_bruto_kgs
                    && this.peso_real_pieza_bruto == other.peso_real_pieza_bruto
                    && this.peso_real_pieza_neto == other.peso_real_pieza_neto
                    && this.scrap_natural == other.scrap_natural
                    && this.peso_neto_sap == other.peso_neto_sap
                    && this.peso_bruto_sap == other.peso_bruto_sap
                    && this.balance_scrap == other.balance_scrap
                    && this.ordenes_por_pieza == other.ordenes_por_pieza
                    && this.peso_rollo_usado_real_kgs == other.peso_rollo_usado_real_kgs
                    && this.peso_bruto_total_piezas_kgs == other.peso_bruto_total_piezas_kgs
                    && this.peso_neto_total_piezas_kgs == other.peso_neto_total_piezas_kgs
                    && this.scrap_ingenieria_buenas_mas_ajuste == other.scrap_ingenieria_buenas_mas_ajuste
                    && this.peso_neto_total_piezas_ajuste == other.peso_neto_total_piezas_ajuste
                    && this.peso_punta_y_colas_reales == other.peso_punta_y_colas_reales
                    && this.balance_scrap_real == other.balance_scrap_real
                ;
        }

        public override bool Equals(object obj) => Equals(obj as class_v3);
        public override int GetHashCode() => (linea, planta, hora).GetHashCode();
    }
}