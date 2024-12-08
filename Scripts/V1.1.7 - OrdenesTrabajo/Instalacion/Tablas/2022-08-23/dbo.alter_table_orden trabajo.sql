
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
*  23/08/2022	Alfredo Xochitemol		Se agrega campo para zona de maquina
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'orden_trabajo','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE orden_trabajo ADD id_zona_falla INT NULL
			PRINT 'Se ha creado la columna id_zona_falla en la tabla orden_trabajo'

			  -- restriccion de clave foranea
			alter table [orden_trabajo]
			 add constraint FK_orden_id_zona_falla
			  foreign key (id_zona_falla)
			  references OT_zona_falla(id);
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla orden_trabajo NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
