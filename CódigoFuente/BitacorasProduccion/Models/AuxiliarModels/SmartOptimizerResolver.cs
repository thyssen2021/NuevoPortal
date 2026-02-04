using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Portal_2_0.Models.AuxiliarModels
{
    public class SmartOptimizerResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            // REGLA 1: MATAR EL PESO MUERTO (Archivos binarios)
            // Si la propiedad se llama "Data" y está dentro de "CTZ_Files", la ignoramos.
            // Así el archivo viaja ligero (solo nombre e ID), sin los 50MB de contenido.
            if (prop.PropertyName == "Data" && member.DeclaringType == typeof(CTZ_Files))
            {
                prop.ShouldSerialize = instance => false;
                prop.Ignored = true;
                return prop;
            }

            // REGLA 2: CORTAR EL CORDÓN UMBILICAL (Referencias al Padre)
            // Si estamos serializando un Material o cualquier hijo, y encontramos una propiedad
            // que apunta de regreso a "CTZ_Projects", la ignoramos para evitar bucles.
            if (prop.PropertyType == typeof(CTZ_Projects))
            {
                prop.ShouldSerialize = instance => false;
                prop.Ignored = true;
                return prop;
            }

            // REGLA 3 (Opcional): Si tienes otras tablas con binarios, agrégalas aquí.
            // Ejemplo: if (prop.PropertyType == typeof(byte[])) prop.Ignored = true;

            return prop;
        }
    }
}