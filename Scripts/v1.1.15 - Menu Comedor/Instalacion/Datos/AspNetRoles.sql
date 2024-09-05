use [Portal_2_0]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*****************************************************************************
*  Tipo de objeto:       Table
*  Funcion:				 Inserta datos de la tabla AspNetRoles
*  Autor :				 Alfredo Xochitemol Cruz
*  Fecha de Creación:	 09/13/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  12/05/2022	Alfredo Xochitemol		 Se agregan roles para módulo de IT
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	--------------- MODULO IT - MATRIZ DE REQUERIMIENTOS ------------------------
	--permiso para Menu de comedor RH
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RH_Menu_Comedor_Puebla' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RH_Menu_Comedor_Puebla')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Menu_Comedor_Visualizar_Puebla' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'Menu_Comedor_Visualizar_Puebla')
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RH_Menu_Comedor_Silao' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],descripcion)VALUES(LOWER(NEWID()) ,'RH_Menu_Comedor_Silao', 'Permite editar el menú del comedor para Silao')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Menu_Comedor_Visualizar_Silao' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name], descripcion)VALUES(LOWER(NEWID()) ,'Menu_Comedor_Visualizar_Silao', 'Permite Visualizar el menú del comedor Silao')
	END

	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RH_Menu_Comedor_SLP' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],descripcion)VALUES(LOWER(NEWID()) ,'RH_Menu_Comedor_SLP', 'Permite editar el menú del comedor para SLP')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Menu_Comedor_Visualizar_SLP' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name], descripcion)VALUES(LOWER(NEWID()) ,'Menu_Comedor_Visualizar_SLP', 'Permite Visualizar el menú del comedor SLP')
	END



PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
