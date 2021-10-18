SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_registra_mensaje_envio_correo','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_registra_mensaje_envio_correo]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_mensaje_envio_correo] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_registra_mensaje_envio_correo]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Obtiene los correos de cumpleaños
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 10/15/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(
	 @Correo VARCHAR(255),
	 @Mensaje VARCHAR(100),
	 @Exito BIT --default 
) 
AS   
	DECLARE @Fecha DATETIME = GETDATE()

	UPDATE [dbo].[log_envio_correo] set mensaje = @Mensaje, enviado=@Exito
			WHERE 
				email=@Correo 
				AND DAY(fecha) = DAY(@Fecha) 
				AND MONTH(fecha) = MONTH(@Fecha) 
				AND YEAR(fecha) = YEAR(@Fecha)   
GO
	IF object_id(N'sp_registra_mensaje_envio_correo','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_mensaje_envio_correo] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_registra_mensaje_envio_correo] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END