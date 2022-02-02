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
*  10/07/2021	Alfredo Xochitemol		 Se agregan roles de bit�coras
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Admin' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'Admin')
	END
	--USUARIOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='Usuarios' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'Usuarios')
	END
	--RECURSOS HUMANOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='RecursosHumanos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'RecursosHumanos')
	END
	--BITACORAS REGISTRO
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BitacoraProduccionRegistro' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'BitacoraProduccionRegistro')
	END

	--BITACORAS CATALOGOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BitacoraProduccionCatalogos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'BitacoraProduccionCatalogos')
	END

	--REPORTE PESADAS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='ReportePesadas' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'ReportePesadas')
	END

	--REPORTE PFA REGISTRO DE FORMATOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PFA_RegistroFormato' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PFA_RegistroFormato')
	END

	--REPORTE PFA AUTORIZACI�N DE FORMATOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PFA_AutorizacionFormato' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PFA_AutorizacionFormato')
	END

	--REPORTE PFA EDICI�N CAT�LOGOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PFA_AdministracionCatalogos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PFA_AdministracionCatalogos')
	END

	--REPORTE PFA Visualizaci�n DE FORMATOS
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PFA_VisualizacionFormato' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PFA_VisualizacionFormato')
	END

	---------------- P�LIZAS MANUALES ----------------
	-- Registros
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_CreacionRegistros' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_CreacionRegistros')
	END

	-- Validaci�n por �reas
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_ValidacionPorArea' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_ValidacionPorArea')
	END

	-- Autorizaci�n (DOBLE VALIDACION)
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_AutorizacionControlling' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_AutorizacionControlling')
	END

	-- Autorizaci�n (DOBLE VALIDACION)
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_DireccionAutorizacion' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_DireccionAutorizacion')
	END

	-- Registro de poliza (Contabilidad)
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_RegistroContabilidad' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_RegistroContabilidad')
	END

	-- Cat�logos de P�liza
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_catalogos' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_catalogos')
	END
	-- Reportes de P�liza
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='PolizasManuales_reportes' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'PolizasManuales_reportes')
	END



PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
