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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para comentarios tkmmnet
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'log_envio_correo','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE log_envio_correo ADD id_it_notificacion int NULL
			PRINT 'Se ha creado el campo numero_centro_costo en la tabla Area'		

			  -- restriccion de clave foranea
			alter table log_envio_correo
			 add constraint FK_log_envio_correo_id_it_notificacion
			  foreign key (id_it_notificacion)
			  references IT_notificaciones_recordatorio(id);
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla log_envio_correo NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

