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
                // 1) Verificar si ya existe una asignación PENDIENTE para este proyecto/departamento/planta
                bool exists = db.CTZ_Project_Assignment
                    .Any(a =>
                        a.ID_Project == projectId &&
                        a.ID_Department == (int)department &&
                        a.ID_Plant == plantId &&
                        a.Completition_Date == null   // pendiente = sin fecha de completión
                    );
                if (exists)
                    return; // no duplicamos

                // 2) Preparar la fecha (si no se pasa, usar ahora)
                DateTime assignDate = assignmentDate ?? DateTime.Now;

                // 3) Crear la nueva asignación
                var newAssign = new CTZ_Project_Assignment
                {
                    ID_Project = projectId,
                    ID_Department = (int)department,
                    ID_Employee = null,
                    ID_Plant = plantId,
                    ID_Assignment_Status = (int)AssignmentStatusEnum.PENDING,
                    Assignment_Date = assignDate,
                    Comments = string.Empty
                };
                db.CTZ_Project_Assignment.Add(newAssign);
                db.SaveChanges(); // para obtener newAssign.ID_Assignment

                // 4) Buscar la **última** asignación previa a ésta (por fecha)
                var lastAssign = db.CTZ_Project_Assignment
                    .Where(a =>
                        a.ID_Project == projectId &&
                        a.ID_Department == (int)department &&
                        a.ID_Assignment != newAssign.ID_Assignment
                    )
                    .OrderByDescending(a => a.Assignment_Date)
                    .ThenByDescending(a => a.ID_Assignment)
                    .FirstOrDefault();

                if (lastAssign != null)
                {
                    // 5) Obtener todas las actividades completadas en esa asignación
                    var completedActs = db.CTZ_Assignment_Activity
                        .Where(x => x.ID_Assignment == lastAssign.ID_Assignment && x.IsComplete)
                        .ToList();

                    // 6) Para cada una, clonar el registro a la nueva asignación
                    foreach (var act in completedActs)
                    {
                        db.CTZ_Assignment_Activity.Add(new CTZ_Assignment_Activity
                        {
                            ID_Assignment = newAssign.ID_Assignment,
                            ID_Activity = act.ID_Activity,
                            IsComplete = act.IsComplete,
                        });
                    }
                    db.SaveChanges();
                }
            }
        }


    }
}