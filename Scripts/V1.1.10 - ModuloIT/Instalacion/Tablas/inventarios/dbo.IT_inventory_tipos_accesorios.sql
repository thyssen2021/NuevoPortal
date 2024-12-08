USE Portal_2_0
GO
IF object_id(N'IT_inventory_tipos_accesorios',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_tipos_accesorios]
      PRINT '<<< IT_inventory_tipos_accesorios en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_tipos_accesorios
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_inventory_tipos_accesorios](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](60) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_inventory_tipos_accesorios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_inventory_tipos_accesorios] ADD  CONSTRAINT [DF_IT_inventory_tipos_accesorios_activo]  DEFAULT (1) FOR [activo]
GO

--agrega campo de tipo accesorio
ALTER TABLE [IT_inventory_tipos_accesorios] ADD es_accesorio bit NOT NULL DEFAULT 0
			PRINT 'Se ha creado la columna descripcion en la tabla IT_inventory_items'			

SET IDENTITY_INSERT [IT_inventory_tipos_accesorios] ON 

INSERT [IT_inventory_tipos_accesorios] ([id], [descripcion], [activo]) VALUES (1, N'Monitor', 1)
INSERT [IT_inventory_tipos_accesorios] ([id], [descripcion], [activo]) VALUES (2, N'Teclado',1)
INSERT [IT_inventory_tipos_accesorios] ([id], [descripcion], [activo]) VALUES (3, N'Mouse', 1)
INSERT [IT_inventory_tipos_accesorios] ([id], [descripcion], [activo]) VALUES (4, N'Diadema', 1)
INSERT [IT_inventory_tipos_accesorios] ([id], [descripcion], [activo]) VALUES (5, N'Mochila', 1)

SET IDENTITY_INSERT [IT_inventory_tipos_accesorios] OFF
GO


 	  
IF object_id(N'IT_inventory_tipos_accesorios',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_tipos_accesorios en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_tipos_accesorios  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
