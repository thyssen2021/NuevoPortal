--USE Portal_2_0
GO
IF object_id(N'BG_forecast_cat_historico_ventas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_forecast_cat_historico_ventas]
      PRINT '<<< BG_forecast_cat_historico_ventas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_forecast_cat_historico_ventas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_forecast_cat_historico_ventas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NOT NULL,
	[id_seccion] [int] NOT NULL,
	[fecha] [date] NOT NULL,
	[valor] [float] NOT NULL	
 CONSTRAINT [PK_BG_forecast_cat_historico_ventas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


 -- restriccion de clave foranea
alter table [BG_forecast_cat_historico_ventas]
 add constraint FK_BG_forecast_cat_historico_ventas_id_cliente
  foreign key (id_cliente)
  references BG_forecast_cat_clientes(id);
  
   -- restriccion de clave foranea
alter table [BG_forecast_cat_historico_ventas]
 add constraint FK_BG_IHS_regione_id_seccion
  foreign key (id_seccion)
  references [BG_Forecast_cat_secciones_calculo](id);

  --Restriccion unique
  ALTER TABLE BG_forecast_cat_historico_ventas
ADD CONSTRAINT UQ_historico_ventas UNIQUE (id_cliente, id_seccion, fecha); 

--SET IDENTITY_INSERT [BG_forecast_cat_historico_ventas] ON 

--INSERT [BG_forecast_cat_historico_ventas] ([id], [descripcion], [activo]) VALUES (1, N'CENTER', 1)
--INSERT [BG_forecast_cat_historico_ventas] ([id], [descripcion], [activo]) VALUES (2, N'SOUTH', 1)
--INSERT [BG_forecast_cat_historico_ventas] ([id], [descripcion], [activo]) VALUES (3, N'NORTH-WEST', 1)
--INSERT [BG_forecast_cat_historico_ventas] ([id], [descripcion], [activo]) VALUES (4, N'NORTH', 1)
--INSERT [BG_forecast_cat_historico_ventas] ([id], [descripcion], [activo]) VALUES (5, N'EXPORT', 1)

--SET IDENTITY_INSERT [BG_forecast_cat_historico_ventas] OFF
GO

 	  
IF object_id(N'BG_forecast_cat_historico_ventas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_forecast_cat_historico_ventas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_forecast_cat_historico_ventas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END	
SET ANSI_PADDING OFF
GO
