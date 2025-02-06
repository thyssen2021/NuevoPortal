
DECLARE @count INT;

CREATE TABLE #stats_ddl(
    [id] [int]  ,
	[folio] [varchar](10) ,
	[captura] [smalldatetime] ,
	[titulo] [varchar](150),
	[situacionActual] [varchar](2000) ,
	[objetivo] [varchar](2000) ,
	[descripcion] [varchar](2000) ,
	[comiteAceptada] [tinyint] ,    -- 1 = aceptada; 2 = SinEstatus; 0 = Rechazada;
	[ideaEnEquipo] [tinyint] ,
	[clasificacionClave] [int] , --FK * covertir 1 en null
	[nivelImpactoClave] [int] ,	 --FK * covertir 1 en null
	[enProcesoImplementacion] [tinyint] ,
	[areaImplementacionClave] [int] ,	--FK covertir 0 en null
	[ideaImplementada] [tinyint] ,		
	[implementacionFecha] [smalldatetime] ,
	[reconocimentoClave] [int] ,	--FK *	covertir 1 en null
	[reconocimientoMonto] [decimal](9, 3),
	[clave_planta] [int] ,			--FK FK covertir 0 en null
	[tipo_idea] [varchar](20) 
);

--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select * from Portalthyssenkrupp.ideaMejoraContinua.IdeaMejoraCabecera

SELECT @count = COUNT(*) FROM #stats_ddl;

SET IDENTITY_INSERT IM_Idea_mejora ON 


WHILE @count > 0
BEGIN
	DECLARE @id int = (SELECT TOP(1) id FROM #stats_ddl)
	DECLARE @folio [varchar](10) = (SELECT TOP(1) folio FROM #stats_ddl)
	DECLARE @captura [smalldatetime] = (SELECT TOP(1) captura FROM #stats_ddl)
	DECLARE @titulo [varchar](150) = (SELECT TOP(1) titulo FROM #stats_ddl)
	DECLARE @situacionActual [varchar](2000) = (SELECT TOP(1) situacionActual FROM #stats_ddl)
	DECLARE @objetivo [varchar](2000) = (SELECT TOP(1) objetivo FROM #stats_ddl)
	DECLARE @descripcion [varchar](2000) = (SELECT TOP(1) descripcion FROM #stats_ddl)
	DECLARE @comiteAceptada [tinyint] = (SELECT TOP(1) comiteAceptada FROM #stats_ddl)
	DECLARE @ideaEnEquipo [tinyint] = (SELECT TOP(1) ideaEnEquipo FROM #stats_ddl)
	DECLARE @clasificacionClave [int] = (SELECT TOP(1) clasificacionClave FROM #stats_ddl)
	DECLARE @nivelImpactoClave [int] = (SELECT TOP(1) nivelImpactoClave FROM #stats_ddl)
	DECLARE @enProcesoImplementacion [tinyint] = (SELECT TOP(1) enProcesoImplementacion FROM #stats_ddl)
	DECLARE @areaImplementacionClave [int] = (SELECT TOP(1) areaImplementacionClave FROM #stats_ddl)
	DECLARE @ideaImplementada [tinyint] = (SELECT TOP(1) ideaImplementada FROM #stats_ddl)	
	DECLARE @implementacionFecha [smalldatetime] = (SELECT TOP(1) implementacionFecha FROM #stats_ddl)
	DECLARE @reconocimentoClave [int] = (SELECT TOP(1) reconocimentoClave FROM #stats_ddl)
	DECLARE @reconocimientoMonto [decimal](9, 3) = (SELECT TOP(1) reconocimientoMonto FROM #stats_ddl)
	DECLARE @clave_planta [int] = (SELECT TOP(1) clave_planta FROM #stats_ddl)			--FK 
	DECLARE @tipo_idea [varchar](20) = (SELECT TOP(1) tipo_idea FROM #stats_ddl)


INSERT INTO [dbo].[IM_Idea_mejora]
           (id
		   ,[folio]
           ,[captura]
           ,[titulo]
           ,[situacionActual]
           ,[objetivo]
           ,[descripcion]
           ,[comiteAceptada]
           ,[ideaEnEquipo]
           ,[clasificacionClave]
           ,[nivelImpactoClave]
           ,[enProcesoImplementacion]
           ,[areaImplementacionClave]
           ,[ideaImplementada]
           ,[implementacionFecha]
           ,[reconocimentoClave]
           ,[reconocimientoMonto]
           ,[clave_planta]
           ,[tipo_idea])
     VALUES
           (@id
		   ,@folio
           ,@captura
           ,@titulo
           ,@situacionActual
           ,@objetivo
           ,@descripcion
           ,@comiteAceptada
           ,@ideaEnEquipo
           ,(SELECT CASE @clasificacionClave
			  WHEN 1 THEN NULL		  
			  ELSE @clasificacionClave
			END)
		   ,(SELECT CASE @nivelImpactoClave
			  WHEN 1 THEN NULL		  
			  ELSE @nivelImpactoClave
			END)
           ,@enProcesoImplementacion
           ,(SELECT CASE @areaImplementacionClave
			  WHEN 0 THEN NULL		  
			  ELSE @areaImplementacionClave
			END)
           ,@ideaImplementada
           ,@implementacionFecha
           ,(SELECT CASE @reconocimentoClave
			  WHEN 1 THEN NULL		  
			  ELSE @reconocimentoClave
			END)
           ,@reconocimientoMonto
           ,(SELECT CASE @clave_planta
			  WHEN 0 THEN NULL		  
			  ELSE @clave_planta
			END)
           ,@tipo_idea)

		       
	--borra el registro actua	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

SET IDENTITY_INSERT IM_Idea_mejora OFF 

--Borra la tabla temporal
DROP TABLE #stats_ddl;


--select * from IM_Idea_mejora


