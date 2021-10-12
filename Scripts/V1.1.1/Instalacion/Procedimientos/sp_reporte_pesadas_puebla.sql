SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_reporte_pesadas_puebla','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_reporte_pesadas_puebla]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_reporte_pesadas_puebla] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_reporte_pesadas_puebla]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Obtiene los registros del reporte de pesadas
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 10/12/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(  
      @cliente VARCHAR(255),
	  @fecha_inicio DATETIME,
	  @fecha_fin DATETIME
 )  
AS   
    BEGIN  
    -- Insert statements for procedure here  
   
        SELECT 
			ISNULL(ROW_NUMBER() OVER (ORDER BY dbo.view_detalle_pesada_union_puebla.[SAP Platina]), 0) as id
			 ,[SAP Platina ]
			 ,[Name 1]
			 ,[Type of Metal]
			 ,AVG([Net weight]) AS 'Peso Neto SAP'
			 ,AVG([Peso Real Pieza Neto]) AS 'Peso Neto (mean)'
			 ,STDEV([Peso Real Pieza Neto]) AS 'Peso Neto (sdtv)'
			 ,COUNT([SAP Platina ]) as 'count'
			 ,AVG([Net weight]) - AVG([Peso Real Pieza Neto]) AS 'Diferencia Peso Neto'
			 ,[Thickness]
			 ,[Width]
			 ,[Advance]
			 ,[peso_teorico]
			 ,AVG([Gross weight]) AS 'Peso Bruto SAP'
			 ,AVG([Peso Real Pieza Bruto]) AS 'Peso Bruto (mean)'
			 ,AVG([Gross weight]) - AVG([Peso Real Pieza Bruto]) AS 'Diferencia SAP Mean'
			 ,AVG([Gross weight]) - [peso_teorico] AS 'Diferencia Peso Bruto SAP-teorico'
			 ,SUM([Total de piezas]) as 'Total de piezas'
			 ,SUM([Peso Etiqueta (Kg)]) AS 'Peso Etiqueta'
		  FROM [cube_tkmm].[dbo].[view_detalle_pesada_union_puebla]
		  where [Peso Real Pieza Neto]>0 AND [Peso Real Pieza Bruto]>0 
		  --busqueda
		  AND [Name 1] LIKE '%'+@cliente+'%'
		  AND [cube_tkmm].[dbo].[view_detalle_pesada_union_puebla].[fecha] between @fecha_inicio AND @fecha_fin
		  group by [SAP Platina ],[Name 1], [Type of Metal], [Thickness], [Width], [Advance], [peso_teorico]
		  order by [SAP PLatina ] ASC
    END  
GO
	IF object_id(N'sp_reporte_pesadas_puebla','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_reporte_pesadas_puebla] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_reporte_pesadas_puebla] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END