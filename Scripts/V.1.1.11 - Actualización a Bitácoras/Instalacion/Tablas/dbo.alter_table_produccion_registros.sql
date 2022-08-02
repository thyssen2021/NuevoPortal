
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
*  27/07/2022	Alfredo Xochitemol		Agrega campo para definir el sap platina2
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'produccion_registros','U') IS NOT NULL
		BEGIN
			ALTER TABLE produccion_registros ADD sap_platina_2 VARCHAR(30) NULL
			
			PRINT 'Se ha creado la columna sap_platina_2 en la tabla produccion_registros'	
		END
		ELSE
		BEGIN
			PRINT 'La tabla produccion_registros NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO