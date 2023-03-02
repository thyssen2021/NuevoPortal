using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clases.Util
{
    public static class TipoRoles
    {
        public const string USERS = "Usuarios";
        public const string ADMIN = "Admin";
        public const string RH = "RecursosHumanos";
        //bitácoras de producción 
        public const string BITACORAS_PRODUCCION_REGISTRO = "BitacoraProduccionRegistro";
        public const string BITACORAS_PRODUCCION_REPORTE = "BitacoraProduccionReporte";
        public const string BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS = "BitacoraProduccionReporteAllAccess";
        public const string BITACORAS_PRODUCCION_CATALOGOS = "BitacoraProduccionCatalogos";
        public const string REPORTES_PESADAS = "ReportePesadas";
        //Premium Freight Approval
        public const string PFA_REGISTRO = "PFA_RegistroFormato";
        public const string PFA_AUTORIZACION = "PFA_AutorizacionFormato";
        public const string PFA_CATALOGOS = "PFA_AdministracionCatalogos";
        public const string PFA_VISUALIZACION = "PFA_VisualizacionFormato";
        //polizas manuales 
        public const string PM_REGISTRO = "PolizasManuales_CreacionRegistros";
        public const string PM_VALIDAR_POR_AREA = "PolizasManuales_ValidacionPorArea";
        public const string PM_AUTORIZAR_CONTROLLING = "PolizasManuales_AutorizacionControlling";
        public const string PM_CONTABILIDAD = "PolizasManuales_RegistroContabilidad";
        public const string PM_CATALOGOS = "PolizasManuales_catalogos";
        public const string PM_DIRECCION = "PolizasManuales_DireccionAutorizacion";
        public const string PM_REPORTES = "PolizasManuales_reportes";
        //Inspección calidad
        public const string INSPECCION_REGISTRO = "Inspeccion_registros";
        public const string INSPECCION_CATALOGOS = "Inspeccion_catalogos";
        public const string INSPECCION_REPORTES = "Inspeccion_reportes";
        //Órdenes de Trabajo
        public const string OT_SOLICITUD = "OrdenesTrabajo_Solicitud";
        public const string OT_ASIGNACION = "OrdenesTrabajo_Asignacion";
        public const string OT_RESPONSABLE = "OrdenesTrabajo_Responsable";
        public const string OT_REPORTE = "OrdenesTrabajo_Reportes";
        public const string OT_CATALOGOS = "OrdenesTrabajo_Catalogos";
        public const string OT_ADMINISTRADOR = "OrdenesTrabajo_Administrador";
        //Plantilla Budget Controlling
        public const string BG_RESPONSABLE = "BG_Responsable_Centro_Costo";
        public const string BG_CONTROLLING = "BG_Controlling";
        public const string BG_REPORTES = "BG_Reportes";
        //Módulo IT - Matriz Requerimientos
        public const string IT_SOLICITUD_USUARIOS = "IT_Solicitud_usuarios";
        public const string IT_MATRIZ_REQUERIMIENTOS_CREAR = "IT_Matriz_requerimientos_crear";
        public const string IT_MATRIZ_REQUERIMIENTOS_DETALLES = "IT_Matriz_requerimientos_detalles";
        public const string IT_MATRIZ_REQUERIMIENTOS_AUTORIZAR = "IT_Matriz_requerimientos_autorizar";
        public const string IT_MATRIZ_REQUERIMIENTOS_CERRAR = "IT_Matriz_requerimientos_cerrar";
        public const string IT_CATALOGOS = "IT_Catalogos";
        //Módulo IT - Inventory
        public const string IT_INVENTORY = "IT_Inventory";
        public const string IT_ASIGNACION_HARDWARE = "IT_Asignacion_Hardware";
        //Módulo de Gastos de Viaje
        public const string GV_SOLICITUD = "GV_SOLICITUD";
        public const string GV_JEFE_DIRECTO = "GV_JEFE_DIRECTO";
        public const string GV_CONTROLLING = "GV_CONTROLLING";
        public const string GV_CONTABILIDAD = "GV_CONTABILIDAD";
        public const string GV_NOMINA = "GV_NOMINA";
        public const string GV_REPORTES = "GV_CONTABILIDAD";
        public const string GV_CATALOGOS = "GV_CATALOGOS";
        public const string GV_AUTORIZACION = "GV_AUTORIZACION";
        //Módulo IT - Mantenimiento
        public const string IT_MANTENIMIENTO_REGISTRO = "IT_Mantenimento_registro";
        //Modulo IT - Checklists
        public const string IT_CHECKLIST_SITE = "IT_Check_site";
        public const string IT_CHECKLIST_EQUIPOS = "IT_Check_equipos";
        //Módulo RH - Menu comedor 
        public const string RH_MENU_COMEDOR_PUEBLA = "RH_Menu_Comedor_Puebla";
        public const string RH_MENU_COMEDOR_VISUALIZAR_PUEBLA = "Menu_Comedor_Visualizar_Puebla";
    }
}
