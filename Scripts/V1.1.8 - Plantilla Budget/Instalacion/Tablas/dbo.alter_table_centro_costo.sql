use[Portal_2_0]
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
*  31/03/2022	Alfredo Xochitemol		Agrega campo para definir class 1 y class 2
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'budget_centro_costo','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE budget_centro_costo ADD class_1 VARCHAR(30) NULL
			PRINT 'Se ha creado la columna class_1 en la tabla budget_centro_costo'
			
			ALTER TABLE budget_centro_costo ADD class_2 VARCHAR(30) NULL
			PRINT 'Se ha creado la columna class_2 en la tabla budget_centro_costo'

			ALTER TABLE budget_centro_costo ADD activo bit NOT NULL DEFAULT 1
			PRINT 'Se ha creado la columna activo en la tabla budget_centro_costo'
			 
		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_centro_costo NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
