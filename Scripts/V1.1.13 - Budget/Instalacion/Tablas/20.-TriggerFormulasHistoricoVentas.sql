/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea un trigger para mantener actualizado el valor value add sales
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/07/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

-- Trigger para VAS
IF OBJECT_ID ('DIS_BG_historico_value_added_sales', 'TR') IS NOT NULL  
   DROP TRIGGER DIS_BG_historico_value_added_sales;  

GO
create trigger DIS_BG_historico_value_added_sales
  on BG_forecast_cat_historico_ventas
  for insert, delete, update
 as 
	--declaracion de variables
   declare @fecha datetime
   declare @id_cliente int
   declare @id_seccion int
   declare @total_sales float --total sales
   declare @material_cost float --material cost
   declare @cost_outside_processor float -- cost outside proccesor
   declare @value_add_sales_new float  -- total sales - material cost - cost outside
   declare @new_value float  -- total sales - material cost - cost outside

  
  --Determina cual fue el evento
  --Inserted / modified
  if ((select count(*) from inserted) = 1 AND NOT exists (select * from deleted)) OR ((select count(*) from inserted) = 1  AND (select count(*) from deleted) = 1)
   begin
   	--Asignación de variables
	select @fecha= fecha from inserted
	select @id_cliente= id_cliente from inserted
	select @id_seccion= id_seccion from inserted

	--Total sales
	 if	((select id_seccion from inserted) = 1) 
		 begin
			select @total_sales = valor from inserted
		 end	
	 else -- Si no es total sales
		 begin
			SET @total_sales = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 1),0) as valor)
		 end
	--material cost
	 if	((select id_seccion from inserted) = 2) 
		 begin
			select @material_cost = valor from inserted
		 end	
	 else -- Si no es total sales
		 begin
			SET @material_cost = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 2),0) as valor)
		 end
	--COST outsite proccesor
	 if	((select id_seccion from inserted) = 3) 
		 begin
			select @cost_outside_processor = valor from inserted
		 end	
	 else -- Si no es total sales
		 begin
			SET @cost_outside_processor = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 3),0) as valor)
		 end	
  
   end
   --deleted
   if NOT exists (select * from inserted) AND (select count(*) from deleted) = 1
   begin
   	--Asignación de variables
	select @fecha= fecha from deleted
	select @id_cliente= id_cliente from deleted
	select @id_seccion= id_seccion from deleted

     --Total sales
	 if	((select id_seccion from deleted) = 1) 
		 begin
			select @total_sales = 0
		 end	
	 else -- Si no es total sales
		 begin
			SET @total_sales = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 1),0) as valor)
		 end
	--material cost
	 if	((select id_seccion from deleted) = 2) 
		 begin
			select @material_cost = 0
		 end	
	 else -- Si no es total sales
		 begin
			SET @material_cost = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 2),0) as valor)
		 end
	--COST outsite proccesor
	 if	((select id_seccion from deleted) = 3) 
		 begin
			select @cost_outside_processor = 0
		 end	
	 else -- Si no es total sales
		 begin
			SET @cost_outside_processor = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 3),0) as valor)
		 end
   end
  
  --Actualiza Value Added Sales
  SET @value_add_sales_new = @total_sales-@material_cost-@cost_outside_processor
  --SET @value_add_sales_old = (SELECT COALESCE ((select  valor from BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 4),0) as valor)

  --Crea
  IF NOT EXISTS(select * FROM BG_forecast_cat_historico_ventas WHERE fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 4 ) AND @id_cliente is not null
	begin
		INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 4, @fecha, @value_add_sales_new)
		INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 19, @fecha, @value_add_sales_new)
	end
 ELSE --Modifica
   begin
		if @id_cliente is not null
		begin 
			Update BG_forecast_cat_historico_ventas set valor = @value_add_sales_new where fecha = @fecha AND id_cliente = @id_cliente AND (id_seccion = 4 OR id_seccion =19)
		end
	end
 --elimina
 IF @value_add_sales_new = 0
	 begin
		delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND (id_seccion = 4 OR id_seccion =19)
	 end
