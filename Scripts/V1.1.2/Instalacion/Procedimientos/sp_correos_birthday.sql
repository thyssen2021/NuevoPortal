SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_correos_birthday','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_correos_birthday]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_correos_birthday] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_correos_birthday]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Registra un intento de envío o aumenta los intentos en uno
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 10/12/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/

AS   
    BEGIN  
    -- Insert statements for procedure here  
           
			SELECT 
				e.correo,
				e.nombre,
				e.apellido1,
				e.apellido2
			FROM empleados as e
			WHERE 
				DAY(e.nueva_fecha_nacimiento) = DAY(GETDATE()) AND MONTH(e.nueva_fecha_nacimiento) = MONTH(GETDATE())
				AND NOT EXISTS(
				-- regresa falso en caso de que ya se haya enviado el correo el día de hoy
							SELECT * FROM log_envio_correo 
							WHERE 
								email=e.correo 
								AND (enviado=1 OR intentos_envio>=3)								 
								AND YEAR(fecha) = YEAR(GETDATE()) 
							)
    END  
GO
	IF object_id(N'sp_correos_birthday','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_correos_birthday] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_correos_birthday] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END