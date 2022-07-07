using Bitacoras.Util;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_itemsMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [Display(Name = "Plant")]
        public int id_planta { get; set; }

        [Display(Name = "Type")]
        public int id_inventory_type { get; set; }

        [Display(Name = "Active?")]
        public bool active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        [RequiredIf("WarrantyRequired", true, ErrorMessage = "The field {0} is Required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> purchase_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Inactive Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [RequiredIf("active", false, ErrorMessage = "The field {0} is Required.")]
        public Nullable<System.DateTime> inactive_date { get; set; }

        [MaxLength(250, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Comments")]
        public string comments { get; set; }

        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Hostname")]
        public string hostname { get; set; }

        [MaxLength(80, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Brand")]
        public string brand { get; set; }

        [MaxLength(80, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Model")]
        public string model { get; set; }

        [MaxLength(60, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Serial Number")]
        public string serial_number { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "End Warranty")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [RequiredIf("WarrantyRequired", true, ErrorMessage = "The field {0} is Required.")]
        public Nullable<System.DateTime> end_warranty { get; set; }

        [Display(Name = "MAC LAN")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "The length for {0} must be {1} characters.")]
        //[RegularExpression("^(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})$", ErrorMessage = "Use the format 00:00:00:00:00:00")]
        public string mac_lan { get; set; }

        [Display(Name = "MAC WLAN")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "The length for {0} must be {1} characters.")]
        //[RegularExpression("^(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})$", ErrorMessage = "Use the format 00:00:00:00:00:00")]
        public string mac_wlan { get; set; }

        [Display(Name = "Processor")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string processor { get; set; }

        [Display(Name = "Total Physical Memory (MB)")]
        //  [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Format. Use only two decimals.")]
        [Range(0, 999999.99)]
        public Nullable<decimal> total_physical_memory_mb { get; set; }

        [Display(Name = "Maintenance Period (months)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> maintenance_period_months { get; set; }

        //[Display(Name = "Physical Status")]
        //[MaxLength(150, ErrorMessage = "The max length for {0} is {1} characters.")]
        //public string physical_status { get; set; }       

        [Display(Name = "CPU speed (MHz)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> cpu_speed_mhz { get; set; }

        [Display(Name = "Operation System")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string operation_system { get; set; }

        [Display(Name = "OS bits")]
        [Range(32, 64, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> bits_operation_system { get; set; }

        [Display(Name = "Number of CPUs")]
        [Range(0, Int16.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> number_of_cpus { get; set; }

        [Display(Name = "Printer Ubication")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters")]
        public string printer_ubication { get; set; }

        [Display(Name = "IP Address")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "This is not a valid IP.")]
        public string ip_adress { get; set; }

        [Display(Name = "Cost Center")]
        [MaxLength(8, ErrorMessage = "The max length for {0} is {1} characters")]
        public string cost_center { get; set; }

        [Display(Name = "Storage (MB)")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Range(0, 999999.99)]
        public Nullable<int> movil_device_storage_mb { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Format. Use only two decimals.")]
        [Range(0, 99.99)]
        [Display(Name = "Inches")]
        public Nullable<decimal> inches { get; set; }

        [Display(Name = "IMEI 1")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "The field {0} must be {2} characters long.")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Only numbers are allowed.")]
        public string imei_1 { get; set; }

        [Display(Name = "IMEI 2")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "The field {0} must be {2} characters long.")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Only numbers are allowed.")]
        public string imei_2 { get; set; }

        [Display(Name = "Code")]
        [MaxLength(20, ErrorMessage = "The max length for {0} is {1} characters")]
        public string code { get; set; }

        [Display(Name = "Accessories")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters")]
        public string accessories { get; set; }

        [Display(Name = "Accessory Type")]
        public Nullable<int> id_tipo_accesorio { get; set; }    

        [Display(Name = "Physical Server")]
        public Nullable<int> physical_server { get; set; }

    }

    [MetadataType(typeof(IT_inventory_itemsMetadata))]
    public partial class IT_inventory_items
    {
        // para covertir megas a gb
        [Display(Name = "Total Physical Memory (GB)")]
        [Range(0, 9999.99)]
        public Nullable<decimal> total_physical_memory_gb
        {
            get
            {
                decimal? val = null;

                if (this.total_physical_memory_mb.HasValue)
                    val = this.total_physical_memory_mb / (decimal)1024;

                if (val.HasValue)
                    return Decimal.Round(val.Value, 2);
                else
                    return null;
            }
            set
            {
               this.total_physical_memory_mb = value * 1024;
            }
        }

        // para covertir megas a gb
        [Display(Name = "Device Storage (GB)")]
        [Range(0, 9999.99)]
        public Nullable<decimal> movil_device_storage_gb
        {
            get
            {
                decimal? val = null;

                if (this.movil_device_storage_mb.HasValue)
                    val = this.movil_device_storage_mb / (decimal)1024;

                if (val.HasValue)
                    return Decimal.Round(val.Value, 2);
                else
                    return null;
            }
            set
            {
                decimal? val = value * 1024;

                if (val.HasValue)
                {
                    this.movil_device_storage_mb = Decimal.ToInt32(val.Value);
                }
                else
                {
                    this.movil_device_storage_mb = null;
                }

            }
        }

        //para filtros
        [NotMapped]
        [Display(Name = "Is virtual?")]
        public bool es_servidor_virtual { get; set; }


        //para filtros
        [NotMapped]
        public int? filtro_id_planta { get; set; }

        [NotMapped]
        public string filtro_hostname { get; set; }

        [NotMapped]
        public string filtro_model { get; set; }

        [NotMapped]
        public int? filtro_pagina { get; set; }

        [NotMapped]
        public bool? filtro_activo { get; set; }

        [NotMapped]
        [Display(Name = "Number of hard drives")]
        public int NumberOfHardDrives
        {
            get
            {
                if (this.IT_inventory_hard_drives == null)
                    return 0;
                return this.IT_inventory_hard_drives.Count;
            }
        }

        [NotMapped]
        [Display(Name = "Total Disk Space (GB)")]
        public decimal? TotalDiskSpace
        {
            get
            {
                if (this.IT_inventory_hard_drives == null)
                    return 0;

                decimal? val = this.IT_inventory_hard_drives.Sum(x => x.total_drive_space_gb);

                if(val.HasValue)
                    return Decimal.Round(val.Value, 2);
                else
                    return null;
            }

        }


        [NotMapped]
        public string ConcatInfoSmartphone  //no incluye equipo
        {
            get
            {
                string info = "(" + id + ") ";
                try
                {
                    if (this.plantas != null)
                        info += " - " + plantas.descripcion + " ";
                    if (!string.IsNullOrEmpty(this.brand))
                        info += " - " + brand + " ";
                    if (!string.IsNullOrEmpty(this.model))
                        info += " - " + model + " ";
                    if (!string.IsNullOrEmpty(this.serial_number))
                        info += " - " + serial_number + " ";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        }

        [NotMapped]
        public string ConcatInfoGeneral
        {
            get
            {
                //string info = "(" + id + ") ";
                string info = String.Empty;
                try
                {

                    if (!string.IsNullOrEmpty(this.hostname))
                        info += " (" + hostname + ") - ";
                    if (this.plantas != null)
                        info += plantas.descripcion + " ";
                    if (!string.IsNullOrEmpty(this.brand))
                        info += " - " + brand + " ";
                    if (!string.IsNullOrEmpty(this.model))
                        info += " - " + model + " ";
                    if (!string.IsNullOrEmpty(this.serial_number))
                        info += " - " + serial_number + " ";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        } 
        
        [NotMapped]
        public string ConcatAccesoriesInfo
        {
            get
            {
                //string info = "(" + id + ") ";
                string info = String.Empty;
                try
                {

                    if (this.IT_inventory_tipos_accesorios!=null)
                        info += " (" + this.IT_inventory_tipos_accesorios.descripcion + ") ";
                    if (this.plantas != null)
                        info += " - " + plantas.descripcion + " ";
                    if (!string.IsNullOrEmpty(this.brand))
                        info += " - " + brand + " ";
                    if (!string.IsNullOrEmpty(this.model))
                        info += " - " + model + " ";
                    if (!string.IsNullOrEmpty(this.serial_number))
                        info += " - " + serial_number + " ";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        }

        //para foolproof
        [NotMapped]
        public bool WarrantyRequired
        {
            get
            {
                if (this.purchase_date.HasValue || this.end_warranty.HasValue)
                    return true;
                else
                    return false;
            }
        }


    }

    //clases para definir si aplica cada tipo en los formularios
    public static class FiltersInventoryTypes
    {
        public const string BUSQUEDA = "BUSQUEDA";
        public const string EDICION = "EDICION";
        public const string INDEX = "INDEX";

    }

    public class FiltersInventoryUtil
    {
        public string tipoFormulario { get; set; }
        public IT_inventory_hardware_type tipoHardware { get; set; }

        //debe ser el nombre de la variable tal cual viene definida en el modelo
        public string nombreCampo { get; set; }


        private static List<IT_inventory_hardware_type> _allTypes;
        private static IT_inventory_hardware_type _laptop;
        private static IT_inventory_hardware_type _desktop;
        private static IT_inventory_hardware_type _server;
        private static IT_inventory_hardware_type _monitor;
        private static IT_inventory_hardware_type _printer;
        private static IT_inventory_hardware_type _label_printer;
        private static IT_inventory_hardware_type _pda;
        private static IT_inventory_hardware_type _tablet;
        private static IT_inventory_hardware_type _radios;
        private static IT_inventory_hardware_type _ap;
        private static IT_inventory_hardware_type _scanners;
        private static IT_inventory_hardware_type _smartphone;
        private static IT_inventory_hardware_type _accessories;
        private static IT_inventory_hardware_type _virtual_server;



        //referencias para cada uno de los tipos de hw
        private static IT_inventory_hardware_type laptop()
        {
            if (_laptop == null)
                _laptop = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.LAPTOP);

            return _laptop;
        }

        private static IT_inventory_hardware_type desktop()
        {
            if (_desktop == null)
                _desktop = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.DESKTOP);

            return _desktop;
        }
        private static IT_inventory_hardware_type server()
        {
            if (_server == null)
                _server = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.SERVER);

            return _server;
        }
        private static IT_inventory_hardware_type virtual_server()
        {
            if (_virtual_server == null)
                _virtual_server = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.VIRTUAL_SERVER);

            return _virtual_server;
        }
        private static IT_inventory_hardware_type monitor()
        {
            if (_monitor == null)
                _monitor = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.MONITOR);

            return _monitor;
        }
        private static IT_inventory_hardware_type printer()
        {
            if (_printer == null)
                _printer = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.PRINTER);

            return _printer;
        }

        private static IT_inventory_hardware_type label_printer()
        {
            if (_label_printer == null)
                _label_printer = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.LABEL_PRINTER);

            return _label_printer;
        }

        private static IT_inventory_hardware_type pda()
        {
            if (_pda == null)
                _pda = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.PDA);

            return _pda;
        }

        private static IT_inventory_hardware_type tablet()
        {
            if (_tablet == null)
                _tablet = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.TABLET);

            return _tablet;
        }

        private static IT_inventory_hardware_type radio()
        {
            if (_radios == null)
                _radios = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.RADIO);

            return _radios;
        }

        private static IT_inventory_hardware_type ap()
        {
            if (_ap == null)
                _ap = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.AP);

            return _ap;
        }
        private static IT_inventory_hardware_type scanner()
        {
            if (_scanners == null)
                _scanners = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.SCANNERS);

            return _scanners;
        }

        private static IT_inventory_hardware_type smartphone()
        {
            if (_smartphone == null)
                _smartphone = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.SMARTPHONE);

            return _smartphone;
        }

        private static IT_inventory_hardware_type accessories()
        {
            if (_accessories == null)
                _accessories = GetAllTypes().FirstOrDefault(x => x.descripcion == IT_Tipos_Hardware.ACCESSORIES);

            return _accessories;
        }

        /// <summary>
        /// Retorna un listado con todos los tipos de hardware
        /// </summary>
        /// <returns></returns>
        public static List<IT_inventory_hardware_type> GetAllTypes()
        {

            if (_allTypes == null)
            {
                List<IT_inventory_hardware_type> lista = new List<IT_inventory_hardware_type>();

                using (var db = new Portal_2_0Entities())
                {
                    lista = db.IT_inventory_hardware_type.ToList();
                }
                _allTypes = lista;
            }

            return _allTypes;

        }



        /// <summary>
        /// variable estatica con la relacion de filtros
        /// </summary>
        private static List<FiltersInventoryUtil> _filtrosBusqueda = new List<FiltersInventoryUtil>(){ 
            //búsqueda Laptop
            new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Desktop
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Server
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
             //búsqueda Virtual Server
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
              //búsqueda Monitor
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
               //búsqueda Printers
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Label Printers
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda PDAs
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Tablet
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Radios
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //búsqueda Aps
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
             //búsqueda Scanners
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
           //búsqueda smarrphones
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
              //búsqueda accessories
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
          //  ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_tipo_accesorio), tipoFormulario = FiltersInventoryTypes.BUSQUEDA}
            //--FORMULARIOS DESKTOP
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.bits_operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.cpu_speed_mhz), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.number_of_cpus), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.mac_lan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS Server
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.bits_operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.cpu_speed_mhz), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.number_of_cpus), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.mac_lan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.physical_server), tipoFormulario = FiltersInventoryTypes.EDICION}
             //--FORMULARIOS Virtual Server
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.bits_operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.cpu_speed_mhz), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.number_of_cpus), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.mac_lan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.physical_server), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS Laptop
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.bits_operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.cpu_speed_mhz), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.number_of_cpus), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.mac_lan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS Monitor
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.inches), tipoFormulario = FiltersInventoryTypes.EDICION}
             //--FORMULARIOS Printers
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.printer_ubication), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.ip_adress), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS LaBEL Printers
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.printer_ubication), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.cost_center), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.ip_adress), tipoFormulario = FiltersInventoryTypes.EDICION}
           
            //--FORMULARIOS PDAs
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
             //--FORMULARIOS TABLET
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.movil_device_storage_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.inches), tipoFormulario = FiltersInventoryTypes.EDICION}
              //--FORMULARIOS RADIO
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
              //--FORMULARIOS AP
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.mac_lan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.ip_adress), tipoFormulario = FiltersInventoryTypes.EDICION}
             //--FORMULARIOS SCANNERS
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.accessories), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.code), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS SMARTPHONE
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.processor), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.total_physical_memory_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.movil_device_storage_mb), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.mac_wlan), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.imei_1), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.imei_2), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--FORMULARIOS accesories
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_tipo_accesorio), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.EDICION}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.inches), tipoFormulario = FiltersInventoryTypes.EDICION}
            //--INDEX DESKTOP
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = desktop(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.INDEX}
              //--INDEX laptop
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = laptop(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.INDEX}
              //--INDEX server
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = server(),nombreCampo =  nameof(IT_inventory_items.physical_server), tipoFormulario = FiltersInventoryTypes.INDEX}
              //--INDEX virtual server
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.hostname), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.operation_system), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = virtual_server(),nombreCampo =  nameof(IT_inventory_items.physical_server), tipoFormulario = FiltersInventoryTypes.INDEX}
                 //--INDEX monitor
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = monitor(),nombreCampo =  nameof(IT_inventory_items.inches), tipoFormulario = FiltersInventoryTypes.INDEX}
               //--INDEX printer
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = printer(),nombreCampo =  nameof(IT_inventory_items.printer_ubication), tipoFormulario = FiltersInventoryTypes.INDEX}   
            //--INDEX label_printer
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = label_printer(),nombreCampo =  nameof(IT_inventory_items.printer_ubication), tipoFormulario = FiltersInventoryTypes.INDEX}
            //--INDEX PDAs
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = pda(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
                 //--INDEX tablet
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = tablet(),nombreCampo =  nameof(IT_inventory_items.inches), tipoFormulario = FiltersInventoryTypes.INDEX}
            //--INDEX Radio
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = radio(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            //--INDEX Aps
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = ap(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            //--INDEX Scanners
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = scanner(),nombreCampo =  nameof(IT_inventory_items.code), tipoFormulario = FiltersInventoryTypes.INDEX}
            //--INDEX smartphone
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = smartphone(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}
               //--INDEX accessories
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_inventory_type), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_tipo_accesorio), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.active), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.brand), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.id_planta), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.model), tipoFormulario = FiltersInventoryTypes.INDEX}
            ,new FiltersInventoryUtil {tipoHardware = accessories(),nombreCampo =  nameof(IT_inventory_items.serial_number), tipoFormulario = FiltersInventoryTypes.INDEX}

        };


        public static bool AplicaCampo(string tipoFormulario, string id_tipo_hardware, string nombreCampo)
        {
            try
            {
                int id_hw = int.Parse(id_tipo_hardware);

                if (_filtrosBusqueda.Any(x =>
                   x.tipoFormulario == tipoFormulario
                   && x.tipoHardware.id == id_hw
                   && x.nombreCampo == nombreCampo))
                {
                    return true;
                }
                //si no hay coincidencia retorna false
                return false;
            }
            catch
            {
                //retorna false en caso de error
                return false;
            }

        }
    }
}




