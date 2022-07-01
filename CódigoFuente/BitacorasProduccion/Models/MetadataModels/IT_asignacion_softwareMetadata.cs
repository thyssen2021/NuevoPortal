using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //para realizar la comparacion    

        public bool Equals(IT_asignacion_software other)
        {
            if (other is null)
                return false;

            return this.id_sistemas == other.id_sistemas
                && this.id_empleado == other.id_empleado
                && this.id_inventory_software == other.id_inventory_software
                && this.id_inventory_software_version == other.id_inventory_software_version              
                ;
        }

        public override bool Equals(object obj) => Equals(obj as IT_asignacion_software);
        public override int GetHashCode() => (id_sistemas, id_empleado, id_inventory_software, id_inventory_software_version).GetHashCode();
    }
}