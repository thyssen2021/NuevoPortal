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
*  07/09/2022	Alfredo Xochitemol		 Se agrega campo para fecha de renovación (fin)
*  18/10/2022   Alfredo Xochitemol		 Se agrega campo para fecha de renovación (inicio) 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_inventory_cellular_line','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE IT_inventory_cellular_line ADD fecha_renovacion_inicio DATETIME NULL
			PRINT 'Se ha creado la columna fecha_renovacion en la tabla IT_inventory_cellular_line'

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_cellular_line NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

