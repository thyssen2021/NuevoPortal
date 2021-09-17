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
*  
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Admin' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'Admin')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Usuarios' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'Usuarios')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RecursosHumanos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RecursosHumanos')
	END


PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
