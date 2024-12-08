USE [Portal_2_0]

IF object_id(N'biblioteca_digital',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[biblioteca_digital]
      PRINT '<<< biblioteca_digital en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para almacenar archivos del portal
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 11/30/2021
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/
GO

/****** Object:  Table [dbo].[biblioteca_digital]    Script Date: 13/02/2021 08:54:08 a. m. ******/


CREATE TABLE [dbo].[biblioteca_digital](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](80) NOT NULL,
	[MimeType] [nvarchar](80) NOT NULL,
	[Datos] [varbinary](max) NOT NULL,
 CONSTRAINT [pk_biblioteca_digital] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF object_id(N'biblioteca_digital',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< biblioteca_digital en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear el biblioteca_digital  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
