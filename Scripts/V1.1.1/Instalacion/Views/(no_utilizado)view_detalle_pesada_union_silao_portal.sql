USE [cube_tkmm]
GO
GO
DROP VIEW IF EXISTS dbo.view_detalle_pesada_union_silao_portal ;  

/****** Object:  View [dbo].[view_detalle_pesada_union_silao_portal]    Script Date: 10/11/2021 12:35:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[view_detalle_pesada_union_silao_portal]
AS
SELECT TOP (100) PERCENT  
			      ISNULL(ROW_NUMBER() OVER (ORDER BY dbo.view_union_bitacoras_silao.[SAP Platina]), 0) as id, dbo.view_union_bitacoras_silao.[SAP Platina], dbo.mm_v3.[Type of Selling], dbo.view_union_bitacoras_silao.Fecha, dbo.mm_v3.[Type of Metal], dbo.clientes.[Name 1], dbo.class_v3.Shape, dbo.class_v3.[Customer part number], 
                  dbo.class_v3.Surface, dbo.class_v3.Mill, dbo.class_v3.[Width - Metr], dbo.class_v3.[Gauge - Metric], dbo.class_v3.[Length(mm)], dbo.view_union_bitacoras_silao.[Número de Parte  de cliente], dbo.view_union_bitacoras_silao.[Tipo de Material], 
                  dbo.view_union_bitacoras_silao.[SAP Rollo], dbo.view_union_bitacoras_silao.Material, dbo.view_union_bitacoras_silao.Turno, dbo.view_union_bitacoras_silao.[Orden SAP], dbo.view_union_bitacoras_silao.[Pieza por Golpe], dbo.view_union_bitacoras_silao.[N° de Rollo], dbo.view_union_bitacoras_silao.[Peso Etiqueta (Kg)], 
                  dbo.view_union_bitacoras_silao.[Peso de rollo usado], dbo.view_union_bitacoras_silao.[Total de piezas], dbo.view_union_bitacoras_silao.[Peso Real Pieza Bruto], dbo.mm_v3.Plnt, dbo.mm_v3.MS, dbo.mm_v3.[Type of Material], dbo.mm_v3.[Business Model], 
                  dbo.mm_v3.[IHS number 1], dbo.mm_v3.[IHS number 2], dbo.mm_v3.[Net weight], dbo.mm_v3.[Gross weight], dbo.mm_v3.Thickness, dbo.mm_v3.Width, dbo.mm_v3.[Head and Tail allowed scrap], dbo.mm_v3.[Pieces per car], 
                  dbo.mm_v3.Thickness AS Expr1, dbo.mm_v3.Width AS Expr2, dbo.mm_v3.Advance, STR(dbo.mm_v3.Thickness) AS THICK, STR(dbo.mm_v3.Width) AS widt, STR(dbo.mm_v3.Advance) AS advan, 
                  dbo.mm_v3.Thickness * dbo.mm_v3.Width * dbo.mm_v3.Advance * 7.85 / 1000000 AS peso_teorico_acero, YEAR(dbo.view_union_bitacoras_silao.Fecha) AS year, MONTH(dbo.view_union_bitacoras_silao.Fecha) AS month, 
                  dbo.mm_v3.Thickness * dbo.mm_v3.Width * dbo.mm_v3.Advance * 2.73 / 1000000 AS peso_teorico_aluminio, dbo.view_union_bitacoras_silao.[Peso Real Pieza Neto]
FROM     dbo.clientes RIGHT OUTER JOIN
                  dbo.class_v3 ON dbo.clientes.Customer = dbo.class_v3.Customer RIGHT OUTER JOIN
                  dbo.view_union_bitacoras_silao ON dbo.class_v3.Object = dbo.view_union_bitacoras_silao.[SAP Platina] LEFT OUTER JOIN
                  dbo.mm_v3 ON dbo.view_union_bitacoras_silao.[SAP Platina] = dbo.mm_v3.Material
WHERE  (dbo.view_union_bitacoras_silao.[SAP Platina] <> N'Temporal') AND (dbo.mm_v3.[Type of Selling] = N'SERIE')
ORDER BY dbo.view_union_bitacoras_silao.Fecha
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3408
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'view_detalle_pesada_union_silao_portal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'view_detalle_pesada_union_silao_portal'
GO


