SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_registra_intento_envio_birthday','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_registra_intento_envio_birthday]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_intento_envio_birthday] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_registra_intento_envio_birthday]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Obtiene los correos de cumplea�os
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creaci�n: 10/12/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(
	 @Correo VARCHAR(255)
) 
AS   
	DECLARE @Fecha DATETIME = GETDATE()

	--Si no existe un registro lo crea
	IF NOT EXISTS(SELECT * FROM log_envio_correo WHERE email=@Correo AND DAY(fecha) = DAY(@Fecha) AND MONTH(fecha) = MONTH(@Fecha) 	AND YEAR(fecha) = YEAR(@Fecha) )		
	BEGIN
			INSERT INTO [dbo].[log_envio_correo]
			   ([email]
			   ,[categoria]
			   ,[fecha]
			   ,[intentos_envio]
			   ,[mensaje]
			   ,[enviado])
			 VALUES
				   (@Correo
				   ,'Birthday'
				   ,@Fecha
				   ,1
				   ,null
				   ,0)
	END
		ELSE
		--En caso de que ya exista actualiza los intentos en uno
		BEGIN
			UPDATE [dbo].[log_envio_correo] set intentos_envio = (intentos_envio +1) 
						WHERE 
								email=@Correo 
								AND DAY(fecha) = DAY(@Fecha) 
								AND MONTH(fecha) = MONTH(@Fecha) 
								AND YEAR(fecha) = YEAR(@Fecha) 

		END
  
GO
	IF object_id(N'sp_registra_intento_envio_birthday','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_intento_envio_birthday] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_registra_intento_envio_birthday] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END