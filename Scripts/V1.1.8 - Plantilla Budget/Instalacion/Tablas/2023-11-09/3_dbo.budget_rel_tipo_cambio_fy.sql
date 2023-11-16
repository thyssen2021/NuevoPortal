--USE Portal_2_0
GO
IF object_id(N'budget_rel_tipo_cambio_fy',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_tipo_cambio_fy]
      PRINT '<<< budget_rel_tipo_cambio_fy en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_rel_tipo_cambio_fy
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_tipo_cambio_fy](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_anio_fiscal][int] NOT NULL,
	[id_tipo_cambio][int] NOT NULL,
	[cantidad] [float] NOT NULL
	
 CONSTRAINT [PK_budget_rel_tipo_cambio_fy] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_rel_tipo_cambio_fy]
 add constraint FK_budget_rel_tipo_cambio_fy_id_budget_anio_fiscal
  foreign key (id_budget_anio_fiscal)
  references budget_anio_fiscal(id);

  -- restriccion de clave foranea
  alter table [budget_rel_tipo_cambio_fy]
 add constraint FK_budget_rel_tipo_cambio_fy_id_tipo_cambio
  foreign key (id_tipo_cambio)
  references budget_tipo_cambio(id);

-- restriccion unique
 alter table [budget_rel_tipo_cambio_fy]
  add constraint UQ_budget_rel_tipo_cambio_fy_anio_tipo_cambio
  unique (id_tipo_cambio,id_budget_anio_fiscal);
GO

SET IDENTITY_INSERT [budget_rel_tipo_cambio_fy] ON 

INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (1,1,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (2,1,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (3,2,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (4,2,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (5,3,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (6,3,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (7,4,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (8,4,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (9,5,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (10,5,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (11,6,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (12,6,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (13,7,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (14,7,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (15,8,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (16,8,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (17,9,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (18,9,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (19,10,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (20,10,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (21,11,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (22,11,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (23,12,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (24,12,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (25,13,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (26,13,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (27,14,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (28,14,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (29,15,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (30,15,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (31,16,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (32,16,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (33,17,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (34,17,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (35,18,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (36,18,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (37,19,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (38,19,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (39,20,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (40,20,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (41,21,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (42,21,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (43,22,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (44,22,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (45,23,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (46,23,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (47,24,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (48,24,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (49,25,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (50,25,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (51,26,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (52,26,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (53,27,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (54,27,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (55,28,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (56,28,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (57,29,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (58,29,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (59,30,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (60,30,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (61,31,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (62,31,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (63,32,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (64,32,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (65,33,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (66,33,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (67,34,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (68,34,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (69,35,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (70,35,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (71,36,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (72,36,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (73,37,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (74,37,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (75,38,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (76,38,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (77,39,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (78,39,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (79,40,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (80,40,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (81,41,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (82,41,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (83,42,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (84,42,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (85,43,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (86,43,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (87,44,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (88,44,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (89,45,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (90,45,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (91,46,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (92,46,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (93,47,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (94,47,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (95,48,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (96,48,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (97,49,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (98,49,2, 1.15)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (99,50,1, 17.07)
INSERT [budget_rel_tipo_cambio_fy] ([id],[id_budget_anio_fiscal], [id_tipo_cambio], [cantidad]) VALUES (100,50,2, 1.15)


SET IDENTITY_INSERT [budget_rel_tipo_cambio_fy] OFF
GO
 	  
IF object_id(N'budget_rel_tipo_cambio_fy',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_tipo_cambio_fy en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_tipo_cambio_fy  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
