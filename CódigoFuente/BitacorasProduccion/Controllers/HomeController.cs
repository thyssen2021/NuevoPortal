using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]

    public class HomeController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        [HttpGet]
        public ActionResult Index()
        {
            #region Widgets Gastos de Viajes

            //Envia los datos para los widget
            if (TieneRol(TipoRoles.GV_SOLICITUD))
            {
                //anticipo
                List<EnlaceWidget> EnlacesMisSolicitudesAnticipoGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Creadas (sin enviar)", cantidad = GetMisSolicitudesAnticipoGVCount("CREADO"), Enlace="/GV_solicitud/Solicitudes?estatus=CREADO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "En proceso", cantidad = GetMisSolicitudesAnticipoGVCount("EN_PROCESO"), Enlace="/GV_solicitud/Solicitudes?estatus=EN_PROCESO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetMisSolicitudesAnticipoGVCount("RECHAZADAS"), Enlace="/GV_solicitud/Solicitudes?estatus=RECHAZADAS", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Por Confirmar", cantidad = GetMisSolicitudesAnticipoGVCount("POR_CONFIRMAR"), Enlace="/GV_solicitud/Solicitudes?estatus=POR_CONFIRMAR", tipo="warning"  },
                    new EnlaceWidget {Descripcion = "Finalizadas", cantidad = GetMisSolicitudesAnticipoGVCount("FINALIZADO"), Enlace="/GV_solicitud/Solicitudes?estatus=FINALIZADO", tipo="success"  },
                };

                ViewBag.EnlacesMisSolicitudesAnticipoGV = EnlacesMisSolicitudesAnticipoGV;

                //comprobacion
                List<EnlaceWidget> EnlacesMisSolicitudesComprobacionGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetMisSolicitudesComprobacionGVCount("PENDIENTES"), Enlace="/ComprobacionGV/Pendientes?estatus=PENDIENTES", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Iniciadas (sin enviar)", cantidad = GetMisSolicitudesComprobacionGVCount("CREADO"), Enlace="/ComprobacionGV/Pendientes?estatus=CREADO", tipo="warning"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetMisSolicitudesComprobacionGVCount("RECHAZADAS"), Enlace="/ComprobacionGV/Pendientes?estatus=RECHAZADAS", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "En proceso", cantidad = GetMisSolicitudesComprobacionGVCount("EN_PROCESO"), Enlace="/ComprobacionGV/Pendientes?estatus=EN_PROCESO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Finalizadas", cantidad = GetMisSolicitudesComprobacionGVCount("FINALIZADO"), Enlace="/ComprobacionGV/Pendientes?estatus=FINALIZADO", tipo="success"  },
                };

                ViewBag.EnlacesMisSolicitudesComprobacionGV = EnlacesMisSolicitudesComprobacionGV;
            }

            //Envia los datos para los widget
            if (TieneRol(TipoRoles.GV_JEFE_DIRECTO))
            {
                //anticipo
                List<EnlaceWidget> EnlacesSolicitudesJefeDirectoAnticipoGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesJefeDirectoAnticipoGVCount("ENVIADO_A_JEFE"), Enlace="/GV_solicitud/SolicitudesJefeDirecto?estatus=ENVIADO_A_JEFE", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesJefeDirectoAnticipoGVCount("RECHAZADO_JEFE"), Enlace="/GV_solicitud/SolicitudesJefeDirecto?estatus=RECHAZADO_JEFE", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Autorizadas", cantidad = GetSolicitudesJefeDirectoAnticipoGVCount("AUTORIZADAS_JEFE"), Enlace="/GV_solicitud/SolicitudesJefeDirecto?estatus=AUTORIZADAS_JEFE", tipo="success"  },
                         };

                ViewBag.EnlacesSolicitudesJefeDirectoAnticipoGV = EnlacesSolicitudesJefeDirectoAnticipoGV;

                //comprobacion
                List<EnlaceWidget> EnlacesSolicitudesJefeDirectoComprobacionGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesJefeDirectoComprobacionGVCount("PENDIENTES"), Enlace="/ComprobacionGV/SolicitudesJefeDirecto?estatus=PENDIENTES", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesJefeDirectoComprobacionGVCount("RECHAZADAS"), Enlace="/ComprobacionGV/SolicitudesJefeDirecto?estatus=RECHAZADAS", tipo="info"  },
                    new EnlaceWidget {Descripcion = "En proceso", cantidad = GetSolicitudesJefeDirectoComprobacionGVCount("EN_PROCESO"), Enlace="/ComprobacionGV/SolicitudesJefeDirecto?estatus=EN_PROCESO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Finalizadas", cantidad = GetSolicitudesJefeDirectoComprobacionGVCount("FINALIZADO"), Enlace="/ComprobacionGV/SolicitudesJefeDirecto?estatus=FINALIZADO", tipo="success"  },
                         };

                ViewBag.EnlacesSolicitudesJefeDirectoComprobacionGV = EnlacesSolicitudesJefeDirectoComprobacionGV;


            }

            //Envia los datos para los widget
            if (TieneRol(TipoRoles.GV_CONTROLLING))
            {
                //anticipo
                List<EnlaceWidget> EnlacesSolicitudesControllingAnticipoGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesControllingAnticipoGVCount("PENDIENTES_PROPIAS"), Enlace="/GV_solicitud/SolicitudesControlling?estatus=PENDIENTES_PROPIAS", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Pendientes Departamento", cantidad = GetSolicitudesControllingAnticipoGVCount("ENVIADO_CONTROLLING"), Enlace="/GV_solicitud/SolicitudesControlling?estatus=ENVIADO_CONTROLLING", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesControllingAnticipoGVCount("RECHAZADO_CONTROLLING"), Enlace="/GV_solicitud/SolicitudesControlling?estatus=RECHAZADO_CONTROLLING", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Autorizadas", cantidad = GetSolicitudesControllingAnticipoGVCount("AUTORIZADAS_CONTROLLING"), Enlace="/GV_solicitud/SolicitudesControlling?estatus=AUTORIZADAS_CONTROLLING", tipo="success"  },
                         };

                ViewBag.EnlacesSolicitudesControllingAnticipoGV = EnlacesSolicitudesControllingAnticipoGV;

                //comprobacion
                List<EnlaceWidget> EnlacesSolicitudesControllingComprobacionGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesControllingComprobacionGVCount("PENDIENTES"), Enlace="/ComprobacionGV/SolicitudesControlling?estatus=PENDIENTES", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "En proceso", cantidad = GetSolicitudesControllingComprobacionGVCount("EN_PROCESO"), Enlace="/ComprobacionGV/SolicitudesControlling?estatus=EN_PROCESO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesControllingComprobacionGVCount("RECHAZADAS"), Enlace="/ComprobacionGV/SolicitudesControlling?estatus=RECHAZADAS", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Finalizadas", cantidad = GetSolicitudesControllingComprobacionGVCount("FINALIZADO"), Enlace="/ComprobacionGV/SolicitudesControlling?estatus=FINALIZADO", tipo="success"  },
                         };

                ViewBag.EnlacesSolicitudesControllingComprobacionGV = EnlacesSolicitudesControllingComprobacionGV;

            }
            //Envia los datos para los widget
            if (TieneRol(TipoRoles.GV_NOMINA))
            {
                //anticipo
                List<EnlaceWidget> EnlacesSolicitudesNominaAnticipoGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesNominaAnticipoGVCount("PENDIENTES_PROPIAS"), Enlace="/GV_solicitud/SolicitudesNomina?estatus=PENDIENTES_PROPIAS", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Pendientes Departamento", cantidad = GetSolicitudesNominaAnticipoGVCount("PENDIENTES_GENERAL"), Enlace="/GV_solicitud/SolicitudesNomina?estatus=PENDIENTES_GENERAL", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesNominaAnticipoGVCount("RECHAZADO_NOMINA"), Enlace="/GV_solicitud/SolicitudesNomina?estatus=RECHAZADO_NOMINA", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Autorizadas", cantidad = GetSolicitudesNominaAnticipoGVCount("AUTORIZADAS_NOMINA"), Enlace="/GV_solicitud/SolicitudesNomina?estatus=AUTORIZADAS_NOMINA", tipo="info"  },
                         };

                ViewBag.EnlacesSolicitudesNominaAnticipoGV = EnlacesSolicitudesNominaAnticipoGV;
                
                //comprobacion
                List<EnlaceWidget> EnlacesSolicitudesNominaComprobacionGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesNominaComprobacionGVCount("PENDIENTES"), Enlace="/ComprobacionGV/SolicitudesNomina?estatus=PENDIENTES", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesNominaComprobacionGVCount("RECHAZADAS"), Enlace="/ComprobacionGV/SolicitudesNomina?estatus=RECHAZADAS", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Finalizadas", cantidad = GetSolicitudesNominaComprobacionGVCount("FINALIZADO"), Enlace="/ComprobacionGV/SolicitudesNomina?estatus=FINALIZADO", tipo="success"  },
                         };

                ViewBag.EnlacesSolicitudesNominaComprobacionGV = EnlacesSolicitudesNominaComprobacionGV;
            
            }
            //Envia los datos para los widget
            if (TieneRol(TipoRoles.GV_CONTABILIDAD))
            {
                //anticipo
                List<EnlaceWidget> EnlacesSolicitudesContabilidadAnticipoGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesContabilidadAnticipoGVCount("PENDIENTES_PROPIAS"), Enlace="/GV_solicitud/SolicitudesContabilidad?estatus=PENDIENTES_PROPIAS", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Pendientes Departamento", cantidad = GetSolicitudesContabilidadAnticipoGVCount("PENDIENTES_GENERAL"), Enlace="/GV_solicitud/SolicitudesContabilidad?estatus=PENDIENTES_GENERAL", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesContabilidadAnticipoGVCount("RECHAZADO_CONTABILIDAD"), Enlace="/GV_solicitud/SolicitudesContabilidad?estatus=RECHAZADO_CONTABILIDAD", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Confirmadas", cantidad = GetSolicitudesContabilidadAnticipoGVCount("CONFIRMADAS"), Enlace="/GV_solicitud/SolicitudesContabilidad?estatus=CONFIRMADAS", tipo="info"  },
                };

                ViewBag.EnlacesSolicitudesContabilidadAnticipoGV = EnlacesSolicitudesContabilidadAnticipoGV;

               //comprobacion
                List<EnlaceWidget> EnlacesSolicitudesContabilidadComprobacionGV = new List<EnlaceWidget>
                {
                    new EnlaceWidget {Descripcion = "Pendientes", cantidad = GetSolicitudesContabilidadComprobacionGVCount("PENDIENTES"), Enlace="/ComprobacionGV/SolicitudesContabilidad?estatus=PENDIENTES", tipo="danger"  },
                    new EnlaceWidget {Descripcion = "En proceso", cantidad = GetSolicitudesContabilidadComprobacionGVCount("EN_PROCESO"), Enlace="/ComprobacionGV/SolicitudesContabilidad?estatus=EN_PROCESO", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Rechazadas", cantidad = GetSolicitudesContabilidadComprobacionGVCount("RECHAZADAS"), Enlace="/ComprobacionGV/SolicitudesContabilidad?estatus=RECHAZADAS", tipo="info"  },
                    new EnlaceWidget {Descripcion = "Finalizado", cantidad = GetSolicitudesContabilidadComprobacionGVCount("FINALIZADO"), Enlace="/ComprobacionGV/SolicitudesContabilidad?estatus=FINALIZADO", tipo="success"  },
                };

                ViewBag.EnlacesSolicitudesContabilidadComprobacionGV = EnlacesSolicitudesContabilidadComprobacionGV;
            }

            #endregion

            return View();
        }

        [AllowAnonymous]
        public ActionResult ErrorPermisos()
        {
            return View();
        }

        #region widget GV

        [NonAction]
        public int GetMisSolicitudesAnticipoGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_solicitud
                .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                  && (x.estatus == estatus || estatus == "ALL"
                                  || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                  || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                  || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                  || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                  || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                   || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                  ))
                                  || (estatus == "POR_CONFIRMAR" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                  || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                  ) && !x.fecha_confirmacion_usuario.HasValue)
                                  || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                  || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                  || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                  )
              )
              .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesControllingAnticipoGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_solicitud
                       .Where(x => ((x.id_controlling.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_controlling == empleado.id))
                     && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                     (estatus == "PENDIENTES_PROPIAS" && x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING) ||
                     (estatus == "AUTORIZADAS_CONTROLLING" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                     || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                     || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                     || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                      || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                     || x.estatus == GV_solicitud_estatus.FINALIZADO))
                     )
                 )
                 .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesNominaAnticipoGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_solicitud
                 .Where(x => ((x.id_nomina.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_nomina == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "AUTORIZADAS_NOMINA" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesContabilidadAnticipoGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_solicitud
                   .Where(x => ((x.id_contabilidad.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_contabilidad == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "CONFIRMADAS" && (x.estatus == GV_solicitud_estatus.FINALIZADO || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD))
                    )
                )
                .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesJefeDirectoAnticipoGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_solicitud
           .Where(x => (x.id_jefe_directo == empleado.id)
              && (x.estatus == estatus || (estatus == "ALL" && x.estatus != GV_solicitud_estatus.CREADO) ||
              (estatus == "AUTORIZADAS_JEFE" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                              || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                              || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                               || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                              || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                              || x.estatus == GV_solicitud_estatus.FINALIZADO))
              )
          )
          .Count();

            return totalDeRegistros;
        }

        [NonAction]
        public int GetMisSolicitudesComprobacionGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            if (estatus == "PENDIENTES")
                //en caso de pendientes la consulta cambia
                totalDeRegistros = db.GV_solicitud
                    .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                     && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                 )
                 .Count();
            else
                totalDeRegistros = db.GV_comprobacion
                  .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                   && (
                   x.estatus == estatus ||
                   (estatus == GV_comprobacion_estatus.CREADO && (x.estatus == GV_comprobacion_estatus.CREADO
                                                   ))
                                   || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                   || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                   || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                   || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                   ))
                                   || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                   || x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                   || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                   || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                   )
               )
               .Count();

            return totalDeRegistros;
        }

        [NonAction]
        public int GetSolicitudesJefeDirectoComprobacionGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_comprobacion
                       .Where(x => (x.id_jefe_directo == empleado.id)
                               && (
                                x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD
                                                               ))
                                                               )
                           )
                .Count();

            return totalDeRegistros;
        }

        [NonAction]
        public int GetSolicitudesControllingComprobacionGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_comprobacion
                                 .Where(x => (x.id_controlling == empleado.id || x.estatus == GV_comprobacion_estatus.FINALIZADO)
                                 && (
                                 x.estatus == estatus ||
                                 (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                                 ))
                                                 || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                                 || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                                 || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                                 || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                                 ))
                                                 || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                                 || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                                 )
                             )
                  .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesNominaComprobacionGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_comprobacion
                    .Where(x => (x.id_nomina == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                                               //|| (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               //                //|| x.estatus == GV_comprobacion_estatus.ENVIADO_Nomina)
                                                               //                )
                                                               //                )
                                                               )
                           )
                .Count();

            return totalDeRegistros;
        }
        [NonAction]
        public int GetSolicitudesContabilidadComprobacionGVCount(string estatus)
        {
            int totalDeRegistros = 0;

            empleados empleado = obtieneEmpleadoLogeado();

            totalDeRegistros = db.GV_comprobacion
                    .Where(x => (x.id_contabilidad == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               // || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD
                                                               )
                                                               )
                                                               )
                           )
                 .Count();

            return totalDeRegistros;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
