using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
        [MaxLength(15)]
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
        //obtiene el segmento asociado o null en caso de no existir
        [NotMapped]
        [Display(Name = "Rel Segmento")]
        public BG_IHS_rel_segmentos RelSegmento
        {
            get
            {
                BG_IHS_rel_segmentos item = null;
                using (var db = new Portal_2_0Entities())
                {
                    item = db.BG_IHS_rel_segmentos.Where(x => x.global_production_segment == this.global_production_segment && x.flat_rolled_steel_usage.HasValue).FirstOrDefault();
                }

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
                BG_IHS_regiones item = null;
                using (var db = new Portal_2_0Entities())
                {
                    item = db.BG_IHS_regiones.Where(x => x.BG_IHS_rel_regiones.Any(y => y.production_plant == this.production_plant && y.id_bg_ihs_region.HasValue)).FirstOrDefault();
                }

                return item;
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
        public List<BG_IHS_rel_demanda> GetDemanda(List<BG_IHS_cabecera> cabeceraTabla, String demanda)
        {
            List<BG_IHS_rel_demanda> list = new List<BG_IHS_rel_demanda>();
            //using (var db = new Portal_2_0Entities())
            //{
            foreach (var item in cabeceraTabla)
            {
                switch (demanda)
                {
                    case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                        //retorna la demanda según el cliente o en segundo termnio la demanda original
                        var d = this.BG_IHS_rel_demanda.FirstOrDefault(x => x.fecha == item.fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER && x.cantidad != null);
                        //si no está vacio, asigna el origen de tipo customer
                        if (d != null)
                            d.origen_datos = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER;
                        //en caso de que sea null busca la demanda original
                        else
                        {
                            d = this.BG_IHS_rel_demanda.FirstOrDefault(x => x.fecha == item.fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
                            if (d != null)
                                d.origen_datos = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL;
                        }
                        list.Add(d);
                        break;
                    case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                        //retorna la demanda original o null
                        var d2 = this.BG_IHS_rel_demanda.FirstOrDefault(x => x.fecha == item.fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
                        if (d2 != null)
                            d2.origen_datos = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL;
                        list.Add(d2);
                        break;
                    default:
                        break;
                }
            }
            //}

            return list;
        }
        /// <summary>
        /// Retorna los cuartos del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_rel_cuartos> GetCuartos(List<BG_IHS_rel_demanda> meses, List<BG_IHS_cabecera_cuartos> cabeceraTabla, String demanda)
        {
            List<BG_IHS_rel_cuartos> list = new List<BG_IHS_rel_cuartos>();

            foreach (var item in cabeceraTabla)
            {
                DateTime fechaInicial = new DateTime(item.anio, item.quarter * 3 - 2, 1);
                DateTime fechaFinal = new DateTime(item.anio, item.quarter * 3, 1).AddMonths(1).AddDays(-1);

                var rel = new Models.BG_IHS_rel_cuartos
                {
                    cuarto = item.quarter,
                    anio = item.anio,
                    //origen 
                    //cantidad
                };

                //obtiene los items que abarca el periodo
                List<BG_IHS_rel_demanda> mesesPeriodo = meses.Where(x => x != null && x.fecha >= fechaInicial && x.fecha <= fechaFinal).ToList();

                //verifica si tiene meses en el periodo
                if (mesesPeriodo.Count > 0)
                {
                    rel.cantidad = mesesPeriodo.Sum(x => x.cantidad);
                    rel.origen_datos = Enum_BG_origen_cuartos.Calculado;
                }
                else //si no tiene meses lo toma directamente de la tabla cuartos
                {
                    var cuarto = this.BG_IHS_rel_cuartos.Where(x => x.anio == item.anio && x.cuarto == item.quarter).FirstOrDefault();

                    if (cuarto != null)
                    {
                        rel = cuarto;
                        rel.origen_datos = Enum_BG_origen_cuartos.IHS;
                    }

                }
                list.Add(rel);
            }
            return list;
        }
        /// <summary>
        /// Retorna los años del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_item_anios> GetAnios(List<BG_IHS_rel_demanda> meses, List<BG_IHS_cabecera_anios> cabeceraTabla, String demanda)
        {
            List<BG_IHS_item_anios> list = new List<BG_IHS_item_anios>();


            foreach (var item in cabeceraTabla)
            {
                DateTime fechaInicial = new DateTime(item.anio, 1, 1);
                DateTime fechaFinal = fechaInicial.AddYears(1).AddDays(-1);

                var rel = new Models.BG_IHS_item_anios
                {
                    anio = item.anio,
                    //origen 
                    //cantidad
                };

                //obtiene los items que abarca el periodo

                List<BG_IHS_rel_demanda> mesesPeriodo = meses.Where(x => x != null && x.fecha >= fechaInicial && x.fecha <= fechaFinal).ToList();

                //verifica si tiene meses en el periodo
                if (mesesPeriodo.Count > 0)
                {
                    rel.cantidad = mesesPeriodo.Sum(x => x.cantidad);
                    rel.origen_datos = Enum_BG_origen_anios.Calculado;
                }
                else //si no tiene meses lo toma directamente de la tabla cuartos
                {
                    var cuartos = this.BG_IHS_rel_cuartos.Where(x => x.anio == item.anio).ToList();

                    if (cuartos.Count > 0)
                    {
                        rel.cantidad = cuartos.Sum(x => x.cantidad);
                        rel.origen_datos = Enum_BG_origen_anios.IHS;
                    }

                }
                list.Add(rel);
            }

            return list;
        }
        /// <summary>
        /// Retorna los años del un objeto de IHS según los parámetros recibidos
        /// </summary>
        /// <param name="demanda"></param>
        /// <returns></returns>        
        public List<BG_IHS_item_anios> GetAniosFY(List<BG_IHS_rel_demanda> meses, List<BG_IHS_cabecera_anios> cabeceraTabla, String demanda)
        {
            List<BG_IHS_item_anios> list = new List<BG_IHS_item_anios>();


            foreach (var item in cabeceraTabla)
            {
                DateTime fechaInicial = new DateTime(item.anio-1, 10, 1);
                DateTime fechaFinal = fechaInicial.AddYears(1).AddDays(-1);

                var rel = new Models.BG_IHS_item_anios
                {
                    anio = item.anio-1,
                    //origen 
                    //cantidad
                };

                //obtiene los items que abarca el periodo

                List<BG_IHS_rel_demanda> mesesPeriodo = meses.Where(x => x != null && x.fecha >= fechaInicial && x.fecha <= fechaFinal).ToList();

                //verifica si tiene meses en el periodo
                if (mesesPeriodo.Count > 0)
                {
                    rel.cantidad = mesesPeriodo.Sum(x => x.cantidad);
                    rel.origen_datos = Enum_BG_origen_anios.Calculado;
                }
                else //si no tiene meses lo toma directamente de la tabla cuartos
                {
                    var cuartos = this.BG_IHS_rel_cuartos.Where(x => x.anio == item.anio).ToList();

                    if (cuartos.Count > 0)
                    {
                        rel.cantidad = cuartos.Sum(x => x.cantidad);
                        rel.origen_datos = Enum_BG_origen_anios.IHS;
                    }

                }

                rel.region = this._Region!=null? this._Region.descripcion: "SIN DEFINIR";

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
                catch (Exception) { /* do nothing */ }

                for (int i = anoInicio; i <= anoFin; i++)
                {
                    for (int j = 1; j <= 12; j++)
                    {
                        DateTime fecha = new DateTime(i, j, 1);
                        list.Add(
                            new BG_IHS_cabecera()
                            {
                                text = fecha.ToString("MMM yyyy").ToUpper(),
                                fecha = fecha
                            });
                    }
                }

            }
            return list;
        }

        /// <summary>
        /// Obtiene los titulos de las cabeceras para los cuartos
        /// </summary>
        /// <returns></returns>
        public static List<BG_IHS_cabecera_cuartos> GetCabeceraCuartos()
        {
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
            return list;
        }

        /// <summary>
        /// Obtiene los titulos de las cabeceras para los cuartos
        /// </summary>
        /// <returns></returns>
        public static List<BG_IHS_cabecera_anios> GetCabeceraAnios()
        {
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

                    anioMenor = anoInicioDemanda < anioInicioCuartos ? anoInicioDemanda : anioInicioCuartos;
                    anioMayor = anoFinDemanda > anioFinCuartos ? anoFinDemanda : anioFinCuartos;
                }
                catch (Exception) { /* do nothing */ }

                for (int i = anioMenor; i <= anioMayor; i++)
                {

                    list.Add(
                        new BG_IHS_cabecera_anios()
                        {
                            text = i.ToString(),
                            anio = i,
                        });

                }

            }
            return list;
        }
        /// <summary>
        /// Obtiene los titulos de las cabeceras para los anios FY
        /// </summary>
        /// <returns></returns>
        public static List<BG_IHS_cabecera_anios> GetCabeceraAniosFY()
        {
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

                    anioMenor = anoInicioDemanda < anioInicioCuartos ? anoInicioDemanda : anioInicioCuartos;
                    anioMayor = anoFinDemanda > anioFinCuartos ? anoFinDemanda : anioFinCuartos;
                }
                catch (Exception) { /* do nothing */ }

                for (int i = anioMenor; i <= anioMayor; i++)
                {

                    list.Add(
                        new BG_IHS_cabecera_anios()
                        {
                            text = "FY " + (i - 1).ToString().Substring(2, 2) + "-" + i.ToString().Substring(2, 2),
                            anio = i-1,
                        });

                }

            }
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
        public string region {get; set;}
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