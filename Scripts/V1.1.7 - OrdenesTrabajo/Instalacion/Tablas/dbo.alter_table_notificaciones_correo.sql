
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
*  18/04/2022	Alfredo Xochitemol		Se agrega campo para manipular los correos especiales
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'notificaciones_correo','U') IS NOT NULL
		BEGIN
			ALTER TABLE notificaciones_correo ADD correo VARCHAR(70) NULL
			PRINT 'Se ha creado la columna correo en la tabla notificaciones_correo'

			ALTER TABLE notificaciones_correo ADD clave_planta Int NULL
			PRINT 'Se ha creado la columna clave_planta en la tabla notificaciones_correo'

			  -- restriccion de clave foranea
			alter table [notificaciones_correo]
			 add constraint FK_notificaciones_correo_planta
			  foreign key (clave_planta)
			  references plantas(clave);
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla notificaciones_correo NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO
