
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
*  16/02/2022	Alfredo Xochitemol		Agrega campo para definir el área del empleado
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'empleados','U') IS NOT NULL
		BEGIN
			ALTER TABLE empleados ADD id_area INT NULL
			PRINT 'Se ha creado la columna id_area en la tabla empleados'
			
			  -- restriccion de clave foranea
			alter table [empleados]
			 add constraint FK_empleados_area
			  foreign key (id_area)
			  references Area(clave);
		END
		ELSE
		BEGIN
			PRINT 'La tabla empleados NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
