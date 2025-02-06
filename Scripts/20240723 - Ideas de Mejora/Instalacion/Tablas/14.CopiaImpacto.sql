DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[id] [int] ,
	[ideaMejoraClave] [int] ,
	[catalogoIdeaMejoraImpactoClave] [int],
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.Impacto

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_rel_impacto ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @ideaMejoraClave int = (SELECT TOP(1) ideaMejoraClave FROM #stats_ddl)
	DECLARE @catalogoIdeaMejoraImpactoClave int = (SELECT TOP(1) catalogoIdeaMejoraImpactoClave FROM #stats_ddl)


INSERT INTO [dbo].[IM_rel_impacto]
           (id
		   ,[ideaMejoraClave]
           ,[catalogoIdeaMejoraImpactoClave])
     VALUES
           (@id
		   ,@ideaMejoraClave
           ,@catalogoIdeaMejoraImpactoClave
          )

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_rel_impacto OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;


--select * from IM_rel_impacto
--select * from Portalthyssenkrupp.ideaMejoraContinua.Impacto

