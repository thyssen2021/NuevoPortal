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
*  12/05/2022	Alfredo Xochitemol		 Se agregan roles para módulo de IT
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	--------------- MODULO REMISIONES MANUALES ------------------------
	--permiso para Menu de comedor RH
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RM_Detalles' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RM_Detalles')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RM_Creacion' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RM_Creacion')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RM_Aprobar' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RM_Aprobar')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RM_Regularizar' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RM_Regularizar')
	END	
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RM_Reportes' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RM_Reportes')
	END

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
