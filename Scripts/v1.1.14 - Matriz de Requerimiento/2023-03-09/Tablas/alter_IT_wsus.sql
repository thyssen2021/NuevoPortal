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
*  03/02/2022	Alfredo Xochitemol		 Se agrega tipo de solicitud
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_wsus','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE IT_wsus ADD fecha datetime NOT NULL DEFAULT GETDATE()
			PRINT 'Se ha creado la columna fecha en la tabla IT_wsus'
						
		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_wsus NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO




