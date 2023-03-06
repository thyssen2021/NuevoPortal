--use [Portal_2_0]
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
*  10/07/2021	Alfredo Xochitemol		 Se agregan roles de bitácoras
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	-- Reportes de Póliza
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_SOLICITUD' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_SOLICITUD')
	END	
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_JEFE_DIRECTO' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_JEFE_DIRECTO')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_CONTROLLING' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_CONTROLLING')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_CONTABILIDAD' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_CONTABILIDAD')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_NOMINA' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_NOMINA')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_REPORTES' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_REPORTES')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_CATALOGOS' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_CATALOGOS')
	END
	--Permiso de autorizacion especial
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='GV_AUTORIZACION' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'GV_AUTORIZACION')
	END

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
