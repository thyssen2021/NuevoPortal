use Portal_2_0;
-- DROP TABLE #stats_ddl

DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[clave] [int]  NULL,
	[activo] [bit] NULL,
	[remisionCabeceraClave] [int]  NULL,
	[capturaFecha] [smalldatetime]  NULL,
	[numeroParteCliente] [varchar](50)  NULL,
	[numeroMaterial] [varchar](50)  NULL,
	[numeroLote] [varchar](50)  NULL,
	[numeroRollo] [varchar](50)  NULL,
	[cantidad] [float]  NULL,
	[peso] [float]  NULL,
	[remisionSap] [varchar](20) NULL);

INSERT INTO #stats_ddl 
SELECT clave, activo, remisionCabeceraClave, capturaFecha, numeroParteCliente,numeroMaterial,numeroLote, numeroRollo, cantidad 
	,peso, remisionSap
FROM Portalthyssenkrupp.remision.RemisionElemento order by clave asc

SET IDENTITY_INSERT [dbo].[RM_elemento] ON 

SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN

	INSERT INTO [dbo].[RM_elemento]
           (
		    [clave]
		   ,[activo]
           ,[remisionCabeceraClave]
           ,[capturaFecha]
           ,[numeroParteCliente]
           ,[numeroMaterial]
           ,[numeroLote]
           ,[numeroRollo]
           ,[cantidad]
           ,[peso]
           ,[remisionSap]
		   )
     VALUES
           (
		   (SELECT TOP(1) clave FROM #stats_ddl)
		   ,(SELECT TOP(1) activo FROM #stats_ddl)
           ,(SELECT TOP(1) remisionCabeceraClave FROM #stats_ddl)
           ,(SELECT TOP(1) capturaFecha FROM #stats_ddl)
           ,(SELECT TOP(1) numeroParteCliente FROM #stats_ddl)
           ,(SELECT TOP(1) numeroMaterial FROM #stats_ddl)
           ,(SELECT TOP(1) numeroLote FROM #stats_ddl)
           ,(SELECT TOP(1) numeroRollo FROM #stats_ddl)
           ,(SELECT TOP(1) cantidad FROM #stats_ddl)
           ,(SELECT TOP(1) peso FROM #stats_ddl)
           ,(SELECT TOP(1) remisionSap FROM #stats_ddl)
		   )

    DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl 
END

SET IDENTITY_INSERT [dbo].[RM_elemento] OFF
DROP TABLE #stats_ddl;
