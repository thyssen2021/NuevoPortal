--use[Portal_2_0]
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
*  22/02/2023	Alfredo Xochitemol		 Se agrega comentario para la cantidadad
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'budget_cantidad','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE budget_cantidad ADD comentario VARCHAR(150) NULL
			PRINT 'Se ha creado la columna class_1 en la tabla budget_cantidad'			
			
			 
		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_cantidad NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
