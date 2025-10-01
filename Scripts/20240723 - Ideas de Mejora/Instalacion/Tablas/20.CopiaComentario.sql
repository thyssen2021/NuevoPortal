DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[id] [int] ,
	[ideaMejoraClave] [int] ,  --FK
	[captura] [smalldatetime] ,
	[numeroEmpleado] [varchar](6) ,
	[texto] [varchar](1000) ,
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.Comentario where texto <> ''

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_rel_comentario ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @ideaMejoraClave int = (SELECT TOP(1) ideaMejoraClave FROM #stats_ddl)
	DECLARE @captura smalldatetime = (SELECT TOP(1) captura FROM #stats_ddl)
	DECLARE @numeroEmpleado varchar(6) = (SELECT TOP(1) numeroEmpleado FROM #stats_ddl)
	DECLARE @texto varchar(1000) = (SELECT TOP(1) texto FROM #stats_ddl)

INSERT INTO [dbo].[IM_rel_comentario]
           (id
		   ,[ideaMejoraClave]
           ,[captura]
		   ,[numeroEmpleado]
		   ,[id_empleado]
		   ,[texto]
		   )
     VALUES
           (@id
		   ,@ideaMejoraClave
		   ,@captura
           ,@numeroEmpleado
		   , (SELECT top 1 id from empleados where numeroEmpleado =@numeroEmpleado)
			,@texto
		  )

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_rel_comentario OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;

--select * from IM_rel_comentario
--select * from Portalthyssenkrupp.ideaMejoraContinua.Comentario 

