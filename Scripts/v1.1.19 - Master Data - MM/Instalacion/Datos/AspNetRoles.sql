--use [Portal_2_0]
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

	--------------- MODULO CICLO/REGISTRO DE UNIDADES ------------------------
	-- Permiso para administrador SCDM
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='SCDM_MM_ADMINISTRADOR' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'SCDM_MM_ADMINISTRADOR',N'Permiso para el administrador de la herramienta de Master Data MM.')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='SCDM_MM_CREACION_SOLICITUDES' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'SCDM_MM_CREACION_SOLICITUDES',N'Permite crear solicitudes de Master Data MM.')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='SCDM_MM_APROBACION_SOLICITUDES' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'SCDM_MM_APROBACION_SOLICITUDES',N'Aprueba Solicitudes de Master Data MM.')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='SCDM_MM_REPORTES' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'SCDM_MM_REPORTES',N'Acceso a los reportes y m�tricas de Master Data MM.')
	END
	
	
	

	--reportes
	--crear solicitudes
	--aprobar solicitudes

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO             

