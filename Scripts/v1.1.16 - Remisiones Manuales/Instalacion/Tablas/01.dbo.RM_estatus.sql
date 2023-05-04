--USE Portal_2_0
GO
IF object_id(N'RM_estatus',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_estatus]
      PRINT '<<< RM_estatus en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_estatus
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_estatus](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RM_Estatus_1] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[RM_estatus] ON 
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (1, 1, N'Creada')
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (2, 1, N'Editada')
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (3, 1, N'Aprobada')
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (4, 1, N'Regularizada')
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (5, 1, N'Cancelada')
GO
INSERT [dbo].[RM_estatus] ([clave], [activo], [descripcion]) VALUES (6, 1, N'Impresa')
GO

GO
SET IDENTITY_INSERT [dbo].[RM_estatus] OFF
	  
IF object_id(N'RM_estatus',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_estatus en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_estatus  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
