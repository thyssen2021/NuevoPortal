SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id(N'sp_registra_evento_excepcion','P') IS NOT NULL
BEGIN
      DROP PROCEDURE [dbo].[sp_registra_evento_excepcion]
      PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_evento_excepcion] en Base de Datos:' + 
	  db_name() + ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'
END
GO
CREATE PROCEDURE [dbo].[sp_registra_evento_excepcion]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Inserta una excepción no controlada en BD
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 09/09/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(  
      @IdUsuario VARCHAR(15) ,  
      @TipoEvento CHAR(1) ,  
	  @Fecha DATETIME,
      @Origen VARCHAR(200) ,  
      @Descripcion NVARCHAR(4000) ,  
      @Gravedad TINYINT = 0 ,  
      @NumeroError INT = 0 
 )  
AS   
    BEGIN  
    -- Insert statements for procedure here  
   
        INSERT  INTO registro_eventos  
                ( IdUsuario ,  
                  FechaEvento ,  
                  TipoEvento ,  
                  Origen ,  
                  Descripcion ,  
                  Gravedad ,  
                  NumeroError  
                )  
        VALUES  ( @IdUsuario ,  
                  @Fecha ,  
                  @TipoEvento ,  
                  @Origen ,  
                  @Descripcion ,  
                  @Gravedad ,  
                  @NumeroError  
                )     
    END  
GO
	IF object_id(N'sp_registra_evento_excepcion','P') IS NOT NULL
	BEGIN
		PRINT '<<< STORED PROCEDURE [dbo].[sp_registra_evento_excepcion] en Base de Datos:' + 
		db_name() + ' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el STORED PROCEDURE [dbo].[sp_registra_evento_excepcion] en ' +
		  'la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END