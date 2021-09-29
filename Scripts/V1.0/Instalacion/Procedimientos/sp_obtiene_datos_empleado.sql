SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_obtiene_datos_empleado','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_obtiene_datos_empleado]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_obtiene_datos_empleado] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_obtiene_datos_empleado]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Obtiene los datos de un emplado por num de empleado
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 09/15/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(  
      @numEmpleado VARCHAR(255)
 )  
AS   
    BEGIN  
    -- Insert statements for procedure here  
   
       SELECT [clave]
		  ,[activo]
		  ,[numeroEmpleado]
		  ,[nombre]
		  ,[apellido1]
		  ,[apellido2]
		  ,[nacimientoFecha]
		  ,[correo]
		  ,[telefono]
		  ,[extension]
		  ,[celular]
		  ,[nivel]
		  ,[puesto]
		  ,[compania]
		  ,[ingresoFecha]
		  ,[bajaFecha]
	  FROM [Empleado]
	  WHERE numeroEmpleado = @numEmpleado
    END  
GO
	IF object_id(N'sp_obtiene_datos_empleado','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_obtiene_datos_empleado] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_obtiene_datos_empleado] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END