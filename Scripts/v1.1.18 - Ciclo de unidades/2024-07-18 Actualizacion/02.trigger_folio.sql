
--drop trigger DIS_RU_registros_inserted
GO
 create trigger DIS_RU_registros_inserted
  on RU_registros
  for insert
  as
   declare @contador int
   declare @id int
   declare @id_planta int
   declare @prefijo varchar(4)

   select @id = id from inserted
   select @id_planta = id_planta from inserted

   -- Obtiene el siguiente folio según la planta
   seT @contador = (SELECT COUNT(*) FROM RU_registros WHERE @id_planta = id_planta)

   --Determina el prefijo
   SET @prefijo = (CASE
    WHEN @id_planta = 1 THEN 'PUE'
    WHEN @id_planta = 2 THEN 'SIL'
    WHEN @id_planta = 3 THEN 'SAL'
    WHEN @id_planta = 4 THEN 'C&B'
	WHEN @id_planta = 5 THEN 'SLP'
	ELSE 'N/D'
	END)
	
   UPDATE RU_registros SET folio = @prefijo+'-'+CAST(FORMAT(@contador, '000000') as varchar) WHERE id = @id



