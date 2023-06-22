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


BEGIN TRANSACTION
		IF object_id(N'clientes','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE clientes ADD ciudad varchar(120) NULL
			PRINT 'Se ha creado el campo ciudad en la tabla clientes'	
			
			ALTER TABLE clientes ADD codigo_postal varchar(15) NULL
			PRINT 'Se ha creado el campo codigo_postal en la tabla clientes'			
			
			ALTER TABLE clientes ADD calle varchar(120) NULL
			PRINT 'Se ha creado el campo calle en la tabla clientes'	
			
			ALTER TABLE clientes ADD estado varchar(6) NULL
			PRINT 'Se ha creado el campo estadi en la tabla clientes'	

		END
		ELSE
		BEGIN
			PRINT 'La tabla clientes NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

