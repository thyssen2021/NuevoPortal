using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentitySample.Models;

namespace Portal_2_0.Models
{
    public static class AssignmentService
    {
        /// <summary>
        /// Crea una única asignación para un proyecto, departamento y planta dados.
        /// </summary>
        /// <param name="projectId">ID del proyecto a asignar</param>
        /// <param name="department">Departamento (enum)</param>
        /// <param name="plantId">ID de planta</param>
        /// <param name="assignmentDate">
        /// Fecha de asignación; si es null, se usará DateTime.Now.
        /// </param>
        public static void AssignProjectToDepartment(
            int projectId,
            DepartmentEnum department,
            int plantId,
            DateTime? assignmentDate = null)    // parámetro opcional
        {
            using (var db = new Portal_2_0Entities())
            {
                // 1) Verificar si ya existe una asignación para este proyecto/departamento/planta
                bool exists = db.CTZ_Project_Assignment
                    .Any(a =>
                        a.ID_Project == projectId &&
                        a.ID_Department == (int)department &&
                        a.ID_Plant == plantId
                    );

                if (exists)
                {
                    // Ya existe, no crear duplicado
                    return;
                }

                // 2) Preparar la fecha (si no se pasa, usar ahora)
                DateTime assignDate = assignmentDate ?? DateTime.Now;

                // 3) Crear la asignación con ID_Employee = null (aún no se ha tomado acción)
                var assignment = new CTZ_Project_Assignment
                {
                    ID_Project = projectId,
                    ID_Department = (int)department,
                    ID_Employee = null,
                    ID_Plant = plantId,
                    ID_Assignment_Status = (int)AssignmentStatusEnum.PENDING,
                    Assignment_Date = assignDate,      // uso de la fecha opcional
                    Comments = string.Empty
                };

                db.CTZ_Project_Assignment.Add(assignment);

                // 4) Guardar en la base de datos
                db.SaveChanges();
            }
        }

    }
}