USE Portal_2_0
GO
IF object_id(N'budget_cuenta_sap',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_cuenta_sap]
      PRINT '<<< budget_cuenta_sap en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_cuenta_sap
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_cuenta_sap](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_mapping][int] NOT NULL,
	[sap_account][varchar](8) NOT NULL,	
	[name][varchar](40) NOT NULL,	
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_budget_cuenta_sap] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_cuenta_sap]
 add constraint FK_budget_cuenta_sap_id_mapping
  foreign key (id_mapping)
  references budget_mapping(id);

  -- restricción default
ALTER TABLE budget_cuenta_sap ADD  CONSTRAINT [DF_budget_cuenta_sap_activo]  DEFAULT (1) FOR [activo]
GO
	  

	  
SET IDENTITY_INSERT [budget_cuenta_sap] ON 

INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (1,1,'610010','Gastos de viaje - No Deducibles',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (2,1,'610030','Gastos de viaje - hospedaje',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (3,1,'610040','Gastos de viaje - comidas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (4,1,'610070','Gastos de viaje - vuelos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (5,1,'610071','Gastos de viaje - transporte terrestre',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (6,1,'610072','Gastos de viaje - renta de automovil',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (7,1,'610073','Gastos de Viaje - Casetas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (8,1,'610074','Gastos de Viaje - Combustible',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (9,1,'610080','Gastos de viaje - varios',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (10,2,'650030','Honorarios legales & profesionales',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (11,3,'650031','Honorarios de auditoria',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (12,4,'650032','Honorarios de consultoría',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (13,5,'610090','Capacitación',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (14,5,'610091','Festejos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (15,6,'630060','Renta de telefono',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (16,6,'630065','Comunicaciones',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (17,6,'630066','Mantenimiento de sistema de telefono',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (18,7,'630067','Otros gastos - EDI',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (19,7,'650011','Software - IT',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (20,7,'650012','Software - tk',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (21,7,'650019','Redes',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (22,7,'652000','Accesorios',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (23,7,'652100','Hardware',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (24,7,'652200','Mantenimiento IT',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (25,7,'653000','Renta - IT',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (26,8,'630040','Seguro',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (27,8,'706020','Gastos de automóvil - seguros',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (28,9,'630050','Renta de oficina y edificio',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (29,9,'650001','Mensajería',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (30,9,'651000','Gastos de oficina',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (31,9,'651010','Mobiliario de Oficina',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (32,9,'651020','Mantenimiento de Oficinas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (33,9,'651100','Gastos Administrativos RHQ Allocations',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (34,9,'705040','Gastos de café',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (35,9,'705050','Gastos de renta de equipo de la oficina',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (36,10,'650018','TKMNA Consult  Exp',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (37,10,'650002','Servicios de almacenamiento',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (38,10,'612010','Gastos medicos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (39,10,'650025','Infraestructura - IT',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (40,10,'650040','Administrative charges C&B - Debit',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (41,10,'650041','Administrative Charges C&B - Credit',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (42,10,'651101','Servicio de administración de personal',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (43,10,'660000','Otros gastos generales',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (44,10,'660010','Suscripciones / libros',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (45,10,'660011','Uniformes',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (46,10,'660012','Servicio de Transporte',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (47,10,'660020','Consumibles',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (48,10,'690100','Derechos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (49,10,'700000','Cargos del banco',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (50,10,'700300','Recargos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (51,10,'700600','Interéses recibidos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (52,10,'700840','Gastos No Deducibles',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (53,10,'704090','Gastos de cafetería',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (54,10,'705020','Gastos de la oficina - Limpieza',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (55,10,'706000','Gastos de automóvil - general',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (56,10,'706010','Gastos de automóvil - reparaciones',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (57,10,'704080','Gastos de cocina',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (58,10,'704100','Gastos misceláneos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (59,10,'700310','Actualizaciones Y Multas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (60,10,'704020','Obsequios de la empresa',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (61,10,'700910','Predial - Detroit / Wayne County',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (62,11,'630010','Electricidad',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (63,11,'630020','Agua',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (64,11,'630025','Gas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (65,12,'704110','Prueba del laboratorio & análisis',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (66,12,'704200','Compra de Equipo de Medicion',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (67,12,'704220','Calibraciones',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (68,12,'707000','Mantenimiento de edificios',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (69,12,'707010','Mantenimiento de grúas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (70,12,'707020','Mantenimiento de montacargas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (71,12,'707021','Arrendamiento de montacargas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (72,12,'707030','Mantenimiento de maquinas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (73,12,'707040','Mantenimiento Varios',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (74,12,'707050','Mantenimiento Troqueles',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (75,12,'707062','Electrotecnica, Electronica',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (76,12,'707063','Materiales De Instalacion Electrica',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (77,12,'707064','Cojinetes-Bearings',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (78,12,'707065','Compresores',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (79,12,'707066','Reparacion de Rack''s Metalicos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (80,12,'707067','Rasquetas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (81,12,'707068','Reparacion de Bandas Transportadoras',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (82,12,'707100','Programas de Seguridad',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (83,12,'707110','Programas de Productividad',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (84,12,'707120','Prueba & análisis industriales',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (85,12,'707130','Equipo de Medición',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (86,12,'707140','Calibraciones',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (87,12,'708000','Artículos para el almacen - varios',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (88,12,'708010','Artículos para el almacen - formatos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (89,12,'708011','Consumibles para producción',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (90,12,'708020','Lubricantes, Aceites, Grasas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (91,12,'708021','Hidraulica Y Neumatica',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (92,12,'708022','Productos Textiles',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (93,12,'708040','Equipo de Protección Personal',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (94,12,'708050','Artículos para el almacen - herramientas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (95,12,'708060','Gastos del almacen - Seguridad',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (96,10,'708070','Vigilancia',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (97,12,'708080','Gastos del almacen - Residuos Peligrosos',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (98,12,'708210','Gastos del almacen - limpieza',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (99,12,'708251','Herramientas de Oxicorte',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (100,12,'708252','Flejadoras',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (101,12,'708253','Sorteo de Material',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (102,12,'709010','Señaletica',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (103,12,'630030','Otros gastos de mantenimiento',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (104,12,'708200','Almacén - Herramientas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (105,12,'707061','Elementos De Union',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (106,12,'707060','Mantenimiento a Tuberías',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (107,12,'611020','Programas de Seguridad',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (108,13,'708120','Flete - Traspaso entre Plantas',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (109,13,'708160','Flete Salida Puebla',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (110,13,'708161','Flete Salida Silao',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (111,13,'708162','Flete Salida Saltillo',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (112,14,'708170','Compra de Polines y Racks de Madrera',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (113,14,'708180','Gastos del almacen - fleje',1)
INSERT [budget_cuenta_sap] ([id], [id_mapping], [sap_account],[name], [activo]) VALUES (114,14,'708190','Materiales Para Embalaje',1)


SET IDENTITY_INSERT [budget_cuenta_sap] OFF
GO

IF object_id(N'budget_cuenta_sap',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_cuenta_sap en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_cuenta_sap  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
