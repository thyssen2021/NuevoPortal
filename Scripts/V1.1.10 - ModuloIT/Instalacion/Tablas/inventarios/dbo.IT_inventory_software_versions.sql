USE Portal_2_0
GO
IF object_id(N'IT_inventory_software_versions',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_software_versions]
      PRINT '<<< IT_inventory_software_versions en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_software_versions
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_software_versions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_inventory_software] [int] NOT NULL,
	[version][varchar](50) NOT NULL,
	[activo] [bit] NOT NULL DEFAULT 1, 
 CONSTRAINT [PK_IT_inventory_software_versions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_inventory_software_versions]
 add constraint FK_IT_inventory_id_inventory_software
  foreign key (id_inventory_software)
  references IT_inventory_software(id);
  

-- restriccion unique
 alter table [IT_inventory_software_versions]
  add constraint UQ_inventory_version
  unique (id_inventory_software,[version]);
GO

USE [Portal_2_0]
GO
SET IDENTITY_INSERT [dbo].[IT_inventory_software_versions] ON 
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (1, 12, N'FULL EDITION 2022', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (2, 12, N'LT', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (3, 1, N'2021', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (4, 2, N'19.3.0.9', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (5, 3, N'4.0', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (6, 4, N'42.1.0', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (7, 5, N'41.7', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (8, 6, N'6.2', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (9, 7, N'2016', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (10, 8, N'2013', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (11, 9, N'2013', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (12, 9, N'2016', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (13, 13, N'5.0.07', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (14, 14, N'4.9.04', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (15, 15, N'3.3', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (16, 16, N'2.3', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (17, 19, N'6.4-4', 1)
GO
INSERT [dbo].[IT_inventory_software_versions] ([id], [id_inventory_software], [version], [activo]) VALUES (18, 20, N'2021', 1)
GO
SET IDENTITY_INSERT [dbo].[IT_inventory_software_versions] OFF
GO

 	  
IF object_id(N'IT_inventory_software_versions',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_software_versions en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_software_versions  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
