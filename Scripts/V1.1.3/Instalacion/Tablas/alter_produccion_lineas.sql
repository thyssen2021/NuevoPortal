
GO
use Portal_2_0
/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 26/10/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  28/06/2021	Alfredo Xochitemol		Agrega campo para direccion ip
******************************************************************************/


		IF object_id(N'dbo.produccion_lineas','U') IS NOT NULL
		BEGIN			

			-- Se agrega nueva columna para nueva_fecha_nacimiento
			ALTER TABLE dbo.produccion_lineas ADD [ip] VARCHAR(25) NULL 
			PRINT 'Se ha agregado la columna ip a la tabla catalogo.empleado'
		
		END
		ELSE
		BEGIN
			PRINT 'La tabla catalogo.Empleado NO EXISTE, no se puede crear las columnas'
		END
		
GO


