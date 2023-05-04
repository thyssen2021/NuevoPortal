use Portal_2_0;
-- DROP TABLE #stats_ddl

DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[clave] [int]   NULL,
	[activo] [bit]  NULL,
	[remisionNumero] [varchar](50) NULL,
	[almacenClave] [int]  NULL,
	[transporteOtro] [varchar](50)  NULL,
	[transporteProveedorClave] [int] NULL,
	[nombreChofer] [varchar](50)  NULL,
	[clienteClave] [int] NULL,
	[clienteOtro] [varchar](50)  NULL,
	[placaTractor] [varchar](50)  NULL,
	[placaRemolque] [varchar](50)  NULL,
	[horarioDescarga] [varchar](50)  NULL,
	[enviadoAClave] [int] NULL,
	[enviadoAOtro] [varchar](50)  NULL,
	[clienteOtroDireccion] [varchar](100)  NULL,
	[enviadoAOtroDireccion] [varchar](100)  NULL,
	[motivoClave] [int]  NULL,
	[motivoTexto] [varchar](1500)  NULL,
	[retornaMaterial] [bit]  NULL);

INSERT INTO #stats_ddl 
SELECT * FROM Portalthyssenkrupp.remision.RemisionCabecera order by clave asc

SET IDENTITY_INSERT [dbo].[RM_cabecera] ON 

SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN

DECLARE @transporteProveedorClave INT = (SELECT TOP(1) transporteProveedorClave FROM #stats_ddl);
DECLARE @clienteClave INT = (SELECT TOP(1) clienteClave FROM #stats_ddl);
DECLARE @enviadoAClave INT = (SELECT TOP(1) enviadoAClave FROM #stats_ddl);
DECLARE @motivoClave INT = (SELECT TOP(1) motivoClave FROM #stats_ddl);

	INSERT INTO [dbo].[RM_cabecera]
           (
		   [clave]
		   ,[activo]
           ,[remisionNumero]
           ,[almacenClave]
           ,[transporteOtro]
           ,[transporteProveedorClave]
           ,[nombreChofer]
           ,[clienteClave]
           ,[clienteOtro]
           ,[placaTractor]
           ,[placaRemolque]
           ,[horarioDescarga]
           ,[enviadoAClave]
           ,[enviadoAOtro]
           ,[clienteOtroDireccion]
           ,[enviadoAOtroDireccion]
           ,[motivoClave]
           ,[motivoTexto]
           ,[retornaMaterial]
		   ,[ultimoEstatus]
		   )
     VALUES
           (
		   (SELECT TOP(1) clave FROM #stats_ddl)
		   ,(SELECT TOP(1) activo FROM #stats_ddl)
           ,(SELECT TOP(1) remisionNumero FROM #stats_ddl)
           ,(SELECT TOP(1) almacenClave FROM #stats_ddl)
           ,(SELECT TOP(1) transporteOtro FROM #stats_ddl)
           ,IIF(
			@transporteProveedorClave = 0,
			NULL,
			@transporteProveedorClave)
           ,(SELECT TOP(1) nombreChofer FROM #stats_ddl)
           ,IIF(
			@clienteClave = 0,
			NULL,
			@clienteClave)
           ,(SELECT TOP(1) clienteOtro FROM #stats_ddl)
           ,(SELECT TOP(1) placaTractor FROM #stats_ddl)
           ,(SELECT TOP(1) placaRemolque FROM #stats_ddl)
           ,(SELECT TOP(1) horarioDescarga FROM #stats_ddl)
           ,IIF(
			@enviadoAClave = 0,
			NULL,
			@enviadoAClave)
           ,(SELECT TOP(1) enviadoAOtro FROM #stats_ddl)
           ,(SELECT TOP(1) clienteOtroDireccion FROM #stats_ddl)
           ,(SELECT TOP(1) enviadoAOtroDireccion FROM #stats_ddl)
           ,IIF(
			@motivoClave = 0,
			NULL,
			@motivoClave)
           ,(SELECT TOP(1) motivoTexto FROM #stats_ddl)
           ,(SELECT TOP(1) retornaMaterial FROM #stats_ddl)
		   ,ISNULL((SELECT TOP(1) catalogoEstatusClave FROM Portalthyssenkrupp.remision.RemisionEstatus 
				WHERE remisionCabeceraClave = (SELECT TOP(1) clave FROM #stats_ddl) AND catalogoEstatusClave <> 6 ORDER BY clave DESC ),NULL)
		   )
    DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl 
END

SET IDENTITY_INSERT [dbo].[RM_cabecera] OFF
DROP TABLE #stats_ddl;

--coloca como inactivo si no tiene ultimo estatus
UPDATE RM_cabecera set activo=0 where ultimoEstatus is null
--se coloca como regularizada si el motivo
UPDATE RM_cabecera set ultimoEstatus=4 where motivoClave = 14 and ultimoEstatus <> 5