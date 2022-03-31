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
    using System.Linq;
    using System.Web;

    public partial class budget_centro_costo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_centro_costo()
        {
            this.budget_rel_fy_centro = new HashSet<budget_rel_fy_centro>();
            this.budget_responsables = new HashSet<budget_responsables>();
        }

        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Departament")]
        public int id_budget_departamento { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Name")]
        public string descripcion { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(6, MinimumLength = 2)]
        [Display(Name = "Cost Center Number")]
        public string num_centro_costo { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Class 1")]
        public string class_1 { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Class 2")]
        public string class_2 { get; set; }

        [Display(Name = "Status")]
        public bool activo { get; set; }
        //concatena
        public string ConcatCentro
        {
            get
            {
                return string.Format("({0}) {1} - {2}", num_centro_costo, budget_departamentos.budget_plantas.descripcion, descripcion).ToUpper();
            }
        }

        //para el archivo de importación 
        [Display(Name = "Document")]
        public HttpPostedFileBase PostedFile { get; set; }

        [Display(Name = "Fiscal Year")]
        public int id_anio_fiscal { get; set; }


        //DEfine variables para rel anio centro
        public budget_rel_fy_centro REL_ANIO_CENTRO_ACTUAL_FORECAST = null;
        public budget_rel_fy_centro REL_ANIO_CENTRO_PROXIMO_BUDGET = null;

        //retorna el Centro de costo Actual
        public budget_rel_fy_centro RelAnioCentroActual_Forecast()
        {

            if (REL_ANIO_CENTRO_ACTUAL_FORECAST != null)
            {
                return REL_ANIO_CENTRO_ACTUAL_FORECAST;
            }

            List<budget_rel_fy_centro> rels = budget_rel_fy_centro.Where(x => x.id_centro_costo == this.id).ToList();


            foreach (budget_rel_fy_centro item in rels)
            {

                DateTime timeInicial = new DateTime(item.budget_anio_fiscal.anio_inicio, item.budget_anio_fiscal.mes_inicio, 1, 0, 0, 0);
                DateTime timeFinal = new DateTime(item.budget_anio_fiscal.anio_fin, item.budget_anio_fiscal.mes_fin, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
                DateTime timeActual = DateTime.Now;

                if ((timeActual >= timeInicial) && (timeActual <= timeFinal))
                {
                    REL_ANIO_CENTRO_ACTUAL_FORECAST = item;

                    return REL_ANIO_CENTRO_ACTUAL_FORECAST;
                }
            }

            return null;

        }

        //retorna el Centro de costo Actual
        public budget_rel_fy_centro RelAnioCentroProximo_Budget()
        {

            if (REL_ANIO_CENTRO_PROXIMO_BUDGET != null)
            {
                return REL_ANIO_CENTRO_PROXIMO_BUDGET;
            }

            List<budget_rel_fy_centro> rels = budget_rel_fy_centro.Where(x => x.id_centro_costo == this.id).ToList();


            foreach (budget_rel_fy_centro item in rels)
            {

                DateTime timeInicial = new DateTime(item.budget_anio_fiscal.anio_inicio, item.budget_anio_fiscal.mes_inicio, 1, 0, 0, 0);
                DateTime timeFinal = new DateTime(item.budget_anio_fiscal.anio_fin, item.budget_anio_fiscal.mes_fin, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
                DateTime timeActual = DateTime.Now.AddYears(1);

                if ((timeActual >= timeInicial) && (timeActual <= timeFinal))
                {
                    REL_ANIO_CENTRO_PROXIMO_BUDGET = item;

                    return REL_ANIO_CENTRO_PROXIMO_BUDGET;
                }
            }

            return null;

        }

        public virtual budget_departamentos budget_departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_fy_centro> budget_rel_fy_centro { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_responsables> budget_responsables { get; set; }
    }
}
