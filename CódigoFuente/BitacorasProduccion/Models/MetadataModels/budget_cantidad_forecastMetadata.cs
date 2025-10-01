using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_cantidad_forecastMetadata
    {
        
    }

    [MetadataType(typeof(budget_cantidad_forecastMetadata))]
    public partial class budget_cantidad_forecast
    {
        //para binding de ajax
        public string numero_cuenta_sap { get; set; }

        // Constructor que recibe un budget_cantidad
        public budget_cantidad_forecast(budget_cantidad bc)
        {
            this.id = bc.id;
            this.id_budget_rel_fy_centro = bc.id_budget_rel_fy_centro;
            this.id_cuenta_sap = bc.id_cuenta_sap;
            this.mes = bc.mes;
            this.currency_iso = bc.currency_iso;
            this.cantidad = bc.cantidad;
            this.comentario = bc.comentario;
            this.moneda_local_usd = bc.moneda_local_usd;
        }
    }
}