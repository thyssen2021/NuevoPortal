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
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (18,2,'4120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (19,2,'4130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (20,2,'4135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (21,2,'4211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (22,2,'4212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (23,2,'4230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (24,2,'4300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (25,2,'4310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (26,2,'4400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (27,2,'4401',N'Blanker Line 1 - Erfurt',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (28,2,'4402',N'Blanker Line 2',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (29,2,'4403',N'Blanker Line 3',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (30,3,'4404',N'Slitter',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (31,3,'4405',N'Weingarten 161',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (32,3,'4406',N'Weingarten 163',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (33,3,'4500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (34,3,'4600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (35,3,'4700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (36,3,'4800',N'Warehouse',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (37,3,'4890',N'Spare Parts',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (38,3,'4900',N'Scheduling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (39,3,'6000',N'Operations Silao',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (40,3,'6100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (41,3,'6120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (42,3,'6130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (43,1,'6135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (44,1,'6211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (45,1,'6212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (46,1,'6230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (47,1,'6300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (48,1,'6310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (49,1,'6400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (50,1,'6401',N'Blanker Line 1',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (51,1,'6402',N'Blanker Line 2',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (52,1,'6403',N'Blanker Line 3',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (53,1,'6404',N'Cizalla',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (54,1,'6500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (55,5,'6600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (56,5,'6700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (57,5,'6800',N'W&S',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (58,5,'6890',N'Spare Parts',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (59,5,'6900',N'Scheduling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (60,5,'7000',N'Operations Saltillo',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (61,5,'7100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (62,5,'7120',N'SHE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (63,5,'7130',N'HR',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (64,5,'7135',N'Health Services',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (65,5,'7211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (66,5,'7212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (67,5,'7230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (68,5,'7300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (69,5,'7310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (70,5,'7400',N'Production',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (71,5,'7500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (72,5,'7600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (73,5,'7700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (74,5,'7800',N'W&S',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (75,1,'8000',N'Operations SLP',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (76,1,'8100',N'Ajuste Material Cost ',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (77,3,'8120',N'HSE',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (78,3,'8130',N'RH',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (79,3,'8135',N'SO',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (80,3,'8211',N'IT',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (81,3,'8212',N'Controlling',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (82,3,'8230',N'Purchasing',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (83,3,'8300',N'Quality',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (84,3,'8310',N'CI & QPS',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (85,1,'8500',N'Engineering',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (86,1,'8600',N'Tool & Die',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (87,1,'8700',N'Maintenance',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (88,1,'8800',N'Warehouse',1); 
INSERT INTO GV_centros_costo(id, clave_planta, centro_costo, departamento, activo) VALUES (89,1,'8890',N'Spare Parts',1); 


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
