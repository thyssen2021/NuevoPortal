SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'spEmpleado','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[spEmpleado]
      PRINT '<<< STORED PROCEDURE [dbo].[spEmpleado] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Inserta una excepción no controlada en BD
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 09/09/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
CREATE PROCEDURE [dbo].[spEmpleado] 
@opcion int = 0, 
@clave int = 0, 
@activo  bit= 0,
@numeroEmpleado varchar(6)='',
@nombre varchar(50)='',
@apellido1 varchar(50)='',
@apellido2 varchar(50)='',
@compania int = 0
AS
BEGIN
	SET NOCOUNT ON;
	/*
	opciones
	1 inserta
	2 modifica por clave
	3 borrado logico por clave
	4 busca todos
	5 busca activos por cualquiero campo
	*/

IF (@opcion=1)--Inserta  
BEGIN
	IF ( NOT EXISTS (SELECT * FROM [empleados] WHERE [numeroEmpleado]= @numeroEmpleado)  )
	BEGIN
		INSERT INTO [empleados](
			[activo],
			[numeroEmpleado],
			[nombre],
			[apellido1],
			[apellido2],
			[compania]
		)VALUES(
			@activo,
			@numeroEmpleado ,
			@nombre,
			@apellido1,
			@apellido2,
			@compania
		)
		SELECT @@IDENTITY AS  [clave]
	END
	ELSE
	BEGIN
		SELECT [clave] FROM [empleados] WHERE [numeroEmpleado]= @numeroEmpleadO
	END

END--IF (@opcion=1)--Inserta  

IF (@opcion=2)--modifica por clave 
BEGIN
	UPDATE [empleados]
	   SET 
		[activo] = @activo,
		[numeroEmpleado] =@numeroEmpleado,
		[nombre] = @nombre,
		[apellido1] = @apellido1,
		[apellido2] = @apellido2
	 WHERE 
		[clave]=@clave
END--IF (@opcion=2)--modifica por clave  

IF (@opcion=3)--borrado logico por clave  
BEGIN
	UPDATE [empleados]
	SET 
		[activo] = 0
	 WHERE 
		[clave]=@clave

END--IF (@opcion=3)--borrado logico por clave  

IF (@opcion=4)--busca todos  
BEGIN
	SELECT
		[empleados].[clave],
		[empleados].[activo],
		[empleados].[numeroEmpleado],
		[empleados].[correo],
		[empleados].[nombre],
		[empleados].[apellido1],
		[empleados].[apellido2],
		[empleados].[nacimientoFecha],
		[empleados].[telefono],
		[empleados].[extension],
		[empleados].[celular],
		[empleados].[nivel],
		[empleados].[puesto],
		[empleados].[compania],
		[empleados].[ingresoFecha],
		[empleados].[bajaFecha]
	FROM
		[empleados]
	ORDER BY
		[empleados].[clave]
END--IF (@opcion=4)--busca todos  

IF (@opcion=5)--busca activos por cualquiero campo
BEGIN
	SELECT
		[empleados].[clave],
		[empleados].[activo],
		[empleados].[numeroEmpleado],
		[empleados].[correo],
		[empleados].[nombre],
		[empleados].[apellido1],
		[empleados].[apellido2],
		[empleados].[nacimientoFecha],
		[empleados].[telefono],
		[empleados].[extension],
		[empleados].[celular],
		[empleados].[nivel],
		[empleados].[puesto],
		[empleados].[compania],
		[empleados].[ingresoFecha],
		[empleados].[bajaFecha]
	FROM
		[empleados]
	WHERE
		([empleados].[activo] = @activo) AND
		(CASE	
		WHEN @clave != 0 AND [empleados].[clave] = @clave THEN 1
		WHEN @clave = 0 THEN 1
		ELSE 0 
		END) = 1  AND
		(CASE	
		WHEN @nombre != '' AND [empleados].[nombre] = @nombre THEN 1
		WHEN @nombre = '' THEN 1
		ELSE 0 
		END) = 1  AND
		(CASE	
		WHEN @apellido1 != '' AND [empleados].[apellido1] =@apellido1  THEN 1
		WHEN @apellido1 = '' THEN 1
		ELSE 0 
		END) = 1  AND
		(CASE	
		WHEN @apellido2 != '' AND [empleados].[apellido2] = @apellido2 THEN 1
		WHEN @apellido2 = '' THEN 1
		ELSE 0 
		END) = 1
	ORDER BY
		CAST([empleados].[numeroEmpleado] AS INT)

END--IF (@opcion=5)--busca activos por cualquier campo

END

GO
	IF object_id(N'spEmpleado','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[spEmpleado] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[spEmpleado] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END