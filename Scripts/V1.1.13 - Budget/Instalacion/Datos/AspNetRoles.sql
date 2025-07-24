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

	
	-- budget arlett
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BUDGET_IHS' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'BUDGET_IHS', N'Permite el Acceso al sitema de Budget(Arlett).')
	END	
		IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BUDGET_IHS_CATALOGOS' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'BUDGET_IHS_CATALOGOS', N'Permite acceso a los catálogos del sistema de Budget(Arlett)')
	END	
	

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              

