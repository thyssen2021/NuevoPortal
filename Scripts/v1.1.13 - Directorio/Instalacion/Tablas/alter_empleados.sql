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
*  23/08/2022	Alfredo Xochitemol		 Se agrega campo para comentarios 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'empleados','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE empleados ADD id_fotografia INT NULL
			PRINT 'Se ha creado la columna id_fotografia en la tabla empleados'

			-- restriccion de clave foranea
			alter table empleados
			add constraint FK_empleados_id_fotografia
			foreign key (id_fotografia)
			references biblioteca_digital(id);

			ALTER TABLE empleados ADD mostrar_telefono bit NOT NULL DEFAULT 1
			PRINT 'Se ha creado la columna mostrar_telefono en la tabla empleados'
					   

		END
		ELSE
		BEGIN
			PRINT 'La tabla empleados NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

