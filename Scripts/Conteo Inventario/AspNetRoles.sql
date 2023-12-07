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
	-- Permiso para vigilancia
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='CI_CONTEO_INVENTARIO' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'CI_CONTEO_INVENTARIO',N'Permite el registro de los inventarios de planta.')
	END
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='CI_CONTEO_INVENTARIO_ADMIN' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[descripcion])VALUES(LOWER(NEWID()) ,'CI_CONTEO_INVENTARIO_ADMIN',N'Permite cargar cat�logos de Inventarios')
	END
	

PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO             

