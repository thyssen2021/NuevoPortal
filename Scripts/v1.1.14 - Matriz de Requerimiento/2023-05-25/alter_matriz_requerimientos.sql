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
*  21/03/2023	Alfredo Xochitemol		 Se aumenta tamaño de campos
******************************************************************************/

BEGIN TRANSACTION
		IF object_id(N'IT_matriz_asignaciones','U') IS NOT NULL
		BEGIN
			
			ALTER TABLE IT_matriz_asignaciones ALTER COLUMN comentario VARCHAR (350);
			PRINT 'Se ha cambiado la longitud del campo COMENTARIO en la tabla IT_matriz_asignaciones'		

			ALTER TABLE IT_matriz_hardware ALTER COLUMN comentario VARCHAR (250);
			PRINT 'Se ha cambiado la longitud del campo COMENTARIO en la tabla IT_matriz_hardware'		
		
			ALTER TABLE IT_matriz_software ALTER COLUMN comentario VARCHAR (250);
			PRINT 'Se ha cambiado la longitud del campo COMENTARIO en la tabla IT_matriz_software'		

			ALTER TABLE IT_matriz_comunicaciones ALTER COLUMN comentario VARCHAR (250);
			PRINT 'Se ha cambiado la longitud del campo COMENTARIO en la tabla IT_matriz_comunicaciones'		

		END
		ELSE
		BEGIN
			PRINT 'La tabla IT_matriz_asignaciones NO EXISTE, no se puede crear las columnas'
		END

COMMIT TRANSACTION

GO

