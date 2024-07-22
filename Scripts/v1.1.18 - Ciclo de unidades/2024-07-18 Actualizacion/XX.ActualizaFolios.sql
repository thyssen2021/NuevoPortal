
--- ACTUALIZA LOS FOLIOS DE LA TABLA RU_registros
-- ¡Importante que no se hayan registrado ningun registro para otras plantas!

DECLARE @count INT;

CREATE TABLE #stats_ddl(
    id INT, 
    fecha_ingreso_vigilancia datetime,
);

INSERT INTO #stats_ddl 
select c.id, c.fecha_ingreso_vigilancia from RU_registros as c 


SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN
	declare @fecha datetime = (SELECT TOP(1) fecha_ingreso_vigilancia FROM #stats_ddl)
	declare @id_temp int = (SELECT TOP(1) id FROM #stats_ddl)

	declare @registros_previos int = (SELECT COUNT(*) FROM RU_registros where fecha_ingreso_vigilancia <= @fecha)
	
	update Ru_registros set folio = 'PUE-'+CAST(FORMAT(@registros_previos, '000000') as varchar) where id = @id_temp
	
	
	DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl;
END

DROP TABLE #stats_ddl;


--update RU_registros set folio ='-'
--select id, folio from RU_registros
--select * from RU_registros

