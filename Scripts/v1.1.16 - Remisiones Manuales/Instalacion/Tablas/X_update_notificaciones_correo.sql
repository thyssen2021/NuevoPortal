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

	--USUARIOS PUEBLA
	SET @CLAVE_NOTIFICACION ='RM_PUEBLA'
	SET @ID_EMPLEADO= 149; --JULIO CESAR CUAUTLE FLORES
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END
	
	SET @ID_EMPLEADO= 176; --USIEL OTAÑEZ NARVAEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 436; --EDGAR IVAN ZARATE CRUZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 140; --GERARDO SALVADOR CENDEJAS NEGRETE
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 161; --JOSE ARMANDO CORDOVA HIDALGO
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 159; --SERGIO FLORES SANCHEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 255; --PABLO SANCHEZ VELEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 152; --ALFONSO MIRON LOPEZ	
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 190; --ALEJANDRO ISMAEL HERRERA RODRIGUEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 191; --ANDRES MARQUEZ JIMENEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 192; --JUAN DORANTES GOMEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 103; --ERICK DIAZ REYES
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 163; --JESUS ANDRES LOPEZ QUEZADA
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

    --USUARIOS SILAO
	SET @CLAVE_NOTIFICACION ='RM_SILAO'
	SET @ID_EMPLEADO= 309; --BALTAZAR CHAGOYA AGUILAR
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 313; --ALEJANDRO HERNANDEZ VELAZQUEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 315; --JOSE FABIAN CALDERON SABANERO
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 388; --JESUS EDUARDO BUENO RODRIGUEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 335; --VICTOR MANUEL HERNANDEZ RUIZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 322; --FRANCISCO EDUARDO HERNANDEZ PEREZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 320; --ALEJANDRO CANO RANGEL
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 306; --FRANCISCO JAVIER MUÑIZ MURILLO
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 392; --CLAUDIA ELIZABETH CASTELAN BANDA
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 326; --PEDRO DANIEL SORIA GONZALEZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 397; --JUANA MARIA MUÑIZ PEREZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 377; --LINO ALCANTAR CADENA
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 398; --MARTHA MERCEDES VIEJAR CASTILLO
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 314; --JOSE GUADALUPE VILLANUEVA CAUDILLO
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 327; --LUIS FERNANDO VACA RAMIREZ
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 364; --HUGO CESAR RAMIREZ MIRANDA
	IF NOT EXISTS (SELECT * FROM [dbo].[notificaciones_correo] where id_empleado=@ID_EMPLEADO AND descripcion = @CLAVE_NOTIFICACION )
	BEGIN
		INSERT INTO [dbo].[notificaciones_correo]([id_empleado],[descripcion],[activo])VALUES(@ID_EMPLEADO, @CLAVE_NOTIFICACION,1)
	END

	SET @ID_EMPLEADO= 348; --PEDRO PATIÑO MOSQUEDA
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

