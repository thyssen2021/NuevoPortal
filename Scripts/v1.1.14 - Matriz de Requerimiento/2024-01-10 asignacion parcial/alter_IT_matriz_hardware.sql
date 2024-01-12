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
		IF object_id(N'IT_matriz_hardware','U') IS NOT NULL
		BEGIN
			
		ALTER TABLE dbo.IT_matriz_hardware 
			ADD asignado_a int null; --empleado		 

			alter table [IT_matriz_hardware]
			add constraint FK_IT_matriz_hardware_asignado_a
			foreign key (asignado_a)
			references empleados(id);

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_matriz_asignaciones NO EXISTE, no se puede crear las columnas'
		END

--COMMIT TRANSACTION

GO



