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
    }

    public enum ActionKey
    {
        View,
        Edit,
        Approve,
        // …otros verbos…
    }
}