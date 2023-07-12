use Portal_2_0
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
*  21/03/2023	Alfredo Xochitemol		 Se agrega campo para placa  de plataforma 
*	 									 1 y 2.
******************************************************************************/

--select * from RM_cabecera

BEGIN TRANSACTION
		IF object_id(N'RM_cabecera','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE RM_cabecera ADD proveedorClave int NULL
			PRINT 'Se ha creado el campo proveedorClave en la tabla RM_cabecera'	

			  -- restriccion de clave foranea
			alter table [dbo].[RM_cabecera]
			add constraint FK_RM_cabecera_proveedorClave
			foreign key (proveedorClave)
	        references proveedores(clave);
			
			ALTER TABLE RM_cabecera ADD proveedorOtro varchar(50) NULL
			PRINT 'Se ha creado el campo proveedorOtro en la tabla RM_cabecera'			

			ALTER TABLE RM_cabecera ADD proveedorOtroDireccion varchar(100) NULL
			PRINT 'Se ha creado el campo proveedorOtroDireccion en la tabla RM_cabecera'		
			
			ALTER TABLE RM_cabecera ADD EnviadoAProveedorClave int NULL
			PRINT 'Se ha creado el campo EnviadoAProveedorClave en la tabla RM_cabecera'	

			--cliente otro a null
			ALTER TABLE RM_cabecera ALTER COLUMN clienteOtro varchar(50) NULL
			
			--cliente otro direccion a null
			ALTER TABLE RM_cabecera ALTER COLUMN clienteOtroDireccion varchar(100) NULL
			
			  -- restriccion de clave foranea
			alter table [dbo].[RM_cabecera]
			add constraint FK_RM_cabecera_EnviadoAProveedorClave
			foreign key (EnviadoAProveedorClave)
	        references proveedores(clave);


		END
		ELSE
		BEGIN
			PRINT 'La tabla RM_cabecera NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

