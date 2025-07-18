using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Portal_2_0.Models
{

    public class BG_IHS_itemMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Core Nameplate Region Mnemonic")]
        [MaxLength(15)]
        public string core_nameplate_region_mnemonic { get; set; }

        [Display(Name = "Core Nameplate Plant Mnemonic")]
        [MaxLength(15)]
        public string core_nameplate_plant_mnemonic { get; set; }

        [Display(Name = "Mnemonic-Vehicle")]
        [MaxLength(15)]
        public string mnemonic_vehicle { get; set; }

        [Required]
        [Display(Name = "Mnemonic-Vehicle/Plant")]
        [MaxLength(100)]
        public string mnemonic_vehicle_plant { get; set; }

        [Display(Name = "Mnemonic-Platform")]
        [MaxLength(15)]
        public string mnemonic_platform { get; set; }

        [Display(Name = "Mnemonic-Plant")]
        [MaxLength(15)]
        public string mnemonic_plant { get; set; }

        [Display(Name = "Region")]
        [MaxLength(25)]
        public string region { get; set; }

        [Display(Name = "Market")]
        [MaxLength(10)]
        public string market { get; set; }

        [Display(Name = "Country/Territory")]
        [MaxLength(30)]
        public string country_territory { get; set; }

        [Required]
        [Display(Name = "Production Plant")]
        [MaxLength(50)]
        public string production_plant { get; set; }

        [Display(Name = "City")]
        [MaxLength(50)]
        public string city { get; set; }

        [Display(Name = "Plant State/Province")]
        [MaxLength(50)]
        public string plant_state_province { get; set; }

        [Display(Name = "Source Plant")]
        [MaxLength(60)]
        public string source_plant { get; set; }

        [Display(Name = "Source Plant Country/Territory")]
        [MaxLength(30)]
        public string source_plant_country_territory { get; set; }

        [Display(Name = "Source Plant Region")]
        [MaxLength(30)]
        public string source_plant_region { get; set; }

        [Display(Name = "Design Parent")]
        [MaxLength(50)]
        public string design_parent { get; set; }

        [Display(Name = "Engineering Group")]
        [MaxLength(30)]
        public string engineering_group { get; set; }

        [Display(Name = "Manufacturer Group")]
        [MaxLength(30)]
        public string manufacturer_group { get; set; }

        [Display(Name = "Manufacturer")]
        [MaxLength(30)]
        public string manufacturer { get; set; }

        [Display(Name = "Sales Parent")]
        [MaxLength(30)]
        public string sales_parent { get; set; }

        [Display(Name = "Production Brand")]
        [MaxLength(30)]
        public string production_brand { get; set; }

        [Display(Name = "Platform Design Owner")]
        [MaxLength(30)]
        public string platform_design_owner { get; set; }

        [Display(Name = "Architecture")]
        [MaxLength(30)]
        public string architecture { get; set; }

        [Display(Name = "Platform")]
        [MaxLength(40)]
        public string platform { get; set; }

        [Display(Name = "Program")]
        [MaxLength(30)]
        public string program { get; set; }

        [Display(Name = "Production Nameplate")]
        [MaxLength(30)]
        public string production_nameplate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "SOP (Start of Production)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime sop_start_of_production { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "EOP (End of Production)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public System.DateTime eop_end_of_production { get; set; }

        [Display(Name = "Lifecycle (Time)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Ingrese un número válido.")]
        public int lifecycle_time { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        [MaxLength(100)]
        public string vehicle { get; set; }

        [Display(Name = "Assembly Type")]
        [MaxLength(40)]
        public string assembly_type { get; set; }

        [Display(Name = "Strategic Group")]
        [MaxLength(40)]
        public string strategic_group { get; set; }

        [Display(Name = "Sales Group")]
        [MaxLength(40)]
        public string sales_group { get; set; }

        [Display(Name = "Global Nameplate")]
        [MaxLength(40)]
        public string global_nameplate { get; set; }

        [Display(Name = "Primary Design Center")]
        [MaxLength(40)]
        public string primary_design_center { get; set; }

        [Display(Name = "Primary Design Country/Territory")]
        [MaxLength(40)]
        public string primary_design_country_territory { get; set; }

        [Display(Name = "Primary Design Region")]
        [MaxLength(40)]
        public string primary_design_region { get; set; }

        [Display(Name = "Secondary Design Center")]
        [MaxLength(40)]
        public string secondary_design_center { get; set; }

        [Display(Name = "Secondary Design Country/Territory")]
        [MaxLength(40)]
        public string secondary_design_country_territory { get; set; }

        [Display(Name = "Secondary Design Region")]
        [MaxLength(40)]
        public string secondary_design_region { get; set; }

        [Display(Name = "GVW Rating")]
        [MaxLength(30)]
        public string gvw_rating { get; set; }

        [Display(Name = "GVW Class")]
        [MaxLength(30)]
        public string gvw_class { get; set; }

        [Display(Name = "Car/Truck")]
        [MaxLength(30)]
        public string car_truck { get; set; }

        [Display(Name = "Production Type")]
        [MaxLength(10)]
        public string production_type { get; set; }

        [Required]
        [Display(Name = "Global Production Segment")]
        [MaxLength(30)]
        public string global_production_segment { get; set; }

        [Display(Name = "Regional Sales Segment")]
        [MaxLength(30)]
        public string regional_sales_segment { get; set; }

        [Display(Name = "Global Production Price Class")]
        [MaxLength(30)]
        public string global_production_price_class { get; set; }

        [Display(Name = "Global Sales Segment")]
        [MaxLength(10)]
        public string global_sales_segment { get; set; }

        [Display(Name = "Global Sales Sub-Segment")]
        [MaxLength(15)]
        public string global_sales_sub_segment { get; set; }

        [Display(Name = "Global Sales Price Class")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Ingrese un número válido.")]
        public int global_sales_price_class { get; set; }

        [Display(Name = "Short-Term Risk Rating")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Ingrese un número válido.")]
        public int short_term_risk_rating { get; set; }

        [Display(Name = "Long-Term Risk Rating")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Ingrese un número válido.")]
        public int long_term_risk_rating { get; set; }

        [Required]
        [Display(Name = "Origen")]
        [MaxLength(8)]
        public string origen { get; set; }

        [Required]
        [Display(Name = "Porcentaje scrap")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        [Range(0, 100)]
        public decimal porcentaje_scrap { get; set; }
    }

    [MetadataType(typeof(BG_IHS_itemMetadata))]
    public partial class BG_IHS_item : IEquatable<BG_IHS_item>
    {
        private BG_IHS_regiones _cachedRegion;

        [NotMapped]
        public int versionIHS { get; set; }

        //obtiene el segmento asociado o null en caso de no existir
        [NotMapped]
        [Display(Name = "Rel Segmento")]
        public BG_IHS_segmentos RelSegmento
        {
            get
            {
                BG_IHS_segmentos item = null;

                item = this.BG_IHS_versiones.BG_IHS_segmentos.Where(x => x.global_production_segment == this.global_production_segment && x.flat_rolled_steel_usage.HasValue).FirstOrDefault();

                return item;
            }
        }

        //obtiene la region asociada o null en caso de no existir
        [NotMapped]
        [Display(Name = "Región")]
        public BG_IHS_regiones _Region
        {
            get
            {
                if (_cachedRegion == null)
                {
                    // Se busca sólo en la primera invocación
                    _cachedRegion = BG_IHS_versiones
                        .BG_IHS_rel_regiones
                        .FirstOrDefault(x =>
                            x.BG_IHS_plantas.mnemonic_plant == this.mnemonic_plant)
                        ?.BG_IHS_regiones;
                }
                return _cachedRegion;
            }
        }


        //concatena el nombre
        [NotMapped]
        public string ConcatCodigo
        {
            get
            {
                return string.Format("{0}_{1}{2}{3}", mnemonic_vehicle_plant, vehicle, "{" + production_plant + "}", sop_start_of_production.HasValue ? sop_start_of_production.Value.ToString("yyyy-MM") : String.Empty).ToUpper();
            }
        }

        /// <summary>
        /// Retorna la demanda del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_rel_demanda> GetDemanda(
     List<BG_IHS_cabecera> cabeceras,
     string demanda)
        {
            // 1) Pre‐filtrar y agrupar por fecha en un lookup
            var todas = this.BG_IHS_rel_demanda
                .Where(x =>
                    x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER ||
                    x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL)
                .ToList();

            // Lookup<DateTime, BG_IHS_rel_demanda>
            var lookup = todas.ToLookup(x => x.fecha);

            var resultado = new List<BG_IHS_rel_demanda>(cabeceras.Count);

            // 2) Ahora cada fecha la buscas en O(1) y luego filtras sólo en ese pequeñísimo grupo
            foreach (var cab in cabeceras)
            {
                BG_IHS_rel_demanda sel = null;
                var grupo = lookup[cab.fecha.Value];

                if (demanda == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER)
                {
                    sel = grupo
                        .FirstOrDefault(x => x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER && x.cantidad != null)
                       ?? grupo.FirstOrDefault(x => x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);

                    if (sel != null)
                        sel.origen_datos = (sel.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER)
                            ? Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER
                            : Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL;
                }
                else if (demanda == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL)
                {
                    sel = grupo.FirstOrDefault(x => x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
                    if (sel != null)
                        sel.origen_datos = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL;
                }

                resultado.Add(sel);
            }

            return resultado;
        }

        /// <summary>
        /// Retorna los cuartos del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_rel_cuartos> GetCuartos(
    List<BG_IHS_rel_demanda> meses,
    List<BG_IHS_cabecera_cuartos> cabeceraTabla,
    string demanda)
        {
            // 1) Pre-agrupar sumas de demanda por año y trimestre
            var sumByQuarter = meses
                .Where(x => x != null)
                .GroupBy(x => (
                    Year: x.fecha.Year,
                    Quarter: (x.fecha.Month + 2) / 3  // Meses 1-3 → Q1, 4-6 → Q2, etc.
                ))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.cantidad)
                );

            // 2) Preparar lookup de los registros IHS ya existentes
            var fallback = this.BG_IHS_rel_cuartos
                .ToDictionary(
                    x => (x.anio, x.cuarto),
                    x => x
                );

            var resultado = new List<BG_IHS_rel_cuartos>(cabeceraTabla.Count);

            // 3) Para cada cabecera, buscar en el diccionario o usar IHS
            foreach (var header in cabeceraTabla)
            {
                var key = (Year: header.anio, Quarter: header.quarter);
                BG_IHS_rel_cuartos rel;

                if (sumByQuarter.TryGetValue(key, out var total))
                {
                    // Hay meses calculados
                    rel = new BG_IHS_rel_cuartos
                    {
                        anio = header.anio,
                        cuarto = header.quarter,
                        cantidad = total,
                        origen_datos = Enum_BG_origen_cuartos.Calculado
                    };
                }
                else if (fallback.TryGetValue(key, out var existing))
                {
                    // No hay meses, usar el dato IHS
                    existing.origen_datos = Enum_BG_origen_cuartos.IHS;
                    rel = existing;
                }
                else
                {
                    // Ni calculado ni IHS: devolvemos cero
                    rel = new BG_IHS_rel_cuartos
                    {
                        anio = header.anio,
                        cuarto = header.quarter,
                        cantidad = 0,
                        origen_datos = Enum_BG_origen_cuartos.Calculado
                    };
                }

                resultado.Add(rel);
            }

            return resultado;
        }

        /// <summary>
        /// Retorna los años del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_item_anios> GetAnios(
    List<BG_IHS_rel_demanda> meses,
    List<BG_IHS_cabecera_anios> cabeceraTabla,
    string demanda)
        {
            // 1) Agrupar SUMA de demanda mensual por año
            var demandaPorAnio = meses
                .Where(x => x != null)
                .GroupBy(x => x.fecha.Year)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.cantidad)
                );

            // 2) Agrupar SUMA de registros IHS de cuartos por año
            var fallbackPorAnio = this.BG_IHS_rel_cuartos
                .GroupBy(x => x.anio)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.cantidad)
                );

            var list = new List<BG_IHS_item_anios>(cabeceraTabla.Count);

            // 3) Para cada año en la cabecera, tomo la suma ya calculada en O(1)
            foreach (var header in cabeceraTabla)
            {
                int anio = header.anio;
                var rel = new BG_IHS_item_anios
                {
                    anio = anio - 1
                };

                if (demandaPorAnio.TryGetValue(anio, out var sumaCalc))
                {
                    // Si hay demanda mensual, la uso
                    rel.cantidad = sumaCalc;
                    rel.origen_datos = Enum_BG_origen_anios.Calculado;
                }
                else if (fallbackPorAnio.TryGetValue(anio, out var sumaIHS))
                {
                    // Si no, uso la suma de los cuartos IHS
                    rel.cantidad = sumaIHS;
                    rel.origen_datos = Enum_BG_origen_anios.IHS;
                }
                else
                {
                    // Ningún dato: cero calculado
                    rel.cantidad = 0;
                    rel.origen_datos = Enum_BG_origen_anios.Calculado;
                }

                // Región (ya cacheada en la propiedad _Region)
                rel.region = this._Region?.descripcion ?? "SIN DEFINIR";

                list.Add(rel);
            }

            return list;
        }

        /// <summary>
        /// Retorna los años del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_item_anios> GetAniosFY(
     List<BG_IHS_rel_demanda> meses,
     List<BG_IHS_cabecera_anios> cabeceraTabla,
     string demanda)
        {
            // 1) Diccionario de sumas de demanda mensual, clave = (FY, Quarter)
            var demLookup = meses
                .Where(x => x != null)
                .GroupBy(x =>
                {
                    // FY = año fiscal final
                    int fy = x.fecha.Month >= 10 ? x.fecha.Year + 1 : x.fecha.Year;
                    int q = (x.fecha.Month + 2) / 3;
                    return (FY: fy, Quarter: q);
                })
                .ToDictionary(
                    g => g.Key,
                    g => (int?)g.Sum(x => x.cantidad)
                );

            // 2) Diccionario de sumas IHS de cuartos, misma clave
            var ihsLookup = this.BG_IHS_rel_cuartos
                .GroupBy(x =>
                {
                    // si es Q4 de calendar, pertenece al FY siguiente
                    int fy = x.cuarto == 4 ? x.anio + 1 : x.anio;
                    return (FY: fy, Quarter: x.cuarto);
                })
                .ToDictionary(
                    g => g.Key,
                    g => (int?)g.Sum(x => x.cantidad)
                );

            var list = new List<BG_IHS_item_anios>(cabeceraTabla.Count);

            foreach (var header in cabeceraTabla)
            {
                var rel = new BG_IHS_item_anios { anio = header.anio - 1 };
                int? total = 0;
                var tipo = Enum_BG_origen_anios.IHS;

                // 3) Ahora sólo 4 lookups
                for (int q = 1; q <= 4; q++)
                {
                    var key = (FY: header.anio, Quarter: q);

                    if (demLookup.TryGetValue(key, out var sumCalc) && sumCalc.HasValue)
                    {
                        total += sumCalc;
                        tipo = Enum_BG_origen_anios.Calculado;
                    }
                    else if (ihsLookup.TryGetValue(key, out var sumIhs) && sumIhs.HasValue)
                    {
                        total += sumIhs;
                        tipo = Enum_BG_origen_anios.IHS;
                    }
                }

                rel.cantidad = total;
                rel.origen_datos = tipo;
                rel.region = this._Region?.descripcion ?? "SIN DEFINIR";

                list.Add(rel);
            }

            return list;
        }


        //para realizar la comparacion  
        public bool Equals(BG_IHS_item other)
        {
            if (other is null)
                return false;

            return
                 this.core_nameplate_region_mnemonic == other.core_nameplate_region_mnemonic
                && this.core_nameplate_plant_mnemonic == other.core_nameplate_plant_mnemonic
                && this.mnemonic_vehicle == other.mnemonic_vehicle
                && this.mnemonic_vehicle_plant == other.mnemonic_vehicle_plant
                && this.mnemonic_platform == other.mnemonic_platform
                && this.mnemonic_plant == other.mnemonic_plant
                && this.region == other.region
                && this.market == other.market
                && this.country_territory == other.country_territory
                && this.production_plant == other.production_plant
                && this.city == other.city
                && this.plant_state_province == other.plant_state_province
                && this.source_plant == other.source_plant
                && this.source_plant_country_territory == other.source_plant_country_territory
                && this.source_plant_region == other.source_plant_region
                && this.design_parent == other.design_parent
                && this.engineering_group == other.engineering_group
                && this.manufacturer_group == other.manufacturer_group
                && this.manufacturer == other.manufacturer
                && this.sales_parent == other.sales_parent
                && this.production_brand == other.production_brand
                && this.platform_design_owner == other.platform_design_owner
                && this.architecture == other.architecture
                && this.platform == other.platform
                && this.program == other.program
                && this.production_nameplate == other.production_nameplate
                && this.sop_start_of_production == other.sop_start_of_production
                && this.eop_end_of_production == other.eop_end_of_production
                && this.lifecycle_time == other.lifecycle_time
                && this.vehicle == other.vehicle
                && this.assembly_type == other.assembly_type
                && this.strategic_group == other.strategic_group
                && this.sales_group == other.sales_group
                && this.global_nameplate == other.global_nameplate
                && this.primary_design_center == other.primary_design_center
                && this.primary_design_country_territory == other.primary_design_country_territory
                && this.primary_design_region == other.primary_design_region
                && this.secondary_design_center == other.secondary_design_center
                && this.secondary_design_country_territory == other.secondary_design_country_territory
                && this.secondary_design_region == other.secondary_design_region
                && this.gvw_rating == other.gvw_rating
                && this.gvw_class == other.gvw_class
                && this.car_truck == other.car_truck
                && this.production_type == other.production_type
                && this.global_production_segment == other.global_production_segment
                && this.regional_sales_segment == other.regional_sales_segment
                && this.global_production_price_class == other.global_production_price_class
                && this.global_sales_segment == other.global_sales_segment
                && this.global_sales_sub_segment == other.global_sales_sub_segment
                && this.global_sales_price_class == other.global_sales_price_class
                && this.short_term_risk_rating == other.short_term_risk_rating
                && this.long_term_risk_rating == other.long_term_risk_rating
                && this.origen == other.origen
                ;
        }

        /// <summary>
        /// Actualiza los valores conforme a otro objeto (excepto version)
        /// </summary>
        /// <param name="item"></param>
        public void Update(BG_IHS_item other)
        {
            this.core_nameplate_region_mnemonic = other.core_nameplate_region_mnemonic;
            this.core_nameplate_plant_mnemonic = other.core_nameplate_plant_mnemonic;
            this.mnemonic_vehicle = other.mnemonic_vehicle;
            this.mnemonic_vehicle_plant = other.mnemonic_vehicle_plant;
            this.mnemonic_platform = other.mnemonic_platform;
            this.mnemonic_plant = other.mnemonic_plant;
            this.region = other.region;
            this.market = other.market;
            this.country_territory = other.country_territory;
            this.production_plant = other.production_plant;
            this.city = other.city;
            this.plant_state_province = other.plant_state_province;
            this.source_plant = other.source_plant;
            this.source_plant_country_territory = other.source_plant_country_territory;
            this.source_plant_region = other.source_plant_region;
            this.design_parent = other.design_parent;
            this.engineering_group = other.engineering_group;
            this.manufacturer_group = other.manufacturer_group;
            this.manufacturer = other.manufacturer;
            this.sales_parent = other.sales_parent;
            this.production_brand = other.production_brand;
            this.platform_design_owner = other.platform_design_owner;
            this.architecture = other.architecture;
            this.platform = other.platform;
            this.program = other.program;
            this.production_nameplate = other.production_nameplate;
            this.sop_start_of_production = other.sop_start_of_production;
            this.eop_end_of_production = other.eop_end_of_production;
            this.lifecycle_time = other.lifecycle_time;
            this.vehicle = other.vehicle;
            this.assembly_type = other.assembly_type;
            this.strategic_group = other.strategic_group;
            this.sales_group = other.sales_group;
            this.global_nameplate = other.global_nameplate;
            this.primary_design_center = other.primary_design_center;
            this.primary_design_country_territory = other.primary_design_country_territory;
            this.primary_design_region = other.primary_design_region;
            this.secondary_design_center = other.secondary_design_center;
            this.secondary_design_country_territory = other.secondary_design_country_territory;
            this.secondary_design_region = other.secondary_design_region;
            this.gvw_rating = other.gvw_rating;
            this.gvw_class = other.gvw_class;
            this.car_truck = other.car_truck;
            this.production_type = other.production_type;
            this.global_production_segment = other.global_production_segment;
            this.regional_sales_segment = other.regional_sales_segment;
            this.global_production_price_class = other.global_production_price_class;
            this.global_sales_segment = other.global_sales_segment;
            this.global_sales_sub_segment = other.global_sales_sub_segment;
            this.global_sales_price_class = other.global_sales_price_class;
            this.short_term_risk_rating = other.short_term_risk_rating;
            this.long_term_risk_rating = other.long_term_risk_rating;
            this.origen = other.origen;

        }

        /// <summary>
        /// Actualiza la demanda conforme a otro objeto.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateDemanda(BG_IHS_item other)
        {
            this.BG_IHS_rel_demanda = new List<BG_IHS_rel_demanda>();

            foreach (var item in other.BG_IHS_rel_demanda)
            {
                this.BG_IHS_rel_demanda.Add(
                    new BG_IHS_rel_demanda
                    {
                        cantidad = item.cantidad,
                        fecha = item.fecha,
                        fecha_carga = item.fecha_carga,
                        tipo = item.tipo,
                    }
                );
            }

        }

        /// <summary>
        /// Actualiza los cuartos conforme a otro objeto.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateCuartos(BG_IHS_item other)
        {
            this.BG_IHS_rel_cuartos = new List<BG_IHS_rel_cuartos>();

            foreach (var item in other.BG_IHS_rel_cuartos)
            {
                this.BG_IHS_rel_cuartos.Add(
                    new BG_IHS_rel_cuartos
                    {
                        cantidad = item.cantidad,
                        cuarto = item.cuarto,
                        anio = item.anio,
                        fecha_carga = item.fecha_carga,
                    }
                );
            }
        }

        public override bool Equals(object obj) => Equals(obj as bom_en_sap);
        public override int GetHashCode() => (core_nameplate_region_mnemonic,
                                    core_nameplate_plant_mnemonic,
                                    mnemonic_vehicle,
                                    mnemonic_vehicle_plant,
                                    mnemonic_platform,
                                    mnemonic_plant,
                                    region,
                                    market,
                                    country_territory,
                                    production_plant,
                                    city,
                                    plant_state_province,
                                    source_plant,
                                    source_plant_country_territory,
                                    source_plant_region,
                                    design_parent,
                                    engineering_group,
                                    manufacturer_group,
                                    manufacturer,
                                    sales_parent,
                                    production_brand,
                                    platform_design_owner,
                                    architecture,
                                    platform,
                                    program,
                                    production_nameplate,
                                    sop_start_of_production,
                                    eop_end_of_production,
                                    lifecycle_time,
                                    vehicle,
                                    assembly_type,
                                    strategic_group,
                                    sales_group,
                                    global_nameplate,
                                    primary_design_center,
                                    primary_design_country_territory,
                                    primary_design_region,
                                    secondary_design_center,
                                    secondary_design_country_territory,
                                    secondary_design_region,
                                    gvw_rating,
                                    gvw_class,
                                    car_truck,
                                    production_type,
                                    global_production_segment,
                                    regional_sales_segment,
                                    global_production_price_class,
                                    global_sales_segment,
                                    global_sales_sub_segment,
                                    global_sales_price_class,
                                    short_term_risk_rating,
                                    long_term_risk_rating,
                                    origen
                                    ).GetHashCode();
    }

    /// <summary>
    /// Clase para utilizades Budget IHS
    /// </summary>
    public static class BG_IHS_UTIL
    {

        public static List<BG_IHS_cabecera> GetCabecera()
        {
            // 1. Obtener una instancia de la caché y definir una clave única para este dato.
            ObjectCache cache = MemoryCache.Default;
            string cacheKey = "GetCabecera_CacheKey";

            // 2. Intentar leer la lista directamente desde la caché.
            var listaCacheada = cache[cacheKey] as List<BG_IHS_cabecera>;

            // 3. Si la lista ya existía en la caché (no es null), la devolvemos inmediatamente.
            if (listaCacheada != null)
            {
                // Esto es súper rápido, devuelve los datos desde la memoria RAM.
                return listaCacheada;
            }

            // 4. Si no estaba en la caché (es null), ejecutamos la lógica original para obtener los datos.
            // --- INICIA TU LÓGICA ORIGINAL ---
            List<BG_IHS_cabecera> list = new List<BG_IHS_cabecera>();
            using (var db = new Portal_2_0Entities())
            {
                //obtiene el menor año de los archivos cargados de ihs
                int anoInicio = 2019, anoFin = 2030;
                try
                {
                    anoInicio = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                    anoFin = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                }
                catch (Exception)
                {
                    // En caso de error, los valores por defecto (2019, 2030) se usan.
                    // Es buena práctica no dejar el catch vacío. Puedes añadir un log si lo necesitas.
                    /* do nothing */
                }

                for (int i = anoInicio; i <= anoFin; i++)
                {
                    for (int j = 1; j <= 12; j++)
                    {
                        CultureInfo ci = new CultureInfo("es-MX");
                        DateTime fecha = new DateTime(i, j, 1);
                        list.Add(
                            new BG_IHS_cabecera()
                            {
                                text = fecha.ToString("MMM yyy", ci).ToUpper(),
                                fecha = fecha
                            });
                    }
                }
            }
            // --- TERMINA TU LÓGICA ORIGINAL ---

            // 5. Antes de devolver la lista, la guardamos en la caché para futuras peticiones.
            // Se define que el dato expirará y se borrará automáticamente de la caché en 15 minutos.
            var politicaCache = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
            };
            cache.Set(cacheKey, list, politicaCache);

            // 6. Devolvemos la lista que acabamos de calcular y guardar.
            return list;
        }

        public static List<BG_IHS_cabecera> GetCabeceraPlantillaDemanda(List<BG_IHS_item> listado)
        {
            List<BG_IHS_cabecera> list = new List<BG_IHS_cabecera>();

            int anoInicio = DateTime.Now.Year;
            int anoFin = 2030;


            // var sop = listado.OrderBy(x => x.sop_start_of_production).Select(x => x.sop_start_of_production).FirstOrDefault();
            //anoInicio = sop != null ? sop.Value.Year : 2019;
            var eop = listado.OrderByDescending(x => x.eop_end_of_production).Select(x => x.eop_end_of_production).FirstOrDefault();
            anoFin = eop != null ? eop.Value.Year : 2030;
            CultureInfo ci = new CultureInfo("en-US");

            for (int i = anoInicio; i <= anoFin; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    DateTime fecha = new DateTime(i, j, 1);
                    list.Add(
                        new BG_IHS_cabecera()
                        {
                            text = fecha.ToString("MMMM yyyy", ci).ToUpper(),
                            fecha = fecha
                        });
                }
            }

            return list;
        }

        /// <summary>
        /// Obtiene los titulos de las cabeceras para los cuartos
        /// </summary>
        /// <returns></returns>
        // Recuerda tener 'using System.Runtime.Caching;' al inicio de tu archivo .cs

        public static List<BG_IHS_cabecera_cuartos> GetCabeceraCuartos()
        {
            // 1. Obtener la instancia de la caché y definir una clave única.
            ObjectCache cache = MemoryCache.Default;
            string cacheKey = "CabeceraCuartos_CacheKey"; // ¡Clave única para este método!

            // 2. Intentar obtener el dato desde la caché.
            var listaCacheada = cache[cacheKey] as List<BG_IHS_cabecera_cuartos>;

            // 3. Si se encontró en caché, devolverla inmediatamente.
            if (listaCacheada != null)
            {
                return listaCacheada;
            }

            // 4. Si no, ejecutar la lógica original para obtener los datos.
            // --- INICIA TU LÓGICA ORIGINAL ---
            List<BG_IHS_cabecera_cuartos> list = new List<BG_IHS_cabecera_cuartos>();
            using (var db = new Portal_2_0Entities())
            {
                //obtiene el menor año de los archivos cargados para los cuartos
                int anoInicioDemanda, anioInicioCuartos, anoFinDemanda, anioFinCuartos;
                int anioMenor = 2019, anioMayor = 2030;
                try
                {
                    anoInicioDemanda = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                    anioInicioCuartos = db.BG_IHS_rel_cuartos.OrderBy(x => x.anio).Select(x => x.anio).FirstOrDefault();
                    anoFinDemanda = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                    anioFinCuartos = db.BG_IHS_rel_cuartos.OrderByDescending(x => x.anio).Select(x => x.anio).FirstOrDefault();

                    anioMenor = anoInicioDemanda < anioInicioCuartos ? anoInicioDemanda : anioInicioCuartos;
                    anioMayor = anoFinDemanda > anioFinCuartos ? anoFinDemanda : anioFinCuartos;
                }
                catch (Exception) { /* do nothing */ }

                for (int i = anioMenor; i <= anioMayor; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        list.Add(
                            new BG_IHS_cabecera_cuartos()
                            {
                                text = "Q" + j + " " + i,
                                anio = i,
                                quarter = j,
                            });
                    }
                }
            }
            // --- TERMINA TU LÓGICA ORIGINAL ---

            // 5. Guardar la lista recién creada en la caché con una política de expiración.
            var politicaCache = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15.0)
            };
            cache.Set(cacheKey, list, politicaCache);

            // 6. Devolver la lista.
            return list;
        }

        /// <summary>
        /// Obtiene los titulos de las cabeceras para los cuartos
        /// </summary>
        /// <returns></returns>
        public static List<BG_IHS_cabecera_anios> GetCabeceraAnios()
        {
            // Obtenemos una instancia de la caché en memoria por defecto de .NET
            ObjectCache cache = MemoryCache.Default;

            // Definimos una clave única para este objeto en la caché
            string cacheKey = "CabeceraAnios_CacheKey";

            // 1. INTENTAR OBTENER EL DATO DESDE LA CACHÉ
            // Se intenta convertir (cast) el objeto de la caché a nuestro tipo de lista.
            var cabeceraCacheada = cache[cacheKey] as List<BG_IHS_cabecera_anios>;

            // 2. VERIFICAR SI TUVIMOS ÉXITO (CACHE HIT)
            if (cabeceraCacheada != null)
            {
                // ¡Genial! El dato ya estaba en memoria. Lo devolvemos inmediatamente.
                // Esto es súper rápido (microsegundos).
                return cabeceraCacheada;
            }
            else // 3. SI NO ESTÁ EN CACHÉ (CACHE MISS)
            {
                // El dato no estaba en memoria, así que hacemos el trabajo pesado una vez.
                // --- INICIA TU LÓGICA ORIGINAL ---
                List<BG_IHS_cabecera_anios> list = new List<BG_IHS_cabecera_anios>();
                using (var db = new Portal_2_0Entities())
                {
                    // (Aquí va todo tu código original que consulta la base de datos)
                    int anoInicioDemanda, anioInicioCuartos, anoFinDemanda, anioFinCuartos;
                    int anioMenor = 2019, anioMayor = 2030;
                    try
                    {
                        anoInicioDemanda = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                        anioInicioCuartos = db.BG_IHS_rel_cuartos.OrderBy(x => x.anio).Select(x => x.anio).FirstOrDefault();
                        anoFinDemanda = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                        anioFinCuartos = db.BG_IHS_rel_cuartos.OrderByDescending(x => x.anio).Select(x => x.anio).FirstOrDefault();
                        if (anoInicioDemanda > 2000 && anioInicioCuartos > 2000 && anoFinDemanda > 2000 && anioFinCuartos > 2000)
                        {
                            anioMenor = anoInicioDemanda < anioInicioCuartos ? anoInicioDemanda : anioInicioCuartos;
                            anioMayor = anoFinDemanda > anioFinCuartos ? anoFinDemanda : anioFinCuartos;
                        }
                    }
                    catch (Exception) { /* do nothing */ }
                    for (int i = anioMenor; i <= anioMayor; i++)
                    {
                        list.Add(new BG_IHS_cabecera_anios() { text = i.ToString(), anio = i, });
                    }
                }
                // --- TERMINA TU LÓGICA ORIGINAL ---

                // 4. GUARDAR EL RESULTADO EN LA CACHÉ ANTES DE DEVOLVERLO
                // Se define una "política de expiración". En este caso, el dato se borrará de la caché en 10 minutos.
                var politicaCache = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15.0)
                };

                // Guardamos la lista recién creada en la caché con su clave y su política.
                cache.Set(cacheKey, list, politicaCache);

                // Devolvemos la lista que acabamos de calcular.
                return list;
            }
        }

        /// <summary>
        /// Obtiene los titulos de las cabeceras para los anios FY
        /// </summary>
        /// <returns></returns>
        public static List<BG_IHS_cabecera_anios> GetCabeceraAniosFY()
        {
            // 1. Obtener la instancia de la caché y definir una clave única.
            ObjectCache cache = MemoryCache.Default;
            string cacheKey = "CabeceraAniosFY_CacheKey"; // ¡Clave única para este método!

            // 2. Intentar obtener el dato desde la caché.
            var listaCacheada = cache[cacheKey] as List<BG_IHS_cabecera_anios>;

            // 3. Si se encontró en caché, devolverla inmediatamente.
            if (listaCacheada != null)
            {
                return listaCacheada;
            }

            // 4. Si no, ejecutar la lógica original para obtener los datos.
            // --- INICIA TU LÓGICA ORIGINAL ---
            List<BG_IHS_cabecera_anios> list = new List<BG_IHS_cabecera_anios>();
            using (var db = new Portal_2_0Entities())
            {
                //obtiene el menor año de los archivos cargados para los cuartos
                int anoInicioDemanda, anioInicioCuartos, anoFinDemanda, anioFinCuartos;
                int anioMenor = 2019, anioMayor = 2030;
                try
                {
                    anoInicioDemanda = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                    anioInicioCuartos = db.BG_IHS_rel_cuartos.OrderBy(x => x.anio).Select(x => x.anio).FirstOrDefault();
                    anoFinDemanda = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                    anioFinCuartos = db.BG_IHS_rel_cuartos.OrderByDescending(x => x.anio).Select(x => x.anio).FirstOrDefault();

                    //solo se aplica cuando los años obtenidos son mayores al 2000
                    if (anoInicioDemanda > 2000 && anioInicioCuartos > 2000 && anoFinDemanda > 2000 && anioFinCuartos > 2000)
                    {
                        anioMenor = anoInicioDemanda < anioInicioCuartos ? anoInicioDemanda : anioInicioCuartos;
                        anioMayor = anoFinDemanda > anioFinCuartos ? anoFinDemanda : anioFinCuartos;
                    }
                }
                catch (Exception) { /* do nothing */ }

                for (int i = anioMenor; i <= anioMayor; i++)
                {
                    list.Add(
                        new BG_IHS_cabecera_anios()
                        {
                            text = "FY " + (i - 1).ToString().Substring(2, 2) + "-" + i.ToString().Substring(2, 2),
                            anio = i - 1,
                        });
                }
            }
            // --- TERMINA TU LÓGICA ORIGINAL ---

            // 5. Guardar la lista recién creada en la caché con una política de expiración.
            var politicaCache = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15.0)
            };
            cache.Set(cacheKey, list, politicaCache);

            // 6. Devolver la lista.
            return list;
        }


    }

    public class BG_IHS_cabecera
    {
        public string text { get; set; }
        public DateTime? fecha { get; set; }
    }
    public class BG_IHS_cabecera_cuartos
    {
        public string text { get; set; }
        public int anio { get; set; }
        public int quarter { get; set; }
    }
    public class BG_IHS_cabecera_anios
    {
        public string text { get; set; }
        public int anio { get; set; }
    }

    //clase para retorna valores a la tabla
    public class BG_IHS_item_anios
    {
        public int? cantidad { get; set; }
        public int anio { get; set; }
        public string region { get; set; }
        public Enum_BG_origen_anios origen_datos { get; set; }
    }


    public enum Enum_BG_origen_cuartos
    {
        Calculado,
        IHS,
    }
    public enum Enum_BG_origen_anios
    {
        Calculado,
        IHS,
    }
}