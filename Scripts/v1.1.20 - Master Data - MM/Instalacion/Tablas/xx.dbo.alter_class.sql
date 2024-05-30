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
*  21/03/2023	Alfredo Xochitemol		 Se aumenta tamaño de campos
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'plantas','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE plantas ADD aplica_solicitud_scdm bit default 1;
			PRINT 'Se ha agregado la columna aplica_solicitud_scds'		
		
		END
		ELSE
		BEGIN
			PRINT 'La tabla plantas NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

