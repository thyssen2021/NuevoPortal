--USE Portal_2_0
GO
IF object_id(N'RM_transporte_proveedor',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_transporte_proveedor]
      PRINT '<<< RM_transporte_proveedor en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_transporte_proveedor
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_transporte_proveedor](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TransporteProveedor] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RM_transporte_proveedor] ON 
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (1, 1, N'TRANSPORTES 4DL S.A. DE C.V.')
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (2, 1, N'TRANSPORTES S D C S. DE R.L. DE C.V')
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (3, 1, N'MULTITRASLADOS INTERNACIONALES')
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (4, 1, N'TRANSPORTES AXIS S.A. DE C.V.')
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (5, 1, N'LOGISTICA GUTIERREZ S. DE R.L. DE C')
GO
INSERT [dbo].[RM_transporte_proveedor] ([clave], [activo], [descripcion]) VALUES (6, 1, N'JESUS ALBERTO ORTIZ GAONA')
GO
SET IDENTITY_INSERT [dbo].[RM_transporte_proveedor] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[RM_transporte_proveedor] OFF
	  
IF object_id(N'RM_transporte_proveedor',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_transporte_proveedor en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_transporte_proveedor  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
