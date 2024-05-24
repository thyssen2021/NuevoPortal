using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thought.vCards;
using QRCoder;


namespace Portal_2_0.Controllers
{
    [AllowAnonymous]
    public class UtilidadesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Utilidades
        public ActionResult vcard()
        {
            vcard model = new vcard { 
                empresa = "thyssenkrupp Materials de México (tkmm)",
                website = "https://www.thyssenkrupp-materials.mx/",
                planta_pais = "México"
            };

            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                              "id",
                                                              "ConcatNumEmpleadoNombre"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult vcard(vcard vcard_model)
        {

            
            // Generate vCard
            vCard vCard = new vCard();
            vCard.GivenName = vcard_model.nombre;
            vCard.FamilyName = vcard_model.apellidos;

            vCard.Organization = vcard_model.empresa;
            vCard.Title = vcard_model.puesto;

            if(!string.IsNullOrEmpty(vcard_model.phone_1))
                vCard.Phones.Add(new vCardPhone(vcard_model.phone_1, vCardPhoneTypes.Work));
            
            if(!string.IsNullOrEmpty(vcard_model.phone_2))
                vCard.Phones.Add(new vCardPhone(vcard_model.phone_2, vCardPhoneTypes.Cellular));

            vCard.EmailAddresses.Add(new vCardEmailAddress(vcard_model.email, vCardEmailAddressType.Internet, ItemType.WORK));

            vCardDeliveryAddress address = new vCardDeliveryAddress();
            address.AddressType = new List<vCardDeliveryAddressTypes> { vCardDeliveryAddressTypes.Work };
            address.Street = vcard_model.planta_calle;
            address.City = vcard_model.planta_ciudad;
            address.Region = vcard_model.planta_estado;
            address.PostalCode = vcard_model.planta_codigo_postal;
            address.Country = vcard_model.planta_pais;           
            vCard.DeliveryAddresses.Add(address);

            //sitio web
            vCardWebsiteCollection vCardWebsite = new vCardWebsiteCollection();
            vCardWebsite website = new vCardWebsite();
            website.WebsiteType = vCardWebsiteTypes.Work;
            website.IsWorkSite = true;
            website.Url = vcard_model.website;
            vCard.Websites.Add(website);

            // Save vCard data to string
            vCardStandardWriter writer = new vCardStandardWriter();
            StringWriter stringWriter = new StringWriter();
            writer.Write(vCard, stringWriter);

            vcard_model.qrCodeText = stringWriter.ToString();

            //genera el codigo QR
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(vcard_model.qrCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);

            //obtiene el icono
           
            Bitmap im = new Bitmap (vcard_model.icoPath);

            Bitmap QrBitmap = QrCode.GetGraphic(15, Color.FromArgb(0,159,245), Color.White, icon: im);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            vcard_model.qrURI = QrUri;


            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)));
            ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                              nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
                        
            ViewBag.MensajeAlert = new MensajesSweetAlert("Se generó el código QR correctamente", TipoMensajesSweetAlerts.SUCCESS);

            return View(vcard_model);


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }

    //Extension method to convert Bitmap to Byte Array
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}