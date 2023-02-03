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
*  03/02/2022	Alfredo Xochitemol		 Se agrega tipo de solicitud
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_matriz_requerimientos','U') IS NOT NULL
		BEGIN
			

			ALTER TABLE IT_matriz_requerimientos ADD tipo varchar(20) NOT NULL DEFAULT 'CREACION'
			PRINT 'Se ha creado la columna tipo en la tabla IT_matriz_requerimientos'
			-- restricion check
			ALTER TABLE [IT_matriz_requerimientos] ADD CONSTRAINT CK_it_matriz_requerimientos_tipo CHECK ([tipo] IN 
			('CREACION', 'MODIFICACION')
			)
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_inventory_hardware_type NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO




