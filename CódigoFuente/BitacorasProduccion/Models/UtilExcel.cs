using Clases.Util;
using DocumentFormat.OpenXml.InkML;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Portal_2_0.Models
{
    public class UtilExcel
    {
        public static List<string> ListadoCabeceraIHS = new List<string> {
            //"Core Nameplate Region Mnemonic",
            //"Core Nameplate Plant Mnemonic",
            "Mnemonic-Vehicle",
            "Mnemonic-Vehicle/Plant",
            "Mnemonic-Platform",
            //"Mnemonic-Plant",
            "Region",
            "Market",
            //"Country/Territory",
            "Production Plant",
            "City",
            "Plant State/Province",
            "Source Plant",
            //"Source Plant Country/Territory",
            "Source Plant Region",
            "Design Parent",
            "Engineering Group",
            "Manufacturer Group",
            "Manufacturer",
            "Sales Parent",
            "Production Brand",
            "Platform Design Owner",
            "Architecture",
            "Platform",
            "Program",
            "Production Nameplate",
            "SOP (Start of Production)",  //datetime
            "EOP (End of Production)",  //datetime
            "Lifecycle (Time)",         //int
            "Vehicle",
            "Assembly Type",
            "Strategic Group",
            "Sales Group",
            "Global Nameplate",
            "Primary Design Center",
            //"Primary Design Country/Territory",
            "Primary Design Region",
            "Secondary Design Center",
            //"Secondary Design Country/Territory",
            "Secondary Design Region",
            "GVW Rating",
            "GVW Class",
            "Car/Truck",
            "Production Type",
            "Global Production Segment",
            "Regional Sales Segment",
            "Global Production Price Class",
            "Global Sales Segment",
            "Global Sales Sub-Segment",
            "Global Sales Price Class", //int
            "Short-Term Risk Rating",//int
            "Long-Term Risk Rating", //int
        };

        public static List<string> ListadoCabeceraReporteBudget = new List<string> {
            "Coils & Slitter",
            "Business & Plant",
            "Business",
            "Additional Processes",
            "Production Processes",
            "SAP - INVOICE CODE",
            "PREVIOUS SAP INVOICE CODE",
            "INVOICED TO:",
            "NUMBER SAP CLIENT",
            "OWN /CM",
            "ROUTE",
            "PLANT",
            "EXTERNAL PROCESSOR",
            "MILL",
            "SAP MASTER COIL",
            "PART NUMBER",
            "Mnemonic-Vehicle/Plant",
            "Propulsion System Type",
            "MATERIAL TYPE",
            "Scrap Consolidation [Yes / No]",
            "Scrap Consolidation [Yes / No]",
            "MATERIAL COST / PART [USD]",
            "COST OF OUTSIDE PROCESSOR /PART [USD]",
        };

        ///<summary>
        ///Lee un archivo de excel y carga el listado de bom
        ///</summary>
        ///<return>
        ///Devuelve un List<bom_en_sap> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        //public static List<bom_en_sap> LeeBom(HttpPostedFileBase streamPostedFile, ref bool valido)
        //{
        //    List<bom_en_sap> lista = new List<bom_en_sap>();

        //    //crea el reader del archivo
        //    using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
        //    {
        //        //obtiene el dataset del archivo de excel
        //        var result = reader.AsDataSet();

        //        //estable la variable a false por defecto
        //        valido = false;

        //        //recorre todas las hojas del archivo
        //        foreach (DataTable table in result.Tables)
        //        {
        //            //busca si existe una hoja llamada "dante"
        //            if (table.TableName.ToUpper() == "SHEET1" || table.TableName.ToUpper() == "HOJA1")
        //            {
        //                valido = true;

        //                //se obtienen las cabeceras
        //                List<string> encabezados = new List<string>();

        //                for (int i = 0; i < table.Columns.Count; i++)
        //                {
        //                    string title = table.Rows[0][i].ToString();

        //                    if (!string.IsNullOrEmpty(title))
        //                        encabezados.Add(title.ToUpper());
        //                }

        //                //verifica que la estrura del archivo sea válida
        //                if (!encabezados.Contains("MATERIAL NO.") || !encabezados.Contains("PLANT") || !encabezados.Contains("BOM NO.")
        //                    || !encabezados.Contains("ALTERNATIVE BOM") || !encabezados.Contains("ITEM NO") || !encabezados.Contains("BOM COMPONENT"))
        //                {
        //                    valido = false;
        //                    return lista;
        //                }

        //                //la fila cero se omite (encabezado)
        //                for (int i = 1; i < table.Rows.Count; i++)
        //                {
        //                    try
        //                    {
        //                        //variables
        //                        string material = String.Empty;
        //                        string Plnt = String.Empty;
        //                        string BOM = String.Empty;
        //                        string AltBOM = String.Empty;
        //                        string Item = String.Empty;
        //                        string Component = String.Empty;
        //                        Nullable<double> Quantity = null;
        //                        string Un = string.Empty;
        //                        Nullable<System.DateTime> Created = null;
        //                        Nullable<System.DateTime> LastUsed = null;

        //                        //recorre todas los encabezados
        //                        for (int j = 0; j < encabezados.Count; j++)
        //                        {
        //                            //obtiene la cabezara de i
        //                            switch (encabezados[j])
        //                            {
        //                                //obligatorios
        //                                case "MATERIAL NO.":
        //                                    material = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "PLANT":
        //                                    Plnt = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "BOM NO.":
        //                                    BOM = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "ALTERNATIVE BOM":
        //                                    AltBOM = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "ITEM NO":
        //                                    Item = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "BOM COMPONENT":
        //                                    Component = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "COMPONENT QTY":
        //                                    if (Double.TryParse(table.Rows[i][j].ToString(), out double q))
        //                                        Quantity = q;
        //                                    break;
        //                                case "UNIT OF MEASURE":
        //                                    Un = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "DATE CREATED":
        //                                    if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
        //                                        Created = Convert.ToDateTime(table.Rows[i][j].ToString());
        //                                    break;
        //                                case "LAST DATE USED":
        //                                    if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
        //                                        LastUsed = Convert.ToDateTime(table.Rows[i][j].ToString());
        //                                    break;
        //                            }
        //                        }


        //                        //agrega a la lista con los datos leidos
        //                        lista.Add(new bom_en_sap()
        //                        {
        //                            Material = material,
        //                            Plnt = Plnt,
        //                            BOM = BOM,
        //                            AltBOM = AltBOM,
        //                            Item = Item,
        //                            Component = Component,
        //                            Un = Un,
        //                            Quantity = Quantity,
        //                            Created = Created,
        //                            LastDateUsed = LastUsed

        //                        });
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        System.Diagnostics.Debug.Print("Error: " + e.Message);
        //                    }
        //                }

        //            }
        //        }

        //    }

        //    return lista;
        //}

        ///<summary>
        ///Lee un archivo de excel y carga el listado con el respaldo de bitacora
        ///</summary>
        ///<return>
        ///Devuelve un List<produccion_respaldp> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<produccion_respaldo> LeeRespaldoBitacora(HttpPostedFileBase streamPostedFile, ref bool valido)
        {
            List<produccion_respaldo> lista = new List<produccion_respaldo>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    //busca si existe una hoja llamada "dante"
                    if (table.TableName.ToUpper() == "ROLLOS")
                    {
                        valido = true;

                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[3][i].ToString();

                            if (!string.IsNullOrEmpty(title))
                                encabezados.Add(title.ToUpper());
                        }


                        //verifica que la estrura del archivo sea válida
                        if (!encabezados.Contains("OPERADOR") || !encabezados.Contains("SUPERVISOR") || !encabezados.Contains("TIPO DE MATERIAL")
                            || !encabezados.Contains("SAP ROLLO") || !encabezados.Contains("MATERIAL") || !encabezados.Contains("FECHA"))
                        {
                            valido = false;
                            return lista;
                        }

                        //las primeras cinco filas se omite (encabezado)
                        for (int i = 5; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                //variables

                                String planta = String.Empty;
                                String linea = String.Empty;
                                string supervisor = String.Empty;
                                string operador = String.Empty;
                                string sap_platina = String.Empty;
                                string tipo_material = String.Empty;
                                string numero_parte_cliente = String.Empty;
                                string sap_rollo = String.Empty;
                                string material = String.Empty;
                                DateTime fecha = DateTime.MinValue;
                                string turno = String.Empty;
                                DateTime hora = DateTime.MinValue;
                                string orden_sap = String.Empty;
                                string orden_sap_2 = String.Empty;
                                Nullable<double> piezas_por_golpe = null;
                                String numero_rollo = String.Empty;
                                String lote_rollo = String.Empty;
                                Nullable<double> peso_etiqueta = null;
                                Nullable<double> peso_regreso_rollo_real = null;
                                Nullable<double> peso_rollo_usado = null;
                                Nullable<double> peso_bascula_kgs = null;
                                Nullable<double> total_piezas = null;
                                Nullable<double> numero_golpes = null;
                                Nullable<double> peso_despunte_kgs = null;
                                Nullable<double> peso_cola_kgs = null;
                                Nullable<double> porcentaje_punta_y_colas = null;
                                Nullable<double> total_piezas_ajuste = null;
                                Nullable<double> peso_bruto_kgs = null;
                                Nullable<double> peso_real_pieza_bruto = null;
                                Nullable<double> peso_real_pieza_neto = null;
                                Nullable<double> scrap_natural = null;
                                Nullable<double> peso_neto_sap = null;
                                Nullable<double> peso_bruto_sap = null;
                                Nullable<double> balance_scrap = null;
                                Nullable<double> ordenes_por_pieza = null;
                                Nullable<double> peso_rollo_usado_real_kgs = null;
                                Nullable<double> peso_bruto_total_piezas_kgs = null;
                                Nullable<double> peso_neto_total_piezas_kgs = null;
                                Nullable<double> scrap_ingenieria_buenas_mas_ajuste = null;
                                Nullable<double> peso_neto_total_piezas_ajuste = null;
                                Nullable<double> peso_punta_y_colas_reales = null;
                                Nullable<double> balance_scrap_real = null;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //si operador es vacio pasa a la siguiente linea
                                    if (String.IsNullOrEmpty(table.Rows[i][2].ToString()))
                                    {
                                        break;
                                    }

                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {

                                        case "PLANTA":
                                            planta = table.Rows[i][j].ToString();
                                            break;
                                        case "LINEA":
                                            linea = table.Rows[i][j].ToString();
                                            break;
                                        case "OPERADOR":
                                            operador = table.Rows[i][j].ToString();
                                            break;
                                        case "SUPERVISOR":
                                            supervisor = table.Rows[i][j].ToString();
                                            break;
                                        case "SAP PLATINA":
                                            sap_platina = table.Rows[i][j].ToString();
                                            break;
                                        case "TIPO DE MATERIAL":
                                            tipo_material = table.Rows[i][j].ToString();
                                            break;
                                        case "NÚMERO DE PARTE DE CLIENTE":
                                            numero_parte_cliente = table.Rows[i][j].ToString();
                                            break;
                                        case "SAP ROLLO":
                                            sap_rollo = table.Rows[i][j].ToString();
                                            break;
                                        case "MATERIAL":
                                            material = table.Rows[i][j].ToString();
                                            break;
                                        case "FECHA":
                                            if (DateTime.TryParse(table.Rows[i][j].ToString(), out DateTime f))
                                                fecha = f;
                                            break;
                                        case "TURNO":
                                            turno = table.Rows[i][j].ToString();
                                            break;
                                        case "HORA":
                                            if (DateTime.TryParse(table.Rows[i][j].ToString(), out DateTime h))
                                            {
                                                fecha = fecha.AddHours(h.Hour).AddMinutes(h.Minute).AddSeconds(h.Second);
                                                hora = fecha;
                                            }
                                            break;
                                        case "ORDEN SAP":
                                            orden_sap = table.Rows[i][j].ToString();
                                            break;
                                        case "ORDEN EN SAP 2":
                                            orden_sap_2 = table.Rows[i][j].ToString();
                                            break;
                                        case "PIEZA POR GOLPE":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ppg))
                                                piezas_por_golpe = ppg;
                                            break;
                                        case "N° DE ROLLO":
                                            numero_rollo = table.Rows[i][j].ToString();
                                            break;
                                        case "LOTE DE ROLLO":
                                            lote_rollo = table.Rows[i][j].ToString();
                                            break;
                                        case "PESO ETIQUETA (KG)":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pe))
                                                peso_etiqueta = pe;
                                            break;
                                        case "PESO DE REGRESO DE ROLLO REAL":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double prrr))
                                                peso_regreso_rollo_real = prrr;
                                            break;
                                        case "PESO DE ROLLO USADO":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pru))
                                                peso_rollo_usado = pru;
                                            break;
                                        case "PESO BÁSCULA KGS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pbk))
                                                peso_bascula_kgs = pbk;
                                            break;
                                        case "TOTAL DE PIEZAS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double tp))
                                                total_piezas = tp;
                                            break;
                                        case "NUMERO DE GOLPES":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ng))
                                                numero_golpes = ng;
                                            break;
                                        case "PESO DESPUNTE KGS.":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pd))
                                                peso_despunte_kgs = pd;
                                            break;
                                        case "PESO COLA KGS.":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pc))
                                                peso_cola_kgs = pc;
                                            break;
                                        case "PORCENTAJE DE PUNTAS Y COLAS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ppc))
                                                porcentaje_punta_y_colas = ppc;
                                            break;
                                        case "TOTAL DE PIEZAS DE AJUSTES":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double tpa))
                                                total_piezas_ajuste = tpa;
                                            break;
                                        case "PESO BRUTO KGS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pb))
                                                peso_bruto_kgs = pb;
                                            break;
                                        case "PESO REAL PIEZA BRUTO":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double prpb))
                                                peso_real_pieza_bruto = prpb;
                                            break;
                                        case "PESO REAL PIEZA NETO":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double prpn))
                                                peso_real_pieza_neto = prpn;
                                            break;
                                        case "SCRAP NATURAL":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double sn))
                                                scrap_natural = sn;
                                            break;
                                        case "PESO NETO SAP":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pns))
                                                peso_neto_sap = pns;
                                            break;
                                        case "PESO BRUTO SAP":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pbs))
                                                peso_bruto_sap = pbs;
                                            break;
                                        case "BALANCE DE SCRAP":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double bs))
                                                balance_scrap = bs;
                                            break;
                                        case "ORDENES POR PIEZA":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double opp))
                                                ordenes_por_pieza = opp;
                                            break;
                                        case "PESO DE ROLLO USADO REAL KG":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double prur))
                                                peso_rollo_usado_real_kgs = prur;
                                            break;
                                        case "PESO BRUTO TOTAL PIEZAS KG":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pbtp))
                                                peso_bruto_total_piezas_kgs = pbtp;
                                            break;
                                        case "PESO NETOTOTAL PIEZAS KG":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pntp))
                                                peso_neto_total_piezas_kgs = pntp;
                                            break;
                                        case "SCRAP DE INGENIERÍA (BUENAS + AJUSTE) TOTAL PIEZAS KG":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double siba))
                                                scrap_ingenieria_buenas_mas_ajuste = siba;
                                            break;
                                        case "PESO NETO TOTAL PIEZAS DE AJUSTE KGS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pntpa))
                                                peso_neto_total_piezas_ajuste = pntpa;
                                            break;
                                        case "PESO PUNTAS Y COLAS REALES KG":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ppcr))
                                                peso_punta_y_colas_reales = ppcr;
                                            break;
                                        case "BALANCE DE SCRAP REAL":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double bsr))
                                                balance_scrap_real = bsr;
                                            break;
                                    }
                                }

                                //no agrega si no hay operador
                                if (!String.IsNullOrEmpty(operador))
                                {
                                    //agrega a la lista con los datos leidos
                                    lista.Add(new produccion_respaldo()
                                    {
                                        fecha_carga = DateTime.Now,
                                        planta = !String.IsNullOrEmpty(planta) ? planta : null,
                                        linea = !String.IsNullOrEmpty(linea) ? linea : null,
                                        supervisor = !String.IsNullOrEmpty(supervisor) ? supervisor : null,
                                        operador = !String.IsNullOrEmpty(operador) ? operador : null,
                                        sap_platina = !String.IsNullOrEmpty(sap_platina) ? sap_platina : null,
                                        tipo_material = !String.IsNullOrEmpty(tipo_material) ? tipo_material : null,
                                        numero_parte_cliente = !String.IsNullOrEmpty(numero_parte_cliente) ? numero_parte_cliente : null,
                                        sap_rollo = !String.IsNullOrEmpty(sap_rollo) ? sap_rollo : null,
                                        material = !String.IsNullOrEmpty(material) ? material : null,
                                        fecha = fecha,
                                        turno = !String.IsNullOrEmpty(turno) ? turno : null,
                                        hora = hora,
                                        orden_sap = !String.IsNullOrEmpty(orden_sap) ? orden_sap : null,
                                        orden_sap_2 = !String.IsNullOrEmpty(orden_sap_2) ? orden_sap_2 : null,
                                        pieza_por_golpe = piezas_por_golpe,
                                        numero_rollo = !String.IsNullOrEmpty(numero_rollo) ? numero_rollo : null,
                                        lote_rollo = !String.IsNullOrEmpty(lote_rollo) ? lote_rollo : null,
                                        peso_etiqueta = peso_etiqueta,
                                        peso_regreso_rollo_real = peso_regreso_rollo_real,
                                        peso_rollo_usado = peso_rollo_usado,
                                        peso_bascula_kgs = peso_bascula_kgs,
                                        total_piezas = total_piezas,
                                        numero_golpes = numero_golpes,
                                        peso_despunte_kgs = peso_despunte_kgs,
                                        peso_cola_kgs = peso_cola_kgs,
                                        porcentaje_punta_y_colas = porcentaje_punta_y_colas,
                                        total_piezas_ajuste = total_piezas_ajuste,
                                        peso_bruto_kgs = peso_bruto_kgs,
                                        peso_real_pieza_bruto = peso_real_pieza_bruto,
                                        peso_real_pieza_neto = peso_real_pieza_neto,
                                        scrap_natural = scrap_natural,
                                        peso_neto_sap = peso_neto_sap,
                                        peso_bruto_sap = peso_bruto_sap,
                                        balance_scrap = balance_scrap,
                                        ordenes_por_pieza = ordenes_por_pieza,
                                        peso_rollo_usado_real_kgs = peso_rollo_usado_real_kgs,
                                        peso_bruto_total_piezas_kgs = peso_bruto_total_piezas_kgs,
                                        peso_neto_total_piezas_kgs = peso_neto_total_piezas_kgs,
                                        scrap_ingenieria_buenas_mas_ajuste = scrap_ingenieria_buenas_mas_ajuste,
                                        peso_neto_total_piezas_ajuste = peso_neto_total_piezas_ajuste,
                                        peso_punta_y_colas_reales = peso_punta_y_colas_reales,
                                        balance_scrap_real = balance_scrap_real,
                                    });
                                }



                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.Print("Error: " + e.Message);
                            }
                        }

                    }
                }

            }

            return lista;
        }


        ///<summary>
        ///Lee un archivo de excel y carga el listado de class
        ///</summary>
        ///<return>
        ///Devuelve un List<class_v3> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        //public static List<class_v3> LeeClass(HttpPostedFileBase streamPostedFile, ref bool valido)
        //{
        //    List<class_v3> lista = new List<class_v3>();

        //    //crea el reader del archivo
        //    using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
        //    {
        //        //obtiene el dataset del archivo de excel
        //        var result = reader.AsDataSet();

        //        //estable la variable a false por defecto
        //        valido = false;

        //        //recorre todas las hojas del archivo
        //        foreach (DataTable table in result.Tables)
        //        {
        //            //busca si existe una hoja llamada "dante"
        //            if (table.TableName.ToUpper() == "SHEET1" || table.TableName.ToUpper() == "HOJA1")
        //            {
        //                valido = true;

        //                //se obtienen las cabeceras
        //                List<string> encabezados = new List<string>();

        //                for (int i = 0; i < table.Columns.Count; i++)
        //                {
        //                    string title = table.Rows[0][i].ToString();

        //                    if (!string.IsNullOrEmpty(title))
        //                        encabezados.Add(title.ToUpper());
        //                }

        //                //verifica que la estrura del archivo sea válida
        //                if (!encabezados.Contains("OBJECT") || !encabezados.Contains("GRADE") || !encabezados.Contains("CUSTOMER PART NUMBER")
        //                    || !encabezados.Contains("SURFACE") || !encabezados.Contains("MILL"))
        //                {
        //                    valido = false;
        //                    return lista;
        //                }

        //                //la fila cero se omite (encabezado)
        //                for (int i = 1; i < table.Rows.Count; i++)
        //                {
        //                    try
        //                    {
        //                        //variables
        //                        string Object = String.Empty;
        //                        string Grape = String.Empty;
        //                        string Customer = String.Empty;
        //                        string Shape = String.Empty;
        //                        string Customer_part_number = String.Empty;
        //                        string Surface = String.Empty;
        //                        string Gauge___Metric = String.Empty;
        //                        string Mill = String.Empty;
        //                        string Width___Metr = String.Empty;
        //                        string Length_mm_ = String.Empty;
        //                        //nuevos valores
        //                        string commodity = string.Empty;
        //                        string flatness_metric = string.Empty;
        //                        string surface_treatment = string.Empty;
        //                        string coating_weight = string.Empty;
        //                        string customer_part_msa = string.Empty;
        //                        string outer_diameter_coil = string.Empty;
        //                        string inner_diameter_coil = string.Empty;


        //                        //recorre todas los encabezados
        //                        for (int j = 0; j < encabezados.Count; j++)
        //                        {
        //                            //obtiene la cabezara de i
        //                            switch (encabezados[j])
        //                            {
        //                                //obligatorios
        //                                case "OBJECT":
        //                                    Object = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "GRADE":
        //                                    Grape = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "CUSTOMER":
        //                                    Customer = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "SHAPE":
        //                                    Shape = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "CUSTOMER PART NUMBER":
        //                                    Customer_part_number = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "SURFACE":
        //                                    Surface = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "GAUGE - METRIC":
        //                                    Gauge___Metric = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "MILL":
        //                                    Mill = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "WIDTH - METR":
        //                                    Width___Metr = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "LENGTH(MM)":
        //                                    Length_mm_ = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "COMMODITY":
        //                                    commodity = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "FLATNESS - METRIC":
        //                                    flatness_metric = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "SURFACE TREATMENT":
        //                                    surface_treatment = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "COATING WEIGHT":
        //                                    coating_weight = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "CUSTOMER PART 2 (MSA)":
        //                                    customer_part_msa = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "OUTER DIAMETER OF COIL":
        //                                    outer_diameter_coil = table.Rows[i][j].ToString();
        //                                    break;
        //                                case "INNER DIAMETER OF COIL":
        //                                    inner_diameter_coil = table.Rows[i][j].ToString();
        //                                    break;

        //                            }
        //                        }


        //                        //agrega a la lista con los datos leidos
        //                        lista.Add(new class_v3()
        //                        {
        //                            Object = Object,
        //                            Grade = Grape,
        //                            Customer = Customer,
        //                            Shape = Shape,
        //                            Customer_part_number = Customer_part_number,
        //                            Surface = Surface,
        //                            Gauge___Metric = Gauge___Metric,
        //                            Mill = Mill,
        //                            Width___Metr = Width___Metr,
        //                            Length_mm_ = Length_mm_,
        //                            commodity = commodity,
        //                            flatness_metric = flatness_metric,
        //                            surface_treatment = surface_treatment,
        //                            coating_weight = coating_weight,
        //                            customer_part_msa = customer_part_msa,
        //                            outer_diameter_coil = outer_diameter_coil,
        //                            inner_diameter_coil = inner_diameter_coil,
        //                            activo = true,
        //                        });
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        System.Diagnostics.Debug.Print("Error: " + e.Message);
        //                    }
        //                }

        //            }
        //        }

        //    }

        //    return lista;
        //}


        //public static List<mm_v3> LeeMM(HttpPostedFileBase streamPostedFile, ref bool valido)
        //{
        //    // Asegúrate de llamar esto una sola vez (por ejemplo en Global.asax):
        //    // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //    var lista = new List<mm_v3>();
        //    var lookup = new Dictionary<string, mm_v3>();

        //    using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
        //    {
        //        var ds = reader.AsDataSet();
        //        valido = false;

        //        foreach (DataTable table in ds.Tables)
        //        {
        //            var sheet = table.TableName.ToUpper();
        //            if (sheet != "SHEET1" && sheet != "HOJA1")
        //                continue;

        //            valido = true;

        //            // Construir mapa de encabezados → lista de índices
        //            var headerMap = table.Rows[0].ItemArray
        //                .Select((h, i) => new { hdr = (h ?? "").ToString().ToUpper(), i })
        //                .Where(x => !string.IsNullOrWhiteSpace(x.hdr))
        //                .GroupBy(x => x.hdr)
        //                .ToDictionary(g => g.Key, g => g.Select(x => x.i).ToList());

        //            // Validar columnas obligatorias
        //            var required = new[]
        //            {
        //        "MATERIAL", "PLNT", "MS",
        //        "MATERIAL DESCRIPTION", "TYPE OF MATERIAL", "TYPE OF METAL"
        //    };
        //            if (required.Any(h => !headerMap.ContainsKey(h)))
        //                return lista;

        //            // Helpers por índice
        //            Func<DataRow, int, string> GetStringIdx = (row, idx) => row[idx]?.ToString();
        //            Func<DataRow, int, double?> GetDoubleIdx = (row, idx) =>
        //                double.TryParse(row[idx]?.ToString(), out var d) ? (double?)d : null;
        //            Func<DataRow, int, int?> GetIntIdx = (row, idx) =>
        //                int.TryParse(row[idx]?.ToString(), out var n) ? (int?)n : null;
        //            Func<DataRow, int, bool?> GetBoolIdx = (row, idx) =>
        //            {
        //                var txt = row[idx]?.ToString();
        //                if (string.IsNullOrWhiteSpace(txt)) return null;
        //                if (bool.TryParse(txt, out var b)) return b;
        //                return true; // cualquier texto → true
        //            };
        //            Func<DataRow, int, DateTime?> GetDateIdx = (row, idx) =>
        //            {
        //                var txt = row[idx]?.ToString();
        //                if (string.IsNullOrWhiteSpace(txt)) return null;
        //                if (DateTime.TryParse(txt, out var dtFull)) return dtFull;
        //                var m = Regex.Match(txt.Trim(), @"^(\d{4})-(\d{1,2})$");
        //                if (m.Success
        //                    && int.TryParse(m.Groups[1].Value, out var y)
        //                    && int.TryParse(m.Groups[2].Value, out var mo)
        //                    && mo >= 1 && mo <= 12)
        //                {
        //                    return new DateTime(y, mo, 1);
        //                }
        //                return null;
        //            };

        //            // Recorrer filas de datos
        //            for (int r = 1; r < table.Rows.Count; r++)
        //            {
        //                var row = table.Rows[r];

        //                // Claves únicas
        //                var mat = headerMap["MATERIAL"]
        //                               .Select(i => GetStringIdx(row, i))
        //                               .FirstOrDefault(s => !string.IsNullOrEmpty(s));
        //                var plnt = headerMap["PLNT"]
        //                               .Select(i => GetStringIdx(row, i))
        //                               .FirstOrDefault(s => !string.IsNullOrEmpty(s));
        //                if (string.IsNullOrWhiteSpace(mat) || string.IsNullOrWhiteSpace(plnt))
        //                    continue;

        //                var key = $"{mat}|{plnt}";
        //                if (!lookup.TryGetValue(key, out mm_v3 item))
        //                {
        //                    item = new mm_v3
        //                    {
        //                        Material = mat,
        //                        Plnt = plnt,
        //                        activo = true
        //                    };
        //                    lookup[key] = item;
        //                    lista.Add(item);
        //                }

        //                // — Texto —
        //                if (headerMap.TryGetValue("MS", out var msCols))
        //                    item.MS = GetStringIdx(row, msCols[0]);
        //                if (headerMap.TryGetValue("TYPE OF MATERIAL", out var tomCols))
        //                    item.Type_of_Material = GetStringIdx(row, tomCols[0]);
        //                if (headerMap.TryGetValue("TYPE OF METAL", out var tmeCols))
        //                    item.Type_of_Metal = GetStringIdx(row, tmeCols[0]);
        //                if (headerMap.TryGetValue("OLD MATERIAL NO.", out var oldCols))
        //                    item.Old_material_no_ = GetStringIdx(row, oldCols[0]);
        //                if (headerMap.TryGetValue("HEAD AND TAILS SCRAP CONCILIAT", out var htcCols))
        //                    item.Head_and_Tails_Scrap_Conciliation = GetStringIdx(row, htcCols[0]);
        //                if (headerMap.TryGetValue("ENGINEERING SCRAP CONCILIATION", out var escCols))
        //                    item.Engineering_Scrap_conciliation = GetStringIdx(row, escCols[0]);
        //                if (headerMap.TryGetValue("BUSINESS MODEL", out var bmCols))
        //                    item.Business_Model = GetStringIdx(row, bmCols[0]);
        //                if (headerMap.TryGetValue("RE-APPLICATION", out var raCols))
        //                    item.Re_application = GetStringIdx(row, raCols[0]);
        //                if (headerMap.TryGetValue("VEHICLE NUMBER 1", out var ihs1Cols))
        //                    item.IHS_number_1 = GetStringIdx(row, ihs1Cols[0]);
        //                if (headerMap.TryGetValue("VEHICLE NUMBER 2", out var ihs2Cols))
        //                    item.IHS_number_2 = GetStringIdx(row, ihs2Cols[0]);
        //                if (headerMap.TryGetValue("VEHICLE NUMBER 3", out var ihs3Cols))
        //                    item.IHS_number_3 = GetStringIdx(row, ihs3Cols[0]);
        //                if (headerMap.TryGetValue("PROP SYS DEG", out var ihs4Cols))
        //                    item.IHS_number_4 = GetStringIdx(row, ihs4Cols[0]);
        //                if (headerMap.TryGetValue("S  1 PROG", out var ihs5Cols))
        //                    item.IHS_number_5 = GetStringIdx(row, ihs5Cols[0]);
        //                if (headerMap.TryGetValue("TYPE OF SELLING", out var tosCols))
        //                    item.Type_of_Selling = GetStringIdx(row, tosCols[0]);
        //                if (headerMap.TryGetValue("PACKAGE PIECES", out var ppCols))
        //                    item.Package_Pieces = GetStringIdx(row, ppCols[0]);
        //                // unidad_medida y size_dimensions
        //                if (headerMap.TryGetValue("BUN", out var bunCols))
        //                    item.unidad_medida = GetStringIdx(row, bunCols[0]);
        //                //else if (headerMap.TryGetValue("UN.", out var unCols))
        //                //    item.unidad_medida = GetStringIdx(row, unCols[0]);

        //                if (headerMap.TryGetValue("SIZE/DIMENSIONS", out var sdCols))
        //                    item.size_dimensions = GetStringIdx(row, sdCols[0]);
        //                // double_pieces, coil_position, Tipo_de_Transporte, Type_of_Pallet
        //                if (headerMap.TryGetValue("DOUBLE PIECES", out var dpCols))
        //                    item.double_pieces = GetStringIdx(row, dpCols[0]);
        //                if (headerMap.TryGetValue("COIL/SLITTER POSITION", out var cpCols))
        //                    item.coil_position = GetStringIdx(row, cpCols[0]);
        //                if (headerMap.TryGetValue("TIPO TRANSPORTE", out var tdtCols))
        //                    item.Tipo_de_Transporte = GetStringIdx(row, tdtCols[0]);
        //                if (headerMap.TryGetValue("TYPE PALLET", out var topCols))
        //                    item.Type_of_Pallet = GetStringIdx(row, topCols[0]);

        //                // — Duplicados: Gross Weight y Net Weight —
        //                if (headerMap.TryGetValue("GROSS WEIGHT", out var gwCols))
        //                {
        //                    if (gwCols.Count > 0) item.Gross_weight = GetDoubleIdx(row, gwCols[0]);
        //                    if (gwCols.Count > 1) item.Un_ = GetStringIdx(row, gwCols[1]);
        //                }
        //                if (headerMap.TryGetValue("NET WEIGHT", out var nwCols))
        //                {
        //                    if (nwCols.Count > 0) item.Net_weight = GetDoubleIdx(row, nwCols[0]);
        //                    if (nwCols.Count > 1) item.Un_1 = GetStringIdx(row, nwCols[1]);
        //                }

        //                // — Numéricos restantes —
        //                if (headerMap.TryGetValue("THICKNESS", out var thCols))
        //                    item.Thickness = GetDoubleIdx(row, thCols[0]);
        //                if (headerMap.TryGetValue("WIDTH", out var wCols))
        //                    item.Width = GetDoubleIdx(row, wCols[0]);
        //                if (headerMap.TryGetValue("ADVANCE", out var advCols))
        //                    item.Advance = GetDoubleIdx(row, advCols[0]);
        //                if (headerMap.TryGetValue("HEAD AND TAIL ALLOWED SCRAP", out var htasCols))
        //                    item.Head_and_Tail_allowed_scrap = GetDoubleIdx(row, htasCols[0]);
        //                if (headerMap.TryGetValue("PIECES PER CAR", out var ppcCols))
        //                    item.Pieces_per_car = GetDoubleIdx(row, ppcCols[0]);
        //                if (headerMap.TryGetValue("INITIAL WEIGHT", out var iwCols))
        //                    item.Initial_Weight = GetDoubleIdx(row, iwCols[0]);
        //                if (headerMap.TryGetValue("MIN WEIGHT", out var minCols))
        //                    item.Min_Weight = GetDoubleIdx(row, minCols[0]);
        //                if (headerMap.TryGetValue("MAXIMUM WEIGHT", out var maxCols))
        //                    item.Maximum_Weight = GetDoubleIdx(row, maxCols[0]);
        //                if (headerMap.TryGetValue("STROKE PIECES", out var spCols))
        //                    item.num_piezas_golpe = GetIntIdx(row, spCols[0]);
        //                if (headerMap.TryGetValue("ANGLE A", out var aACols))
        //                    item.angle_a = GetDoubleIdx(row, aACols[0]);
        //                if (headerMap.TryGetValue("ANGLE B", out var aBCols))
        //                    item.angle_b = GetDoubleIdx(row, aBCols[0]);
        //                if (headerMap.TryGetValue("REAL GROSS WEIGHT (KG)", out var rgwCols))
        //                    item.real_gross_weight = GetDoubleIdx(row, rgwCols[0]);
        //                if (headerMap.TryGetValue("REAL NET WEIGHT (KG)", out var rnwCols))
        //                    item.real_net_weight = GetDoubleIdx(row, rnwCols[0]);
        //                if (headerMap.TryGetValue("TOLERANCE MAX WT +VE", out var tmaxpCols))
        //                    item.maximum_weight_tol_positive = GetDoubleIdx(row, tmaxpCols[0]);
        //                if (headerMap.TryGetValue("TOLERANCE MAX WT -VE", out var tmaxnCols))
        //                    item.maximum_weight_tol_negative = GetDoubleIdx(row, tmaxnCols[0]);
        //                if (headerMap.TryGetValue("TOLERANCE MIN WT +VE", out var tminpCols))
        //                    item.minimum_weight_tol_positive = GetDoubleIdx(row, tminpCols[0]);
        //                if (headerMap.TryGetValue("TOLERANCE MIN WT -VE", out var tminnCols))
        //                    item.minimum_weight_tol_negative = GetDoubleIdx(row, tminnCols[0]);
        //                if (headerMap.TryGetValue("PIECES PACKAGE", out var ppacCols))
        //                    item.Pieces_Pac = GetDoubleIdx(row, ppacCols[0]);
        //                if (headerMap.TryGetValue("STACKS PACKGE", out var spacCols))
        //                    item.Stacks_Pac = GetDoubleIdx(row, spacCols[0]);

        //                // — Booleano —
        //                if (headerMap.TryGetValue("ALMACEN NTE", out var anCols))
        //                    item.Almacen_Norte = GetBoolIdx(row, anCols[0]);

        //                // — Fechas —
        //                if (headerMap.TryGetValue("TKMM SOP", out var sopCols))
        //                    item.Tkmm_SOP = GetDateIdx(row, sopCols[0]);
        //                if (headerMap.TryGetValue("TKMM EOP", out var eopCols))
        //                    item.Tkmm_EOP = GetDateIdx(row, eopCols[0]);

        //                // — Descripción por idioma —
        //                var lang = headerMap.TryGetValue("LANGUAGE", out var lgCols)
        //                           ? GetStringIdx(row, lgCols[0])
        //                           : null;
        //                var desc = headerMap["MATERIAL DESCRIPTION"]
        //                               .Select(i => GetStringIdx(row, i))
        //                               .FirstOrDefault();
        //                if (lang == "EN") item.Material_Description = desc;
        //                else if (lang == "ES") item.material_descripcion_es = desc;
        //            }


        //            // Solo la primera hoja relevante
        //            break;
        //        }
        //    }

        //    return lista;
        //}

        // Clase auxiliar para la configuración de columnas dinámicas.
        private class DynamicColumnInfo
        {
            public int DataColumnIndex { get; set; }
            public string Currency { get; set; }
            public bool Local { get; set; }
            public DateTime ReportDate { get; set; }
            public int FiscalYearId { get; set; }
        }

        public static List<budget_cantidad> BudgetLeeConcentrado(
            HttpPostedFileBase stream,
            int filaInicio,
            ref bool valido,
            ref string msjError,
            ref int noEncontrados,
            ref List<budget_rel_comentarios> listComentarios,
            ref List<int> idRels)
        {
            var lista = new List<budget_cantidad>();
            valido = true;

            // Declarar el HashSet para evitar duplicados.
            HashSet<int> idRelsSet = new HashSet<int>();

            using (var db = new Portal_2_0Entities())
            {
                // Diccionario de cuentas SAP (clave: sap_account)
                var cuentasDict = db.budget_cuenta_sap.ToDictionary(c => c.sap_account);

                // Diccionario de relaciones FY/CC (clave: (id_anio_fiscal, num_centro_costo))
                var fyccDict = db.budget_rel_fy_centro
                                 .ToList()
                                 .ToDictionary(x => (x.id_anio_fiscal, x.budget_centro_costo.num_centro_costo));

                // Diccionario de años fiscales, clave por anio_inicio.
                var fyDict = db.budget_anio_fiscal.ToDictionary(x => x.anio_inicio);

                using (var reader = ExcelReaderFactory.CreateReader(stream.InputStream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];

                    // Leer encabezados fijos (se asume que la fila de encabezados es filaInicio - 1)
                    var encabezadosOriginal = new List<string>();
                    var encabezadosMayus = new List<string>();
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        string title = table.Rows[filaInicio - 1][col].ToString();
                        encabezadosOriginal.Add(title);
                        encabezadosMayus.Add(!string.IsNullOrEmpty(title) ? title.ToUpperInvariant() : string.Empty);
                    }

                    // Validar que existan las columnas fijas requeridas.
                    if (!encabezadosMayus.Contains("SAP ACCOUNT") ||
                        !encabezadosMayus.Contains("NAME") ||
                        !encabezadosMayus.Contains("COST CENTER") ||
                        !encabezadosMayus.Contains("DEPARTMENT"))
                    {
                        msjError = "No cuenta con los encabezados correctos.";
                        valido = false;
                        return lista;
                    }

                    // Índices de columnas fijas.
                    int idxSAP = encabezadosMayus.IndexOf("SAP ACCOUNT");
                    int idxCC = encabezadosMayus.IndexOf("COST CENTER");

                    // Precalcular la configuración de las columnas dinámicas.
                    var dynamicColumns = new List<DynamicColumnInfo>();
                    CultureInfo enUS = new CultureInfo("en-US");

                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        string header = encabezadosMayus[j];
                        string anioMesStr = string.Empty;
                        DateTime fecha;
                        int fiscalYearId;

                        if (header == "MXN")
                        {
                            anioMesStr = table.Rows[2][j].ToString();
                        }
                        else if (header == "USD")
                        {
                            if (j - 1 < 0) continue;
                            anioMesStr = table.Rows[2][j - 1].ToString();
                        }
                        else if (header == "EUR")
                        {
                            if (j - 2 < 0) continue;
                            anioMesStr = table.Rows[2][j - 2].ToString();
                        }
                        else if (header == "LOCAL USD")
                        {
                            if (j - 3 < 0) continue;
                            anioMesStr = table.Rows[2][j - 3].ToString();
                        }
                        else
                        {
                            continue;
                        }

                        // Extraer la parte de la fecha (se espera formato "MMM-yy" al final de la cadena)
                        var parts = anioMesStr.Split(' ');
                        string datePart = parts.Last();

                        if (DateTime.TryParseExact(datePart, "MMM-yy", enUS, DateTimeStyles.None, out fecha))
                        {
                            int anioInicio = (fecha.Month >= 10) ? fecha.Year : fecha.Year - 1;
                            if (fyDict.TryGetValue(anioInicio, out var fy))
                            {
                                fiscalYearId = fy.id;
                                dynamicColumns.Add(new DynamicColumnInfo
                                {
                                    DataColumnIndex = j,
                                    Currency = (header == "LOCAL USD") ? "USD" : header,
                                    Local = (header == "LOCAL USD"),
                                    ReportDate = fecha,
                                    FiscalYearId = fiscalYearId
                                });
                            }
                        }
                    }

                    // Procesar cada fila de datos a partir de filaInicio.
                    for (int i = filaInicio; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            string sap_account = table.Rows[i][idxSAP].ToString();
                            string cc_ = table.Rows[i][idxCC].ToString();

                            // Buscar la cuenta SAP
                            if (!cuentasDict.TryGetValue(sap_account, out var cuenta))
                                continue;

                            // Recorrer columnas dinámicas
                            foreach (var col in dynamicColumns)
                            {
                                string cellStr = table.Rows[i][col.DataColumnIndex].ToString();
                                if (!decimal.TryParse(cellStr, out decimal cantidad))
                                    cantidad = 0;

                                // ==> 1) BUSCAR o CREAR la relación FY/CC AUNQUE LA CANTIDAD SEA 0
                                if (!fyccDict.TryGetValue((col.FiscalYearId, cc_), out var fy_cc))
                                {
                                    // Buscar centro
                                    var centroCosto = db.budget_centro_costo.FirstOrDefault(c => c.num_centro_costo == cc_);
                                    if (centroCosto == null)
                                    {
                                        noEncontrados++;
                                        continue; // Pasa a siguiente columna
                                    }

                                    // Crear la relación
                                    fy_cc = new budget_rel_fy_centro
                                    {
                                        id_anio_fiscal = col.FiscalYearId,
                                        id_centro_costo = centroCosto.id,
                                        estatus = true
                                    };
                                    db.budget_rel_fy_centro.Add(fy_cc);
                                    db.SaveChanges();

                                    fyccDict.Add((col.FiscalYearId, cc_), fy_cc);
                                }

                                // Agrega el ID al set, independientemente de la cantidad
                                idRelsSet.Add(fy_cc.id);

                                // ==> 2) SOLO si cantidad != 0, agregar la fila a 'lista'
                                cantidad = Decimal.Round(cantidad, 2);
                                if (cantidad != 0)
                                {
                                    lista.Add(new budget_cantidad
                                    {
                                        id_budget_rel_fy_centro = fy_cc.id,
                                        id_cuenta_sap = cuenta.id,
                                        mes = col.ReportDate.Month,
                                        currency_iso = col.Currency,
                                        cantidad = cantidad,
                                        moneda_local_usd = col.Local
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print("Error en la fila " + i + ": " + ex.Message);
                        }
                    }
                }
            }

            // Al finalizar, convertir el hashset a un list
            idRels = idRelsSet.ToList();

            return lista;
        }





        /// <summary>
        /// Lee un archivo y obtiene un List de budget_cantidad con los valores leidos
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="valido"></param>
        /// <param name="msjError"></param>
        /// <param name="noEncontrados"></param>
        /// <returns></returns>
        public static List<budget_cantidad> LeeActual(HttpPostedFileBase stream, int filaInicio, ref bool valido, ref string msjError, ref int noEncontrados, ref List<budget_rel_comentarios> listComentarios, ref List<int> idRels)
        {

            //obtiene todas las cuentas
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cantidad> lista = new List<budget_cantidad>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(stream.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;


                valido = true;

                ////verifica si tiene centro de costo
                //costCenter = table.Rows[4][2].ToString();
                //if (String.IsNullOrEmpty(costCenter))
                //{
                //    msjError = "No ingresó centro de costo en la celda 'C5'.";
                //    valido = false;
                //    return lista;
                //}

                ////verifica si el centro de costo es el mismo que el archivo excel
                //if (costCenter != excelViewModel.num_centro_costo)
                //{
                //    msjError = "El centro de costo del archivo no coincide.";
                //    valido = false;
                //    return lista;
                //}


                //se obtienen las cabeceras
                List<string> encabezados = new List<string>();

                for (int i = 0; i < result.Tables[0].Columns.Count; i++)
                {
                    string title = result.Tables[0].Rows[filaInicio - 1][i].ToString();

                    if (!string.IsNullOrEmpty(title))
                        encabezados.Add(title.ToUpper());
                }

                //verifica que la estrura del archivo sea válida
                if (!encabezados.Contains("SAP ACCOUNT") || !encabezados.Contains("NAME") || !encabezados.Contains("COST CENTER")
                    || !encabezados.Contains("DEPARTMENT")
                    )
                {
                    msjError = "No cuenta con los encabezados correctos.";
                    valido = false;
                    return lista;
                }

                //completa la información para los encabezados
                List<EncabezadoExcelBudget> ListObjectEncabezados = new List<EncabezadoExcelBudget>();
                //recorre todas los encabezados
                for (int j = 0; j < encabezados.Count; j++)
                {
                    //Si puede converir un encabezado en fecha
                    DateTime fecha = DateTime.Now;

                    if (DateTime.TryParse(encabezados[j], out fecha))
                    {
                        //obtiene id rel fy
                        budget_anio_fiscal fy = budget_anio_fiscal.Get_Anio_Fiscal(fecha);
                        //obtiene el rel fy_centro
                        //budget_rel_fy_centro rel_Fy_Centro = db.budget_rel_fy_centro.Where(x => x.id_anio_fiscal == fy.id && x.id_centro_costo == stream.id).FirstOrDefault();

                        ListObjectEncabezados.Add(new EncabezadoExcelBudget
                        {
                            texto = encabezados[j],
                            fecha = fecha,
                            anio_Fiscal = fy,
                            //  rel_fy = rel_Fy_Centro

                        });
                    }


                }

                //verifica que la estrura del archivo sea válida
                if (ListObjectEncabezados.Count != 36)
                {
                    msjError = "La plantilla no cuenta con todos los meses.";
                    valido = false;
                    return lista;
                }

                //obtiene los ids de rel_fy_centro
                //idRels = ListObjectEncabezados.Select(x => x.rel_fy.id).Distinct().ToList();

                //Se consultan las cuentas sap
                List<budget_cuenta_sap> listCuentas = db.budget_cuenta_sap.ToList();
                List<budget_rel_fy_centro> listFYsCCs = db.budget_rel_fy_centro.ToList();


                //la fila cero se omite (encabezado)


                for (int i = filaInicio; i < result.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        //variables
                        string sap_account = result.Tables[0].Rows[i][1].ToString();
                        string cc_ = result.Tables[0].Rows[i][3].ToString();

                        //obtiene la cuenta
                        budget_cuenta_sap cuenta = listCuentas.Where(x => x.sap_account == sap_account).FirstOrDefault();

                        //recorre todas los encabezados
                        for (int j = 0; j < encabezados.Count; j++)
                        {

                            if (ListObjectEncabezados.Any(x => x.texto == encabezados[j]))
                            {
                                decimal cantidad = 0;
                                Decimal.TryParse(result.Tables[0].Rows[i][j].ToString(), out cantidad);
                                cantidad = Decimal.Round(cantidad, 2);


                                //obtiene los datos del encabezado
                                EncabezadoExcelBudget encabezado = ListObjectEncabezados.Where(x => x.texto == encabezados[j]).FirstOrDefault();

                                //obteine el fy cc
                                budget_rel_fy_centro fy_cc = listFYsCCs.FirstOrDefault(x => x.id_anio_fiscal == encabezado.anio_Fiscal.id && x.budget_centro_costo.num_centro_costo == cc_);

                                if (fy_cc != null && !idRels.Contains(fy_cc.id))
                                    idRels.Add(fy_cc.id);

                                if (cuenta != null && fy_cc != null && cantidad != 0)
                                {
                                    lista.Add(new budget_cantidad()
                                    {
                                        id_budget_rel_fy_centro = fy_cc.id,
                                        id_cuenta_sap = cuenta.id,
                                        mes = encabezado.fecha.Month,
                                        currency_iso = "USD",
                                        cantidad = cantidad
                                    });
                                }
                                else if (cuenta == null)
                                {
                                    noEncontrados++;
                                }

                            }
                            //busca por un comentario
                            else if (encabezados[j].ToUpper().Contains("COMENTARIO") && cuenta != null)
                            {
                                string comentarios = UsoStrings.RecortaString(result.Tables[0].Rows[i][j].ToString(), 200);
                                //obtiene los datos del encabezado
                                EncabezadoExcelBudget encabezado = ListObjectEncabezados.Where(x => x.texto == encabezados[j - 3]).FirstOrDefault();
                                //obteine el fy cc
                                budget_rel_fy_centro fy_cc = listFYsCCs.FirstOrDefault(x => x.id_anio_fiscal == encabezado.anio_Fiscal.id && x.budget_centro_costo.num_centro_costo == cc_);

                                if (!String.IsNullOrEmpty(comentarios))

                                    listComentarios.Add(new budget_rel_comentarios
                                    {
                                        id_budget_rel_fy_centro = fy_cc.id, //id correspondiente al primer año de la plantilla
                                        id_cuenta_sap = cuenta.id,
                                        comentarios = comentarios
                                    });

                            }

                        }

                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Print("Error: " + e.Message);
                    }
                }




            }

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado del menu del comedor
        ///</summary>
        ///<return>
        ///Devuelve un List<RH_menu_comedor_platillos> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<RH_menu_comedor_platillos> LeeMenuComedor(HttpPostedFileBase streamPostedFile, ref List<string> errores, int plantaClave)
        {
            List<RH_menu_comedor_platillos> lista = new List<RH_menu_comedor_platillos>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                //valido = false;

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {

                    //se obtienen las cabeceras
                    List<EncabezadoTableMenu> encabezados = new List<EncabezadoTableMenu>();
                    int filaCabera = 0;
                    int columnaPlatillo = -1;

                    #region determina columnas

                    //recorre todas las filas y columnas hasta encontrar la columna platillo
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            if (table.Rows[i][j].ToString().ToUpper().Trim() == "PLATILLO")
                                columnaPlatillo = j;
                        }
                        //si encontró la columna rompe el for
                        if (columnaPlatillo != -1)
                            break;
                    }

                    //si no se pudo encontrar la columna platillo manda error
                    if (columnaPlatillo == -1)
                    {
                        errores.Add("No se puedo encontrar una columna llamada 'PLATILLO' en la hoja " + table.TableName.ToUpper());
                        return lista;
                    }

                    //recorre todas las filas hasta encontrar la cabecerar
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        encabezados = new List<EncabezadoTableMenu>();
                        //recorre todas las celdas de la fila
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            if (DateTime.TryParse(table.Rows[i][j].ToString(), out DateTime fechaC))
                                encabezados.Add(new EncabezadoTableMenu
                                {
                                    fecha = table.Rows[i][j].ToString(),
                                    columna = j
                                });
                        }

                        //comprueba cuantos correctos hubo
                        if (encabezados.Count >= 5) //requiere un minimo de 5 dias
                        {
                            filaCabera = i;
                            break;
                        }
                    }


                    //verifica que la estrura del archivo sea válida
                    if (encabezados.Count < 5)
                    {
                        errores.Add("Hubo un error al leer la hoja " + table.TableName.ToUpper() + ". No se pudo encontrar el header de la tabla.");
                        return lista;
                    }

                    #endregion

                    //la fila cero se omite (encabezado)
                    for (int i = filaCabera + 2; i < table.Rows.Count; i++)
                    {

                        try
                        {
                            //variables
                            string platillo_tipo = string.Empty;
                            string platillo_nombre = string.Empty;
                            DateTime fecha = new DateTime(2000, 01, 01);
                            int? kcal = null;
                            string kcalstring = string.Empty;

                            //obtiene el platillo (el mismo para toda la fila)
                            platillo_tipo = table.Rows[i][columnaPlatillo].ToString();

                            //recorre todas los encabezados
                            foreach (var encabezadoFecha in encabezados)
                            {
                                //obtiene la fecha
                                if (DateTime.TryParse(table.Rows[filaCabera][encabezadoFecha.columna].ToString(), out DateTime f))
                                    fecha = f;
                                //obtiene nombre del platillo
                                platillo_nombre = table.Rows[i][encabezadoFecha.columna].ToString();
                                //kcal
                                kcalstring = Regex.Match(table.Rows[i][encabezadoFecha.columna + 1].ToString(), @"\d+").Value;

                                if (Int32.TryParse(kcalstring, out int kc))
                                    kcal = kc;

                                // --- CAMBIO INICIA: Nueva lógica de validación y recorte ---

                                // Si el nombre está vacío, simplemente salta al siguiente.
                                if (string.IsNullOrEmpty(platillo_nombre))
                                {
                                    continue;
                                }

                                // Si el nombre es demasiado corto, registra el error y salta al siguiente.
                                if (platillo_nombre.Length < 2)
                                {
                                   // errores.Add($"Hoja: '{table.TableName}', Fila: {i + 1} - El platillo '{platillo_nombre}' es demasiado corto (mínimo 2 caracteres) y será omitido.");
                                    continue; // Salta al siguiente platillo en la fila
                                }

                                // Si el nombre es demasiado largo, lo RECORTA a 100 caracteres.
                                if (platillo_nombre.Length > 100)
                                {
                                    platillo_nombre = UsoStrings.RecortaString(platillo_nombre, 100);
                                }

                                // Ahora, el código para agregar a la lista se ejecuta con un nombre de platillo
                                // que garantizamos que tiene una longitud válida.
                                if (kcal.HasValue && !string.IsNullOrEmpty(platillo_tipo) && platillo_tipo.Length <= 30)
                                {
                                    lista.Add(new RH_menu_comedor_platillos()
                                    {
                                        orden_display = i - filaCabera - 1,
                                        tipo_platillo = UsoStrings.RecortaString(platillo_tipo.Trim(), 50),
                                        nombre_platillo = platillo_nombre, // Ya está limpio y con la longitud correcta
                                        fecha = fecha,
                                        kcal = kcal,
                                        id_planta = plantaClave
                                    });
                                }
                                // --- CAMBIO TERMINA ---
                            }

                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }
                    //final del recorrido de filas, para la hoja actual
                    //finalRecorrido:
                    //System.Diagnostics.Debug.WriteLine("Recorrido finalizaso para la hoja: " + table.TableName);
                }

            }

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado de epo
        ///</summary>
        ///<return>
        ///Devuelve un List<IT_epo> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<IT_epo> LeeEpo(HttpPostedFileBase streamPostedFile, ref string msj)
        {
            List<IT_epo> lista = new List<IT_epo>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();
                DateTime fecha = DateTime.Now;

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista = new List<IT_epo>();

                    //se obtienen las cabeceras
                    List<string> encabezados = new List<string>();
                    //List<string> encabezadosTest = new List<string>() { "SYSTEM NAME", "USER NAME", "OPERATING SYSTEM", "GROUP NAME", "ASSIGNMENT PATH" };

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string title = table.Rows[0][i].ToString();

                        if (!string.IsNullOrEmpty(title))
                            encabezados.Add(title.ToUpper());
                    }

                    ////verifica los encabezados principales del archivo enviado
                    //foreach (var s in encabezadosTest)
                    //{
                    //    if (!encabezados.Contains(s))
                    //    {
                    //        msj = "No se encontró la columna: " + s;
                    //        return lista;
                    //    }
                    //}

                    //la fila cero se omite (encabezado)
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            //variables
                            string system_name = String.Empty, username = String.Empty, operating_system = string.Empty, is_laptop_text = string.Empty;
                            string last_communication = String.Empty, group_name = string.Empty, mac_address = string.Empty, os_type = string.Empty;
                            string system_manufacturer = String.Empty, system_model = string.Empty, system_serial_number = string.Empty;
                            string assignment_path = String.Empty;

                            int? numbers_cpus = null, cpu_speed = null, total_disk_space = null, total_c_drive_space = null;
                            double? total_physical_memory = null;
                            bool? is_laptop = null;



                            //recorre todas los encabezados
                            for (int j = 0; j < encabezados.Count; j++)
                            {
                                //obtiene la cabezara de i
                                switch (encabezados[j])
                                {
                                    //obligatorios
                                    case "SYSTEM NAME":
                                    case "NOMBRE DE SISTEMA":
                                        system_name = table.Rows[i][j].ToString();
                                        break;
                                    case "USER NAME":
                                    case "NOMBRE DE USUARIO":
                                        username = table.Rows[i][j].ToString();
                                        break;
                                    case "OPERATING SYSTEM":
                                    case "SISTEMA OPERATIVO":
                                        operating_system = table.Rows[i][j].ToString();
                                        break;
                                    case "IS LAPTOP":
                                    case "ES UN PORTÁTIL":
                                        is_laptop_text = table.Rows[i][j].ToString();
                                        is_laptop = is_laptop_text.ToUpper() == "YES" ? true : false;
                                        break;
                                    case "GROUP NAME":
                                    case "NOMBRE DE GRUPO":
                                        group_name = table.Rows[i][j].ToString();
                                        break;
                                    case "OS type":
                                    case "TIPO DE SO":
                                        os_type = table.Rows[i][j].ToString();
                                        break;
                                    case "MAC ADDRESS":
                                    case "DIRECCIÓN MAC":
                                        mac_address = table.Rows[i][j].ToString();
                                        break;
                                    case "LAST COMMUNICATION":
                                    case "ÚLTIMA COMUNICACIÓN":
                                        last_communication = table.Rows[i][j].ToString();
                                        break;
                                    case "NUMBER OF CPUS":
                                    case "NÚMERO DE CPU":
                                        if (Int32.TryParse(table.Rows[i][j].ToString(), out int cpus))
                                            numbers_cpus = cpus;
                                        break;
                                    case "CPU SPEED (MHZ)":
                                    case "VELOCIDAD DE CPU (MHZ)":
                                        if (Int32.TryParse(table.Rows[i][j].ToString(), out int cpu_s))
                                            cpu_speed = cpu_s;
                                        break;
                                    case "SYSTEM MANUFACTURER":
                                    case "FABRICANTE DEL SISTEMA":
                                        system_manufacturer = table.Rows[i][j].ToString();
                                        break;
                                    case "SYSTEM MODEL":
                                    case "MODELO DEL SISTEMA":
                                        system_model = table.Rows[i][j].ToString();
                                        break;
                                    case "SYSTEM SERIAL NUMBER":
                                    case "NÚMERO DE SERIE DEL SISTEMA":
                                        system_serial_number = table.Rows[i][j].ToString();
                                        break;
                                    case "TOTAL DISK SPACE":
                                    case "ESPACIO TOTAL EN DISCO":
                                        if (Int32.TryParse(table.Rows[i][j].ToString().Replace("MB", string.Empty), out int total_disk_int))
                                            total_disk_space = total_disk_int;
                                        break;
                                    case "TOTAL C DRIVE SPACE":
                                    case "ESPACIO TOTAL EN DISCO C":
                                        if (Int32.TryParse(table.Rows[i][j].ToString().Replace("MB", string.Empty), out int total_c_drive_int))
                                            total_c_drive_space = total_c_drive_int;
                                        break;
                                    case "TOTAL PHYSICAL MEMORY":
                                    case "MEMORIA FÍSICA TOTAL":
                                        if (double.TryParse(table.Rows[i][j].ToString().Replace("MB", string.Empty), out double total_physical_double))
                                            total_physical_memory = total_physical_double;
                                        break;
                                    case "ASSIGNMENT PATH":
                                    case "RUTA DE ASIGNACIÓN":
                                        assignment_path = table.Rows[i][j].ToString();
                                        break;
                                }
                            }
                            //agrega a la lista con los datos leidos
                            lista.Add(new IT_epo()
                            {
                                system_name = system_name,
                                username = username,
                                operating_system = operating_system,
                                is_laptop = is_laptop,
                                group_name = group_name,
                                os_type = os_type,
                                mac_address = mac_address,
                                numbers_of_cpus = numbers_cpus,
                                cpu_speed = cpu_speed,
                                system_manufacturer = system_manufacturer,
                                system_serial_number = system_serial_number,
                                total_disk_space_mb = total_disk_space,
                                total_c_drive_space_mb = total_c_drive_space,
                                assigment_path = assignment_path,
                                total_physical_memory_mb = total_physical_memory,
                                system_model = system_model,
                                last_communication = last_communication,
                                fecha = fecha
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }

                    //envia la lista tras la primera iteracion
                    return lista;
                }
            }

            return lista;
        }  ///<summary>
           ///Lee un archivo de excel y carga el listado de epo
           ///</summary>
           ///<return>
           ///Devuelve un List<IT_epo> con los datos leidos
           ///</return>
           ///<param name="streamPostedFile">
           ///Stream del archivo recibido en el formulario
           ///</param>
        public static List<IT_wsus> LeeWSUS(HttpPostedFileBase streamPostedFile, ref string msj)
        {
            List<IT_wsus> lista = new List<IT_wsus>();
            DateTime fecha = DateTime.Now;

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista = new List<IT_wsus>();

                    //se obtienen las cabeceras
                    List<string> encabezados = new List<string>();
                    List<string> encabezadosTest = new List<string>() { "NAME" };

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string title = table.Rows[0][i].ToString();

                        if (!string.IsNullOrEmpty(title))
                            encabezados.Add(title.ToUpper());
                    }

                    //verifica los encabezados principales del archivo enviado
                    foreach (var s in encabezadosTest)
                    {
                        if (!encabezados.Contains(s))
                        {
                            msj = "No se encontró la columna: " + s;
                            return lista;
                        }
                    }

                    //la fila cero se omite (encabezado)
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            //variables
                            string name = String.Empty, operating_system = string.Empty, ip = string.Empty;

                            //recorre todas los encabezados
                            for (int j = 0; j < encabezados.Count; j++)
                            {
                                //obtiene la cabezara de i
                                switch (encabezados[j])
                                {
                                    //obligatorios
                                    case "NAME":
                                    case "NOMBRE":
                                        name = table.Rows[i][j].ToString();
                                        break;
                                    case "IP":
                                    case "IP ADDRESS":
                                        ip = table.Rows[i][j].ToString();
                                        break;
                                    case "OPERATING SYSTEM":
                                    case "SO":
                                        operating_system = table.Rows[i][j].ToString();
                                        break;
                                }
                            }
                            //agrega a la lista con los datos leidos
                            lista.Add(new IT_wsus()
                            {
                                name = name,
                                operating_system = operating_system,
                                ip = ip,
                                fecha = fecha
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }

                    //envia la lista tras la primera iteracion
                    return lista;
                }
            }

            return lista;
        }



        ///<summary>
        ///Lee un archivo de excel y carga el listado de epo
        ///</summary>
        ///<return>
        ///Devuelve un List<IT_epo> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<IT_Export4Import> LeeExport4Import(HttpPostedFileBase streamPostedFile, ref string msj)
        {
            List<IT_Export4Import> lista = new List<IT_Export4Import>();
            DateTime fecha = DateTime.Now;

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista = new List<IT_Export4Import>();

                    //se obtienen las cabeceras
                    List<string> encabezados = new List<string>();
                    List<string> encabezadosTest = new List<string>() { "USERNAME", "LASTNAME", "FIRSTNAME", "TKBIRTH", "TKSEX", "TKEMPNO" };

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string title = table.Rows[0][i].ToString();

                        if (!string.IsNullOrEmpty(title))
                            encabezados.Add(title.ToUpper());
                    }

                    //verifica los encabezados principales del archivo enviado
                    foreach (var s in encabezadosTest)
                    {
                        if (!encabezados.Contains(s))
                        {
                            msj = "No se encontró la columna: " + s;
                            return lista;
                        }
                    }

                    //la fila cero se omite (encabezado)
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            IT_Export4Import item = new IT_Export4Import();

                            //recorre todas los encabezados
                            for (int j = 0; j < encabezados.Count; j++)
                            {
                                //obtiene la cabezara de i
                                switch (encabezados[j])
                                {
                                    //obligatorios
                                    case "USERNAME":
                                        item.C8ID = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "TKSIR":
                                        item.tksir = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKTITLE":
                                        item.tktitle = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKNAMEPREFIX":
                                        item.tknameprefix = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "LASTNAME":
                                        item.lastName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "FIRSTNAME":
                                        item.firstName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKBIRTH":
                                        item.tkbirth = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 15);
                                        break;
                                    case "TKSEX":
                                        item.tksex = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;
                                    case "TKSTREET":
                                        item.tkstreet = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKPOSTALCODE":
                                        item.tkpostalcode = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKPOSTALADDRESS":
                                        item.tkpostaladdress = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKADDADDON":
                                        item.tkaddaddon = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKFEDST":
                                        item.tkfedst = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKCOUNTRY":
                                        item.tkcountry = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "TKCOUNTRYKEY":
                                        item.tkcountrykey = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKNATIONALITY":
                                        item.tknationality = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 3);
                                        break;
                                    case "TKPREFLANG":
                                        item.tkpreflang = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKEMPNO":
                                        item.tkempno = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "TKFKZ6":
                                        item.tkfkz6 = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 6);
                                        break;
                                    case "TKFKZEXT":
                                        item.tkfkzext = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 6);
                                        break;
                                    case "TKUNIQUEID":
                                        item.tkuniqueid = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "TKPSTATUS":
                                        item.tkpstatus = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKCOSTCENTER":
                                        item.tkcostcenter = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 4);
                                        break;
                                    case "TKDEPARTMENT":
                                        item.tkdepartment = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "TKFUNCTION":
                                        item.tkfunction = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "TKORGSTREET":
                                        item.tkorgstreet = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "TKORGPOSTALCODE":
                                        item.tkorgpostalcode = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKORGPOSTALADDRESS":
                                        item.tkorgpostaladdress = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "TKORGADDONADDR":
                                        item.tkorgaddonaddr = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "TKORGFEDST":
                                        item.tkorgfedst = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKORGCOUNTRY":
                                        item.tkorgcountry = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 40);
                                        break;
                                    case "TKORGCOUNTRYKEY":
                                        item.tkorgcountrykey = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKAPSITE":
                                        item.tkapsite = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKORGKEY":
                                        item.tkorgkey = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "TKBUILDING":
                                        item.tkbuilding = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "EMAIL":
                                        item.email = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 120);
                                        break;
                                    case "TKAREACODE":
                                        item.tkareacode = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKPHONEEXT":
                                        item.tkphoneext = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKORGFAX":
                                        item.tkorgfax = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKMOBILE":
                                        item.tkmobile = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "TKGODFATHER":
                                        item.tkgodfather = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 8);
                                        break;
                                    case "TKPREFDELMETHOD":
                                        item.tkprefdelmethod = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;
                                    case "TKEDATEORG":
                                        item.tkedateorg = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 15);
                                        break;
                                    case "TKEDATETRUST":
                                        item.tkedatetrust = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 15);
                                        break;
                                    case "TKLDATEORG":
                                        item.tkldateorg = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 15);
                                        break;
                                    case "TKLREASON":
                                        item.tklreason = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "SHARES":
                                        item.shares = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;
                                    case "SUPERVISORYBOARDELECTION":
                                        item.supervisoryboardelection = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;
                                    case "TKBKZ":
                                        item.tkbkz = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TKINSIDE":
                                        item.tkinside = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;

                                }
                            }
                            //agrega a la lista con los datos leidos
                            item.updateTime = fecha;
                            lista.Add(item);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }

                    //envia la lista tras la primera iteracion
                    return lista;
                }
            }

            return lista;
        }
        ///<summary>
        ///Lee un archivo de excel y carga el listado de epo
        ///</summary>
        ///<return>
        ///Devuelve un List<IT_epo> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<IT_UsuariosActivos> LeeUsuariosActivos(HttpPostedFileBase streamPostedFile, ref string msj)
        {
            List<IT_UsuariosActivos> lista = new List<IT_UsuariosActivos>();
            DateTime fecha = DateTime.Now;

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista = new List<IT_UsuariosActivos>();

                    //se obtienen las cabeceras
                    List<string> encabezados = new List<string>();
                    List<string> encabezadosTest = new List<string>() { "APELLIDOPATERNO", "APELLIDOMATERNO", "APELLIDOMATERNO", "DEPARTAMENTO", "8ID" };

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string title = table.Rows[0][i].ToString();

                        if (!string.IsNullOrEmpty(title))
                            encabezados.Add(title.ToUpper().Replace(" ", ""));
                    }

                    //verifica los encabezados principales del archivo enviado
                    foreach (var s in encabezadosTest)
                    {
                        if (!encabezados.Contains(s))
                        {
                            msj = "No se encontró la columna: " + s;
                            return lista;
                        }
                    }

                    //la fila cero se omite (encabezado)
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            IT_UsuariosActivos item = new IT_UsuariosActivos();

                            //recorre todas los encabezados
                            for (int j = 0; j < encabezados.Count; j++)
                            {
                                //obtiene la cabezara de i
                                switch (encabezados[j])
                                {
                                    //obligatorios
                                    case "NÚMERO":
                                        item.Numero = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 6);
                                        break;
                                    case "APELLIDOPATERNO":
                                        item.ApellidoPaterno = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "APELLIDOMATERNO":
                                        item.ApellidoMaterno = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "NOMBRE(S)":
                                        item.Nombre = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "ANTIGÜEDAD":
                                        item.Antiguedad = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 15);
                                        break;
                                    case "PUESTO":
                                        item.Puesto = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "DEPARTAMENTO":
                                        if (j == 6)
                                            item.Departamento = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        else
                                            item.DepartamentoNum = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 6);
                                        break;
                                    case "PLANTA":
                                        item.Planta = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "ÁREA":
                                        item.Area = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "GÉNERO":
                                        item.Genero = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 1);
                                        break;
                                    case "8ID":
                                        item.C8ID = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 8);
                                        break;



                                }
                            }
                            //agrega a la lista con los datos leidos
                            item.updateTime = fecha;
                            lista.Add(item);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }

                    //envia la lista tras la primera iteracion
                    return lista;
                }
            }

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado de epo
        ///</summary>
        ///<return>
        ///Devuelve un List<IT_epo> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<IT_ExportUsers> LeeExportUsers(HttpPostedFileBase streamPostedFile, ref string msj)
        {
            List<IT_ExportUsers> lista = new List<IT_ExportUsers>();
            DateTime fecha = DateTime.Now;

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista = new List<IT_ExportUsers>();

                    //se obtienen las cabeceras
                    List<string> encabezados = new List<string>();
                    List<string> encabezadosTest = new List<string>() { "USERPRINCIPALNAME", "DISPLAYNAME", "SURNAME", "MAIL", "GIVENNAME" };

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string title = table.Rows[0][i].ToString();

                        if (!string.IsNullOrEmpty(title))
                            encabezados.Add(title.ToUpper());
                    }

                    //verifica los encabezados principales del archivo enviado
                    foreach (var s in encabezadosTest)
                    {
                        if (!encabezados.Contains(s))
                        {
                            msj = "No se encontró la columna: " + s;
                            return lista;
                        }
                    }

                    //la fila cero se omite (encabezado)
                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            IT_ExportUsers item = new IT_ExportUsers();

                            //recorre todas los encabezados
                            for (int j = 0; j < encabezados.Count; j++)
                            {
                                //obtiene la cabezara de i
                                switch (encabezados[j])
                                {
                                    //obligatorios
                                    case "USERPRINCIPALNAME":
                                        string[] separadores = table.Rows[i][j].ToString().Split('@');
                                        if (separadores.Length > 0)
                                            item.C8ID = UsoStrings.RecortaString(separadores[0], 14);
                                        item.userPrincipalName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "DISPLAYNAME":
                                        item.displayName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "SURNAME":
                                        item.surname = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "MAIL":
                                        item.mail = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 100);
                                        break;
                                    case "GIVENNAME":
                                        item.givenName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "ID":
                                        item.C_id = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 50);
                                        break;
                                    case "USERTYPE":
                                        item.userType = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 12);
                                        break;
                                    case "JOBTITLE":
                                        item.jobTitle = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "DEPARTMENT":
                                        item.department = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 25);
                                        break;
                                    case "ACCOUNTENABLED":
                                        item.accountEnabled = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "USAGELOCATION":
                                        item.usageLocation = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "STREETADDRESS":
                                        item.streetAddress = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "STATE":
                                        item.state = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "COUNTRY":
                                        item.country = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "OFFICELOCATION":
                                        item.officeLocation = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 20);
                                        break;
                                    case "CITY":
                                        item.city = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 30);
                                        break;
                                    case "POSTALCODE":
                                        item.postalCode = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "TELEPHONENUMBER":
                                        item.telephoneNumber = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 25);
                                        break;
                                    case "MOBILEPHONE":
                                        item.mobilePhone = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 25);
                                        break;
                                    case "ALTERNATEEMAILADDRESS":
                                        item.alternateEmailAddress = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 120);
                                        break;
                                    case "AGEGROUP":
                                        item.ageGroup = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "CONSENTPROVIDEDFORMINOR":
                                        item.consentProvidedForMinor = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "LEGALAGEGROUPCLASSIFICATION":
                                        item.legalAgeGroupClassification = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 10);
                                        break;
                                    case "COMPANYNAME":
                                        item.companyName = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "CREATIONTYPE":
                                        item.creationType = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "DIRECTORYSYNCED":
                                        item.directorySynced = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "INVITATIONSTATE":
                                        item.invitationState = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 5);
                                        break;
                                    case "IDENTITYISSUER":
                                        item.identityIssuer = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 60);
                                        break;
                                    case "CREATEDDATETIME":
                                        item.createdDateTime = UsoStrings.RecortaString(table.Rows[i][j].ToString(), 30);
                                        break;


                                }
                            }
                            //agrega a la lista con los datos leidos
                            item.updateTime = fecha;
                            lista.Add(item);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }

                    //envia la lista tras la primera iteracion
                    return lista;
                }
            }

            return lista;
        }


        ///<summary>
        ///Lee un archivo de excel y carga formato de remisiones provisionales
        ///</summary>
        ///<return>
        ///Devuelve un List<RM_elementos> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<RM_elemento> LeeFormatoRemisionesProvisionales(HttpPostedFileBase streamPostedFile, ref string status, ref string msj)
        {
            List<RM_elemento> lista = new List<RM_elemento>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //para la primera hoja
                DataTable table = result.Tables[0];


                //se obtienen las cabeceras
                List<string> encabezados = new List<string>();

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string title = table.Rows[0][i].ToString();

                    if (!string.IsNullOrEmpty(title))
                        encabezados.Add(title.ToUpper());
                }

                List<string> titulos = new List<string> { "No. Parte Clte.", "Material", "Lote", "No. Bobina", "Piezas", "Peso Neto" };
                foreach (string t in titulos)
                {
                    if (!encabezados.Contains(t.ToUpper()))
                    {
                        status = "ERROR";
                        msj = "El archivo no contiene una columna llamada: " + t;
                        return lista;
                    }
                }

                //la fila cero se omite (encabezado)
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    try
                    {
                        //variables
                        string NoParteCliente = String.Empty, material = String.Empty, lote = string.Empty, numRollo = string.Empty;

                        double piezas = 0.0, peso = 0.0;

                        //recorre todas los encabezados
                        for (int j = 0; j < encabezados.Count; j++)
                        {
                            //obtiene la cabezara de i
                            switch (encabezados[j])
                            {
                                //obligatorios
                                case "NO. PARTE CLTE.":
                                    NoParteCliente = table.Rows[i][j].ToString();
                                    break;
                                case "MATERIAL":
                                    material = table.Rows[i][j].ToString();
                                    break;
                                case "LOTE":
                                    lote = table.Rows[i][j].ToString();
                                    break;
                                case "NO. BOBINA":
                                    numRollo = table.Rows[i][j].ToString();
                                    break;
                                case "PIEZAS":
                                    if (Double.TryParse(table.Rows[i][j].ToString(), out double q))
                                        piezas = q;
                                    break;
                                case "PESO NETO":
                                    if (Double.TryParse(table.Rows[i][j].ToString(), out double p))
                                        peso = p;
                                    break;

                            }
                        }


                        //agrega a la lista con los datos leidos
                        lista.Add(new RM_elemento()
                        {
                            numeroParteCliente = NoParteCliente,
                            numeroMaterial = material,
                            numeroLote = lote,
                            numeroRollo = numRollo,
                            cantidad = piezas,
                            peso = peso,
                        });
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Print("Error: " + e.Message);
                    }
                }


            }

            status = "SUCCESS";
            msj = "Se leyeron " + lista.Count() + " elementos.";

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado de bom
        ///</summary>
        ///<return>
        ///Devuelve un List<CI_conteo_inventario> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<CI_conteo_inventario> LeeInventarioSAP(HttpPostedFileBase streamPostedFile, ref bool valido, ref string mensaje)
        {
            List<CI_conteo_inventario> lista = new List<CI_conteo_inventario>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    //busca la primera hoja del documento
                    if (result.Tables.IndexOf(table) == 0)
                    {
                        valido = true;

                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[0][i].ToString();

                            if (!string.IsNullOrEmpty(title))
                                encabezados.Add(title.ToUpper());
                        }

                        //verifica que la estrura del archivo sea válida
                        if (!encabezados.Contains("PLANT") || !encabezados.Contains("STORAGE LOCATION") || !encabezados.Contains("STORAGE BIN")
                            || !encabezados.Contains("MATERIAL") || !encabezados.Contains("BATCH"))
                        {
                            mensaje = "No se pudieron leer el nombre de las columnas o tienen otro nombre.";
                            valido = false;
                            return lista;
                        }

                        //la fila cero se omite (encabezado)
                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                //variables
                                string plant = String.Empty;
                                string storage_location = String.Empty;
                                string storage_bin = String.Empty;
                                string material = String.Empty;
                                string batch = String.Empty;
                                string shipto_number = String.Empty;
                                string material_description = String.Empty;
                                string IHS_number = String.Empty;
                                int? pieces = null;
                                int? unrestricted = null;
                                int? blocked = null;
                                int? in_quality = null;
                                double? value_stock = null;
                                string base_unit_measure = string.Empty;
                                //double? gauge = null;
                                //double? gauge_min = null;
                                //double? gauge_max = null;
                                //double? altura = null;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "PLANT":
                                            plant = table.Rows[i][j].ToString();
                                            break;
                                        case "STORAGE LOCATION":
                                            storage_location = table.Rows[i][j].ToString();
                                            break;
                                        case "STORAGE BIN":
                                            storage_bin = table.Rows[i][j].ToString();
                                            break;
                                        case "MATERIAL":
                                            material = table.Rows[i][j].ToString();
                                            break;
                                        case "BATCH":
                                            batch = table.Rows[i][j].ToString();
                                            break;
                                        case "SHIPTO NUMBER":
                                            shipto_number = table.Rows[i][j].ToString();
                                            break;
                                        case "MATERIAL DESCRIPTION":
                                            material_description = table.Rows[i][j].ToString();
                                            break;
                                        case "IHS NUMBER 1":
                                            IHS_number = table.Rows[i][j].ToString();
                                            break;
                                        case "PIECES":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double pieces_result))
                                                pieces = (int)pieces_result;
                                            break;
                                        case "UNRESTRICTED":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double unrestricted_result))
                                                unrestricted = (int)unrestricted_result;
                                            break;
                                        case "BLOCKED":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double blocked_result))
                                                blocked = (int)blocked_result;
                                            break;
                                        case "IN QUALITY INSP.":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double in_quality_result))
                                                in_quality = (int)in_quality_result;
                                            break;
                                        case "VALUE OF STOCK":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double value_stock_result))
                                                value_stock = value_stock_result;
                                            break;
                                        case "BASE UNIT OF MEASURE":
                                            base_unit_measure = table.Rows[i][j].ToString();
                                            break;
                                    }
                                }


                                //agrega a la lista con los datos leidos
                                if (!string.IsNullOrEmpty(plant))
                                    lista.Add(new CI_conteo_inventario()
                                    {
                                        plant = plant,
                                        storage_location = storage_location,
                                        storage_bin = storage_bin,
                                        material = material,
                                        batch = batch,
                                        ship_to_number = shipto_number,
                                        material_description = material_description,
                                        ihs_number = IHS_number,
                                        pieces = pieces,
                                        unrestricted = unrestricted,
                                        blocked = blocked,
                                        in_quality = in_quality,
                                        value_stock = value_stock,
                                        base_unit_measure = base_unit_measure,

                                    });
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.Print("Error: " + e.Message);
                            }
                        }

                    }
                }

            }

            return lista;
        }
        ///<summary>
        ///Lee un archivo de excel y carga el listado de bom
        ///</summary>
        ///<return>
        ///Devuelve un List<CI_Tolerancias> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<CI_Tolerancias> LeeToleranciasSAP(HttpPostedFileBase streamPostedFile, ref bool valido)
        {
            List<CI_Tolerancias> lista = new List<CI_Tolerancias>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    //busca la primera hoja del documento
                    if (result.Tables.IndexOf(table) == 0)
                    {
                        valido = true;

                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[0][i].ToString();

                            if (!string.IsNullOrEmpty(title))
                                encabezados.Add(title.ToUpper());
                        }

                        //verifica que la estrura del archivo sea válida
                        if (!encabezados.Contains("MATL.") || !encabezados.Contains("GAUGE") || !encabezados.Contains("GAUGE MIN")
                            || !encabezados.Contains("GAUGE MAX"))
                        {
                            valido = false;
                            return lista;
                        }

                        //la fila cero se omite (encabezado)
                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                //variables                              
                                string material = String.Empty;

                                double? gauge = null;
                                double? gauge_min = null;
                                double? gauge_max = null;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "MATL.":
                                            material = table.Rows[i][j].ToString();
                                            break;
                                        case "GAUGE":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double gauge_result))
                                                gauge = gauge_result;
                                            break;
                                        case "GAUGE MIN":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double gauge_min_result))
                                                gauge_min = gauge_min_result;
                                            break;
                                        case "GAUGE MAX":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double gauge_max_result))
                                                gauge_max = gauge_max_result;
                                            break;

                                    }
                                }


                                //agrega a la lista con los datos leidos
                                if (!string.IsNullOrEmpty(material))
                                    lista.Add(new CI_Tolerancias()
                                    {
                                        material = material,
                                        gauge = gauge,
                                        gauge_min = gauge_min,
                                        gauge_max = gauge_max,
                                    });
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.Print("Error: " + e.Message);
                            }
                        }

                    }
                }

            }

            return lista;
        }


        ///<summary>
        ///Lee un archivo de excel y carga el listado de IHS
        ///</summary>
        ///<return>
        ///Devuelve un List<BG_IHS_item> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<BG_Forecast_item> LeeReporteBudget(HttpPostedFileBase streamPostedFile, string select_hoja, ref string estatus, ref string msj)
        {
            List<BG_Forecast_item> lista = new List<BG_Forecast_item>();
            CultureInfo provider = new CultureInfo("en-US");

            // Crear el lector del Excel a partir del stream del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                // Convertir el Excel en un DataSet
                var result = reader.AsDataSet();

                // Verificar que la hoja requerida exista
                if (!result.Tables.Contains(select_hoja))
                {
                    estatus = "ERROR";
                    msj = "El documento no tiene una hoja llamada: " + select_hoja;
                    return lista;
                }

                // Obtener directamente la hoja por su nombre
                DataTable table = result.Tables[select_hoja];

                // Suponemos que la primera fila (fila 0) es el encabezado
                int filaEncabezado = 0;
                int totalColumnas = table.Columns.Count;

                // Crear un arreglo de encabezados, de modo que se preserve la posición de cada columna
                string[] headers = new string[totalColumnas];
                for (int col = 0; col < totalColumnas; col++)
                {
                    headers[col] = table.Rows[filaEncabezado][col].ToString().Trim();
                }

                // Validar que se encuentren todos los encabezados requeridos
                foreach (string requiredHeader in UtilExcel.ListadoCabeceraReporteBudget)
                {
                    if (!headers.Contains(requiredHeader))
                    {
                        estatus = "ERROR";
                        msj = "La hoja no cuenta con una columna llamada: " + requiredHeader;
                        return lista;
                    }
                }

                int posCounter = 1;
                // Recorrer cada fila de datos (a partir de la fila siguiente a los encabezados)
                for (int row = filaEncabezado + 1; row < table.Rows.Count; row++)
                {
                    // Verificar si la fila está vacía (se puede ajustar la condición según las columnas clave)
                    if (string.IsNullOrEmpty(table.Rows[row][1].ToString()) &&
                        string.IsNullOrEmpty(table.Rows[row][2].ToString()))
                        break;

                    BG_Forecast_item bg = new BG_Forecast_item { pos = posCounter++ };

                    // Recorrer cada columna de la fila
                    for (int col = 0; col < totalColumnas; col++)
                    {
                        // Se obtiene el nombre del encabezado para la columna actual
                        string headerName = headers[col];
                        // Se obtiene y limpia el valor de la celda
                        string cellValue = table.Rows[row][col].ToString()
                            .Replace("\r", String.Empty)
                            .Replace("\n", String.Empty);

                        // Mapear el valor de la celda a la propiedad del objeto según el nombre del encabezado
                        switch (headerName)
                        {
                            case "Coils & Slitter":
                                bg.cat_1 = cellValue;
                                break;
                            case "Business & Plant":
                                bg.business_and_plant = cellValue;
                                break;
                            case "Business":
                                bg.cat_2 = cellValue;
                                break;
                            case "Additional Processes":
                                bg.cat_3 = cellValue;
                                break;
                            case "Production Processes":
                                bg.cat_4 = cellValue;
                                break;
                            case "A / D":
                                bg.calculo_activo = cellValue.ToUpper() == "A";
                                break;
                            case "SAP - INVOICE CODE":
                                bg.sap_invoice_code = cellValue;
                                break;
                            case "PREVIOUS SAP INVOICE CODE":
                                bg.previous_sap_invoice_code = cellValue;
                                break;
                            case "INVOICED TO:":
                                bg.invoiced_to = cellValue;
                                break;
                            case "NUMBER SAP CLIENT":
                                bg.number_sap_client = cellValue;
                                break;
                            case "SHIPPED TO:":
                                bg.shipped_to = cellValue;
                                break;
                            case "OWN /CM":
                                bg.own_cm = cellValue;
                                break;
                            case "ROUTE":
                                bg.route = cellValue;
                                break;
                            case "PLANT":
                                bg.plant = cellValue;
                                break;
                            case "EXTERNAL PROCESSOR":
                                bg.external_processor = cellValue;
                                break;
                            case "MILL":
                                bg.mill = cellValue;
                                break;
                            case "SAP MASTER COIL":
                                bg.sap_master_coil = cellValue;
                                break;
                            case "PART DESCRIPTION":
                                bg.part_description = cellValue;
                                break;
                            case "PART NUMBER":
                                bg.part_number = cellValue;
                                break;
                            case "PRODUCTION LINE":
                                bg.production_line = cellValue;
                                break;
                            case "Mnemonic-Vehicle/Plant":
                                bg.mnemonic_vehicle_plant = cellValue;
                                break;
                            case "VEHICLE  -  IHS 1":
                                bg.vehicle = cellValue;
                                break;
                            case "Propulsion System Type":
                                bg.propulsion_system_type = cellValue;
                                break;
                            case "PARTS / AUTO [ - ]":
                                bg.parts_auto = double.TryParse(cellValue, out double parts_auto)
                                    ? parts_auto : (double?)null;
                                break;
                            case "STROKES / AUTO [ - ]":
                                bg.strokes_auto = double.TryParse(cellValue, out double strokes_auto)
                                    ? strokes_auto : (double?)null;
                                break;
                            case "MATERIAL TYPE":
                                bg.material_type = cellValue;
                                break;
                            case "SHAPE":
                                bg.shape = cellValue;
                                break;
                            case "INICIAL WEIGHT / PART [kg]":
                                bg.initial_weight_part = double.TryParse(cellValue, out double initial_weight_part)
                                    ? initial_weight_part : (double?)null;
                                break;
                            case "NET WEIGHT / PART [kg]":
                                bg.net_weight_part = double.TryParse(cellValue, out double net_weight_part)
                                    ? net_weight_part : (double?)null;
                                break;
                            case "Scrap Consolidation [Yes / No]":
                                bg.scrap_consolidation = cellValue.ToUpper() == "YES";
                                break;
                            case "VENTAS / PART [USD]":
                                bg.ventas_part = double.TryParse(cellValue, out double ventas_part)
                                    ? ventas_part : (double?)null;
                                break;
                            case "MATERIAL COST / PART [USD]":
                                bg.material_cost_part = double.TryParse(cellValue, out double material_cost_part)
                                    ? material_cost_part : (double?)null;
                                break;
                            case "COST OF OUTSIDE PROCESSOR /PART [USD]":
                                bg.cost_of_outside_processor = double.TryParse(cellValue, out double cost_of_outside_processor)
                                    ? cost_of_outside_processor : (double?)null;
                                break;
                            case "Additional Material Cost/PART [USD]":
                                bg.additional_material_cost_part = double.TryParse(cellValue, out double additional_material_cost_part)
                                    ? additional_material_cost_part : (double?)null;
                                break;
                            case "Outgoing freight / PART [USD]":
                                bg.outgoing_freight_part = double.TryParse(cellValue, out double outgoing_freight_part)
                                    ? outgoing_freight_part : (double?)null;
                                break;
                            case "Inicio Demanda(YYYY-MM)":
                                if (DateTime.TryParseExact(cellValue, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaInicio))
                                {
                                    bg.inicio_demanda = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
                                }
                                break;
                            case "Fin Demanda(YYYY-MM)":
                                if (DateTime.TryParseExact(cellValue, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaFin))
                                {
                                    bg.fin_demanda = new DateTime(fechaFin.Year, fechaFin.Month, 1);
                                }
                                break;
                            case "Trans Silao  - SLP (YYYY-MM)":
                                bg.trans_silao_slp = cellValue;
                                break;
                            case "Outgoing freight":
                                bg.outgoing_freight = cellValue;
                                break;
                            case "Freights Income":
                                bg.freights_income = cellValue;
                                break;
                            case "Freights Income USD / PART":
                                bg.freights_income_usd_part = double.TryParse(cellValue, out double freights_income_usd)
                                    ? freights_income_usd : (double?)null;
                                break;
                            case "Handling USD / PART":
                                bg.maniobras_usd_part = double.TryParse(cellValue, out double maniobras_usd_part)
                                    ? maniobras_usd_part : (double?)null;
                                break;
                            case "Customs Expenses USD / PART":
                                bg.customs_expenses = double.TryParse(cellValue, out double customs_expenses)
                                    ? customs_expenses : (double?)null;
                                break;
                            case "Wooden Pallet USD/Part":
                                bg.wooden_pallet_usd_part = double.TryParse(cellValue, out double wooden_pallet_usd_part)
                                    ? wooden_pallet_usd_part : (double?)null;
                                break;
                            case "Packaging Price USD/Part":
                                bg.packaging_price_usd_part = double.TryParse(cellValue, out double packaging_price_usd_part)
                                    ? packaging_price_usd_part : (double?)null;
                                break;
                            case "Neopreno USD/Paq":
                                bg.neopreno_usd_part = double.TryParse(cellValue, out double neopreno_usd_part)
                                    ? neopreno_usd_part : (double?)null;
                                break;
                            case "MuestraAdvertencia":
                                bg.mostrar_advertencia = !string.IsNullOrEmpty(cellValue);
                                break;
                            default:
                                // Si el encabezado está vacío o no se reconoce, se ignora.
                                break;
                        }
                    }

                    lista.Add(bg);
                }

                estatus = "OK";
                msj = "Archivo leído correctamente";
                return lista;
            }
        }


        /// <summary>
        /// Lee la plantilla de demanda de cliente
        /// </summary>
        /// <param name="streamPostedFile"></param>
        /// <param name="valido"></param>
        /// <returns></returns>
        public static List<BG_IHS_rel_demanda> LeePlantillaDemanda(HttpPostedFileBase streamPostedFile, ref bool valido, ref string msjError, int? versionIHS = null)
        {
            List<BG_IHS_rel_demanda> lista = new List<BG_IHS_rel_demanda>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                using (var db = new Portal_2_0Entities())
                {
                    var listIHSItems = db.BG_IHS_item;
                    var listIHSVersions = db.BG_IHS_versiones;

                    //recorre todas las hojas del archivo
                    foreach (DataTable table in result.Tables)
                    {
                        //realiza el proceso unicamente para la primera hoja
                        if (result.Tables.IndexOf(table) == 0)
                        {

                            valido = true;

                            //se obtienen las cabeceras
                            List<string> encabezados = new List<string>();

                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                string title = table.Rows[0][i].ToString();

                                if (!string.IsNullOrEmpty(title))
                                    encabezados.Add(title.ToUpper());
                            }

                            //verifica que la estrura del archivo sea válida
                            if (!encabezados.Contains("ID") && !encabezados.Contains("MONTH REPORT"))
                            {
                                msjError = "El archivo no cuenta con la columna 'ID' ni 'MONTH REPORT'";
                                valido = false;
                                return lista;
                            }

                            // El valor será el ID del item.
                            var itemsDictionary = db.BG_IHS_item.Where(x=>x.id_ihs_version == versionIHS)
                                .ToDictionary(
                                    item => (item.id_ihs_version, item.vehicle, item.mnemonic_vehicle_plant), // Llave de búsqueda
                                    item => item.id                                                           // Valor a obtener
                                );

                            //la fila cero se omite (encabezado)
                            for (int i = 1; i < table.Rows.Count; i++)
                            {
                                //System.Diagnostics.Debug.WriteLine("Procesando: " + i + "/" + table.Rows.Count);
                                try
                                {
                                    //variables
                                    int idItem = 0;
                                    CultureInfo ci = new CultureInfo("es-MX");

                                    //obtiene el ihs version y convierte la fecha para toda la fila
                                    BG_IHS_versiones version_ihs = null;
                                    int version_ihs_id = 0;
                                    try
                                    {
                                        if (encabezados[0] == "MONTH REPORT" && DateTime.TryParse(table.Rows[i][0].ToString(), out DateTime mesR))
                                        {
                                            version_ihs = listIHSVersions.FirstOrDefault(x => x.periodo.Month == mesR.Month && x.periodo.Year == mesR.Year);
                                            version_ihs_id = version_ihs.id;
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        msjError = "No existe una version de IHS para mes: " + table.Rows[i][0].ToString() + ", es necesaria primero crearla en el sistema.";
                                        valido = false;
                                        return lista;
                                    }

                                    //recorre todas los encabezados
                                    for (int j = 0; j < encabezados.Count; j++)
                                    {
                                        //obtiene la cabezara de i
                                        switch (encabezados[j])
                                        {
                                            case "ID":
                                                string mnemonicVehiclePlant2 = table.Rows[i][encabezados.IndexOf("MNEMONIC-VEHICLE/PLANT")].ToString();
                                                string vehicleText2 = table.Rows[i][encabezados.IndexOf("VEHICLE (IHS)")].ToString();

                                                // 1. Validamos que la versión (versionIHS) no sea nula antes de buscar
                                                if (versionIHS.HasValue)
                                                {
                                                    // 2. Creamos la llave usando .Value para obtener el 'int' (no el 'int?')
                                                    var searchKey = (versionIHS.Value, vehicleText2, mnemonicVehiclePlant2);

                                                    // Ahora la llave tiene el tipo correcto (int, string, string) y la búsqueda funcionará
                                                    itemsDictionary.TryGetValue(searchKey, out idItem);
                                                }
                                                // Si versionIHS es nulo, idItem simplemente se quedará en 0 y no se encontrará nada.

                                                break;
                                            case "MONTH REPORT":
                                                //Obtiene el id_ihs_item en base al mes de reporte y mnemonicvehicleplant
                                                DateTime mesReporte = DateTime.Parse(table.Rows[i][0].ToString());

                                                string MnemonicVehiclePlant = table.Rows[i][encabezados.IndexOf("MNEMONIC-VEHICLE/PLANT")].ToString();
                                                string vehicleText = table.Rows[i][encabezados.IndexOf("VEHICLE")].ToString();

                                                //para validar si un elemento existe compara el mnemonic_vehicle_plant y vehicle text
                                                var itemBusca = listIHSItems.FirstOrDefault(x => x.mnemonic_vehicle_plant == MnemonicVehiclePlant
                                                    && x.vehicle == vehicleText
                                                    && x.BG_IHS_versiones.periodo.Month == mesReporte.Month && x.BG_IHS_versiones.periodo.Year == mesReporte.Year
                                                    && x.id_ihs_version == version_ihs_id
                                                    );

                                                if (itemBusca != null || idItem != 0)
                                                    idItem = itemBusca.id;
                                                else
                                                {
                                                    //agregar como ihs origen = "USER"
                                                    System.Diagnostics.Debug.WriteLine("No se encuentra ihs item");

                                                    //si existe el mnemonic, pero no hay coincidencia en text, lo ignoara
                                                    bool existeMnemonic = listIHSItems.Any(x => x.mnemonic_vehicle_plant == MnemonicVehiclePlant
                                                    && x.BG_IHS_versiones.periodo.Month == mesReporte.Month && x.BG_IHS_versiones.periodo.Year == mesReporte.Year
                                                    && x.id_ihs_version == version_ihs_id
                                                    );

                                                    if (!existeMnemonic)
                                                    {

                                                        BG_IHS_item newItem = new BG_IHS_item
                                                        {
                                                            id_ihs_version = version_ihs_id,
                                                            core_nameplate_region_mnemonic = table.Rows[i][1]?.ToString() ?? string.Empty,
                                                            core_nameplate_plant_mnemonic = table.Rows[i][2]?.ToString() ?? string.Empty,
                                                            mnemonic_vehicle = table.Rows[i][3]?.ToString() ?? string.Empty,
                                                            mnemonic_vehicle_plant = string.IsNullOrWhiteSpace(table.Rows[i][4]?.ToString())
            ? throw new Exception($"El campo 'mnemonic_vehicle_plant' en la fila {i + 1} es obligatorio y está vacío.")
            : table.Rows[i][4]?.ToString(),
                                                            mnemonic_platform = table.Rows[i][5]?.ToString() ?? string.Empty,
                                                            mnemonic_plant = table.Rows[i][6]?.ToString() ?? string.Empty,
                                                            region = table.Rows[i][7]?.ToString() ?? string.Empty,
                                                            market = table.Rows[i][8]?.ToString() ?? string.Empty,
                                                            country_territory = table.Rows[i][9]?.ToString() ?? string.Empty,
                                                            production_plant = table.Rows[i][10]?.ToString() ?? string.Empty,
                                                            city = table.Rows[i][11]?.ToString() ?? string.Empty,
                                                            plant_state_province = table.Rows[i][12]?.ToString() ?? string.Empty,
                                                            source_plant = table.Rows[i][13]?.ToString() ?? string.Empty,
                                                            source_plant_country_territory = table.Rows[i][14]?.ToString() ?? string.Empty,
                                                            source_plant_region = table.Rows[i][15]?.ToString() ?? string.Empty,
                                                            design_parent = table.Rows[i][16]?.ToString() ?? string.Empty,
                                                            engineering_group = table.Rows[i][17]?.ToString() ?? string.Empty,
                                                            manufacturer_group = table.Rows[i][18]?.ToString() ?? string.Empty,
                                                            manufacturer = table.Rows[i][19]?.ToString() ?? string.Empty,
                                                            sales_parent = table.Rows[i][20]?.ToString() ?? string.Empty,
                                                            production_brand = table.Rows[i][21]?.ToString() ?? string.Empty,
                                                            platform_design_owner = table.Rows[i][22]?.ToString() ?? string.Empty,
                                                            architecture = table.Rows[i][23]?.ToString() ?? string.Empty,
                                                            platform = table.Rows[i][24]?.ToString() ?? string.Empty,
                                                            program = table.Rows[i][25]?.ToString() ?? string.Empty,
                                                            production_nameplate = table.Rows[i][26]?.ToString() ?? string.Empty,
                                                            sop_start_of_production = DateTime.TryParse(table.Rows[i][27]?.ToString(), out var sopDate)
            ? sopDate
            : throw new Exception($"El campo 'sop_start_of_production' en la fila {i + 1} es obligatorio y debe tener un formato válido."),
                                                            eop_end_of_production = DateTime.TryParse(table.Rows[i][28]?.ToString(), out var eopDate)
            ? eopDate
            : throw new Exception($"El campo 'eop_end_of_production' en la fila {i + 1} es obligatorio y debe tener un formato válido."),
                                                            lifecycle_time = Int32.TryParse(table.Rows[i][29]?.ToString(), out var lifecycle) ? lifecycle : 0,
                                                            vehicle = table.Rows[i][30]?.ToString() ?? string.Empty,
                                                            assembly_type = table.Rows[i][31]?.ToString() ?? string.Empty,
                                                            strategic_group = table.Rows[i][32]?.ToString() ?? string.Empty,
                                                            sales_group = table.Rows[i][33]?.ToString() ?? string.Empty,
                                                            global_nameplate = table.Rows[i][34]?.ToString() ?? string.Empty,
                                                            primary_design_center = table.Rows[i][35]?.ToString() ?? string.Empty,
                                                            primary_design_country_territory = table.Rows[i][36]?.ToString() ?? string.Empty,
                                                            primary_design_region = table.Rows[i][37]?.ToString() ?? string.Empty,
                                                            secondary_design_center = table.Rows[i][38]?.ToString() ?? string.Empty,
                                                            secondary_design_country_territory = table.Rows[i][39]?.ToString() ?? string.Empty,
                                                            secondary_design_region = table.Rows[i][40]?.ToString() ?? string.Empty,
                                                            gvw_rating = table.Rows[i][41]?.ToString() ?? string.Empty,
                                                            gvw_class = table.Rows[i][42]?.ToString() ?? string.Empty,
                                                            car_truck = table.Rows[i][43]?.ToString() ?? string.Empty,
                                                            production_type = table.Rows[i][44]?.ToString() ?? string.Empty,
                                                            global_production_segment = table.Rows[i][45]?.ToString() ?? string.Empty,
                                                            regional_sales_segment = table.Rows[i][46]?.ToString() ?? string.Empty,
                                                            global_production_price_class = table.Rows[i][47]?.ToString() ?? string.Empty,
                                                            global_sales_segment = table.Rows[i][48]?.ToString() ?? string.Empty,
                                                            global_sales_sub_segment = table.Rows[i][49]?.ToString() ?? string.Empty,
                                                            global_sales_price_class = Int32.TryParse(table.Rows[i][50]?.ToString(), out var salesPriceClass) ? salesPriceClass : 0,
                                                            short_term_risk_rating = Int32.TryParse(table.Rows[i][51]?.ToString(), out var shortTermRisk) ? shortTermRisk : 0,
                                                            long_term_risk_rating = Int32.TryParse(table.Rows[i][52]?.ToString(), out var longTermRisk) ? longTermRisk : 0,
                                                            origen = Bitacoras.Util.BG_IHS_Origen.USER,
                                                            porcentaje_scrap = 0.03M
                                                        };

                                                        //agrega los cuartos de IHS item

                                                        //agrega a la bd
                                                        try
                                                        {
                                                            db.Configuration.ValidateOnSaveEnabled = false;
                                                            if (version_ihs_id > 0)
                                                            {

                                                                db.BG_IHS_item.Add(newItem);

                                                                db.SaveChanges();
                                                                //agrega a la lista de búsqueda
                                                                newItem.BG_IHS_versiones = version_ihs;
                                                                //listIHSItems.Add(newItem);

                                                                idItem = newItem.id;

                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            System.Diagnostics.Debug.WriteLine(ex.Message);
                                                        }
                                                        finally
                                                        {
                                                            db.Configuration.ValidateOnSaveEnabled = true; // Habilita la validación de nuevo
                                                        }
                                                    }
                                                }
                                                break;

                                            default:
                                                //Si puede converir un encabezado en fecha
                                                DateTime fecha = DateTime.Now;

                                                if (idItem > 0 && DateTime.TryParseExact(encabezados[j], "MMM yyyy", ci, DateTimeStyles.None, out fecha))
                                                {

                                                    ////obtiene la cantidad
                                                    int? cantidad = null;

                                                    if (double.TryParse(table.Rows[i][j].ToString(), out double cantidad_result))
                                                        cantidad = (int)Math.Round(cantidad_result, 0);

                                                    //if (idItem == 47203 && fecha.Year == 2023 && fecha.Month == 9)
                                                    //{
                                                    //    System.Diagnostics.Debug.Print("Mazda Dic 2023");
                                                    //}

                                                    lista.Add(new BG_IHS_rel_demanda
                                                    {
                                                        id_ihs_item = idItem,
                                                        fecha = fecha,
                                                        tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER,
                                                        cantidad = cantidad,
                                                    });
                                                }
                                                //else
                                                //{
                                                //    System.Diagnostics.Debug.WriteLine("No se puede convertir: " + encabezados[j]);
                                                //}

                                                break;
                                        }
                                    }


                                    ////agrega a la lista con los datos leidos
                                    //lista.Add(new BG_IHS_rel_demanda()
                                    //{
                                    //    id = idItem,

                                    //});
                                }
                                catch (Exception e)
                                {
                                    System.Diagnostics.Debug.Print("Error: " + e.Message);
                                }
                            }
                        }

                    }
                }

            }

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado de IHS
        ///</summary>
        ///<return>
        ///Devuelve un List<BG_IHS_item> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<BG_IHS_item> LeeIHS(HttpPostedFileBase streamPostedFile, ref bool valido, ref string msjError, ref DateTime periodo)
        {
            List<BG_IHS_item> lista = new List<BG_IHS_item>();
            CultureInfo provider = new CultureInfo("en-US");
            string hoja = "LVP Production";

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                //verifica que tenga la hoja
                if (!result.Tables.Contains(hoja))
                {
                    valido = false;
                    msjError = "El documento no tiene una hoja llamada: " + hoja;
                }

                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {

                    //busca si existe una hoja llamada "dante"
                    if (table.TableName == hoja)
                    {

                        //obtiene el forcastRelese
                        string periodoString = table.Rows[2][1].ToString();

                        if (!DateTime.TryParse(periodoString, out DateTime periodoResult))
                        {
                            valido = false;
                            msjError = "No se pudo obtener el ForecastRelease en la celda B3.";
                            continue;
                            //throw new Exception(msjError);
                        }
                        else
                        {
                            periodo = periodoResult;
                            valido = true;
                        }

                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();
                        int filaEncabezado = 4;

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[filaEncabezado][i].ToString();

                            if (!string.IsNullOrEmpty(title))
                                encabezados.Add(title);
                        }

                        //verifica que el archivo tenga todas las columnas necesarias
                        foreach (string title in UtilExcel.ListadoCabeceraIHS)
                        {
                            if (!encabezados.Contains(title))
                            {
                                valido = false;
                                msjError = "La hoja no cuenta con una columna llamada: " + title + " . La cabecera de la tabla debe estar en la fila 5.";
                                return lista;
                            }
                        }

                        //la fila 4 se omite (encabezado)
                        for (int i = 5; i < table.Rows.Count + 4; i++)
                        {
                            BG_IHS_item bg = new BG_IHS_item();

                            try
                            {
                                //variables
                                string core_nameplate_region_mnemonic = string.Empty;
                                string core_nameplate_plant_mnemonic = string.Empty;
                                string mnemonic_vehicle = string.Empty;
                                string mnemonic_vehicle_plant = string.Empty;
                                string mnemonic_platform = string.Empty;
                                string mnemonic_plant = string.Empty;
                                string region = string.Empty;
                                string market = string.Empty;
                                string country_territory = string.Empty;
                                string production_plant = string.Empty;
                                string city = string.Empty;
                                string plant_state_province = string.Empty;
                                string source_plant = string.Empty;
                                string source_plant_country_territory = string.Empty;
                                string source_plant_region = string.Empty;
                                string design_parent = string.Empty;
                                string engineering_group = string.Empty;
                                string manufacturer_group = string.Empty;
                                string manufacturer = string.Empty;
                                string sales_parent = string.Empty;
                                string production_brand = string.Empty;
                                string platform_design_owner = string.Empty;
                                string architecture = string.Empty;
                                string platform = string.Empty;
                                string program = string.Empty;
                                string production_nameplate = string.Empty;
                                DateTime sop_start_of_production = new DateTime(2000, 01, 01); //fecha por defecto
                                DateTime eop_end_of_production = new DateTime(2000, 01, 01); //fecha por defecto
                                int lifecycle_time = 0;
                                string vehicle = string.Empty;
                                string assembly_type = string.Empty;
                                string strategic_group = string.Empty;
                                string sales_group = string.Empty;
                                string global_nameplate = string.Empty;
                                string primary_design_center = string.Empty;
                                string primary_design_country_territory = string.Empty;
                                string primary_design_region = string.Empty;
                                string secondary_design_center = string.Empty;
                                string secondary_design_country_territory = string.Empty;
                                string secondary_design_region = string.Empty;
                                string gvw_rating = string.Empty;
                                string gvw_class = string.Empty;
                                string car_truck = string.Empty;
                                string production_type = string.Empty;
                                string global_production_segment = string.Empty;
                                string regional_sales_segment = string.Empty;
                                string global_production_price_class = string.Empty;
                                string global_sales_segment = string.Empty;
                                string global_sales_sub_segment = string.Empty;
                                int global_sales_price_class = 0; //int 
                                int short_term_risk_rating = 0;       //int 
                                int long_term_risk_rating = 0;         //int


                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "Core Nameplate Region Mnemonic":
                                            core_nameplate_region_mnemonic = table.Rows[i][j].ToString();
                                            break;
                                        case "Core Nameplate Plant Mnemonic":
                                            core_nameplate_plant_mnemonic = table.Rows[i][j].ToString();
                                            break;
                                        case "Mnemonic-Vehicle":
                                            mnemonic_vehicle = table.Rows[i][j].ToString();
                                            break;
                                        case "Mnemonic-Vehicle/Plant":
                                            mnemonic_vehicle_plant = table.Rows[i][j].ToString();
                                            break;
                                        case "Mnemonic-Platform":
                                            mnemonic_platform = table.Rows[i][j].ToString();
                                            break;
                                        case "Mnemonic-Plant":
                                            mnemonic_plant = table.Rows[i][j].ToString();
                                            break;
                                        case "Region":
                                            region = table.Rows[i][j].ToString();
                                            break;
                                        case "Market":
                                            market = table.Rows[i][j].ToString();
                                            break;
                                        case "Country/Territory":
                                            country_territory = table.Rows[i][j].ToString();
                                            break;
                                        case "Production Plant":
                                            production_plant = table.Rows[i][j].ToString();
                                            break;
                                        case "City":
                                            city = table.Rows[i][j].ToString();
                                            break;
                                        case "Plant State/Province":
                                            plant_state_province = table.Rows[i][j].ToString();
                                            break;
                                        case "Source Plant":
                                            source_plant = table.Rows[i][j].ToString();
                                            break;
                                        case "Source Plant Country/Territory":
                                            source_plant_country_territory = table.Rows[i][j].ToString();
                                            break;
                                        case "Source Plant Region":
                                            source_plant_region = table.Rows[i][j].ToString();
                                            break;
                                        case "Design Parent":
                                            design_parent = table.Rows[i][j].ToString();
                                            break;
                                        case "Engineering Group":
                                            engineering_group = table.Rows[i][j].ToString();
                                            break;
                                        case "Manufacturer Group":
                                            manufacturer_group = table.Rows[i][j].ToString();
                                            break;
                                        case "Manufacturer":
                                            manufacturer = table.Rows[i][j].ToString();
                                            break;
                                        case "Sales Parent":
                                            sales_parent = table.Rows[i][j].ToString();
                                            break;
                                        case "Production Brand":
                                            production_brand = table.Rows[i][j].ToString();
                                            break;
                                        case "Platform Design Owner":
                                            platform_design_owner = table.Rows[i][j].ToString();
                                            break;
                                        case "Architecture":
                                            architecture = table.Rows[i][j].ToString();
                                            break;
                                        case "Platform":
                                            platform = table.Rows[i][j].ToString();
                                            break;
                                        case "Program":
                                            program = table.Rows[i][j].ToString();
                                            break;
                                        case "Production Nameplate":
                                            production_nameplate = table.Rows[i][j].ToString();
                                            break;
                                        case "SOP (Start of Production)":
                                            if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                                sop_start_of_production = Convert.ToDateTime(table.Rows[i][j].ToString());
                                            break;
                                        case "EOP (End of Production)":
                                            if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                                eop_end_of_production = Convert.ToDateTime(table.Rows[i][j].ToString());
                                            break;
                                        case "Lifecycle (Time)":
                                            if (Int32.TryParse(table.Rows[i][j].ToString(), out int lc))
                                                lifecycle_time = lc;
                                            break;
                                        case "Vehicle":
                                            vehicle = table.Rows[i][j].ToString();
                                            break;
                                        case "Assembly Type":
                                            assembly_type = table.Rows[i][j].ToString();
                                            break;
                                        case "Strategic Group":
                                            strategic_group = table.Rows[i][j].ToString();
                                            break;
                                        case "Sales Group":
                                            sales_group = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Nameplate":
                                            global_nameplate = table.Rows[i][j].ToString();
                                            break;
                                        case "Primary Design Center":
                                            primary_design_center = table.Rows[i][j].ToString();
                                            break;
                                        case "Primary Design Country/Territory":
                                            primary_design_country_territory = table.Rows[i][j].ToString();
                                            break;
                                        case "Primary Design Region":
                                            primary_design_region = table.Rows[i][j].ToString();
                                            break;
                                        case "Secondary Design Center":
                                            secondary_design_center = table.Rows[i][j].ToString();
                                            break;
                                        case "Secondary Design Country/Territory":
                                            secondary_design_country_territory = table.Rows[i][j].ToString();
                                            break;
                                        case "Secondary Design Region":
                                            secondary_design_region = table.Rows[i][j].ToString();
                                            break;
                                        case "GVW Rating":
                                            gvw_rating = table.Rows[i][j].ToString();
                                            break;
                                        case "GVW Class":
                                            gvw_class = table.Rows[i][j].ToString();
                                            break;
                                        case "Car/Truck":
                                            car_truck = table.Rows[i][j].ToString();
                                            break;
                                        case "Production Type":
                                            production_type = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Production Segment":
                                            global_production_segment = table.Rows[i][j].ToString();
                                            break;
                                        case "Regional Sales Segment":
                                            regional_sales_segment = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Production Price Class":
                                            global_production_price_class = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Sales Segment":
                                            global_sales_segment = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Sales Sub-Segment":
                                            global_sales_sub_segment = table.Rows[i][j].ToString();
                                            break;
                                        case "Global Sales Price Class":
                                            if (Int32.TryParse(table.Rows[i][j].ToString(), out int gspc))
                                                global_sales_price_class = gspc;
                                            break;
                                        case "Short-Term Risk Rating":
                                            if (Int32.TryParse(table.Rows[i][j].ToString(), out int strr))
                                                short_term_risk_rating = strr;
                                            break;
                                        case "Long-Term Risk Rating":
                                            if (Int32.TryParse(table.Rows[i][j].ToString(), out int ltrr))
                                                long_term_risk_rating = ltrr;
                                            break;
                                    }

                                    //si es fecha lo debe agregar objeto actual                                 
                                    string fechaString = encabezados[j].Replace(' ', '-');
                                    if (DateTime.TryParseExact(fechaString, "MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                                    {
                                        //canvierte la cantidad a numero
                                        if (Int32.TryParse(table.Rows[i][j].ToString(), out int cantidad))
                                        {
                                            if (cantidad > 0)
                                                bg.BG_IHS_rel_demanda.Add(new BG_IHS_rel_demanda
                                                {
                                                    cantidad = cantidad,
                                                    fecha = date,
                                                    //fecha_carga = null,
                                                    tipo = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL,
                                                });
                                        }
                                    }

                                    //si es un cuarto, debe guardar el valor                         
                                    string pattern = @"^Q\d\s\d{4}";
                                    string input = encabezados[j];
                                    Match m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
                                    if (m.Success)
                                    {
                                        //separa el string en 2
                                        string[] valores = input.Split(' ');
                                        valores[0] = valores[0].Substring(1);

                                        //canvierte la cantidad a numero
                                        if (Int32.TryParse(table.Rows[i][j].ToString(), out int cantidad))
                                        {
                                            bg.BG_IHS_rel_cuartos.Add(new BG_IHS_rel_cuartos
                                            {
                                                cantidad = cantidad,
                                                cuarto = Int32.Parse(valores[0]),
                                                anio = Int32.Parse(valores[1])
                                            });
                                        }

                                    }
                                }
                                //asigna los valores al item de IHS
                                bg.core_nameplate_region_mnemonic = core_nameplate_region_mnemonic;
                                bg.core_nameplate_plant_mnemonic = core_nameplate_plant_mnemonic;
                                bg.mnemonic_vehicle = mnemonic_vehicle;
                                bg.mnemonic_vehicle_plant = mnemonic_vehicle_plant;
                                bg.mnemonic_platform = mnemonic_platform;
                                bg.mnemonic_plant = mnemonic_plant;
                                bg.region = region;
                                bg.market = market;
                                bg.country_territory = country_territory;
                                bg.production_plant = production_plant;
                                bg.city = city;
                                bg.plant_state_province = plant_state_province;
                                bg.source_plant = source_plant;
                                bg.source_plant_country_territory = source_plant_country_territory;
                                bg.source_plant_region = source_plant_region;
                                bg.design_parent = design_parent;
                                bg.engineering_group = engineering_group;
                                bg.manufacturer_group = manufacturer_group;
                                bg.manufacturer = manufacturer;
                                bg.sales_parent = sales_parent;
                                bg.production_brand = production_brand;
                                bg.platform_design_owner = platform_design_owner;
                                bg.architecture = architecture;
                                bg.platform = platform;
                                bg.program = program;
                                bg.production_nameplate = production_nameplate;
                                bg.sop_start_of_production = sop_start_of_production;
                                bg.eop_end_of_production = eop_end_of_production;
                                bg.lifecycle_time = lifecycle_time; //int
                                bg.vehicle = vehicle;
                                bg.assembly_type = assembly_type;
                                bg.strategic_group = strategic_group;
                                bg.sales_group = sales_group;
                                bg.global_nameplate = global_nameplate;
                                bg.primary_design_center = primary_design_center;
                                bg.primary_design_country_territory = primary_design_country_territory;
                                bg.primary_design_region = primary_design_region;
                                bg.secondary_design_center = secondary_design_center;
                                bg.secondary_design_country_territory = secondary_design_country_territory;
                                bg.secondary_design_region = secondary_design_region;
                                bg.gvw_rating = gvw_rating;
                                bg.gvw_class = gvw_class;
                                bg.car_truck = car_truck;
                                bg.production_type = production_type;
                                bg.global_production_segment = global_production_segment;
                                bg.regional_sales_segment = regional_sales_segment;
                                bg.global_production_price_class = global_production_price_class;
                                bg.global_sales_segment = global_sales_segment;
                                bg.global_sales_sub_segment = global_sales_sub_segment;
                                bg.global_sales_price_class = global_sales_price_class; //int 
                                bg.short_term_risk_rating = short_term_risk_rating;       //int 
                                bg.long_term_risk_rating = long_term_risk_rating;       //int
                                bg.origen = Bitacoras.Util.BG_IHS_Origen.IHS;
                                bg.porcentaje_scrap = 0.03M;

                                // agrega a la lista con los datos leidos
                                lista.Add(bg);
                            }
                            catch (Exception e)
                            {
                                msjError = e.Message;
                                System.Diagnostics.Debug.Print("Error: " + e.Message);
                            }
                        }
                    }
                }

            }

            return lista;
        }

        ///<summary>
        ///Lee un archivo de excel y carga el listado de IHS
        ///</summary>
        ///<return>
        ///Devuelve un List<BG_IHS_item> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<string> LeeHojasArchivo(HttpPostedFileBase streamPostedFile, ref string estatus, ref string msj)
        {
            List<string> lista = new List<string>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();


                //recorre todas las hojas del archivo
                foreach (DataTable table in result.Tables)
                {
                    lista.Add(table.TableName);
                }

            }

            //retorna la ruta correctamente
            estatus = "OK";
            msj = "Archivo leído correctamente";
            return lista;
        }

        public static List<BG_Forecast_cat_historico_scrap> LeeHistoricoScrap(HttpPostedFileBase streamPostedFile, ref bool valido, ref string msjError)
        {

            //obtiene todas las cuentas
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<BG_Forecast_cat_historico_scrap> lista = new List<BG_Forecast_cat_historico_scrap>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(streamPostedFile.InputStream))
            {
                //obtiene el dataset del archivo de excel
                var result = reader.AsDataSet();

                //estable la variable a false por defecto
                valido = false;

                //recorre todas las hojas del archivo
                bool existeHoja = false;

                String costCenter = string.Empty;

                foreach (DataTable table in result.Tables)
                {
                    if (table.TableName.ToUpper() == "HISTORICO")
                        existeHoja = true;
                }

                if (!existeHoja)
                {
                    msjError = "En el archivo seleccionado no existe una la hoja llamada 'Historico'.";
                    return lista;
                }

                foreach (DataTable table in result.Tables)
                {
                    //busca si existe una hoja llamada "Template"
                    if (table.TableName.ToUpper() == "HISTORICO")
                    {
                        valido = true;


                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();
                        int filaEncabezado = 0;

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[filaEncabezado][i].ToString();

                            if (!string.IsNullOrEmpty(title))
                                encabezados.Add(title.ToUpper());
                        }

                        //lita de cabeceras
                        List<string> ListadoCabeceraScrap = new List<string> {
                            "Planta", "Tipo Metal", "Mes (yyyy-MM)", "Valor Scrap", "Valor Ganancia"
                        };

                        //verifica que el archivo tenga todas las columnas necesarias
                        foreach (string title in ListadoCabeceraScrap)
                        {
                            if (!encabezados.Contains(title.ToUpper()))
                            {
                                valido = false;
                                msjError = "La hoja no cuenta con una columna llamada: " + title + ".";
                                return lista;
                            }
                        }

                        //la fila cero se omite (encabezado)
                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                //variables
                                string planta = String.Empty;
                                int id_planta = 1;
                                string tipoMetal = String.Empty;
                                string mes = String.Empty;
                                double valorScrap = 0;
                                double valorGanancia = 0;
                                DateTime fecha = DateTime.Now;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "PLANTA":
                                            planta = table.Rows[i][j].ToString().ToUpper();
                                            if (planta.Contains("PUEBLA"))
                                                id_planta = 1;
                                            else if (planta.Contains("SILAO"))
                                                id_planta = 2;
                                            else if (planta.Contains("SLP"))
                                                id_planta = 5;
                                            break;
                                        case "TIPO METAL":
                                            tipoMetal = table.Rows[i][j].ToString().ToUpper();
                                            break;
                                        case "MES (YYYY-MM)":
                                            mes = table.Rows[i][j].ToString();
                                            fecha = DateTime.Parse(mes);
                                            break;
                                        case "VALOR SCRAP":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double vs))
                                                valorScrap = vs;
                                            break;
                                        case "VALOR GANANCIA":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double vg))
                                                valorGanancia = vg;
                                            break;
                                    }
                                }


                                //agrega a la lista con los datos leidos
                                lista.Add(new BG_Forecast_cat_historico_scrap()
                                {
                                    id_planta = id_planta,
                                    tipo_metal = tipoMetal,
                                    fecha = fecha,
                                    scrap = valorScrap,
                                    scrap_ganancia = valorGanancia

                                });
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.Print("Error: " + e.Message);
                            }
                        }

                    }
                }

            }

            return lista;
        }


    }

}

public class EncabezadoTableMenu
{
    public string fecha { get; set; }
    public int columna { get; set; }
}
