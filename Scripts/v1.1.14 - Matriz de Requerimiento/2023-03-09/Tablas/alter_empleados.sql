--use Portal_2_0
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creaci�n: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  23/08/2022	Alfredo Xochitemol		 Se agrega campo para comentarios 
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'empleados','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE empleados ADD sexo char(1) NULL
			PRINT 'Se ha creado el campo sexo en la tabla empleados'
							   

		END
		ELSE
		BEGIN
			PRINT 'La tabla empleados NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

