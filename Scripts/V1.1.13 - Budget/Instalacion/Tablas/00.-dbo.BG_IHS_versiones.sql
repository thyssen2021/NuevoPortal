--USE Portal_2_0
GO
IF object_id(N'BG_IHS_versiones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_versiones]
      PRINT '<<< BG_IHS_versiones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_versiones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_versiones](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[periodo] [datetime] NOT NULL,
	[nombre] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_BG_IHS_versiones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
 	  
IF object_id(N'BG_IHS_versiones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_versiones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_versiones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

