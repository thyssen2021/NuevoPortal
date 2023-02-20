use Portal_2_0
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
*  10/01/2023	 Alfredo Xochitemol		 Se agrega campo para dar de baja un equipo	
* 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_inventory_items','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE IT_inventory_items ADD baja bit NOT NULL DEFAULT 0
			PRINT 'Se ha creado la columna baja en la tabla IT_inventory_items'
			ALTER TABLE IT_inventory_items ADD fecha_baja DATETIME NULL
			PRINT 'Se ha creado la columna fecha baja en la tabla IT_inventory_items'

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_items NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

