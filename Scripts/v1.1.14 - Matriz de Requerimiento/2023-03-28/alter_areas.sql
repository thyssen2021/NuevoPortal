--use Portal_2_0
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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para comentarios tkmmnet
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'Area','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE Area ADD numero_centro_costo varchar(6) NULL
			PRINT 'Se ha creado el campo numero_centro_costo en la tabla Area'		

			ALTER TABLE Area ADD shared_services bit DEFAULT 0 NOT NULL 
			PRINT 'Se ha creado el campo numero_centro_costo en la tabla Area'		
		END
		ELSE
		BEGIN
			PRINT 'La tabla Area NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

