
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
*  30/06/2022	Alfredo Xochitemol		Se agrega campo de descripcion
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_inventory_items','U') IS NOT NULL
		BEGIN
			ALTER TABLE IT_inventory_items ADD descripcion VARCHAR(120) NULL
			PRINT 'Se ha creado la columna descripcion en la tabla IT_inventory_items'			
			ALTER TABLE IT_inventory_items ADD physical_server int NULL
			PRINT 'Se ha creado la columna descripcion en la tabla IT_inventory_items'			
			
			  -- restriccion de clave foranea
			alter table IT_inventory_items
			 add constraint FK_notificaciones_virtual_server
			  foreign key (physical_server)
			  references IT_inventory_items(id);
		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_items NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
