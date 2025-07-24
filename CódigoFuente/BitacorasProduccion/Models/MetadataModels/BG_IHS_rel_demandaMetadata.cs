using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class BG_IHS_rel_demandaMetadata
    {
    }

    [MetadataType(typeof(BG_IHS_rel_demandaMetadata))]
    public partial class BG_IHS_rel_demanda
    {
        //Origen de los datos (utilizado por el método GB_IHS_item.getDemanda())
        [NotMapped]
        public string origen_datos { get; set; }


        // obtiene el responsable principal
        [NotMapped]
        [Display(Name = "placeholder")]
        public string placeholder_user
        {
            get
            {
                using (var db = new Portal_2_0Entities())
                {
                    try
                    {
                        string placeholder = db.BG_IHS_rel_demanda.AsNoTracking().Where(x => x.fecha == this.fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL && x.id_ihs_item == this.id_ihs_item).Select(x => x.cantidad).FirstOrDefault().ToString();
                        //si existe un registro para la misma fecha de tipo IHS la muestra como placeholder y si no existe
                        return !String.IsNullOrEmpty(placeholder) ? placeholder : "0";

                    }
                    catch
                    {
                        return "0";
                    }

                }

            }

        }

    }
}