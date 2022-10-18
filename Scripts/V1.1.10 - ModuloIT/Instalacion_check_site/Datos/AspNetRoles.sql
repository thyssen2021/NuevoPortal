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
	--permiso para gestionar solicitudes de usuario
	IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] where Name='IT_Check_site' )
	BEGIN
		INSERT INTO [dbo].[AspNetRoles]([Id],[Name])VALUES(LOWER(NEWID()) ,'IT_Check_site')
	END


PRINT '<<<CORRECTO: La TABLA dbo.AspNetRoles ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA AspNetRoles en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
