--USE Portal_2_0
GO
IF object_id(N'proveedores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[proveedores]
      PRINT '<<< proveedores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los proveedores
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[proveedores](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NULL,
	[claveSAP] [varchar](50) NULL,
	[descripcion] [varchar](100) NULL,
	[pais] [varchar](20) NULL,
	[ciudad] [varchar](120) NULL,
	[codigo_postal] [varchar](15) NULL,
	[calle][varchar](120) NULL,
	[estado][varchar](6) NULL,
 CONSTRAINT [PK_proveedores] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--inserta datos
SET IDENTITY_INSERT [dbo].[proveedores] ON 

SET IDENTITY_INSERT [dbo].[proveedores] OFF
GO

	  
IF object_id(N'proveedores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< proveedores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla proveedores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
