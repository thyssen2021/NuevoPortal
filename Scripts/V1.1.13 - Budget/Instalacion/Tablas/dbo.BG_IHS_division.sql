USE Portal_2_0
GO
IF object_id(N'BG_IHS_division',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_division]
      PRINT '<<< BG_IHS_division en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_division
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/26
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_division](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_ihs_item][int] NOT NULL,
	[comentario][varchar](150) NULL,
	[porcentaje_scrap][decimal](3,2) NULL,
	[activo][bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_BG_IHS_division] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [BG_IHS_division]
 add constraint FK_BG_IHS_division_id_ihs_item
  foreign key (id_ihs_item)
  references BG_IHS_item(id);
 	  
IF object_id(N'BG_IHS_division',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_division en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_division  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

