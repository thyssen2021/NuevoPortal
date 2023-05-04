--USE Portal_2_0
GO
IF object_id(N'RM_almacen',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_almacen]
      PRINT '<<< RM_almacen en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_almacen
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_almacen](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[plantaClave] [int] NOT NULL,
	[responsableCorreoElectronico] [varchar](50) NULL,
 CONSTRAINT [PK_Almacen] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RM_almacen] ON 
GO
INSERT [dbo].[RM_almacen] ([clave], [activo], [descripcion], [plantaClave], [responsableCorreoElectronico]) VALUES (1, 1, N'PU01', 1, NULL)
GO
INSERT [dbo].[RM_almacen] ([clave], [activo], [descripcion], [plantaClave], [responsableCorreoElectronico]) VALUES (2, 1, N'PU02', 1, NULL)
GO
INSERT [dbo].[RM_almacen] ([clave], [activo], [descripcion], [plantaClave], [responsableCorreoElectronico]) VALUES (3, 1, N'SI01', 2, NULL)
GO
INSERT [dbo].[RM_almacen] ([clave], [activo], [descripcion], [plantaClave], [responsableCorreoElectronico]) VALUES (4, 1, N'SA01', 3, NULL)
GO
INSERT [dbo].[RM_almacen] ([clave], [activo], [descripcion], [plantaClave], [responsableCorreoElectronico]) VALUES (6, 1, N'CB01', 4, NULL)
GO
SET IDENTITY_INSERT [dbo].[RM_almacen] OFF

 -- restriccion de clave foranea
alter table [dbo].[RM_almacen]
 add constraint FK_RM_almacen_plantaClave
  foreign key (plantaClave)
  references plantas(clave);
	  
IF object_id(N'RM_almacen',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_almacen en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_almacen  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
