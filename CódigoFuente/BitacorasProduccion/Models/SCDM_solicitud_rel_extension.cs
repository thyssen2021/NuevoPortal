//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SCDM_solicitud_rel_extension
    {
        public int id { get; set; }
        public Nullable<int> id_solicitud_rel_item_material { get; set; }
        public Nullable<int> id_solicitud_rel_creacion_referencia { get; set; }
        public int id_cat_storage_location { get; set; }
        public string extension_ejecucion_correcta { get; set; }
        public string extension_mensaje_sap { get; set; }
        public Nullable<int> id_rel_solicitud_extension_usuario { get; set; }
    
        public virtual SCDM_cat_storage_location SCDM_cat_storage_location { get; set; }
        public virtual SCDM_solicitud_rel_extension_usuario SCDM_solicitud_rel_extension_usuario { get; set; }
        public virtual SCDM_solicitud_rel_item_material SCDM_solicitud_rel_item_material { get; set; }
        public virtual SCDM_solicitud_rel_creacion_referencia SCDM_solicitud_rel_creacion_referencia { get; set; }
    }
}
