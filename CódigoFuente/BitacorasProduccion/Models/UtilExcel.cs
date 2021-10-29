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
    }
}