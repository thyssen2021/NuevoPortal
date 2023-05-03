using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Portal_2_0.Models
{
    public class RM_cabeceraMetadata
    {
        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Display(Name = "Activo")]
        public bool activo { get; set; }

        [Display(Name = "Remisión Número")]
        public string remisionNumero { get; set; }

        [Required]
        [Display(Name = "Almacén")]
        public int almacenClave { get; set; }

        [Display(Name = "Transporte (otro)")]
        [StringLength(50, MinimumLength = 1)]
        [RequiredIf("aplicaTransporteOtro", true, ErrorMessage = "El campo {0} es requerido.")]
        public string transporteOtro { get; set; }

        [Display(Name = "Transporte")]
        [RequiredIf("aplicaTransporteOtro", false, ErrorMessage = "El campo {0} es requerido.")]
        public Nullable<int> transporteProveedorClave { get; set; }

        [Required]
        [Display(Name = "Nombre Chofer")]
        [StringLength(50, MinimumLength = 1)]
        public string nombreChofer { get; set; }

        [Display(Name = "Cliente")]
        [RequiredIf("aplicaClienteOtro", false, ErrorMessage = "El campo {0} es requerido.")]
        public Nullable<int> clienteClave { get; set; }

        [Display(Name = "Cliente (otro)")]
        [StringLength(50, MinimumLength = 1)]
        [RequiredIf("aplicaClienteOtro", true, ErrorMessage = "El campo {0} es requerido.")]
        public string clienteOtro { get; set; }

        [Required]
        [Display(Name = "Placa Tractor")]
        [StringLength(50, MinimumLength = 1)]
        public string placaTractor { get; set; }

        [Required]
        [Display(Name = "Placa Remolque")]
        [StringLength(50, MinimumLength = 1)]
        public string placaRemolque { get; set; }

        [Required]
        [Display(Name = "Horario Descarga")]
        [StringLength(50, MinimumLength = 1)]
        public string horarioDescarga { get; set; }

        [Display(Name = "Enviado A")]
        [RequiredIf("aplicaEnviadoOtro", false, ErrorMessage = "El campo {0} es requerido.")]
        public Nullable<int> enviadoAClave { get; set; }

        [Display(Name = "Enviado A (otro)")]
        [StringLength(50, MinimumLength = 1)]
        [RequiredIf("aplicaEnviadoOtro", true, ErrorMessage = "El campo {0} es requerido.")]
        public string enviadoAOtro { get; set; }

        [Display(Name = "Cliente Dirección (otro)")]
        [StringLength(100, MinimumLength = 1)]
        [RequiredIf("aplicaClienteOtro", true, ErrorMessage = "El campo {0} es requerido.")]
        public string clienteOtroDireccion { get; set; }

        [Display(Name = "Enviado Dirección (otro)")]
        [StringLength(100, MinimumLength = 1)]
        [RequiredIf("aplicaEnviadoOtro", true, ErrorMessage = "El campo {0} es requerido.")]
        public string enviadoAOtroDireccion { get; set; }

        [Required]
        [Display(Name = "Motivo")]
        public int motivoClave { get; set; }

        [Required]
        [StringLength(1500, MinimumLength = 1)]
        [Display(Name = "Texto Breve Motivo")]
        public string motivoTexto { get; set; }

        [Display(Name = "¿Retorna Material?")]
        public bool retornaMaterial { get; set; }

        [Display(Name = "Estatus Actual")]
        public Nullable<int> ultimoEstatus { get; set; }
    }

    [MetadataType(typeof(RM_cabeceraMetadata))]
    public partial class RM_cabecera
    {
        //concatena el nombre
        [NotMapped]
        [Display(Name = "Planta")]
        public int id_planta { get; set; }

        [NotMapped]
        [Display(Name = "¿Otro Cliente?")]
        public bool aplicaClienteOtro { get; set; }

        [NotMapped]
        [Display(Name = "¿Enviado a Otro?")]
        public bool aplicaEnviadoOtro { get; set; }

        [NotMapped]
        [Display(Name = "¿Transporte Otro?")]
        public bool aplicaTransporteOtro { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "Observaciones")]
        [StringLength(950, MinimumLength = 1)]
        public string observaciones { get; set; }

        //concatena el nombre
        [NotMapped]
        public string ConcatNumeroRemision
        {
            get
            {
                return string.Format("{0}-{1}", RM_almacen.descripcion, clave.ToString("D6")).ToUpper();
            }
        }

        //Fecha de creacion
        [NotMapped]
        [Display(Name = "Fecha de Creación")]
        public DateTime? FechaCreacion
        {
            get
            {
                if (RM_cambio_estatus.Count == 0)
                    return null;

                return RM_cambio_estatus.OrderBy(x => x.clave).FirstOrDefault().capturaFecha;
            }
        }
        //Nombre Cliente
        [NotMapped]
        [Display(Name = "Nombre Cliente")]
        public string NombreCliente
        {
            get
            {
                if (this.clientes != null)
                    return clientes.descripcion;

                return clienteOtro;
            }
        }
        //Nombre Enviado A
        [NotMapped]
        [Display(Name = "Enviado A")]
        public string EnviadoA
        {
            get
            {
                if (this.clientes1 != null)
                    return clientes1.descripcion;

                return enviadoAOtro;
            }
        }
        //Nombre Transporte
        [NotMapped]
        [Display(Name = "Transporte")]
        public string Transporte
        {
            get
            {
                if (this.RM_transporte_proveedor != null)
                    return RM_transporte_proveedor.descripcion;

                return transporteOtro;
            }
        }

        //Total Cantidad Remisión
        [NotMapped]
        [Display(Name = "Total Cantidad")]
        public double TotalCantidadRemision
        {
            get
            {
                if (RM_elemento.Count == 0)
                    return 0;

                return RM_elemento.Where(x => x.activo.HasValue && x.activo.Value).Sum(x => x.cantidad);
            }
        }
        //Total Peso Remisión
        [NotMapped]
        [Display(Name = "Total Peso")]
        public double TotalPesoRemision
        {
            get
            {
                if (RM_elemento.Count == 0)
                    return 0;

                return RM_elemento.Where(x => x.activo.HasValue && x.activo.Value).Sum(x => x.peso);
            }
        }



        //Creado por
        [NotMapped]
        [Display(Name = "Creada Por")]
        public string CreadaPor
        {
            get
            {
                if (RM_cambio_estatus.Where(x => x.catalogoEstatusClave == 1).Count() == 0)
                    return null;

                return RM_cambio_estatus.Where(x => x.catalogoEstatusClave == 1).OrderBy(x => x.clave).FirstOrDefault().NombreUsuario;
            }
        }

        //public string GetDisplayName(string propertyName) {
        //    string name = string.Empty;
        //    MemberInfo property = typeof(RM_cabeceraMetadata).GetProperty(nameof(RM_cabeceraMetadata.retornaMaterial));
        //    var dd = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        //    if (dd != null)
        //    {
        //        name = dd.Name;
        //    }

        //    return name;
        //}

    }
}