using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public enum ResourceKey
    {
        EditClientPartInformationView,
        EditClientPartInformationSalesSection,
        ManagePermissions,
        EditClientPartInformationEngineeringSection,
        EditClientPartInformationDataManagementSection,
        UpsertQuotes,
        CatalogsForeignTrade,
        CatalogsDataManagement,
        CatalogsEngineering,
    }

    public enum ActionKey
    {
        View,
        Edit,
        Approve,
        // …otros verbos…
    }

    /// <summary>
    /// Ids para la tabla de CTZ_Departments
    /// </summary>
    public enum DepartmentEnum
    {
        Sales = 1,    
        DataManagement = 2,    
        Engineering = 3,
        ForeignTrade = 4,
        Disposition = 5,
    }

    /// <summary>
    /// Ids deben coincidir con CTZ_Assignment_Status.[ID_Assignment_Status]
    /// </summary>
    public enum AssignmentStatusEnum
    {
        PENDING = 1,    // “Pending”
        IN_PROGRESS = 2,
        ON_HOLD = 3,
        REJECTED = 4,
        ON_REVIEWED = 5,
        APPROVED = 6,   
    }
    
    public enum CTZ_RolesEnum
    {
        ADMIN = 1,    // “Pending”
        SALES = 2,
        DATA_MANAGEMENT = 3,
        ENGINEERING = 4,
        FOREING_TRADE = 5,
        DISPOSITION = 6,   
    }
    public enum ProjectAssignmentStatus
    {
        Created,    // aún no hay ninguna asignación
        InProcess,  // hay asignaciones pendientes o en curso
        OnHold,     // al menos una está “ON_HOLD”
        OnReview,   // al menos una está “ON_REVIEWED”
        Rejected,   // al menos una está “REJECTED”
        Finalized,   // todas las asignaciones están cerradas y aprobadas
        ClosedWithoutApproval
    }


    public enum ActionTypeEnum : byte
    {
        HoldOthers = 1,   // reject current + hold all other assignments
        KeepActive = 2,   // reject current, leave others untouched
        FinalizeAll = 3    // reject current + put all active to hold and end flow
    }
}