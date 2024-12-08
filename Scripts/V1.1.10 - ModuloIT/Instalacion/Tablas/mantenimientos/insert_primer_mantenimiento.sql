--establece en 6 meses el periodo de mantenimiento para todos las pcs y laptos
update IT_inventory_items set maintenance_period_months = 6 where id_inventory_type in (1,2) 

--crea un mantenimiento vencido para todos los equipos

DECLARE @count INT;

------ crea tabla temporal ------
CREATE TABLE #IT_inventory_items_dll(
    Id INT
);

INSERT INTO #IT_inventory_items_dll 
select u.Id from IT_inventory_items as u
where u.id_inventory_type in (1,2)

SELECT @count = COUNT(*) FROM #IT_inventory_items_dll;

WHILE @count > 0
BEGIN
    DECLARE @IDTemporal VARCHAR(50) = (SELECT TOP(1) Id FROM #IT_inventory_items_dll);
    
	IF NOT EXISTS (SELECT * FROM [IT_mantenimientos] WHERE id_it_inventory_item = @IDTemporal and fecha_programada = '2000-01-01' )
		BEGIN
			Print 'Insertando mantenimiento'
			INSERT INTO [dbo].[IT_mantenimientos]
           ([id_it_inventory_item]
           ,[id_empleado_responsable]
           ,[id_empleado_sistemas]
           ,[id_iatf_version]
           ,[id_biblioteca_digital]
           ,[fecha_programada]
           ,[fecha_realizacion]
           ,[comentarios])
     VALUES
           (@IDTemporal
           , null
           ,null
           ,null
           ,null
           ,'2000-01-01'
           ,null
           ,null)
		END
	ELSE
		BEGIN
			Print 'Ya tiene mantenimiento progamado'
		END
	

    DELETE TOP (1) FROM #IT_inventory_items_dll
    SELECT @count = COUNT(*) FROM #IT_inventory_items_dll;
END

DROP TABLE #IT_inventory_items_dll;