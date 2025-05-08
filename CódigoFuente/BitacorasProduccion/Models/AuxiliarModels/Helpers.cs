using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public static class VersionService
    {
        public static CTZ_Projects_Versions CreateInitialVersion(
            int projectId, int userId, int statusId, DateTime now)
        {
            using (var db = new Portal_2_0Entities())
            {
                var versions = db.CTZ_Projects_Versions
                                 .Where(v => v.ID_Project == projectId);
                foreach (var v in versions.Where(v => v.Is_Current == true))
                {
                    v.Is_Current = false;
                }

                var ver = new CTZ_Projects_Versions
                {
                    ID_Project = projectId,
                    ID_Created_by = userId,
                    Version_Number = "1.0",
                    Creation_Date = now,
                    Is_Current = true,
                    Comments = "Initial version",
                    ID_Status_Project = statusId
                };
                db.CTZ_Projects_Versions.Add(ver);
                db.SaveChanges();
                return ver;
            }
        }
    }

    public static class HistoryHelper
    {
        public static void CopyMaterialsToHistory(int projectId, int versionId)
        {
            using (var db = new Portal_2_0Entities())
            {
                var materials = db.CTZ_Project_Materials
                                  .Where(m => m.ID_Project == projectId)
                                  .ToList();

                foreach (var m in materials)
                {
                    var hist = new CTZ_Project_Materials_History
                    {
                        //Campos de control
                        ID_Version = versionId,
                        //ID_Project_Material = m.ID_Material,

                        // Campos originales copiados
                        ID_IHS_Item = m.ID_IHS_Item,
                        Max_Production_SP = m.Max_Production_SP,
                        Program_SP = m.Program_SP,
                        Vehicle_version = m.Vehicle_version,
                        SOP_SP = m.SOP_SP,
                        EOP_SP = m.EOP_SP,
                        Real_SOP = m.Real_SOP,
                        Real_EOP = m.Real_EOP,
                        Ship_To = m.Ship_To,
                        Part_Name = m.Part_Name,
                        Part_Number = m.Part_Number,
                        ID_Route = m.ID_Route,
                        Quality = m.Quality,
                        Tensile_Strenght = m.Tensile_Strenght,
                        ID_Material_type = m.ID_Material_type,
                        Thickness = m.Thickness,
                        Width = m.Width,
                        Pitch = m.Pitch,
                        Theoretical_Gross_Weight = m.Theoretical_Gross_Weight,
                        Gross_Weight = m.Gross_Weight,
                        Annual_Volume = m.Annual_Volume,
                        Volume_Per_year = m.Volume_Per_year,
                        ID_Shape = m.ID_Shape,
                        Angle_A = m.Angle_A,
                        Angle_B = m.Angle_B,
                        Blanks_Per_Stroke = m.Blanks_Per_Stroke,
                        Parts_Per_Vehicle = m.Parts_Per_Vehicle,
                        ID_Theoretical_Blanking_Line = m.ID_Theoretical_Blanking_Line,
                        ID_Real_Blanking_Line = m.ID_Real_Blanking_Line,
                        Theoretical_Strokes = m.Theoretical_Strokes,
                        Real_Strokes = m.Real_Strokes,
                        Ideal_Cycle_Time_Per_Tool = m.Ideal_Cycle_Time_Per_Tool,
                        OEE = m.OEE,
                        ID_Project = m.ID_Project,
                        Vehicle = m.Vehicle,
                        Vehicle_2 = m.Vehicle_2,
                        Vehicle_3 = m.Vehicle_3,
                        Vehicle_4 = m.Vehicle_4,
                        ThicknessToleranceNegative = m.ThicknessToleranceNegative,
                        ThicknessTolerancePositive = m.ThicknessTolerancePositive,
                        WidthToleranceNegative = m.WidthToleranceNegative,
                        WidthTolerancePositive = m.WidthTolerancePositive,
                        PitchToleranceNegative = m.PitchToleranceNegative,
                        PitchTolerancePositive = m.PitchTolerancePositive,
                        WeightOfFinalMults = m.WeightOfFinalMults,
                        Multipliers = m.Multipliers,
                        AngleAToleranceNegative = m.AngleAToleranceNegative,
                        AngleATolerancePositive = m.AngleATolerancePositive,
                        AngleBToleranceNegative = m.AngleBToleranceNegative,
                        AngleBTolerancePositive = m.AngleBTolerancePositive,
                        MajorBase = m.MajorBase,
                        MajorBaseToleranceNegative = m.MajorBaseToleranceNegative,
                        MajorBaseTolerancePositive = m.MajorBaseTolerancePositive,
                        MinorBase = m.MinorBase,
                        MinorBaseToleranceNegative = m.MinorBaseToleranceNegative,
                        MinorBaseTolerancePositive = m.MinorBaseTolerancePositive,
                        Flatness = m.Flatness,
                        FlatnessToleranceNegative = m.FlatnessToleranceNegative,
                        FlatnessTolerancePositive = m.FlatnessTolerancePositive,
                        MasterCoilWeight = m.MasterCoilWeight,
                        InnerCoilDiameterArrival = m.InnerCoilDiameterArrival,
                        OuterCoilDiameterArrival = m.OuterCoilDiameterArrival,
                        InnerCoilDiameterDelivery = m.InnerCoilDiameterDelivery,
                        OuterCoilDiameterDelivery = m.OuterCoilDiameterDelivery,
                        PackagingStandard = m.PackagingStandard,
                        SpecialRequirement = m.SpecialRequirement,
                        SpecialPackaging = m.SpecialPackaging,
                        ID_File_CAD_Drawing = m.ID_File_CAD_Drawing,
                        TurnOver = m.TurnOver
                    };
                    db.CTZ_Project_Materials_History.Add(hist);
                }
                db.SaveChanges();
            }
        }
    }
}