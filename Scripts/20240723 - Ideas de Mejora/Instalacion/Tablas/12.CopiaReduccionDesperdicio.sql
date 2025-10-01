
DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[id] [int] ,
	[ideaMejoraClave] [int] ,
	[catalogoIdeaMejoraDesperdicioClave] [int],
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.ReduccionDesperdicio

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_rel_reduccion_desperdicio ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @ideaMejoraClave int = (SELECT TOP(1) ideaMejoraClave FROM #stats_ddl)
	DECLARE @catalogoIdeaMejoraDesperdicioClave int = (SELECT TOP(1) catalogoIdeaMejoraDesperdicioClave FROM #stats_ddl)


INSERT INTO [dbo].[IM_rel_reduccion_desperdicio]
           (id
		   ,[ideaMejoraClave]
           ,[catalogoIdeaMejoraDesperdicioClave])
     VALUES
           (@id
		   ,@ideaMejoraClave
           ,@catalogoIdeaMejoraDesperdicioClave
          )

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_rel_reduccion_desperdicio OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;


--select * from IM_rel_reduccion_desperdicio
--select * from Portalthyssenkrupp.ideaMejoraContinua.ReduccionDesperdicio

