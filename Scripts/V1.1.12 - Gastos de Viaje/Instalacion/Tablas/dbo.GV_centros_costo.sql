USE Portal_2_0
GO
IF object_id(N'GV_centros_costo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[GV_centros_costo]
      PRINT '<<< GV_centros_costo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos GV_centros_costo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/17
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [GV_centros_costo](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[clave_planta][int] NOT NULL, --FK
	[centro_costo][varchar](4) NOT NULL, 
	[departamento][varchar](50) NOT NUll,
	[activo][bit] NOT NULL DEFAULT 1,	
 CONSTRAINT [PK_GV_centros_costo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
alter table [GV_centros_costo]
 add constraint FK_GV_centros_costo_clave_planta
  foreign key (clave_planta)
  references plantas(clave);

GO

--Agrega registros de prueba
SET IDENTITY_INSERT [GV_centros_costo] ON 

INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (1,1,'3100',N'Executive Officer',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (2,1,'3110',N'Sales',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (3,1,'3130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (4,1,'3135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (5,1,'3140',N'Business Steering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (6,1,'3211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (7,1,'3320',N'RMP & IL',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (8,1,'3200',N'Financial Officer',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (9,1,'3212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (10,1,'3220',N'Foreign Commerce',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (11,1,'3230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (12,1,'3240',N'Finance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (13,1,'3250',N'Tax & Legal',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (14,1,'3300',N'Operations Officer',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (15,1,'3120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (16,1,'4000',N'Operations Puebla',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (17,1,'4100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (18,1,'4120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (19,1,'4130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (20,1,'4135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (21,1,'4211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (22,1,'4212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (23,1,'4230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (24,1,'4300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (25,1,'4310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (26,1,'4400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (27,1,'4401',N'Blanker Line 1 - Erfurt',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (28,1,'4402',N'Blanker Line 2',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (29,1,'4403',N'Blanker Line 3',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (30,1,'4404',N'Slitter',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (31,1,'4405',N'Weingarten 161',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (32,1,'4406',N'Weingarten 163',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (33,1,'4500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (34,1,'4600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (35,1,'4700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (36,1,'4800',N'Warehouse',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (37,1,'4890',N'Spare Parts',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (38,1,'4900',N'Scheduling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (39,2,'6000',N'Operations Silao',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (40,2,'6100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (41,2,'6120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (42,2,'6130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (43,2,'6135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (44,2,'6211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (45,2,'6212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (46,2,'6230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (47,2,'6300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (48,2,'6310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (49,2,'6400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (50,2,'6401',N'Blanker Line 1',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (51,2,'6402',N'Blanker Line 2',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (52,2,'6403',N'Blanker Line 3',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (53,2,'6404',N'Cizalla',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (54,2,'6500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (55,2,'6600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (56,2,'6700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (57,2,'6800',N'W&S',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (58,2,'6890',N'Spare Parts',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (59,2,'6900',N'Scheduling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (60,3,'7000',N'Operations Saltillo',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (61,3,'7100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (62,3,'7120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (63,3,'7130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (64,3,'7135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (65,3,'7211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (66,3,'7212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (67,3,'7230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (68,3,'7300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (69,3,'7310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (70,3,'7400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (71,3,'7500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (72,3,'7600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (73,3,'7700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (74,3,'7800',N'W&S',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (75,5,'8000',N'Operations SLP',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (76,5,'8100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (77,5,'8120',N'HSE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (78,5,'8130',N'RH',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (79,5,'8135',N'SO',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (80,5,'8211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (81,5,'8212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (82,5,'8230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (83,5,'8300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (84,5,'8310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (85,5,'8500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (86,5,'8600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (87,5,'8700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (88,5,'8800',N'Warehouse',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (89,5,'8890',N'Spare Parts',1); 



SET IDENTITY_INSERT [GV_centros_costo] OFF
 	  
IF object_id(N'GV_centros_costo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< GV_centros_costo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla GV_centros_costo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
