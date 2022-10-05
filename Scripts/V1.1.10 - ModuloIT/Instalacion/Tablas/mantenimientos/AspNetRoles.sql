use [Portal_2_0]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*****************************************************************************
*  Tipo de objeto:       Table
*  Funcion:				 Inserta datos de la tabla AspNetRoles
*  Autor :				 Alfredo Xochitemol Cruz
*  Fecha de Creaci�n:	 09/13/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  12/05/2022	Alfredo Xochitemol		 Se agregan roles para m�dulo de IT
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	--------------- MODULO IT - MATRIZ DE REQUERIMIENTOS ------------------------
	--permiso para gestionar solicitudes de usuario
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Solicitud_usuarios' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Solicitud_usuarios')
	END
	
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Matriz_requerimientos_crear' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Matriz_requerimientos_crear')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Matriz_requerimientos_detalles' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Matriz_requerimientos_detalles')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Matriz_requerimientos_autorizar' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Matriz_requerimientos_autorizar')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Matriz_requerimientos_cerrar' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Matriz_requerimientos_cerrar')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Catalogos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Catalogos')
	END
	--Inventory
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Inventory' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Inventory')
	END
	--Asignacion
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Asignacion_Hardware' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Asignacion_Hardware')
	END
	--Mantenimiento
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Mantenimento_registro' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Mantenimento_registro')
	END
	

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              