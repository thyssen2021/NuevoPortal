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
*  02/05/2023	Alfredo Xochitemol		 Se agrega campo para comentarios descripcion de Rol
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'AspNetRoles','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE AspNetRoles ADD descripcion varchar(100) NULL
			PRINT 'Se ha creado el campo descripcion en la tabla AspNetRoles'			
			
		END
		ELSE
		BEGIN
			PRINT 'La tabla AspNetRoles NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

