using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class RM_cambio_estatusMetadata
    {

    }

    [MetadataType(typeof(RM_cambio_estatusMetadata))]
    public partial class RM_cambio_estatus
    {
        //Nombre Cliente
        [NotMapped]
        [Display(Name = "Nombre Usuario")]
        public string NombreUsuario
        {
            get
            {
                if (this.empleados != null)
                    return empleados.ConcatNombre;

                return this.nombre_usuario_old;
            }
        }
    }
}