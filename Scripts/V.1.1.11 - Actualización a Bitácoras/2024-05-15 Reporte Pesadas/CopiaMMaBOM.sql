CREATE TABLE #stats_ddl(
	[Material] [nvarchar](30) NOT NULL,
	[Plnt] [nvarchar](30) NOT NULL,
	[Gross weight] [float] NULL,
	[Net weight] [float] NULL,
);

DECLARE @count INT;


--Consulta que obtiene los valores deseados
INSERT INTO #stats_ddl 
select m.Material, m.Plnt, m.[Gross weight], m.[Net weight] from mm_v3 as m


SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN
	DEclare @bruto_t float = (SELECT TOP(1) [Gross weight] FROM #stats_ddl)
	DEclare @neto_t float = (SELECT TOP(1) [Net weight] FROM #stats_ddl)
	declare @planta_t varchar(30) = (SELECT TOP(1) Plnt FROM #stats_ddl)
	declare @material_t varchar(30) = (SELECT TOP(1) Material FROM #stats_ddl)
	
	--PRINT 'material: ' + CONVERT (VARCHAR, @material_t) + ' planta: '+CONVERT (VARCHAR, @planta_t) 
    
    --Inserta el valor en BD
	IF NOT EXISTS (SELECT * FROM [dbo].[bom_pesos] where plant=@planta_t AND material = @material_t )
	BEGIN
		INSERT INTO [dbo].[bom_pesos]([plant],[material],[gross_weight],[net_weight])
     VALUES(@planta_t
           ,@material_t
           ,@bruto_t
           ,@neto_t
           )	
	END	
	


	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

DROP TABLE #stats_ddl;


--select * from DBO.bom_pesos


