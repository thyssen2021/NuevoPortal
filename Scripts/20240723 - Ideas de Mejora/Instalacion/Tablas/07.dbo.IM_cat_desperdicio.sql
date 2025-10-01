GO
IF object_id(N'IM_cat_desperdicio',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_cat_desperdicio]
      PRINT '<<< IM_cat_desperdicio en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE IM_cat_desperdicio(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_IdeaMejoraDesperdicio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT IM_cat_desperdicio ON 
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (1, 1, N'Transporte')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (2, 1, N'Inventario')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (3, 1, N'Movimiento')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (4, 1, N'Sobre produccion')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (5, 1, N'Esperas')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (6, 1, N'Retrabajos')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (7, 1, N'Sobre procesamiento')
GO
INSERT IM_cat_desperdicio ([id], [activo], [descripcion]) VALUES (8, 1, N'Habilidades desperdiciadas')
GO
SET IDENTITY_INSERT IM_cat_desperdicio OFF
GO

IF object_id(N'IM_cat_desperdicio',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_cat_desperdicio en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_cat_desperdicio  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
