using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bitacoras.Util
{
    static public class XmlUtil
    {

        static public Comprobante LeeXML_3_3(Stream stream)
        {

            //crear un objeto el cual tendrá el resultado final, este objeto es el principal
            Comprobante oComprobante;
            //pon la ruta donde tienes tu archivo XML Timbrado
            //string path = @"C:\miXML.xml";

            //creamos un objeto XMLSerializer para deserializar
            XmlSerializer oSerializer = new XmlSerializer(typeof(Comprobante));

            //creamos un flujo el cual recibe nuestro xml
            using (StreamReader reader = new StreamReader(stream))
            {      
                //aqui deserializamos
                oComprobante = (Comprobante)oSerializer.Deserialize(reader);              

                //Deserializamos el complemento timbre fiscal
                foreach (var oComplemento in oComprobante.Complemento)
                {
                    foreach (var oComplementoInterior in oComplemento.Any)
                    {
                        //si el complemento es TimbreFiscalDigital lo deserializamos
                        if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                        {

                            //Objeto para aplicar ahora la deserialización del complemento timbre
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                            //creamos otro flujo para el complemento
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                //y por ultimo deserializamos el complemento
                                oComprobante.TimbreFiscalDigital =
                                    (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }
                    }
                }
            }

            return oComprobante;
        }
        static public  Bitacoras.CFDI_4_0.Comprobante LeeXML_4_0(Stream stream)
        {

            //crear un objeto el cual tendrá el resultado final, este objeto es el principal
            Bitacoras.CFDI_4_0.Comprobante oComprobante;
            //pon la ruta donde tienes tu archivo XML Timbrado
            //string path = @"C:\miXML.xml";

            //creamos un objeto XMLSerializer para deserializar
            XmlSerializer oSerializer = new XmlSerializer(typeof(Bitacoras.CFDI_4_0.Comprobante));

            //creamos un flujo el cual recibe nuestro xml
            using (StreamReader reader = new StreamReader(stream))
            {
                //aqui deserializamos
                oComprobante = (Bitacoras.CFDI_4_0.Comprobante)oSerializer.Deserialize(reader);

                //Deserializamos el complemento timbre fiscal
                foreach (var oComplemento in oComprobante.Complemento)
                {
                    foreach (var oComplementoInterior in oComplemento.Any)
                    {
                        //si el complemento es TimbreFiscalDigital lo deserializamos
                        if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                        {

                            //Objeto para aplicar ahora la deserialización del complemento timbre
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                            //creamos otro flujo para el complemento
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                //y por ultimo deserializamos el complemento
                                oComprobante.TimbreFiscalDigital =
                                    (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }
                    }
                }
            }

            return oComprobante;
        }
    }
}
