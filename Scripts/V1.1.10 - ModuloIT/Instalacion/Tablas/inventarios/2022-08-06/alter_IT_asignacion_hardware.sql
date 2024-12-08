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
		IF object_id(N'it_asignacion_hardware','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE it_asignacion_hardware ADD comentario_desasignacion VARCHAR (255) NULL
			PRINT 'Se ha creado la columna it_asignacion_hardware en la tabla it_asignacion_hardware'

		END
		ELSE
		BEGIN
			PRINT 'La tabla it_asignacion_hardware NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

