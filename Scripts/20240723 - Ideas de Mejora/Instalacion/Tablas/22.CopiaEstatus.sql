DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[id] [int],
	[captura] [smalldatetime],
	[ideaMejoraClave] [int],
	[catalogoIdeaMejoraEstatusClave] [int],
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.Estatus 

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_rel_estatus ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @ideaMejoraClave int = (SELECT TOP(1) ideaMejoraClave FROM #stats_ddl)
	DECLARE @captura smalldatetime = (SELECT TOP(1) captura FROM #stats_ddl)
	DECLARE @catalogoIdeaMejoraEstatusClave int = (SELECT TOP(1) catalogoIdeaMejoraEstatusClave FROM #stats_ddl)
	
INSERT INTO [dbo].[IM_rel_estatus]
           (id
		   ,[ideaMejoraClave]
           ,[captura]
		   ,[catalogoIdeaMejoraEstatusClave]
		   )
     VALUES
           (@id
		   ,@ideaMejoraClave
		   ,@captura
           ,@catalogoIdeaMejoraEstatusClave
		   
		  )

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_rel_estatus OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;

--select * from IM_rel_estatus
--select * from Portalthyssenkrupp.ideaMejoraContinua.Estatus 

