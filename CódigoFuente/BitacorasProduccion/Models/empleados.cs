//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class empleados
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public empleados()
        {
            this.budget_responsables = new HashSet<budget_responsables>();
            this.inspeccion_datos_generales = new HashSet<inspeccion_datos_generales>();
            this.notificaciones_correo = new HashSet<notificaciones_correo>();
            this.orden_trabajo = new HashSet<orden_trabajo>();
            this.orden_trabajo1 = new HashSet<orden_trabajo>();
            this.orden_trabajo2 = new HashSet<orden_trabajo>();
            this.OT_personal_mantenimiento = new HashSet<OT_personal_mantenimiento>();
            this.PFA = new HashSet<PFA>();
            this.PFA_Autorizador = new HashSet<PFA_Autorizador>();
            this.PFA1 = new HashSet<PFA>();
            this.PM_departamentos = new HashSet<PM_departamentos>();
            this.PM_usuarios_capturistas = new HashSet<PM_usuarios_capturistas>();
            this.poliza_manual = new HashSet<poliza_manual>();
            this.poliza_manual1 = new HashSet<poliza_manual>();
            this.poliza_manual2 = new HashSet<poliza_manual>();
            this.poliza_manual3 = new HashSet<poliza_manual>();
            this.poliza_manual4 = new HashSet<poliza_manual>();
            this.produccion_operadores = new HashSet<produccion_operadores>();
            this.produccion_respaldo = new HashSet<produccion_respaldo>();
            this.produccion_supervisores = new HashSet<produccion_supervisores>();
            this.upgrade_usuarios = new HashSet<upgrade_usuarios>();
        }

        public int id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> nueva_fecha_nacimiento { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public Nullable<int> planta_clave { get; set; }
        public Nullable<int> clave { get; set; }

        [Display(Name = "Estatus")]
        public Nullable<bool> activo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(6, MinimumLength = 1)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "S�lo se permiten n�meros.")]
        [Display(Name = "N�mero de Empleado")]
        public string numeroEmpleado { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellido paterno")]
        public string apellido1 { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellido materno")]
        public string apellido2 { get; set; }
        public string nacimientoFecha { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Tel�fono")]
        public string telefono { get; set; }

        [Display(Name = "Extensi�n")]
        public string extension { get; set; }
        [Display(Name = "Celular")]
        public string celular { get; set; }
        [Display(Name = "Nivel")]
        public string nivel { get; set; }

        [Display(Name = "Puesto")]
        public Nullable<int> puesto { get; set; }

        [Display(Name = "Compa�ia")]
        public string compania { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ingresoFecha { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Baja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> bajaFecha { get; set; }

        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "S�lo se permiten n�meros.")]
        public string C8ID { get; set; }

        [Display(Name = "�rea")]
        public Nullable<int> id_area { get; set; }

        //concatena el nombre
        public string ConcatNombre
        {
            get
            {
                return string.Format("{0} {1} {2}", nombre, apellido1, apellido2).ToUpper();
            }
        }

        //concatena el n�mero de empleado con el nombre
        public string ConcatNumEmpleadoNombre
        {
            get
            {
                return string.Format("({0}) {1} {2} {3}", numeroEmpleado, nombre, apellido1, apellido2).ToUpper();
            }
        }

        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_responsables> budget_responsables { get; set; }
        public virtual plantas plantas { get; set; }
        public virtual puesto puesto1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<inspeccion_datos_generales> inspeccion_datos_generales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<notificaciones_correo> notificaciones_correo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_personal_mantenimiento> OT_personal_mantenimiento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA> PFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA_Autorizador> PFA_Autorizador { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA> PFA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_departamentos> PM_departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_usuarios_capturistas> PM_usuarios_capturistas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_operadores> produccion_operadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_respaldo> produccion_respaldo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_supervisores> produccion_supervisores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<upgrade_usuarios> upgrade_usuarios { get; set; }
    }
}
