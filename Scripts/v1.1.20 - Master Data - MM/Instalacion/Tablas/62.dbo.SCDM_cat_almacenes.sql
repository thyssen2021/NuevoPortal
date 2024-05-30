--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_almacenes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_almacenes]
      PRINT '<<< SCDM_cat_almacenes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_almacenes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/01/03
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/



CREATE TABLE [dbo].[SCDM_cat_almacenes](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_planta][int] NOT NULL,
	[warehouse][varchar](3) NOT NULL,
	[storage_type][varchar](3) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_almacenes_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_almacenes] ON 

GO

INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (1, 1, 'PU1','001' , N'Rollos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (2, 1, 'PU1','002' , N'Producto Terminado', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (3, 1, 'PU1','003' , N'Obsoletos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (4, 1, 'PU1','004' , N'Cuarentena', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (5, 1, 'PU1','901' , N'GR area for production', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (6, 1, 'PU1','902' , N'GR area external rcpts', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (7, 1, 'PU1','904' , N'Returns', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (8, 1, 'PU1','910' , N'GI Area General', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (9, 1, 'PU1','911' , N'GI area for cost center', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (10, 1, 'PU1','912' , N'GI area customer order', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (11, 1, 'PU1','913' , N'GI Area - Fixed Assets', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (12, 1, 'PU1','914' , N'GI area production orders', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (13, 1, 'PU1','915' , N'Fixed bin picking area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (14, 1, 'PU1','916' , N'Shipping area deliveries', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (15, 1, 'PU1','917' , N'Quality assurance', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (16, 1, 'PU1','918' , N'Goods issue area contain.', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (17, 1, 'PU1','920' , N'Stock transfers (plant)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (18, 1, 'PU1','921' , N'Stock transfers (StLoc)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (19, 1, 'PU1','922' , N'Posting change area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (20, 1, 'PU1','980' , N'R/3   > R/2 cumulative', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (21, 1, 'PU1','998' , N'Init. entry of inv. data', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (22, 1, 'PU1','999' , N'Differences', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (23, 1, 'PU2','001' , N'Rollos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (24, 1, 'PU2','002' , N'Producto Terminado', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (25, 1, 'PU2','003' , N'Obsoletos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (26, 1, 'PU2','004' , N'Cuarentena', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (27, 1, 'PU2','901' , N'GR area for production', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (28, 1, 'PU2','902' , N'GR area external rcpts', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (29, 1, 'PU2','904' , N'Returns', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (30, 1, 'PU2','910' , N'GI Area General', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (31, 1, 'PU2','911' , N'GI area for cost center', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (32, 1, 'PU2','912' , N'GI area customer order', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (33, 1, 'PU2','913' , N'GI Area - Fixed Assets', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (34, 1, 'PU2','914' , N'GI area production orders', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (35, 1, 'PU2','915' , N'Fixed bin picking area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (36, 1, 'PU2','916' , N'Shipping area deliveries', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (37, 1, 'PU2','917' , N'Quality assurance', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (38, 1, 'PU2','918' , N'Goods issue area contain.', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (39, 1, 'PU2','920' , N'Stock transfers (plant)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (40, 1, 'PU2','921' , N'Stock transfers (StLoc)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (41, 1, 'PU2','922' , N'Posting change area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (42, 1, 'PU2','980' , N'R/3   > R/2 cumulative', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (43, 1, 'PU2','998' , N'Init. entry of inv. data', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (44, 1, 'PU2','999' , N'Differences', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (45, 2, 'SI1','001' , N'Rollos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (46, 2, 'SI1','002' , N'Producto Terminado', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (47, 2, 'SI1','003' , N'Obsoletos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (48, 2, 'SI1','004' , N'Cuarentena', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (49, 2, 'SI1','901' , N'GR area for production', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (50, 2, 'SI1','902' , N'GR area external rcpts', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (51, 2, 'SI1','904' , N'Returns', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (52, 2, 'SI1','910' , N'GI Area General', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (53, 2, 'SI1','911' , N'GI area for cost center', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (54, 2, 'SI1','912' , N'GI area customer order', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (55, 2, 'SI1','913' , N'GI Area - Fixed Assets', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (56, 2, 'SI1','914' , N'GI area production orders', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (57, 2, 'SI1','915' , N'Fixed bin picking area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (58, 2, 'SI1','916' , N'Shipping area deliveries', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (59, 2, 'SI1','917' , N'Quality assurance', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (60, 2, 'SI1','918' , N'Goods issue area contain.', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (61, 2, 'SI1','920' , N'Stock transfers (plant)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (62, 2, 'SI1','921' , N'Stock transfers (StLoc)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (63, 2, 'SI1','922' , N'Posting change area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (64, 2, 'SI1','980' , N'R/3   > R/2 cumulative', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (65, 2, 'SI1','998' , N'Init. entry of inv. data', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (66, 2, 'SI1','999' , N'Differences', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (67, 3, 'SA1','001' , N'Rollos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (68, 3, 'SA1','002' , N'Producto Terminado', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (69, 3, 'SA1','003' , N'Obsoletos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (70, 3, 'SA1','004' , N'Cuarentena', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (71, 3, 'SA1','901' , N'GR area for production', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (72, 3, 'SA1','902' , N'GR area external rcpts', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (73, 3, 'SA1','904' , N'Returns', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (74, 3, 'SA1','910' , N'GI Area General', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (75, 3, 'SA1','911' , N'GI area for cost center', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (76, 3, 'SA1','912' , N'GI area customer order', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (77, 3, 'SA1','913' , N'GI Area - Fixed Assets', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (78, 3, 'SA1','914' , N'GI area production orders', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (79, 3, 'SA1','915' , N'Fixed bin picking area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (80, 3, 'SA1','916' , N'Shipping area deliveries', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (81, 3, 'SA1','917' , N'Quality assurance', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (82, 3, 'SA1','918' , N'Goods issue area contain.', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (83, 3, 'SA1','920' , N'Stock transfers (plant)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (84, 3, 'SA1','921' , N'Stock transfers (StLoc)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (85, 3, 'SA1','922' , N'Posting change area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (86, 3, 'SA1','980' , N'R/3   > R/2 cumulative', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (87, 3, 'SA1','998' , N'Init. entry of inv. data', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (88, 3, 'SA1','999' , N'Differences', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (89, 5, 'SI1','001' , N'Rollos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (90, 5, 'SL1','002' , N'Producto Terminado', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (91, 5, 'SL1','003' , N'Obsoletos', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (92, 5, 'SL1','004' , N'Cuarentena', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (93, 5, 'SL1','901' , N'GR area for production', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (94, 5, 'SL1','902' , N'GR area external rcpts', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (95, 5, 'SL1','904' , N'Returns', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (96, 5, 'SL1','910' , N'GI Area General', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (97, 5, 'SL1','911' , N'GI area for cost center', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (98, 5, 'SL1','912' , N'GI area customer order', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (99, 5, 'SL1','913' , N'GI Area - Fixed Assets', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (100, 5, 'SL1','914' , N'GI area production orders', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (101, 5, 'SL1','915' , N'Fixed bin picking area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (102, 5, 'SL1','916' , N'Shipping area deliveries', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (103, 5, 'SL1','917' , N'Quality assurance', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (104, 5, 'SL1','918' , N'Goods issue area contain.', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (105, 5, 'SL1','920' , N'Stock transfers (plant)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (106, 5, 'SL1','921' , N'Stock transfers (StLoc)', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (107, 5, 'SL1','922' , N'Posting change area', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (108, 5, 'SL1','980' , N'R/3   > R/2 cumulative', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (109, 5, 'SL1','998' , N'Init. entry of inv. data', 1) 
INSERT [dbo].[SCDM_cat_almacenes] ([id], [id_planta], [warehouse],[storage_type],[descripcion], [activo]) VALUES (110, 5, 'SL1','999' , N'Differences', 1) 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_almacenes] OFF
	  
IF object_id(N'SCDM_cat_almacenes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_almacenes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_almacenes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
