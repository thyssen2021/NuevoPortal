--use Portal_2_0
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
*  21/03/2023	Alfredo Xochitemol		 Se aumenta tamaño de campos
******************************************************************************/

--BEGIN TRANSACTION
		IF object_id(N'empleados','U') IS NOT NULL
		BEGIN
			
		ALTER TABLE dbo.empleados 
			ADD tipo_empleado VARCHAR(20) NULL;

			--resticcion check
			ALTER TABLE dbo.empleados 
			ADD CONSTRAINT CK_Empleados_tipo_empleado CHECK (tipo_empleado='EMPLEADO' OR tipo_empleado='PRACTICANTE' OR tipo_empleado='PROVEEDOR');
		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_matriz_asignaciones NO EXISTE, no se puede crear las columnas'
		END

--COMMIT TRANSACTION

GO

select * from empleados
update empleados set tipo_empleado ='EMPLEADO' from empleados where numeroEmpleado <> 'N/A'

