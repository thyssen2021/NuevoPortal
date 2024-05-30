--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_materiales_maquila',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_materiales_maquila]
      PRINT '<<< SCDM_cat_materiales_maquila en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_materiales_maquila
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/01/03
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/



CREATE TABLE [dbo].[SCDM_cat_materiales_maquila](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[numero_material]varchar(10) NOT NULL,
	[planta][varchar](4) NOT NULL,
	[MovAvgPrice] float NOT NULL,
	[StandardPrice] float NOT NULL,
	[activo] [bit] NOT NULL Default 1,
 CONSTRAINT [PK_SCDM_cat_materiales_maquila_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_materiales_maquila] ON 

GO

INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (1,'SM00001', '5190', 1230,1230 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (2,'SM00002', '5190', 55,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (3,'SM00003', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (4,'SM00004', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (5,'SM00005', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (6,'SM00006', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (7,'SM00007', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (8,'SM00008', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (9,'SM00012', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (10,'SM00012', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (11,'SM00014', '5190', 3970,3970 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (12,'SM00016', '5190', 970,970 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (13,'SM00017', '5190', 3430,3430 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (14,'SM00018', '5190', 3740,3740 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (15,'SM00019', '5190', 1070,1070 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (16,'SM00020', '5190', 695,695 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (17,'SM00021', '5190', 1370,1370 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (18,'SM00023', '5190', 1405,1405 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (19,'SM00024', '5190', 685,685 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (20,'SM00025', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (21,'SM00025', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (22,'SM00026', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (23,'SM00026', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (24,'SM00027', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (25,'SM00027', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (26,'SM00028', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (27,'SM00028', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (28,'SM00030', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (29,'SM00030', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (30,'SM00031', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (31,'SM00031', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (32,'SM00033', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (33,'SM00034', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (34,'SM00034', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (35,'SM00043', '5190', 35,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (36,'SM00044', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (37,'SM00044', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (38,'SM00046', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (39,'SM00046', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (40,'SM00048', '5190', 40,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (41,'SM00050', '5190', 110,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (42,'SM00051', '5390', 37,37 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (43,'SM00053', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (44,'SM00054', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (45,'SM00055', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (46,'SM00055', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (47,'SM00055', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (48,'SM00056', '5190', 25,25 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (49,'SM00057', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (50,'SM00057', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (51,'SM00058', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (52,'SM00058', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (53,'SM00059', '5190', 23,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (54,'SM00060', '5190', 40,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (55,'SM00062', '5190', 55,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (56,'SM00062', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (57,'SM00062', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (58,'SM00071', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (59,'SM00071', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (60,'SM00073', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (61,'SM00073', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (62,'SM00075', '5190', 40,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (63,'SM00081', '5190', 25,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (64,'SM00082', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (65,'SM00082', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (66,'SM00082', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (67,'SM00083', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (68,'SM00083', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (69,'SM00083', '8090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (70,'SM00084', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (71,'SM00084', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (72,'SM00091', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (73,'SM00093', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (74,'SM00093', '6090', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (75,'SM00094', '5390', 37,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (76,'SM00101', '5190', 96.03,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (77,'SM00103', '5390', 19.95,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (78,'SM00104', '5190', 41,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (79,'SM00112', '5190', 4827.59,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (80,'SM00113', '5390', 2320,7090 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (81,'SM00121', '5390', 9950,9950 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (82,'SM00131', '5390', 24.5,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (83,'SM00132', '5390', 2610,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (84,'SM00133', '5190', 90,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (85,'SM00134', '5190', 90,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (86,'SM00135', '5390', 90,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (87,'SM00136', '5190', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (88,'SM00136', '5390', 0,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (89,'SM00142', '5390', 34,34 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (90,'SM00144', '5190', 50,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (91,'SM00146', '5190', 45,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (92,'SM00148', '5390', 34.3,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (93,'SM00152', '5390', 34.3,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (94,'SM00154', '5190', 81,81 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (95,'SM00155', '5190', 81,81 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (96,'SM00156', '5390', 34.3,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (97,'SM00159', '5190', 86,86 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (98,'SM00160', '5400', 750,750 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (99,'SM00161', '5390', 62,0 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (100,'SM00162', '5190', 95,95 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (101,'SM00173', '5390', 37.95,37.95 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (102,'SM00182', '5390', 38.5,38.5 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (103,'SM00192', '5390', 43.5,43.5 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (104,'SM00201', '5390', 40.64,40.64 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (105,'SM00202', '5390', 66,66 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (106,'SM00203', '5390', 55.64,55.64 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (107,'SM00204', '5190', 205,205 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (108,'SM00205', '5390', 66.5,66.5 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (109,'SM00211', '5190', 0,22.25 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (110,'SM00211', '5390', 22.25,22.25 , 1) 
INSERT [dbo].[SCDM_cat_materiales_maquila] ([id], numero_material, planta, MovAvgPrice,StandardPrice, [activo]) VALUES (111,'SM00221', '5390', 53.5,53.5 , 1) 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_materiales_maquila] OFF
	  
IF object_id(N'SCDM_cat_materiales_maquila',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_materiales_maquila en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_materiales_maquila  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
