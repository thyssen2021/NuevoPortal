using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    // Modelo principal (aunque no lo usaremos para pedir input, es bueno tenerlo)
    public class SapDetailsQueryViewModel
    {
        public string MaterialNumber { get; set; }
        public string Plant { get; set; }
        public bool GetBatchChars { get; set; } = false;
        public bool QueryExecuted { get; set; } = false;
        public string ErrorMessage { get; set; }
        public List<MaterialDetailsViewModel> Materials { get; set; } = new List<MaterialDetailsViewModel>();
    }

    // Representa la estructura ZSTR_RFC_MATERIAL_DETAILS
    public class MaterialDetailsViewModel
    {
        public string Matnr { get; set; }
        public string Mtart { get; set; }
        public string Bismt { get; set; }
        public decimal Brgew { get; set; }
        public decimal Ntgwe { get; set; }
        public string Meins { get; set; }
        public string Groes { get; set; }
        public string Zzreappl { get; set; }
        public string Zzmtltyp { get; set; }
        public string Zzselltyp { get; set; }
        public string Zzbussmodl { get; set; }
        public string Vmsta { get; set; }
        public string Gewei { get; set; }
        public string Zzmattype { get; set; }
        public string Zzcustscrp { get; set; }
        public string Zzengscrp { get; set; }
        public string Zzihsnum1 { get; set; }
        public string Zzihsnum2 { get; set; }
        public string Zzihsnum3 { get; set; }
        public string Zzihsnum4 { get; set; }
        public string Zzihsnum5 { get; set; }
        public string Zzpkgpcs { get; set; }
        public decimal? Zzthcknss { get; set; }
        public decimal? Zzwidth { get; set; }
        public decimal? Zzadvance { get; set; }
        public decimal? Zzhtalscrp { get; set; }
        public decimal? Zzcarpcs { get; set; }
        public decimal? Zzinitwt { get; set; }
        public decimal? Zzminwt { get; set; }
        public decimal? Zzmaxwt { get; set; }
        public string Zzstkpcs { get; set; }
        public decimal? Zzanglea { get; set; }
        public decimal? Zzangleb { get; set; }
        public decimal? Zzrealntwt { get; set; }
        public decimal? Zzrealgrwt { get; set; }
        public string Zzdouopcs { get; set; }
        public string Zzcoilsltpos { get; set; }
        public decimal? Zzmxwttolp { get; set; }
        public decimal? Zzmxwttoln { get; set; }
        public decimal? Zzmnwttolp { get; set; }
        public decimal? Zzmnwttoln { get; set; }
        public string Zzwh { get; set; }
        public string Zztransp { get; set; }
        public string Zztkmmsop { get; set; }
        public string Zztkmmeop { get; set; }
        public string Zzppackage { get; set; }
        public string Zzspackage { get; set; }
        public string Zzpallet { get; set; }
        public string Zzstamd { get; set; }
        public string Zzidpnum { get; set; }
        public string Zzidtool { get; set; }
        public string Zzidobsol { get; set; }
        public string Zztourd { get; set; }
        public string Zztolmaxwt { get; set; }
        public string Zztolminwt { get; set; }

        public List<DescriptionDataViewModel> Descriptions { get; set; } = new List<DescriptionDataViewModel>();
        public List<PlantDataViewModel> Plants { get; set; } = new List<PlantDataViewModel>();
        public List<CharDataViewModel> Characteristics { get; set; } = new List<CharDataViewModel>();
        public List<BatchDataViewModel> Batches { get; set; } = new List<BatchDataViewModel>();
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
        public DateTime? Valid_From { get; set; }
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

    // Representa ZSTR_RFC_BATCH_DATA
    public class BatchDataViewModel
    {
        public string Charg { get; set; }
        public string Werks { get; set; }
        public DateTime? Ersda { get; set; }
        public DateTime? Vfdat { get; set; }
        public List<CharDataViewModel> BatchChars { get; set; } = new List<CharDataViewModel>();
    }
}