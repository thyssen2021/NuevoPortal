--use [Portal_2_0]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*****************************************************************************
*  Tipo de objeto:       Table
*  Funcion:				 Inserta datos de la tabla de notificaciones Remisiones
*  Autor :				 Alfredo Xochitemol Cruz
*  Fecha de Creación:	 11/05/2023
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  12/05/2022	Alfredo Xochitemol		 Se agregan notificaciones a Remisiones Manuales
******************************************************************************/

IF object_id(N'notificaciones_correo','U') IS NOT NULL
BEGIN

DECLARE @CLAVE_NOTIFICACION AS VARCHAR(100)='RM_SALTILLO'
DECLARE @ID_EMPLEADO AS INT=0

	--USUARIOS SALTILLO
	SET @ID_EMPLEADO= 425; --JORGE GUADALUPE SALINAS GALVAN
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 498; --MONICA RUIZ GARCIA
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 430; --ROBERTO MUZQUIZ DURAN
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END			

	SET @ID_EMPLEADO= 426; --LUIS ARMANDO FERNANDEZ RANGEL
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END



PRINT '<<<CORRECTO: La TABLA dbo.notificaciones_correo ha sido INICIALIZADA en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'     	

END
ELSE
	BEGIN
		PRINT '<<<ERROR: NO ha sido posible INICIALIZAR la TABLA notificaciones_correo en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
GO              

