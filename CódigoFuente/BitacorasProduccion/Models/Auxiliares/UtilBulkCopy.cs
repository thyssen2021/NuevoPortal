using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Portal_2_0.Models
{
    // 1. Hacer la clase 'public' y 'static'
    public static class UtilBulkCopy
    {
        /// <summary>
        /// Método helper genérico para convertir una List<T> en un DataTable.
        /// Usa Reflection para crear las columnas basado en las propiedades de la clase.
        /// </summary>
        // 2. Hacer el método 'public' y 'static'
        public static System.Data.DataTable ToDataTable<T>(List<T> items)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();

            // Usamos BindingFlags.DeclaredOnly para ignorar propiedades heredadas
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            //Crear las Columnas en el DataTable
            foreach (PropertyInfo prop in Props)
            {
                //Manejar tipos que aceptan nulos (Nullable<T>)
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType);

                dataTable.Columns.Add(prop.Name, type);
            }

            //Agregar las Filas al DataTable
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //Insertar el valor de la propiedad en el array de valores
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}