
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
*  23/01/2024	Alfredo Xochitemol		Agrega campo para definir pieza doble
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'mm_v3','U') IS NOT NULL
		BEGIN
			--ALTER TABLE mm_v3 DROP COLUMN sap_platina_2;

			ALTER TABLE mm_v3 ADD num_piezas_golpe int NULL
			
			PRINT 'Se ha creado la columna sap_platina_2 en la tabla mm_v3'	
		END
		ELSE
		BEGIN
			PRINT 'La tabla mm_v3 NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO