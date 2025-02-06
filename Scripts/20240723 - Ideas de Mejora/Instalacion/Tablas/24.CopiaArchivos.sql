DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[Id] [int],
	[clave_idea_mejora] [int],
	[Nombre] [nvarchar](70),
	[MimeType] [nvarchar](50),
	[Datos] [varbinary](max),
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.dbo.archivo_ideas_mejora 

SELECT @count = COUNT(*) FROM #stats_ddl;

--SET IDENTITY_INSERT biblioteca_digital ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) Id FROM #stats_ddl)
	DECLARE @clave_idea_mejora int = (SELECT TOP(1) clave_idea_mejora FROM #stats_ddl)
	DECLARE @nombre varchar(70) = (SELECT TOP(1) nombre FROM #stats_ddl)
	DECLARE @mimetype varchar(50) = (SELECT TOP(1) MimeType FROM #stats_ddl)
	DECLARE @datos varbinary(max) = (SELECT TOP(1) Datos FROM #stats_ddl)


INSERT INTO [dbo].biblioteca_digital
           (
		    Nombre
           ,MimeType
		   ,Datos
		   )
     VALUES
           (
		   @nombre
		   ,@mimetype
           ,@datos		   
		  )

--Obtiene el ultimo id ingresado
DECLARE @id_archivo int = (SELECT TOP(1) Id FROM biblioteca_digital order by id desc)

--Inserta el rel archivo - idea
Insert into IM_rel_archivos (id_idea_mejora, id_archivo) values (@clave_idea_mejora,@id_archivo)

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

--SET IDENTITY_INSERT biblioteca_digital OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;

--select * from IM_rel_archivos
--select count(*) from biblioteca_digital --7124  7297 7588

