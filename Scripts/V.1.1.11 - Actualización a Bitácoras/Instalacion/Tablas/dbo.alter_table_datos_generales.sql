
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
*  17/07/2022	Alfredo Xochitemol		Agrega campo para definir el sap platina2
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'produccion_datos_entrada','U') IS NOT NULL
		BEGIN
			ALTER TABLE produccion_datos_entrada ADD sap_platina_2 VARCHAR(30) NULL
			PRINT 'Se ha creado la columna sap_platina_2 en la tabla produccion_datos_entrada'	
		END
		ELSE
		BEGIN
			PRINT 'La tabla produccion_datos_entrada NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
