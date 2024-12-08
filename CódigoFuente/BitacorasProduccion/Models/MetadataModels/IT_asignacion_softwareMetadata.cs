using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_asignacion_softwareMetadata
    {
    }


    [MetadataType(typeof(IT_asignacion_softwareMetadata))]
    public partial class IT_asignacion_software : IEquatable<IT_asignacion_software>
    {
        [NotMapped]
        public string ConcatSearch
        {
            get
            {
                //string info = "(" + id + ") ";
                string info = String.Empty;
                try
                {
                    if (this.IT_inventory_software != null)
                        info += "(" + this.IT_inventory_software.descripcion + ") ";

                    if (this.empleados != null)
                        info += "[" + this.empleados.ConcatNombre + "] ";

                    if (!String.IsNullOrEmpty(this.usuario))
                        info += "Usuario: " + this.usuario;
                    else
                        info += "Usuario: N/A";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        }

        //para realizar la comparacion  
        public bool Equals(IT_asignacion_software other)
        {
            if (other is null)
                return false;

            return this.id_sistemas == other.id_sistemas
                && this.id_empleado == other.id_empleado
                && this.id_inventory_software == other.id_inventory_software           
                ;
        }

        public override bool Equals(object obj) => Equals(obj as IT_asignacion_software);
        public override int GetHashCode() => (id_sistemas, id_empleado, id_inventory_software).GetHashCode();
    }
}