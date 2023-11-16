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
*  09/11/2023	Alfredo Xochitemol		 Se agrega indicador de si es suma
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'budget_rel_comentarios','U') IS NOT NULL
		BEGIN			
			
			ALTER TABLE budget_rel_comentarios ALTER COLUMN comentarios varchar(200)
			PRINT 'Se ha creado la columna aplica_formula en la tabla budget_rel_comentarios'		

			

		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_rel_comentarios NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION


GO


