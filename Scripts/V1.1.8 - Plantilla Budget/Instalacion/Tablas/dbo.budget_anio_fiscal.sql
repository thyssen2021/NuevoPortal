USE Portal_2_0
GO
IF object_id(N'budget_anio_fiscal',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_anio_fiscal]
      PRINT '<<< budget_anio_fiscal en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_anio_fiscal
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_anio_fiscal](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion][varchar](20) NOT NULL,
	[anio_inicio][int] NOT NULL,
	[mes_inicio][int] NOT NULL,
	[anio_fin][int] NOT NULL,
	[mes_fin][int] NOT NULL,
 CONSTRAINT [PK_budget_anio_fiscal] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [budget_anio_fiscal] ON 

INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (1,N'2020/21',2020,10,2021,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (2,N'2021/22',2021,10,2022,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (3,N'2022/23',2022,10,2023,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (4,N'2023/24',2023,10,2024,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (5,N'2024/25',2024,10,2025,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (6,N'2025/26',2025,10,2026,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (7,N'2026/27',2026,10,2027,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (8,N'2027/28',2027,10,2028,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (9,N'2028/29',2028,10,2029,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (10,N'2029/30',2029,10,2030,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (11,N'2030/31',2030,10,2031,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (12,N'2031/32',2031,10,2032,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (13,N'2032/33',2032,10,2033,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (14,N'2033/34',2033,10,2034,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (15,N'2034/35',2034,10,2035,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (16,N'2035/36',2035,10,2036,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (17,N'2036/37',2036,10,2037,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (18,N'2037/38',2037,10,2038,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (19,N'2038/39',2038,10,2039,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (20,N'2039/40',2039,10,2040,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (21,N'2040/41',2040,10,2041,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (22,N'2041/42',2041,10,2042,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (23,N'2042/43',2042,10,2043,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (24,N'2043/44',2043,10,2044,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (25,N'2044/45',2044,10,2045,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (26,N'2045/46',2045,10,2046,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (27,N'2046/47',2046,10,2047,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (28,N'2047/48',2047,10,2048,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (29,N'2048/49',2048,10,2049,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (30,N'2049/50',2049,10,2050,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (31,N'2050/51',2050,10,2051,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (32,N'2051/52',2051,10,2052,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (33,N'2052/53',2052,10,2053,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (34,N'2053/54',2053,10,2054,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (35,N'2054/55',2054,10,2055,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (36,N'2055/56',2055,10,2056,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (37,N'2056/57',2056,10,2057,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (38,N'2057/58',2057,10,2058,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (39,N'2058/59',2058,10,2059,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (40,N'2059/60',2059,10,2060,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (41,N'2060/61',2060,10,2061,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (42,N'2061/62',2061,10,2062,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (43,N'2062/63',2062,10,2063,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (44,N'2063/64',2063,10,2064,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (45,N'2064/65',2064,10,2065,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (46,N'2065/66',2065,10,2066,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (47,N'2066/67',2066,10,2067,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (48,N'2067/68',2067,10,2068,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (49,N'2068/69',2068,10,2069,09);
INSERT [budget_anio_fiscal] ([id], [descripcion], [anio_inicio],[mes_inicio],[anio_fin],[mes_fin]) VALUES (50,N'2069/70',2069,10,2070,09);


SET IDENTITY_INSERT [budget_anio_fiscal] OFF
GO

 	  
IF object_id(N'budget_anio_fiscal',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_anio_fiscal en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_anio_fiscal  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
