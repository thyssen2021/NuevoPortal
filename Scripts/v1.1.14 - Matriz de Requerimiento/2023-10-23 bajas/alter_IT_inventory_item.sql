--use Portal_2_0
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  21/03/2023	Alfredo Xochitemol		 Se aumenta tamaño de campos
******************************************************************************/

--BEGIN TRANSACTION
		IF object_id(N'IT_inventory_items','U') IS NOT NULL
		BEGIN
			
		ALTER TABLE dbo.IT_inventory_items ADD last_check_int datetime NULL;
		ALTER TABLE dbo.IT_inventory_items ADD os_version varchar(30) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD primary_user varchar(100) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD primary_user_email varchar(100) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD primary_user_display varchar(100) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD compliance bit NULL;
		ALTER TABLE dbo.IT_inventory_items ADD managed_by varchar(20) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD [encrypted] bit NULL;
		ALTER TABLE dbo.IT_inventory_items ADD joinType varchar(30) NULL;
		ALTER TABLE dbo.IT_inventory_items ADD management_certificate_expiration_date datetime NULL;


		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_matriz_asignaciones NO EXISTE, no se puede crear las columnas'
		END

--COMMIT TRANSACTION

