using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class view_valores_forecastMetadata
    {
    }

    [MetadataType(typeof(view_valores_forecastMetadata))]
    public partial class view_valores_forecast : IEquatable<view_valores_forecast>
    {
        [NotMapped]
        private Portal_2_0Entities db = new Portal_2_0Entities();

        //para elementos de BD
        [NotMapped]
        public budget_cuenta_sap CUENTA_SAP = null;
        [NotMapped]
        public budget_anio_fiscal ANIO_FISCAL = null;
        [NotMapped]
        public budget_centro_costo CENTRO_COSTO = null;
        [NotMapped]
        public currency CURRENCY = null;

        public budget_anio_fiscal AnioFiscal()
        {

            if (ANIO_FISCAL == null)
            {
                budget_anio_fiscal item = db.budget_anio_fiscal.Find(id_anio_fiscal);
                ANIO_FISCAL = item;
            }

            return ANIO_FISCAL;

        }

        public budget_centro_costo CentroCosto()
        {

            if (CENTRO_COSTO == null)
            {
                budget_centro_costo item = db.budget_centro_costo.Find(id_centro_costo);
                CENTRO_COSTO = item;
            }

            return CENTRO_COSTO;

        }

        public budget_cuenta_sap CuentaSap()
        {

            if (CUENTA_SAP == null)
            {
                budget_cuenta_sap item = db.budget_cuenta_sap.Find(id_cuenta_sap);
                CUENTA_SAP = item;
            }

            return CUENTA_SAP;

        }

        public currency Currency()
        {

            if (CURRENCY == null)
            {
                currency item = db.currency.Find(currency_iso);
                CURRENCY = item;
            }

            return CURRENCY;

        }

        public Nullable<decimal> TotalMesesUSD()
        {

            return (Enero.HasValue ? Enero.Value : 0)
                + (Febrero.HasValue ? Febrero.Value : 0)
                + (Marzo.HasValue ? Marzo.Value : 0)
                + (Abril.HasValue ? Abril.Value : 0)
                + (Mayo.HasValue ? Mayo.Value : 0)
                + (Junio.HasValue ? Junio.Value : 0)
                + (Julio.HasValue ? Julio.Value : 0)
                + (Agosto.HasValue ? Agosto.Value : 0)
                + (Septiembre.HasValue ? Septiembre.Value : 0)
                + (Octubre.HasValue ? Octubre.Value : 0)
                + (Noviembre.HasValue ? Noviembre.Value : 0)
                + (Diciembre.HasValue ? Diciembre.Value : 0);

        }
        public Nullable<decimal> TotalMesesMXN()
        {

            return (Enero_MXN.HasValue ? Enero_MXN.Value : 0)
                + (Febrero_MXN.HasValue ? Febrero_MXN.Value : 0)
                + (Marzo_MXN.HasValue ? Marzo_MXN.Value : 0)
                + (Abril_MXN.HasValue ? Abril_MXN.Value : 0)
                + (Mayo_MXN.HasValue ? Mayo_MXN.Value : 0)
                + (Junio_MXN.HasValue ? Junio_MXN.Value : 0)
                + (Julio_MXN.HasValue ? Julio_MXN.Value : 0)
                + (Agosto_MXN.HasValue ? Agosto_MXN.Value : 0)
                + (Septiembre_MXN.HasValue ? Septiembre_MXN.Value : 0)
                + (Octubre_MXN.HasValue ? Octubre_MXN.Value : 0)
                + (Noviembre_MXN.HasValue ? Noviembre_MXN.Value : 0)
                + (Diciembre_MXN.HasValue ? Diciembre_MXN.Value : 0);

        }
        public Nullable<decimal> TotalMesesEUR()
        {

            return (Enero_EUR.HasValue ? Enero_EUR.Value : 0)
                + (Febrero_EUR.HasValue ? Febrero_EUR.Value : 0)
                + (Marzo_EUR.HasValue ? Marzo_EUR.Value : 0)
                + (Abril_EUR.HasValue ? Abril_EUR.Value : 0)
                + (Mayo_EUR.HasValue ? Mayo_EUR.Value : 0)
                + (Junio_EUR.HasValue ? Junio_EUR.Value : 0)
                + (Julio_EUR.HasValue ? Julio_EUR.Value : 0)
                + (Agosto_EUR.HasValue ? Agosto_EUR.Value : 0)
                + (Septiembre_EUR.HasValue ? Septiembre_EUR.Value : 0)
                + (Octubre_EUR.HasValue ? Octubre_EUR.Value : 0)
                + (Noviembre_EUR.HasValue ? Noviembre_EUR.Value : 0)
                + (Diciembre_EUR.HasValue ? Diciembre_EUR.Value : 0);

        }
        public Nullable<decimal> TotalMesesUSD_Local()
        {

            return (Enero_USD_LOCAL.HasValue ? Enero_USD_LOCAL.Value : 0)
                + (Febrero_USD_LOCAL.HasValue ? Febrero_USD_LOCAL.Value : 0)
                + (Marzo_USD_LOCAL.HasValue ? Marzo_USD_LOCAL.Value : 0)
                + (Abril_USD_LOCAL.HasValue ? Abril_USD_LOCAL.Value : 0)
                + (Mayo_USD_LOCAL.HasValue ? Mayo_USD_LOCAL.Value : 0)
                + (Junio_USD_LOCAL.HasValue ? Junio_USD_LOCAL.Value : 0)
                + (Julio_USD_LOCAL.HasValue ? Julio_USD_LOCAL.Value : 0)
                + (Agosto_USD_LOCAL.HasValue ? Agosto_USD_LOCAL.Value : 0)
                + (Septiembre_USD_LOCAL.HasValue ? Septiembre_USD_LOCAL.Value : 0)
                + (Octubre_USD_LOCAL.HasValue ? Octubre_USD_LOCAL.Value : 0)
                + (Noviembre_USD_LOCAL.HasValue ? Noviembre_USD_LOCAL.Value : 0)
                + (Diciembre_USD_LOCAL.HasValue ? Diciembre_USD_LOCAL.Value : 0);

        }

        public bool Equals(view_valores_forecast other)
        {
            if (other is null)
                return false;

            return this.id_anio_fiscal == other.id_anio_fiscal
               && this.id_centro_costo == other.id_centro_costo
               && this.id_cuenta_sap == other.id_cuenta_sap
               && this.currency_iso == other.currency_iso
               ;
        }

        public string GetComentario(int id_budget_rel_fy_centro, int id_cuenta_sap, int mes)
        {
            //optimizar!!!!!!!!!!!!!
            string comentario = string.Empty;
            using (var db = new Portal_2_0Entities())
            {
                var cantidad = db.budget_cantidad_forecast.FirstOrDefault(x => x.id_budget_rel_fy_centro == id_budget_rel_fy_centro && x.id_cuenta_sap == id_cuenta_sap && x.mes == mes);

                if (cantidad != null)
                    return cantidad.comentario;
            }
            return comentario;
        }

        public override bool Equals(object obj) => Equals(obj as view_valores_forecast);
        public override int GetHashCode() => (id_anio_fiscal, id_centro_costo, id_cuenta_sap, currency_iso).GetHashCode();

    }
}