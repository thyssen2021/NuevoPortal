using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class log_acceso_emailMetadata
    {
     
    }

    [MetadataType(typeof(log_acceso_emailMetadata))]
    public partial class log_acceso_email
    {
        public string GetResponsable() {
            string result = "NO DISPONIBLE";


            using (var db = new Portal_2_0Entities())
            {
                var equipo = db.IT_inventory_items.Where(x => x.hostname == this.nombre_equipo).FirstOrDefault();

                if (equipo == null)
                    return string.Empty;
                else {
                    List<string> idAsignadosHardware = new List<string>();

                    if (equipo != null)
                    {
                        List<IT_asignacion_hardware_rel_items> rels = equipo.IT_asignacion_hardware_rel_items.ToList();
                        idAsignadosHardware = rels.Where(x => x.IT_asignacion_hardware.es_asignacion_actual).Select(x => x.IT_asignacion_hardware.empleados.ConcatNombre).ToList();
                    }

                    result = String.Join(",", idAsignadosHardware);                    

                    return result;
                }
    

            }

            return result;
        }
    }
}