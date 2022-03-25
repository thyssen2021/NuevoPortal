USE Portal_2_0
GO
IF object_id(N'budget_departamentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_departamentos]
      PRINT '<<< budget_departamentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_departamentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_departamentos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_planta][int] NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_budget_departamentos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_departamentos]
 add constraint FK_budget_departamentos_id_planta
  foreign key (id_budget_planta)
  references budget_plantas(id);

-- restricción default
ALTER TABLE [budget_departamentos] ADD  CONSTRAINT [DF_budget_departamentos_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [budget_departamentos] ON 

INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (1,1,N'Presidencia',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (2,1,N'Ventas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (3,1,N'Recursos Humanos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (4,1,N'Salud Ocupacional',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (5,1,N'Manejo de Negocio',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (6,1,N'Sistemas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (7,1,N'Logística',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (8,1,N'Director Financiero',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (9,1,N'Controlling',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (10,1,N'Comercio',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (11,1,N'Compras',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (12,1,N'Finanzas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (13,1,N'Impuestos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (14,1,N'Director Operativo',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (15,1,N'Salud, seguridad y medio ambiente',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (16,2,N'Gerente de Operaciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (17,2,N'Ajustes Material Cost ',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (18,2,N'Salud, seguridad y medio ambiente',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (19,2,N'Recursos Humanos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (20,2,N'Salud Ocupacional',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (21,2,N'Sistemas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (22,2,N'Controlling',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (23,2,N'Compras',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (24,2,N'Calidad',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (25,2,N'Mejora Continua',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (26,2,N'Producción',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (27,2,N'Blanker Line 1 - Erfurt',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (28,2,N'Blanker Line 2',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (29,2,N'Blanker Line 3',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (30,2,N'Slitter',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (31,2,N'Weingarten 161',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (32,2,N'Weingarten 163',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (33,2,N'Ingeniería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (34,2,N'Matricería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (35,2,N'Mantenimiento',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (36,2,N'Almacén',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (37,2,N'Almacén Refacciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (38,2,N'Programación',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (39,3,N'Gerente de Operaciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (40,3,N'Ajuste Material Cost ',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (41,3,N'Salud, seguridad y medio ambiente',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (42,3,N'Recursos Humanos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (43,3,N'Salud Ocupacional',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (44,3,N'Sistemas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (45,3,N'Controlling',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (46,3,N'Compras',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (47,3,N'Calidad',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (48,3,N'Mejora Continua',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (49,3,N'Producción',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (50,3,N'Blanker Line 1',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (51,3,N'Blanker Line 2',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (52,3,N'Blanker Line 3',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (53,3,N'Cizalla',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (54,3,N'Ingeniería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (55,3,N'Matricería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (56,3,N'Mantenimiento',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (57,3,N'Almacén y programación',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (58,3,N'Almacén Refacciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (59,3,N'Programación',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (60,4,N'Gerente de Operaciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (61,4,N'Ajuste Material Cost ',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (62,4,N'Salud, seguridad y medio ambiente',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (63,4,N'Recursos Humanos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (64,4,N'Salud Ocupacional',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (65,4,N'Sistemas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (66,4,N'Controlling',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (67,4,N'Compras',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (68,4,N'Calidad',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (69,4,N'Mejora Continua',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (70,4,N'Producción',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (71,4,N'Ingeniería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (72,4,N'Matricería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (73,4,N'Mantenimiento',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (74,4,N'Almacén y programación',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (75,5,N'Gerente de Operaciones',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (76,5,N'Ajuste Material Cost ',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (77,5,N'Salud, seguridad y medio ambiente',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (78,5,N'Recursos Humanos',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (79,5,N'Salud Ocupacional',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (80,5,N'Sistemas',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (81,5,N'Controlling',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (82,5,N'Compras',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (83,5,N'Calidad',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (84,5,N'Mejora Continua',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (85,5,N'Producción',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (86,5,N'Ingeniería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (87,5,N'Matricería',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (88,5,N'Mantenimiento',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (89,5,N'Almacén y programación',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (90,6,N'C&B General',1)
INSERT [budget_departamentos] ([id],[id_budget_planta], [descripcion], [activo]) VALUES (91,6,N'Ajustes Material Cost ',1)



SET IDENTITY_INSERT [budget_departamentos] OFF
GO
 	  
IF object_id(N'budget_departamentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_departamentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_departamentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
