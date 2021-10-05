
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 22/09/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
******************************************************************************/


		IF object_id(N'empleados','U') IS NOT NULL
		BEGIN
			ALTER TABLE empleados ADD [8ID] Varchar(8) NULL			
			PRINT 'Se ha agregado la columna "[8ID]" a empleados'
		END
		ELSE
		BEGIN
			PRINT 'La tabla empleadosa NO EXISTE, no se puede crear las columnas'
		END

GO


