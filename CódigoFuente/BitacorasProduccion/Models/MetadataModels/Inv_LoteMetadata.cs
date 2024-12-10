using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class Inv_LoteMetadata
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    [MetadataType(typeof(Inv_LoteMetadata))]
    public partial class Inv_Lote
    {
        public decimal? PiezasMax
        {
            get
            {
                if (Inv_Material?.Espesor == null || Inv_Material?.EspesorMin == null || Pieces == null || Inv_Material.EspesorMin == 0)
                {
                    return null; // Retorna null si algún valor es null o EspesorMin es 0
                }

                return Math.Round((decimal)((Pieces.Value * Inv_Material.Espesor.Value) / Inv_Material.EspesorMin.Value), 3);
            }
        }

        public decimal? PiezasMin
        {
            get
            {
                if (Inv_Material?.Espesor == null || Inv_Material?.EspesorMax == null || Pieces == null || Inv_Material.EspesorMax == 0)
                {
                    return null; // Retorna null si algún valor es null o EspesorMax es 0
                }

                return Math.Round((decimal)((Pieces.Value * Inv_Material.Espesor.Value) / Inv_Material.EspesorMax.Value), 3);
            }
        }
    }
   
}