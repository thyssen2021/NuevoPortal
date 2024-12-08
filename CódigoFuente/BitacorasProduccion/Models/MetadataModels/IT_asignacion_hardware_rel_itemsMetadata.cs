using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_asignacion_hardware_rel_itemsMetadata
    {
    }

    [MetadataType(typeof(IT_asignacion_hardware_rel_itemsMetadata))]
    public partial class IT_asignacion_hardware_rel_items
    {
        public IT_inventory_hardware_type GetHardwareType()
        {
            IT_inventory_hardware_type tipo = null;

            if (this.IT_inventory_items != null)
                tipo = this.IT_inventory_items.IT_inventory_hardware_type;
            else if (this.IT_inventory_items_genericos != null)
                tipo = this.IT_inventory_items_genericos.IT_inventory_hardware_type;

            return tipo;
        }

        public IT_inventory_items GetGenericObject()
        {
            IT_inventory_items _object = new IT_inventory_items();

            //si es tipo inventory item retorna el mismo objetos
            if (this.IT_inventory_items != null)
                _object = this.IT_inventory_items;
            else if (this.IT_inventory_items_genericos != null)
            {
                //propiedades que se reemplazan en item
                _object.id = this.IT_inventory_items_genericos.id;
                _object.id_inventory_type = this.IT_inventory_items_genericos.id_inventory_type; //
                _object.id_tipo_accesorio = this.IT_inventory_items_genericos.id_tipo_accesorio; //
                _object.brand = this.IT_inventory_items_genericos.brand;
                _object.model = this.IT_inventory_items_genericos.model;
                _object.comments = this.IT_inventory_items_genericos.comments;
                _object.active = this.IT_inventory_items_genericos.active;

                _object.IT_inventory_hardware_type = this.IT_inventory_items_genericos.IT_inventory_hardware_type;
                _object.IT_inventory_tipos_accesorios = this.IT_inventory_items_genericos.IT_inventory_tipos_accesorios;

            }

            return _object;
        }

        public bool EsInventario()
        {
            if (this.IT_inventory_items != null)
                return true;
            else if (this.IT_inventory_items_genericos != null)
            {
                return false;
            }

            return false;

        }



        /// <summary>
        /// Determina si la asignación es una asignación actual sin responsable principal
        /// </summary>
        /// <returns></returns>
        public bool TieneAsignacionValida()
        {

            bool result = false;

            using (var db = new Portal_2_0Entities())
            {
                result = db.IT_asignacion_hardware.Any(x => x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == this.id_it_inventory_item) && x.es_asignacion_actual == true && x.id_empleado == x.id_responsable_principal);
            }

            return result;

        }
    }
}