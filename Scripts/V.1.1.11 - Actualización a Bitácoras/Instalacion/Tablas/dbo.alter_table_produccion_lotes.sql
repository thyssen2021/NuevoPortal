
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
*  17/07/2022	Alfredo Xochitemol		Agrega campo para definir marerial
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'produccion_lotes','U') IS NOT NULL
		BEGIN
			ALTER TABLE produccion_lotes ADD sap_platina VARCHAR(30) NULL
			PRINT 'Se ha creado la columna sap_platina en la tabla produccion_lotes'		
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla produccion_lotes NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
