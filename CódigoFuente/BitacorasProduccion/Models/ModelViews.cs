using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    //Para formulario de carga del archivo excel
    public class ExcelViewModel
    {
        [Required(ErrorMessage = "Por favor seleccione un archivo.")]
        //   [RegularExpression(@"^.*\.(xls|xlsx|XLS|XLSX)$", ErrorMessage = "Sólo se permite seleccionar archivos Excel.")]
        [Display(Name = "Archivo Excel")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

    public class ExcelViewInventarioSAPModel
    {
        [Required(ErrorMessage = "Por favor seleccione un archivo.")]
        //   [RegularExpression(@"^.*\.(xls|xlsx|XLS|XLSX)$", ErrorMessage = "Sólo se permite seleccionar archivos Excel.")]
        [Display(Name = "Archivo Excel")]
        public HttpPostedFileBase PostedFile { get; set; }

        [Display(Name = "Planta")]
        public string codigo_sap { get; set; }

    }

    /// <summary>
    /// Modelo para crear inspector de piezas de descarte
    /// </summary>
    public class UserInspectorViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string id_usuario { get; set; }
    }

    /// <summary>
    /// Modelo para crear inspector de piezas de descarte
    /// </summary>
    public class AsignarSoftwareViewModel
    {

        public AsignarSoftwareViewModel()
        {
            this.IT_asignacion_software = new List<IT_asignacion_software>();
        }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Sistemas")]
        public int id_sistemas { get; set; }

        public List<IT_asignacion_software> IT_asignacion_software;


        public empleados EmpleadoUsuario;
        public empleados EmpleadoSistemas;


    }

    /// <summary>
    /// Modelo para el cambio de Jefe Directo
    /// </summary>
    public class CambioJefeViewModel
    {

        public CambioJefeViewModel()
        {

        }

        [Required]
        [Display(Name = "Jefe Actual")]
        public int id_jefe_actual { get; set; }

        [Required]
        [Display(Name = "Nuevo Jefe")]
        public int id_nuevo_jefe { get; set; }


        public empleados JefeActual;
        public empleados Nuevo;


    }
    /// <summary>
    /// Modelo para envio de Correo electrónico de phising
    /// </summary>
    public class EnvioCorreoViewModel
    {

        public EnvioCorreoViewModel()
        {
            this.ccList = new HashSet<string>();
        }

        [Required]
        [Display(Name = "Nombre del Remitente")]
        public string nombreRemitente { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo Remitente")]
        public string correoRemitente { get; set; }

        [Required]
        [Display(Name = "Asunto")]
        public string asunto { get; set; }

        public string mensaje { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<string> ccList { get; set; }


    }


    //clase para vcard
    public class vcard
    {
        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellidos")]
        public string apellidos { get; set; }

        [StringLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Empresa")]
        public string empresa { get; set; }

        [Display(Name = "Planta")]
        public int? id_planta { get; set; }

        [Display(Name = "Empleado")]
        public int? id_empleado { get; set; }

        [Display(Name = "Sitio Web")]
        public string website { get; set; }

        [StringLength(100)]

        [Display(Name = "Puesto")]
        public string puesto { get; set; }

        [StringLength(150)]
        [Display(Name = "Correo electrónico")]
        public string email { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono 1")]
        public string phone_1 { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono 2")]
        public string phone_2 { get; set; }

        public string qrCodeText { get; set; }
        public string qrURI { get; set; }

        //planta
        [StringLength(100)]
        [Display(Name = "Calle")]
        public string planta_calle { get; set; }

        [StringLength(50)]
        [Display(Name = "Ciudad")]
        public string planta_ciudad { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado")]
        public string planta_estado { get; set; }

        [StringLength(50)]
        [Display(Name = "Código Postal")]
        public string planta_codigo_postal { get; set; }

        [StringLength(50)]
        [Display(Name = "País")]
        public string planta_pais { get; set; }

        [StringLength(50)]
        [Display(Name = "Color")]
        public string color { get; set; }


        [Display(Name = "¿incluir Icono?")]
        public bool incluye_icono { get; set; }

        public string icoPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Content/images/logo_ico_sin_fondo.png");
            }
        }

    }

    #region Ideas Mejora Models
    public class EstadisticasIMViewModel
    {
        public List<view_ideas_mejora> listadoIdeasView { get; set; }
        public List<SolicitudesPorEstatusViewModel> SolicitudesPorEstatus { get; set; }
        public List<IdeasPorPlantaViewModel> IdeasPorPlanta { get; set; }
        public List<IdeasPorProponenteViewModel> IdeasPorProponente { get; set; }
        public List<IdeasPorDesperdicioViewModel> IdeasPorDesperdicio { get; set; }
        public List<IdeasPorImpactoViewModel> IdeasPorImpacto { get; set; }
        public List<IdeasPorMesPlantaViewModel> IdeasPorMesPlanta { get; set; }
        public List<SolicitudesPorAreaYPlantaViewModel> SolicitudesPorAreaYPlanta { get; set; }



        // Constructor para inicializar las listas
        public EstadisticasIMViewModel()
        {
            IdeasPorPlanta = new List<IdeasPorPlantaViewModel>();
            SolicitudesPorEstatus = new List<SolicitudesPorEstatusViewModel>();
            IdeasPorProponente = new List<IdeasPorProponenteViewModel>();
            IdeasPorDesperdicio = new List<IdeasPorDesperdicioViewModel>();
            IdeasPorImpacto = new List<IdeasPorImpactoViewModel>();
            IdeasPorMesPlanta = new List<IdeasPorMesPlantaViewModel>();
        }
    }

    public class IdeasPorPlantaViewModel
    {
        public string NombrePlanta { get; set; }
        public int TotalIdeas { get; set; }
    }

    public class SolicitudesPorAreaYPlantaViewModel
    {
        public string AreaNombre { get; set; } // Nombre del área
        public string PlantaNombre { get; set; } // Nombre de la planta
        public int Total { get; set; } // Total de solicitudes por área y planta
    }
    public class IdeasPorMesPlantaViewModel
    {
        public string MesPlanta { get; set; }
        public int Total { get; set; }
        public string PlantaNombre { get; set; } // Nombre de la planta para diferenciar en la gráfica

    }
    public class IdeasPorImpactoViewModel
    {
        public string Impacto { get; set; }
        public int Total { get; set; }
    }

    public class SolicitudesPorEstatusViewModel
    {
        public string Estatus { get; set; }
        public int Total { get; set; }
    }

    public class IdeasPorProponenteViewModel
    {
        public string Tipo { get; set; }
        public int Total { get; set; }
    }

    public class IdeasPorDesperdicioViewModel
    {
        public string Desperdicio { get; set; }
        public int Total { get; set; }
    }
    #endregion
    public class CI_Tolerancias
    {
        public string material { get; set; }
        public double? gauge { get; set; }
        public double? gauge_min { get; set; }
        public double? gauge_max { get; set; }
    }

    public class InventarioHub : Hub
    {
        public void ActualizarDatos()
        {
            // Método que los clientes pueden llamar para notificar cambios
            Clients.All.recibirActualizacion();
        }
        public void EnviarProgresoExcel(int porcentaje, int registrosProcesados, int totalRegistros)
        {
            Clients.All.recibirProgresoExcel(porcentaje, registrosProcesados, totalRegistros);
        }
    }

    public class EnvioCorreoAsignacionSCDM
    {
        public EnvioCorreoAsignacionSCDM()
        {
            correosTO = new List<string>();
            correosCC = new List<string>();
        }

        public int id_departamento { get; set; }
        public string departamento { get; set; }
        public string comentario { get; set; }
        public List<string> correosTO { get; set; }
        public List<string> correosCC { get; set; }
    }

}