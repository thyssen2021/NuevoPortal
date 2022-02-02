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
    }
}
