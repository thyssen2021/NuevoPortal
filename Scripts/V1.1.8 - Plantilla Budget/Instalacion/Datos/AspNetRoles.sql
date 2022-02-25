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
*  02/23/2021	Alfredo Xochitemol		 Se agregan roles de PLANTILLA DE BUDGET
******************************************************************************/

IF object_id(N'AspNetRoles','U') IS NOT NULL
BEGIN

	--------------- PLANTILLA DE BUDGET ------------------------

	-- Responsable de centro de costo
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BG_Responsable_Centro_Costo' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'BG_Responsable_Centro_Costo')
	END
	-- Controlling
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BG_Controlling' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'BG_Controlling')
	END
	-- Reportes (concentrado)
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='BG_Reportes' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'BG_Reportes')
	END

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
