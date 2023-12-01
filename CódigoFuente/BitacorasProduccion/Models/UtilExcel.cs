using Clases.Util;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class UtilExcel
    {
        ///<summary>
        ///Lee un archivo de excel y carga el listado de bom
        ///</summary>
        ///<return>
        ///Devuelve un List<bom_en_sap> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<bom_en_sap> LeeBom(HttpPostedFileBase streamPostedFile, ref bool valido)
        {
            List<bom_en_sap> lista = new List<bom_en_sap>();

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
                    if (table.TableName.ToUpper() == "SHEET1" || table.TableName.ToUpper() == "HOJA1")
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
                        if (!encabezados.Contains("MATERIAL NO.") || !encabezados.Contains("PLANT") || !encabezados.Contains("BOM NO.")
                            || !encabezados.Contains("ALTERNATIVE BOM") || !encabezados.Contains("ITEM NO") || !encabezados.Contains("BOM COMPONENT"))
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
                                string Plnt = String.Empty;
                                string BOM = String.Empty;
                                string AltBOM = String.Empty;
                                string Item = String.Empty;
                                string Component = String.Empty;
                                Nullable<double> Quantity = null;
                                string Un = string.Empty;
                                Nullable<System.DateTime> Created = null;
                                Nullable<System.DateTime> LastUsed = null;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "MATERIAL NO.":
                                            material = table.Rows[i][j].ToString();
                                            break;
                                        case "PLANT":
                                            Plnt = table.Rows[i][j].ToString();
                                            break;
                                        case "BOM NO.":
                                            BOM = table.Rows[i][j].ToString();
                                            break;
                                        case "ALTERNATIVE BOM":
                                            AltBOM = table.Rows[i][j].ToString();
                                            break;
                                        case "ITEM NO":
                                            Item = table.Rows[i][j].ToString();
                                            break;
                                        case "BOM COMPONENT":
                                            Component = table.Rows[i][j].ToString();
                                            break;
                                        case "COMPONENT QTY":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double q))
                                                Quantity = q;
                                            break;
                                        case "UNIT OF MEASURE":
                                            Un = table.Rows[i][j].ToString();
                                            break;
                                        case "DATE CREATED":
                                            if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                                Created = Convert.ToDateTime(table.Rows[i][j].ToString());
                                            break;
                                        case "LAST DATE USED":
                                            if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                                LastUsed = Convert.ToDateTime(table.Rows[i][j].ToString());
                                            break;
                                    }
                                }


                                //agrega a la lista con los datos leidos
                                lista.Add(new bom_en_sap()
                                {
                                    Material = material,
                                    Plnt = Plnt,
                                    BOM = BOM,
                                    AltBOM = AltBOM,
                                    Item = Item,
                                    Component = Component,
                                    Un = Un,
                                    Quantity = Quantity,
                                    Created = Created,
                                    LastDateUsed = LastUsed

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
        public static List<class_v3> LeeClass(HttpPostedFileBase streamPostedFile, ref bool valido)
        {
            List<class_v3> lista = new List<class_v3>();

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
                    if (table.TableName.ToUpper() == "SHEET1" || table.TableName.ToUpper() == "HOJA1")
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
                        if (!encabezados.Contains("OBJECT") || !encabezados.Contains("GRADE") || !encabezados.Contains("CUSTOMER PART NUMBER")
                            || !encabezados.Contains("SURFACE") || !encabezados.Contains("MILL"))
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
                                string Object = String.Empty;
                                string Grape = String.Empty;
                                string Customer = String.Empty;
                                string Shape = String.Empty;
                                string Customer_part_number = String.Empty;
                                string Surface = String.Empty;
                                string Gauge___Metric = String.Empty;
                                string Mill = String.Empty;
                                string Width___Metr = String.Empty;
                                string Length_mm_ = String.Empty;

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "OBJECT":
                                            Object = table.Rows[i][j].ToString();
                                            break;
                                        case "GRADE":
                                            Grape = table.Rows[i][j].ToString();
                                            break;
                                        case "CUSTOMER":
                                            Customer = table.Rows[i][j].ToString();
                                            break;
                                        case "SHAPE":
                                            Shape = table.Rows[i][j].ToString();
                                            break;
                                        case "CUSTOMER PART NUMBER":
                                            Customer_part_number = table.Rows[i][j].ToString();
                                            break;
                                        case "SURFACE":
                                            Surface = table.Rows[i][j].ToString();
                                            break;
                                        case "GAUGE - METRIC":
                                            Gauge___Metric = table.Rows[i][j].ToString();
                                            break;
                                        case "MILL":
                                            Mill = table.Rows[i][j].ToString();
                                            break;
                                        case "WIDTH - METR":
                                            Width___Metr = table.Rows[i][j].ToString();
                                            break;
                                        case "LENGTH(MM)":
                                            Length_mm_ = table.Rows[i][j].ToString();
                                            break;

                                    }
                                }


                                //agrega a la lista con los datos leidos
                                lista.Add(new class_v3()
                                {
                                    Object = Object,
                                    Grade = Grape,
                                    Customer = Customer,
                                    Shape = Shape,
                                    Customer_part_number = Customer_part_number,
                                    Surface = Surface,
                                    Gauge___Metric = Gauge___Metric,
                                    Mill = Mill,
                                    Width___Metr = Width___Metr,
                                    Length_mm_ = Length_mm_,
                                    activo = true,
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
        ///Lee un archivo de excel y carga el listado de MM
        ///</summary>
        ///<return>
        ///Devuelve un List<mm_v3> con los datos leidos
        ///</return>
        ///<param name="streamPostedFile">
        ///Stream del archivo recibido en el formulario
        ///</param>
        public static List<mm_v3> LeeMM(HttpPostedFileBase streamPostedFile, ref bool valido)
        {
            List<mm_v3> lista = new List<mm_v3>();
            List<bom_en_sap> listadoBOM = new List<bom_en_sap>();

            using (var db = new Portal_2_0Entities())
            {
                listadoBOM = db.bom_en_sap.ToList();
            }

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
                    if (table.TableName.ToUpper() == "SHEET1" || table.TableName.ToUpper() == "HOJA1")
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
                        if (!encabezados.Contains("MATERIAL") || !encabezados.Contains("PLNT") || !encabezados.Contains("MS")
                            || !encabezados.Contains("Material Description".ToUpper()) || !encabezados.Contains("Type of Material".ToUpper()) || !encabezados.Contains("Type of Metal".ToUpper()))
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
                                string Plnt = String.Empty;
                                string MS = String.Empty;
                                string Material_Description = String.Empty;
                                string Type_of_Material = String.Empty;
                                string Type_of_Metal = String.Empty;
                                string Old_material_no_ = String.Empty;
                                string Head_and_Tails_Scrap_Conciliation = String.Empty;
                                string Engineering_Scrap_conciliation = String.Empty;
                                string Business_Model = String.Empty;
                                string Re_application = String.Empty;
                                string IHS_number_1 = String.Empty;
                                string IHS_number_2 = String.Empty;
                                string IHS_number_3 = String.Empty;
                                string IHS_number_4 = String.Empty;
                                string IHS_number_5 = String.Empty;
                                string Type_of_Selling = String.Empty;
                                string Package_Pieces = String.Empty;
                                string Un_ = String.Empty;
                                string Un_1 = String.Empty;
                                Nullable<double> Thickness = null;
                                Nullable<double> Width = null;
                                Nullable<double> Advance = null;
                                Nullable<double> Head_and_Tail_allowed_scrap = null;
                                Nullable<double> Pieces_per_car = null;
                                Nullable<double> Initial_Weight = null;
                                Nullable<double> Min_Weight = null;
                                Nullable<double> Maximum_Weight = null;


                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {
                                    //obtiene la cabezara de i
                                    switch (encabezados[j])
                                    {
                                        //obligatorios
                                        case "MATERIAL":
                                            material = table.Rows[i][j].ToString();
                                            break;
                                        case "PLNT":
                                            Plnt = table.Rows[i][j].ToString();
                                            break;
                                        case "MS":
                                            MS = table.Rows[i][j].ToString();
                                            break;
                                        case "MATERIAL DESCRIPTION":
                                            Material_Description = table.Rows[i][j].ToString();
                                            break;
                                        case "TYPE OF MATERIAL":
                                            Type_of_Material = table.Rows[i][j].ToString();
                                            break;
                                        case "TYPE OF METAL":
                                            Type_of_Metal = table.Rows[i][j].ToString();
                                            break;
                                        case "OLD MATERIAL NO.":
                                            Old_material_no_ = table.Rows[i][j].ToString();
                                            break;
                                        case "HEAD AND TAILS SCRAP CONCILIATION":
                                            Head_and_Tails_Scrap_Conciliation = table.Rows[i][j].ToString();
                                            break;
                                        case "ENGINEERING SCRAP CONCILIATION":
                                            Engineering_Scrap_conciliation = table.Rows[i][j].ToString();
                                            break;
                                        case "BUSINESS MODEL":
                                            Business_Model = table.Rows[i][j].ToString();
                                            break;
                                        case "RE-APPLICATION":
                                            Re_application = table.Rows[i][j].ToString();
                                            break;
                                        case "IHS NUMBER 1":
                                            IHS_number_1 = table.Rows[i][j].ToString();
                                            break;
                                        case "IHS NUMBER 2":
                                            IHS_number_2 = table.Rows[i][j].ToString();
                                            break;
                                        case "IHS NUMBER 4":
                                            IHS_number_4 = table.Rows[i][j].ToString();
                                            break;
                                        case "IHS NUMBER 5":
                                            IHS_number_5 = table.Rows[i][j].ToString();
                                            break;
                                        case "TYPE OF SELLING":
                                            Type_of_Selling = table.Rows[i][j].ToString();
                                            break;
                                        case "PACKAGE PIECES":
                                            Package_Pieces = table.Rows[i][j].ToString();
                                            break;
                                        //case "GROSS WEIGHT":
                                        //    if (Double.TryParse(table.Rows[i][j].ToString(), out double gross))
                                        //        Gross_weight = gross;
                                        //    break;
                                        case "UN.":
                                            Un_ = table.Rows[i][j].ToString();
                                            break;
                                        //case "NET WEIGHT":
                                        //    if (Double.TryParse(table.Rows[i][j].ToString(), out double net))
                                        //        Net_weight = net;
                                        //    break;
                                        case "THICKNESS":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double thick))
                                                Thickness = thick;
                                            break;
                                        case "WIDTH":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double wi))
                                                Width = wi;
                                            break;
                                        case "ADVANCE":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ad))
                                                Advance = ad;
                                            break;
                                        case "HEAD AND TAIL ALLOWED SCRAP":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ht))
                                                Head_and_Tail_allowed_scrap = ht;
                                            break;
                                        case "PIECES PER CAR":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double ppc))
                                                Pieces_per_car = ppc;
                                            break;
                                        case "INITIAL WEIGHT":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double iw))
                                                Initial_Weight = iw;
                                            break;
                                        case "MIN WEIGHT":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double minw))
                                                Min_Weight = minw;
                                            break;
                                        case "MAXIMUM WEIGHT":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double maxw))
                                                Maximum_Weight = maxw;
                                            break;

                                    }
                                }

                                //obtiene el valor de peso neto y bruto de BOM
                                #region PesosDeBOM
                                List<bom_en_sap> listTemporalBOM = listadoBOM.Where(x => x.Plnt == Plnt && x.Material == material).ToList();

                                DateTime? fechaCreacion = null, fechaUso = null;
                                double? peso_neto;
                                double? peso_bruto;

                                if (listTemporalBOM.Count > 0)
                                {
                                    fechaCreacion = listTemporalBOM.OrderByDescending(x => x.Created).FirstOrDefault().Created;
                                    fechaUso = listTemporalBOM.OrderByDescending(x => x.LastDateUsed).FirstOrDefault().LastDateUsed;
                                }

                                if (fechaUso.HasValue)
                                {
                                    //obtiene el peso bruto (el de mayor peso)
                                    peso_bruto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso).Max(x => x.Quantity);

                                    //si el peso bruto aparece dos veces, lo marca como valores duplicados
                                    bool duplicado = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity == peso_bruto).Count() > 1;

                                    if (duplicado)
                                    {
                                        //obtiene el peso quitando los duplicados
                                        peso_neto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity != (-0.001)).Select(x => x.Quantity).Distinct().Sum();
                                    }
                                    else
                                    {

                                        //peso bruto + los negativos
                                        peso_neto = peso_bruto + listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity < (-0.001)).Sum(x => x.Quantity);

                                        //se agregan excepciones conocidas

                                        switch (material)
                                        {
                                            case "HD10928": //no tiene scrap
                                                peso_bruto = peso_neto;
                                                break;
                                            default:
                                                break;
                                        }



                                    }
                                }
                                else
                                {
                                    peso_bruto = listTemporalBOM.Where(x => x.Created == fechaCreacion).Max(x => x.Quantity);
                                    //peso bruto + los negativos
                                    peso_neto = peso_bruto + listTemporalBOM.Where(x => x.Created == fechaCreacion && x.Quantity < (-0.001)).Sum(x => x.Quantity);
                                }


                                //actualiza el peso de todos los demas mm que tengan null
                                foreach (var item in lista.Where(x => x.Material == material && !x.Net_weight.HasValue && !x.Gross_weight.HasValue))
                                {
                                    item.Net_weight = peso_neto;
                                    item.Gross_weight = peso_bruto;
                                }
                                //si peso neto y bruto es null toma el valor de mm donde no sea null
                                if (!peso_neto.HasValue && !peso_bruto.HasValue)
                                {
                                    var item = lista.Where(x => x.Material == material && x.Net_weight.HasValue && x.Gross_weight.HasValue).FirstOrDefault();

                                    if (item != null)
                                    {
                                        peso_neto = item.Net_weight;
                                        peso_bruto = item.Gross_weight;
                                    }

                                }


                                #endregion

                                //agrega a la lista con los datos leidos
                                lista.Add(new mm_v3()
                                {
                                    Material = material,
                                    Plnt = Plnt,
                                    MS = MS,
                                    Material_Description = Material_Description,
                                    Type_of_Material = Type_of_Material,
                                    Type_of_Metal = Type_of_Metal,
                                    Old_material_no_ = Old_material_no_,
                                    Head_and_Tails_Scrap_Conciliation = Head_and_Tails_Scrap_Conciliation,
                                    Engineering_Scrap_conciliation = Engineering_Scrap_conciliation,
                                    Business_Model = Business_Model,
                                    Re_application = Re_application,
                                    IHS_number_1 = IHS_number_1,
                                    IHS_number_2 = IHS_number_2,
                                    IHS_number_3 = IHS_number_3,
                                    IHS_number_4 = IHS_number_4,
                                    IHS_number_5 = IHS_number_5,
                                    Type_of_Selling = Type_of_Selling,
                                    Package_Pieces = Package_Pieces,
                                    Gross_weight = peso_bruto,
                                    Un_ = Un_,
                                    Net_weight = peso_neto,
                                    Un_1 = Un_,
                                    Thickness = Thickness,
                                    Width = Width,
                                    Advance = Advance,
                                    Head_and_Tail_allowed_scrap = Head_and_Tail_allowed_scrap,
                                    Pieces_per_car = Pieces_per_car,
                                    Initial_Weight = Initial_Weight,
                                    Min_Weight = Min_Weight,
                                    Maximum_Weight = Maximum_Weight,

                                    activo = true
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
                    string title = result.Tables[0].Rows[filaInicio-1][i].ToString();

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
        public static List<RH_menu_comedor_platillos> LeeMenuComedor(HttpPostedFileBase streamPostedFile, ref List<string> errores)
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
                        if (encabezados.Count > 5)
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
                                if (Int32.TryParse(table.Rows[i][encabezadoFecha.columna + 1].ToString(), out int kc))
                                    kcal = kc;

                                //agrega a la lista con los datos leidos
                                if (!string.IsNullOrEmpty(platillo_nombre) && kcal.HasValue && platillo_tipo.Length <= 30 && !string.IsNullOrEmpty(platillo_tipo))
                                    lista.Add(new RH_menu_comedor_platillos()
                                    {
                                        orden_display = i - filaCabera - 1,
                                        tipo_platillo = UsoStrings.RecortaString(platillo_tipo.Trim(), 50), //quita espacios en blanco al inicio y al final del string
                                        nombre_platillo = UsoStrings.RecortaString(platillo_nombre.Trim(), 100),
                                        fecha = fecha,
                                        kcal = kcal,

                                    });
                                else
                                    goto finalRecorrido; //si no se puede agregar deja de recorrer las filas
                            }

                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print("Error: " + e.Message);
                        }
                    }
                //final del recorrido de filas, para la hoja actual
                finalRecorrido:
                    System.Diagnostics.Debug.WriteLine("Recorrido finalizaso para la hoja: " + table.TableName);
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
    }

    public class EncabezadoTableMenu
    {
        public string fecha { get; set; }
        public int columna { get; set; }
    }
}