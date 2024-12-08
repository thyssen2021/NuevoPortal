use [Portal_2_0]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*****************************************************************************
*  Tipo de objeto:       Table
*  Funcion:				 Inserta datos de la tabla notificaciones_correo
*  Autor :				 Alfredo Xochitemol Cruz
*  Fecha de Creación:	 02/06/2022
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  12/05/2022	Alfredo Xochitemol		 Se agregan roles para módulo de IT
******************************************************************************/

IF object_id(N'notificaciones_correo','U') IS NOT NULL
BEGIN

	--------------- MODULO IT - NOTIFICACION SOLICITUDES ------------------------
	--permiso para gestionar solicitudes de usuario
	DECLARE @ID int

	SELECT @ID = (Select id from empleados where numeroEmpleado = '20368');

	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where descripcion='IT_SOLICITUD_PORTAL' AND id_empleado = @ID )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado], [descripcion],[activo])VALUES(@ID,'IT_SOLICITUD_PORTAL',1)
	END			

PRINT '<<<CORRECTO: La TABLA dbo.notificaciones_correo ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA notificaciones_correo en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              
