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
*  02/15/2021	Alfredo Xochitemol		 Se agregan roles de ORDENES DE TRABAJO
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	--------------- ORDENES DE TRABAJO ------------------------
	-- Solicitud de órdenes de trabajo
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='OrdenesTrabajo_Solicitud' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'OrdenesTrabajo_Solicitud')
	END
	-- Asignación de Solicitudes
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='OrdenesTrabajo_Asignacion' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'OrdenesTrabajo_Asignacion')
	END
	-- Responsable ordenes de Trabajo
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='OrdenesTrabajo_Responsable' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'OrdenesTrabajo_Responsable')
	END
	-- Reportes de Órdenes de trabajo
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='OrdenesTrabajo_Reportes' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'OrdenesTrabajo_Reportes')
	END
	-- Catálogos de Órdenes de trabajo
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='OrdenesTrabajo_Catalogos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'OrdenesTrabajo_Catalogos')
	END
PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
