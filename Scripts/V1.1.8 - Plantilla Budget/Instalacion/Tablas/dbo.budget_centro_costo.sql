USE Portal_2_0
GO
IF object_id(N'budget_centro_costo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_centro_costo]
      PRINT '<<< budget_centro_costo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_centro_costo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_centro_costo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_departamento][int] NOT NULL,
	[descripcion][varchar](40) NOT NULL,
	[num_centro_costo][varchar](6) NOT NULL,
 CONSTRAINT [PK_budget_centro_costo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_centro_costo]
 add constraint FK_budget_centro_costo_id_departamento
  foreign key (id_budget_departamento)
  references budget_departamentos(id);

  
-- restriccion unique
 alter table [budget_centro_costo]
  add constraint UQ_num_centro_costo
  unique (num_centro_costo);

  -- restriccion unique
 alter table [budget_centro_costo]
  add constraint UQ_num_centro_costo_departamento
  unique (num_centro_costo, id_budget_departamento);

 SET IDENTITY_INSERT [budget_centro_costo] ON 

 USE [Portal_2_0]

INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (1,1,N'Executive Officer',N'3100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (2,2,N'Sales',N'3110')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (3,3,N'HR',N'3130')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (4,4,N'Health Services',N'3135')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (5,5,N'Business Steering',N'3140')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (6,6,N'IT',N'3211')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (7,7,N'RMP & IL',N'3320')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (8,8,N'Financial Officer',N'3200')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (9,9,N'Controlling',N'3212')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (10,10,N'Foreign Commerce',N'3220')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (11,11,N'Purchasing',N'3230')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (12,12,N'Finance',N'3240')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (13,13,N'Tax & Legal',N'3250')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (14,14,N'Operations Officer',N'3300')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (15,15,N'SHE',N'3120')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (16,16,N'Operations Puebla',N'4000')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (17,17,N'Ajuste Material Cost ',N'4100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (18,18,N'SHE',N'4120')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (19,19,N'HR',N'4130')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (20,20,N'Health Services',N'4135')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (21,21,N'IT',N'4211')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (22,22,N'Controlling',N'4212')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (23,23,N'Purchasing',N'4230')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (24,24,N'Quality',N'4300')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (25,25,N'CI & QPS',N'4310')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (26,26,N'Production',N'4400')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (27,27,N'Blanker Line 1 - Erfurt',N'4401')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (28,28,N'Blanker Line 2',N'4402')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (29,29,N'Blanker Line 3',N'4403')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (30,30,N'Slitter',N'4404')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (31,31,N'Weingarten 161',N'4405')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (32,32,N'Weingarten 163',N'4406')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (33,33,N'Engineering',N'4500')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (34,34,N'Tool & Die',N'4600')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (35,35,N'Maintenance',N'4700')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (36,36,N'Warehouse',N'4800')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (37,37,N'Spare Parts',N'4890')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (38,38,N'Scheduling',N'4900')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (39,39,N'Operations Silao',N'6000')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (40,40,N'Ajuste Material Cost ',N'6100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (41,41,N'SHE',N'6120')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (42,42,N'HR',N'6130')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (43,43,N'Health Services',N'6135')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (44,44,N'IT',N'6211')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (45,45,N'Controlling',N'6212')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (46,46,N'Purchasing',N'6230')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (47,47,N'Quality',N'6300')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (48,48,N'CI & QPS',N'6310')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (49,49,N'Production',N'6400')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (50,50,N'Blanker Line 1',N'6401')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (51,51,N'Blanker Line 2',N'6402')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (52,52,N'Blanker Line 3',N'6403')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (53,53,N'Cizalla',N'6404')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (54,54,N'Engineering',N'6500')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (55,55,N'Tool & Die',N'6600')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (56,56,N'Maintenance',N'6700')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (57,57,N'W&S',N'6800')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (58,58,N'Spare Parts',N'6890')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (59,59,N'Scheduling',N'6900')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (60,60,N'Operations Saltillo',N'7000')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (61,61,N'Ajuste Material Cost ',N'7100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (62,62,N'SHE',N'7120')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (63,63,N'HR',N'7130')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (64,64,N'Health Services',N'7135')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (65,65,N'IT',N'7211')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (66,66,N'Controlling',N'7212')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (67,67,N'Purchasing',N'7230')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (68,68,N'Quality',N'7300')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (69,69,N'CI & QPS',N'7310')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (70,70,N'Production',N'7400')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (71,71,N'Engineering',N'7500')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (72,72,N'Tool & Die',N'7600')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (73,73,N'Maintenance',N'7700')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (74,74,N'W&S',N'7800')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (75,75,N'Operations SLP',N'8000')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (76,76,N'Ajuste Material Cost ',N'8100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (77,77,N'HSE',N'8120')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (78,78,N'RH',N'8130')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (79,79,N'SO',N'8135')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (80,80,N'IT',N'8211')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (81,81,N'Controlling',N'8212')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (82,82,N'Purchasing',N'8230')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (83,83,N'Quality',N'8300')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (84,84,N'CI & QPS',N'8310')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (85,85,N'Engineering',N'8500')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (86,86,N'Tool & Die',N'8600')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (87,87,N'Maintenance',N'8700')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (88,88,N'Warehouse',N'8800')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (89,89,N'Spare Parts',N'8890')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (90,90,N'CBS',N'9100')
INSERT INTO [dbo].[budget_centro_costo] ([id],[id_budget_departamento],[descripcion],[num_centro_costo]) VALUES (91,91,N'CBS',N'9110')


SET IDENTITY_INSERT [budget_centro_costo] OFF

 	  
IF object_id(N'budget_centro_costo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_centro_costo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_centro_costo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
