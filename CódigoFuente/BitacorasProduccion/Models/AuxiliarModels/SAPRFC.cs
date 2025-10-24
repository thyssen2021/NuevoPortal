using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    // Modelo principal para la nueva página
    public class SapDetailsQueryViewModel
    {
        [Display(Name = "Número de Material")]
        public string MaterialNumber { get; set; }

        [Display(Name = "Planta (Opcional)")]
        public string Plant { get; set; }

        // Propiedades de control
        public bool QueryExecuted { get; set; } = false;
        public string ErrorMessage { get; set; }

        // La lista de resultados que llenaremos desde SAP
        public List<MaterialDetailsViewModel> Materials { get; set; } = new List<MaterialDetailsViewModel>();
    }

    // Representa la estructura principal ZSTR_RFC_MATERIAL_DETAILS
    public class MaterialDetailsViewModel
    {
        public string Matnr { get; set; }
        public string Mtart { get; set; }
        public string Bismt { get; set; }
        public decimal Brgew { get; set; }
        public decimal Ntgwe { get; set; }
        public string Meins { get; set; }        // Base Unit of measure
        public string Groes { get; set; }        // Size/dimensions
        public string Zzreappl { get; set; }     // Flag: Re-Application
        public string Zzmtltyp { get; set; }     // Type of Metal
        public string Zzselltyp { get; set; }    // Selling type
        public string Zzbussmodl { get; set; }   // Business model
        public string Vmsta { get; set; } // Distribution-chain-specific status
        public string Gewei { get; set; }        // Weight Unit
        public string Zzmattype { get; set; }   // Type of Material
        public string Zzcustscrp { get; set; }  // Head and Tails Scrap Conciliation
        public string Zzengscrp { get; set; }   // Engineering Scrap conciliation
        public string Zzihsnum1 { get; set; }   // Vehicle number 1
        public string Zzihsnum2 { get; set; }   // Vehicle number 2
        public string Zzihsnum3 { get; set; }   // Vehicle number 3
        public string Zzihsnum4 { get; set; }   // Propulsion System Design
        public string Zzihsnum5 { get; set; }   // S&P Program
        public string Zzpkgpcs { get; set; }     // Piezas por paquete (NUMC -> string)
        public decimal? Zzthcknss { get; set; }  // Thickness (budget) (DEC -> decimal?)
        public decimal? Zzwidth { get; set; }    // Width (budget) (DEC -> decimal?)
        public decimal? Zzadvance { get; set; }  // Advance (budget) (DEC -> decimal?)
        public decimal? Zzhtalscrp { get; set; } // Head and Tail allowed scrap (DEC -> decimal?)
        public decimal? Zzcarpcs { get; set; }   // Pieces per car (DEC -> decimal?)
        public decimal? Zzinitwt { get; set; }   // Initial Weight (DEC -> decimal?)
        public decimal? Zzminwt { get; set; }    // Min Weight (DEC -> decimal?)
        public decimal? Zzmaxwt { get; set; }    // Maximum Weight (DEC -> decimal?)
        public string Zzstkpcs { get; set; }     // Stroke Pieces (CHAR -> string)
        public decimal? Zzanglea { get; set; }   // Angulo A (DEC -> decimal?)
        public decimal? Zzangleb { get; set; }   // Angulo B (DEC -> decimal?)

        public decimal? Zzrealntwt { get; set; }     // Real Net Weight (KG)
        public decimal? Zzrealgrwt { get; set; }     // Real gross weight (KG)
        public string Zzdouopcs { get; set; }      // Double Pieces
        public string Zzcoilsltpos { get; set; }    // Coil/Slitter position
        public decimal? Zzmxwttolp { get; set; }     // Maximum Weight Tolerance - Positive
        public decimal? Zzmxwttoln { get; set; }     // Maximum weight tolerance - Negative
        public decimal? Zzmnwttolp { get; set; }     // Minimum weight tolerance - Positive
        public decimal? Zzmnwttoln { get; set; }     // Minimum Weight Tolerance Negative
        public string Zzwh { get; set; }           // Almacen Norte
        public string Zztransp { get; set; }       // Tipo Transporte
        public string Zztkmmsop { get; set; }      // tkMM SOP
        public string Zztkmmeop { get; set; }      // tkMM EOP
        public string Zzppackage { get; set; }     // Piezas por paquete
        public string Zzspackage { get; set; }     // Stacks per package
        public string Zzpallet { get; set; }       // Type of pallet
        public string Zzstamd { get; set; }        // Status DM
        public string Zzidpnum { get; set; }       // ID Part Number
        public string Zzidtool { get; set; }       // ID Tool
        public string Zzidobsol { get; set; }      // ID Material Obsoleto
        public string Zztourd { get; set; }        // Tour Description
        public string Zztolmaxwt { get; set; }     // Tolerance of Maximum Weight
        public string Zztolminwt { get; set; }     // Tolerance of Minimum Weight

        public List<DescriptionDataViewModel> Descriptions { get; set; } = new List<DescriptionDataViewModel>();
        public List<PlantDataViewModel> Plants { get; set; } = new List<PlantDataViewModel>();
        public List<CharDataViewModel> Characteristics { get; set; } = new List<CharDataViewModel>();
    }

    // Representa ZSTR_RFC_DESC_DATA
    public class DescriptionDataViewModel
    {
        public string Spras { get; set; }
        public string Maktx { get; set; }
    }

    // Representa ZSTR_RFC_PLANT_DATA
    public class PlantDataViewModel
    {
        public string Werks { get; set; }
        public string Mmsta { get; set; }
        public List<BomItemViewModel> BomItems { get; set; } = new List<BomItemViewModel>();

        // ++ NUEVAS PROPIEDADES PARA PESOS CALCULADOS ++
        [Display(Name = "Peso Bruto Calculado (BOM)")]
        public decimal CalculatedGrossWeight { get; set; }

        [Display(Name = "Peso Neto Calculado (BOM)")]
        public decimal CalculatedNetWeight { get; set; }
    }

    // Representa ZSTR_RFC_BOM_ITEM
    public class BomItemViewModel
    {
        public string Alt_Bom { get; set; }
        public string Item_No { get; set; }
        public string Component { get; set; }
        public string Comp_Desc { get; set; }
        public decimal Quantity { get; set; }
        public string Uom { get; set; }
        public DateTime? Valid_From { get; set; } // Usamos DateTime? (nullable)
        public DateTime? Created_On { get; set; }
    }

    // Representa ZSTR_RFC_CHAR_DATA
    public class CharDataViewModel
    {
        public string Charact { get; set; }
        public string Charact_Desc_En { get; set; }
        public string Charact_Desc_Es { get; set; }
        public string Value_Internal { get; set; }
        public string Value_Desc_En { get; set; }
        public string Value_Desc_Es { get; set; }
        public string Unit { get; set; }
    }
}