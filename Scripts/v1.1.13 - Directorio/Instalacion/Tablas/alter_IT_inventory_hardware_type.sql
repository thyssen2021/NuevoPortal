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
*  23/08/2022	Alfredo Xochitemol		 Se agrega campo para comentarios 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_inventory_hardware_type','U') IS NOT NULL
		BEGIN
			

			ALTER TABLE IT_inventory_hardware_type ADD disponible_en_matriz_rh bit NOT NULL DEFAULT 0
			PRINT 'Se ha creado la columna disponible_en_matriz_rh en la tabla IT_inventory_hardware_type'
			
			ALTER TABLE IT_inventory_hardware_type ADD aplica_descripcion bit NOT NULL DEFAULT 0
			PRINT 'Se ha creado la columna aplica_descripcion en la tabla IT_inventory_hardware_type'  

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_hardware_type NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

