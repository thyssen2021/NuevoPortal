--USE Portal_2_0
GO
IF object_id(N'BG_forecast_cat_clientes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_forecast_cat_clientes]
      PRINT '<<< BG_forecast_cat_clientes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_forecast_cat_clientes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_forecast_cat_clientes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	--[id_ihs_version] [int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_BG_forecast_cat_clientes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [BG_forecast_cat_clientes] ADD  CONSTRAINT [DF_BG_forecast_cat_clientes_activo]  DEFAULT (1) FOR [activo]
GO

SET IDENTITY_INSERT [BG_forecast_cat_clientes] ON 

INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (1, N'Pruebas', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (2, N'Refacciones Honda', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (3, N'Refacciones Honda SLT', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (4, N'Refacciones Otros', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (5, N'Spot', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (6, N'USG', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (7, N'PWO', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (8, N'VOIT', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (9, N'Market Recovering ALU', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (10, N'Market Recovering STEEL', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (11, N'EBNER', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (12, N'Silao - Outfreight (truck rental)', 1)
INSERT [BG_forecast_cat_clientes] ([id], [descripcion], [activo]) VALUES (13, N'Service', 1)

SET IDENTITY_INSERT [BG_forecast_cat_clientes] OFF
GO

 	  
IF object_id(N'BG_forecast_cat_clientes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_forecast_cat_clientes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_forecast_cat_clientes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END	
SET ANSI_PADDING OFF
GO
