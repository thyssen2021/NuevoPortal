using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class AuthorizationService
    {
        private readonly Portal_2_0Entities _db;
        public AuthorizationService(Portal_2_0Entities db)
            => _db = db;


        // Nuevo overload: Convierte los enum a string
        public bool CanPerform(int employeeId,
                               ResourceKey resource,
                               ActionKey action,
                               IDictionary<string, object> context = null)
        {
            // Llama a tu método existente, pasando .ToString()
            return CanPerform(
                employeeId,
                resource.ToString(),
                action.ToString(),
                context
            );
        }

        /// <summary>
        /// Comprueba si <employeeId> puede ejecutar <actionKey> sobre <resourceKey>,
        /// evaluando también las condiciones contextuales.
        /// </summary>
        public bool CanPerform(int employeeId, string resourceKey, string actionKey,
                               IDictionary<string, object> context = null)
        {
            // 1) obtengo recurso y acción
            var resource = _db.CTZ_Resources
                              .FirstOrDefault(r => r.ResourceKey == resourceKey);
            var action = _db.CTZ_Actions
                              .FirstOrDefault(a => a.ActionKey == actionKey);
            if (resource == null || action == null)
                return false;

            // 2) cargo el empleado y su colección de Roles usando Include("CTZ_Roles")
            var user = _db.empleados
                          .Include("CTZ_Roles")                // <–– aquí
                          .FirstOrDefault(e => e.id == employeeId);
            if (user == null)
                return false;

            // 3) extraigo los IDs de rol
            var roleIds = user.CTZ_Roles
                              .Select(r => r.ID_Role)
                              .ToList();
            if (!roleIds.Any())
                return false;

            // 4) filtro los roles que tienen permiso base (CanDo = 1)
            var basePermRoles = _db.CTZ_Role_Permissions
                .Where(rp => roleIds.Contains(rp.ID_Role)
                          && rp.ID_Resource == resource.ID_Resource
                          && rp.ID_Action == action.ID_Action
                          && rp.CanDo)
                .Select(rp => rp.ID_Role)
                .ToList();
            if (!basePermRoles.Any())
                return false;

            // 5) obtengo las condiciones asociadas a esos roles/recursos/acciones
            var condKeys = _db.CTZ_Role_Permission_Conditions
                .Where(rpc => basePermRoles.Contains(rpc.ID_Role)
                           && rpc.ID_Resource == resource.ID_Resource
                           && rpc.ID_Action == action.ID_Action)
                .Select(rpc => rpc.CTZ_Conditions.ConditionKey)
                .ToList();

            // 6) evalúo cada condición; si alguna falla, niego el permiso
            foreach (var condKey in condKeys)
                if (!EvaluateCondition(condKey, employeeId, context))
                    return false;

            // 7) si pasó TODO, apruebo
            return true;
        }

        // Evaluador de condiciones custom
        private bool EvaluateCondition(string conditionKey, int employeeId,
                                       IDictionary<string, object> context)
        {
            switch (conditionKey)
            {
                case "AssignedOrNeverSent":
                    // Si no hubo ningún contexto, o ProjectId viene null, permitimos la acción
                    if (context == null || !context.ContainsKey("ProjectId") || context["ProjectId"] == null)
                    {
                        return true;
                    }

                    int projId = (int)context["ProjectId"];

                    // Obtener departamentos y plantas del usuario
                    var userDeptIds = _db.CTZ_Employee_Departments
                        .Where(ed => ed.ID_Employee == employeeId)
                        .Select(ed => ed.ID_Department);

                    var userPlantIds = _db.CTZ_Employee_Plants
                        .Where(ep => ep.ID_Employee == employeeId)
                        .Select(ep => ep.ID_Plant);

                    // 1) ¿Está asignado el proyecto a alguno de sus deptos/planta?
                    bool isAssigned = _db.CTZ_Project_Assignment
                        .Any(a =>
                            a.ID_Project == projId
                         && a.Completition_Date == null
                         && userDeptIds.Contains(a.ID_Department)
                         && userPlantIds.Contains(a.ID_Plant)
                        );

                    // 2) ¿Nunca se envió Y fue creado por este usuario?
                    bool neverSentAndCreator =
                        !_db.CTZ_Project_Assignment.Any(a => a.ID_Project == projId)
                     && _db.CTZ_Projects.Any(p =>
                            p.ID_Project == projId &&
                            p.ID_Created_By == employeeId
                        );

                    return isAssigned || neverSentAndCreator;

                //case "AssignedToProject":
                //    if (context == null || !context.ContainsKey("ProjectId"))
                //        return false;
                //    int projId = (int)context["ProjectId"];
                //    return _db.CTZ_Project_Assignment
                //              .Any(a => a.ID_Project == projId
                //                     && a.ID_Employee == employeeId
                //                     && a.Completition_Date == null);
                //case "HasPlantAccess":
                //    if (context == null || !context.ContainsKey("PlantId"))
                //        return false;
                //    int plantId = (int)context["PlantId"];
                //    // comprueba en CTZ_Employee_Plants
                //    return _db.CTZ_Employee_Plants
                //              .Any(ep => ep.ID_Employee == employeeId
                //                      && ep.ID_Plant == plantId);
                // añade más condiciones si las necesitas…
                default:
                    return false;
            }
        }
    }
}