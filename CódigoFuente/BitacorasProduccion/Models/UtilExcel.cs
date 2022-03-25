using Bitacoras.Util;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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
                    if (table.TableName.ToUpper() == "SHEET1")
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
                        if (!encabezados.Contains("MATERIAL") || !encabezados.Contains("PLNT") || !encabezados.Contains("BOM")
                            || !encabezados.Contains("ALTBOM") || !encabezados.Contains("ITEM") || !encabezados.Contains("COMPONENT"))
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
                                string Created_by = String.Empty;
                                string BOM1 = String.Empty;
                                string Node = String.Empty;
                                Nullable<double> Quantity = null;
                                string Un = string.Empty;
                                Nullable<System.DateTime> Created = null;

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
                                        case "BOM":
                                            BOM = table.Rows[i][j].ToString();
                                            break;
                                        case "ALTBOM":
                                            AltBOM = table.Rows[i][j].ToString();
                                            break;
                                        case "ITEM":
                                            Item = table.Rows[i][j].ToString();
                                            break;
                                        case "COMPONENT":
                                            Component = table.Rows[i][j].ToString();
                                            break;
                                        case "CREATED BY":
                                            Created_by = table.Rows[i][j].ToString();
                                            break;
                                        case "NODE":
                                            Node = table.Rows[i][j].ToString();
                                            break;
                                        case "QUANTITY":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double q))
                                                Quantity = q;
                                            break;
                                        case "UN":
                                            Un = table.Rows[i][j].ToString();
                                            break;
                                        case "CREATED":
                                            if (!String.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                                Created = Convert.ToDateTime(table.Rows[i][j].ToString());
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
                                    Created_by = Created_by,
                                    BOM1 = BOM,
                                    Node = Node,
                                    Un = Un,
                                    Quantity = Quantity,
                                    Created = Created,
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
                    if (table.TableName.ToUpper() == "SHEET1")
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
                    if (table.TableName.ToUpper() == "SHEET1")
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
                                Nullable<double> Gross_weight = null;
                                string Un_ = String.Empty;
                                Nullable<double> Net_weight = null;
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
                                        case "GROSS WEIGHT":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double gross))
                                                Gross_weight = gross;
                                            break;
                                        case "UN.":
                                            Un_ = table.Rows[i][j].ToString();
                                            break;
                                        case "NET WEIGHT":
                                            if (Double.TryParse(table.Rows[i][j].ToString(), out double net))
                                                Net_weight = net;
                                            break;
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
                                    Gross_weight = Gross_weight,
                                    Un_ = Un_,
                                    Net_weight = Net_weight,
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
       /// <param name="centro"></param>
       /// <param name="valido"></param>
       /// <param name="msjError"></param>
       /// <param name="noEncontrados"></param>
       /// <returns></returns>
        public static List<budget_cantidad> LeeActual(budget_centro_costo centro, ref bool valido, ref string msjError, ref int noEncontrados)
        {

            //obtiene todas las cuentas
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cantidad> lista = new List<budget_cantidad>();

            //crea el reader del archivo
            using (var reader = ExcelReaderFactory.CreateReader(centro.PostedFile.InputStream))
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
                    if (table.TableName.ToUpper() == "TEMPLATE")
                        existeHoja = true;
                }

                if (!existeHoja)
                {
                    msjError = "En el archivo seleccionado no existe una la hoja llamada 'Template'.";
                    return lista;
                }

                foreach (DataTable table in result.Tables)
                {
                    //busca si existe una hoja llamada "Template"
                    if (table.TableName.ToUpper() == "TEMPLATE")
                    {
                        valido = true;

                        //verifica si tiene centro de costo
                        costCenter = table.Rows[4][2].ToString();
                        if (String.IsNullOrEmpty(costCenter))
                        {
                            msjError = "No ingresó centro de costo en la celda 'C5'.";
                            valido = false;
                            return lista;
                        }

                        //verifica si el centro de costo es el mismo que el archivo excel
                        if (costCenter != centro.num_centro_costo)
                        {
                            msjError = "El centro de costo del archivo no coincide.";
                            valido = false;
                            return lista;
                        }


                        //se obtienen las cabeceras
                        List<string> encabezados = new List<string>();

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string title = table.Rows[9][i].ToString();

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
                                budget_rel_fy_centro rel_Fy_Centro = db.budget_rel_fy_centro.Where(x => x.id_anio_fiscal == fy.id && x.id_centro_costo == centro.id).FirstOrDefault();

                                ListObjectEncabezados.Add(new EncabezadoExcelBudget
                                {
                                    texto = encabezados[j],
                                    fecha = fecha,
                                    anio_Fiscal = fy,
                                    rel_fy = rel_Fy_Centro

                                });
                            }
                           

                        }

                        //verifica que la estrura del archivo sea válida
                        if (ListObjectEncabezados.Count!=36)
                        {
                            msjError = "La plantilla no cuenta con todos los meses.";
                            valido = false;
                            return lista;
                        }


                        //Se consultan las cuentas sap
                        List<budget_cuenta_sap> listCuentas = db.budget_cuenta_sap.ToList();


                        //la fila cero se omite (encabezado)
                        for (int i = 10; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                //variables
                                string sap_account = table.Rows[i][1].ToString();

                                //obtiene la cuenta
                                budget_cuenta_sap cuenta = listCuentas.Where(x => x.sap_account == sap_account).FirstOrDefault();

                                //recorre todas los encabezados
                                for (int j = 0; j < encabezados.Count; j++)
                                {                                  

                                    if (ListObjectEncabezados.Any(x=>x.texto == encabezados[j]))
                                    {
                                        decimal cantidad = 0;
                                        Decimal.TryParse(table.Rows[i][j].ToString(), out cantidad);
                                        cantidad = Decimal.Round(cantidad, 2);

                                        //obtiene los datos del encabezado
                                        EncabezadoExcelBudget encabezado = ListObjectEncabezados.Where(x => x.texto == encabezados[j]).FirstOrDefault();

                                        if (cuenta != null && encabezado.rel_fy != null)
                                        {
                                            lista.Add(new budget_cantidad()
                                            {
                                                id_budget_rel_fy_centro = encabezado.rel_fy.id,
                                                id_cuenta_sap = cuenta.id,
                                                mes = encabezado.fecha.Month,
                                                currency_iso = "USD",
                                                cantidad = cantidad
                                            });
                                        }
                                        else if (!string.IsNullOrEmpty(sap_account))
                                        {
                                            noEncontrados++;
                                        }
                                       
                                    }                                   

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
    }
}