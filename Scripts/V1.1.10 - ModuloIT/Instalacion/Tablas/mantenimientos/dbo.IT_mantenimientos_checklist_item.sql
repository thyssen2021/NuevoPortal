USE Portal_2_0
GO
IF object_id(N'IT_mantenimientos_checklist_item',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_mantenimientos_checklist_item]
      PRINT '<<< IT_mantenimientos_checklist_item en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_mantenimientos_checklist_item
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/02
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_mantenimientos_checklist_item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_IT_mantenimientos_checklist_categorias][int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_mantenimientos_checklist_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_mantenimientos_checklist_item] ADD  CONSTRAINT [DF_IT_mantenimientos_checklist_item_activo]  DEFAULT (1) FOR [activo]
GO

  -- restriccion de clave foranea
alter table [IT_mantenimientos_checklist_item]
 add constraint FK_IT_mantenimientos_checklist_item_id_categoria
  foreign key (id_IT_mantenimientos_checklist_categorias)
  references IT_mantenimientos_checklist_categorias(id);	

SET IDENTITY_INSERT [IT_mantenimientos_checklist_item] ON 

INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (1, N'Vaciar Papelera de Reciclaje',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (2, N'Validación de actualizaciones pendientes',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (3, N'Eliminación de archivos temporales',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (4, N'Depuración de unidad C:\ (limpieza de disco)',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (5, N'Revisión de disco duro (chkdsk C:\)',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (6, N'Depuración de programas',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (7, N'Actualizar antivirus',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (8, N'Papel tapiz de Thyssenkupp',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (9, N'Realizar cambio de contraseña',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (10, N'Eliminar tienda y aplicaciones de Microsoft Store',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (11, N'Tipografía de Thyssenkrupp Instalada',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (12, N'Otro',1, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (13, N'Eliminar temporales',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (14, N'Eliminar historias de navegación',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (15, N'Eliminar cookies',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (16, N'Eliminar extensiones no útiles',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (17, N'Eliminar descargas',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (18, N'Extensiones de AdBlock y McAfee',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (19, N'Chrome como navegador de default',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (20, N'Otro',2, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (21, N'Revisión física',3, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (22, N'Sopleatear PC',3, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (23, N'Limpieza de carcasa',3, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (24, N'Limpieza de monitor',3, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (25, N'Limpieza de teclado y mouse',3, 1)
INSERT [IT_mantenimientos_checklist_item] ([id], [descripcion], [id_IT_mantenimientos_checklist_categorias], [activo]) VALUES (26, N'Otro',3, 1)

SET IDENTITY_INSERT [IT_mantenimientos_checklist_item] OFF


GO


 	  
IF object_id(N'IT_mantenimientos_checklist_item',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_mantenimientos_checklist_item en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_mantenimientos_checklist_item  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
