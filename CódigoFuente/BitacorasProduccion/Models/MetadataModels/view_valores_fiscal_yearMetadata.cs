using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class view_valores_fiscal_yearMetadata
    {
    }

    [MetadataType(typeof(view_valores_fiscal_yearMetadata))]
    public partial class view_valores_fiscal_year : IEquatable<view_valores_fiscal_year>
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

        public Nullable<decimal> TotalMeses()
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

        public bool Equals(view_valores_fiscal_year other)
        {
            if (other is null)
                return false;

            return this.id_anio_fiscal == other.id_anio_fiscal
               && this.id_centro_costo == other.id_centro_costo
               && this.id_cuenta_sap == other.id_cuenta_sap
               && this.currency_iso == other.currency_iso
               ;
        }

        public override bool Equals(object obj) => Equals(obj as view_valores_fiscal_year);
        public override int GetHashCode() => (id_anio_fiscal, id_centro_costo, id_cuenta_sap, currency_iso).GetHashCode();

    }
}