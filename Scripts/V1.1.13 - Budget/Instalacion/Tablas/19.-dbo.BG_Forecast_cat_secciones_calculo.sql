--USE Portal_2_0
GO
IF object_id(N'BG_Forecast_cat_secciones_calculo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_Forecast_cat_secciones_calculo]
      PRINT '<<< BG_Forecast_cat_secciones_calculo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los BG_Forecast_cat_secciones_calculo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [BG_Forecast_cat_secciones_calculo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[aplica_formula][bit] NOT NULL default 0,
	[formula][varchar](60) NULL,
	[tipo_dato][varchar](12) NULL,
	[decimales][int] NOT NULL,
	[activo] [bit] NOT NULL default 1	
 CONSTRAINT [PK_BG_Forecast_cat_secciones_calculo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [BG_Forecast_cat_secciones_calculo] ON 

INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (1, N'Total Sales [TUSD]', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (2, N'Material Cost [TUSD]', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (3, N'COST OF OUTSIDE PROCESSOR  /PART  [TUSD]',0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (4, N'Value Added Sales [TUSD]', 1, N'Total Sales - Material Cost - Cost Of Outside Processor','CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (5, N'Processed Tons [to]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (6, N'Engineered Scrap [to]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (7, N'Scrap Consolidation [to]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (8, N'Strokes [ - / 1000 ]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (9, N'Blanks [ - / 1000 ]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (10, N'Additional material cost total [USD]', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (11, N'Outgoing freight total [USD]', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (12, N'Inventory OWN (monthly average) [tons]', 0, NULL,'NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (13, N'Inventory (End of month) [USD]', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (14, N'Freights Income USD / PART', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (15, N'Maniobras USD / PART', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (16, N'Customs Expenses USD / PART', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (17, N'shipment Tons [to]', 1, N'Shipment Tons = Procesed Tons','NUMERIC', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (18, N'SALES Inc. SCRAP [TUSD]', 1, N'Sales Inc. Scrap = Total Sales','CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (19, N'VAS Inc. SCRAP [TUSD]', 1, N'VAS Inc. Scrap = Value Added Sales','CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (20, N'Processing  Inc. SCRAP', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (21, N'Wooden pallets', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (22, N'Standard packaging ', 0, NULL,'CURRENCY', 2, 1)
INSERT [BG_Forecast_cat_secciones_calculo] ([id], [descripcion], [aplica_formula],formula,tipo_dato,decimales, [activo]) VALUES (23, N'PLASTIC STRIPS', 0, NULL,'CURRENCY', 2, 1)

SET IDENTITY_INSERT [BG_Forecast_cat_secciones_calculo] OFF
GO

 	  
IF object_id(N'BG_Forecast_cat_secciones_calculo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_Forecast_cat_secciones_calculo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_Forecast_cat_secciones_calculo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END	
SET ANSI_PADDING OFF
GO