--MIRROR
--Inserted 
  if ((select count(*) from inserted) = 1 AND NOT exists (select * from deleted)) 
   begin
   	--Asignación de variables
	select @fecha= fecha from inserted
	select @id_cliente= id_cliente from inserted
	select @id_seccion= id_seccion from inserted
	select @new_value = valor from inserted

	--Processed Tons
	 if	(@id_seccion = 5) 
		begin --inserta en shipment tons
			INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 17, @fecha, @new_value)
		 end	
	--total Sales
	 if	(@id_seccion = 1) 
		begin --inserta en sales inc scrap
			INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 18, @fecha, @new_value)
		 end	
	
	 
   end
   --updated
   IF ((select count(*) from inserted) = 1 AND (select count (*) from deleted)=1)
   	begin
		--Asignación de variables
		select @fecha= fecha from inserted
		select @id_cliente= id_cliente from inserted
		select @id_seccion= id_seccion from inserted
		select @new_value = valor from inserted

		--Processed Tons
	 if	(@id_seccion = 5) 
		begin --update en shipment tons
			updATE BG_forecast_cat_historico_ventas SET valor = @new_value where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 17
		 end	
	--total Sales
	 if	(@id_seccion = 1) 
		begin --update en sales inc scrap
			updATE BG_forecast_cat_historico_ventas SET valor = @new_value where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 18
		 end	
	
	end
   --deleted
   if NOT exists (select * from inserted) AND (select count(*) from deleted)=1
   begin
   		--Asignación de variables
		select @fecha= fecha from deleted
		select @id_cliente= id_cliente from deleted
		select @id_seccion= id_seccion from deleted

			--Processed Tons
	 if	(@id_seccion = 5) 
		begin --delete en shipment tons
			delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 17
		 end	
	--total Sales
	 if	(@id_seccion = 1) 
		begin --delete en sales inc scrap
			delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 18
		 end

	end
GO
/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea un trigger para mantener actualizado el valor shipment tons
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/07/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/
---- Trigger para shipment tons
--IF OBJECT_ID ('DIS_BG_historico_value_mirror', 'TR') IS NOT NULL  
--   DROP TRIGGER DIS_BG_historico_value_mirror;  

--GO
--create trigger DIS_BG_historico_value_mirror
--  on BG_forecast_cat_historico_ventas
--  for insert, delete, update
-- as 
--	--declaracion de variables
--   declare @fecha datetime
--   declare @id_cliente int
--   declare @id_seccion int
--   declare @new_value float --total sales
   
  
--  --Determina cual fue el evento
--  --Inserted 
--  if (exists (select * from inserted) AND NOT exists (select * from deleted)) 
--   begin
--   	--Asignación de variables
--	select @fecha= fecha from inserted
--	select @id_cliente= id_cliente from inserted
--	select @id_seccion= id_seccion from inserted
--	select @new_value = valor from inserted

--	--Processed Tons
--	 if	(@id_seccion = 5) 
--		begin --inserta en shipment tons
--			INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 17, @fecha, @new_value)
--		 end	
--	--total Sales
--	 if	(@id_seccion = 1) 
--		begin --inserta en sales inc scrap
--			INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 18, @fecha, @new_value)
--		 end	
--	-- Vas 
--	 if	(@id_seccion= 4) 
--		begin --inserta en vas inc scrap
--			INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (@id_cliente, 19, @fecha, @new_value)
--		 end	
	 
--   end
--   --updated
--   IF (exists (select * from inserted) AND exists (select * from deleted))
--   	begin
--		--Asignación de variables
--		select @fecha= fecha from inserted
--		select @id_cliente= id_cliente from inserted
--		select @id_seccion= id_seccion from inserted
--		select @new_value = valor from inserted

--		--Processed Tons
--	 if	(@id_seccion = 5) 
--		begin --update en shipment tons
--			updATE BG_forecast_cat_historico_ventas SET valor = @new_value where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 17
--		 end	
--	--total Sales
--	 if	(@id_seccion = 1) 
--		begin --update en sales inc scrap
--			updATE BG_forecast_cat_historico_ventas SET valor = @new_value where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 18
--		 end	
--	-- Vas 
--	 if	(@id_seccion= 4) 
--		begin --update en vas inc scrap
--			updATE BG_forecast_cat_historico_ventas SET valor = @new_value where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 19
--		 end	
--	end
--   --deleted
--   if NOT exists (select * from inserted) AND exists (select * from deleted)
--   begin
--   		--Asignación de variables
--		select @fecha= fecha from deleted
--		select @id_cliente= id_cliente from deleted
--		select @id_seccion= id_seccion from deleted

--			--Processed Tons
--	 if	(@id_seccion = 5) 
--		begin --delete en shipment tons
--			delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 17
--		 end	
--	--total Sales
--	 if	(@id_seccion = 1) 
--		begin --delete en sales inc scrap
--			delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 18
--		 end	
--	-- Vas 
--	 if	(@id_seccion= 4) 
--		begin --delete en vas inc scrap
--			delete from BG_forecast_cat_historico_ventas where fecha = @fecha AND id_cliente = @id_cliente AND id_seccion = 19
--		 end	
--	end
--GO
    
   
  


	 
----select * from BG_forecast_cat_clientes
----select * from [BG_Forecast_cat_secciones_calculo]
----select * from BG_forecast_cat_historico_ventas

----INSERT INTO BG_forecast_cat_historico_ventas (id_cliente, id_seccion, fecha, valor) values (2, 1, '2023-12-01', '28')
--delete  from BG_forecast_cat_historico_ventas