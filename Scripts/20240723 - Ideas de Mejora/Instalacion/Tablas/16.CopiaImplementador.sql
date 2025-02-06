DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[id] [int] ,
	[ideaMejoraClave] [int] ,  --FK
	[numeroEmpleado] [varchar](6) ,
	[actividad] [varchar](100),
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.Implementador

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_rel_implementador ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @ideaMejoraClave int = (SELECT TOP(1) ideaMejoraClave FROM #stats_ddl)
	DECLARE @numeroEmpleado varchar(6) = (SELECT TOP(1) numeroEmpleado FROM #stats_ddl)
	DECLARE @actividad varchar(100) = (SELECT TOP(1) actividad FROM #stats_ddl)


INSERT INTO [dbo].[IM_rel_implementador]
           (id
		   ,[ideaMejoraClave]
           ,[numeroEmpleado]
		   ,[id_empleado]
		   ,[actividad]
		   )
     VALUES
           (@id
		   ,@ideaMejoraClave
           ,@numeroEmpleado
		   , (SELECT top 1 id from empleados where numeroEmpleado =@numeroEmpleado)
		   ,@actividad
          )

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_rel_implementador OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;

--select * from IM_rel_implementador
--select * from Portalthyssenkrupp.ideaMejoraContinua.Implementador
