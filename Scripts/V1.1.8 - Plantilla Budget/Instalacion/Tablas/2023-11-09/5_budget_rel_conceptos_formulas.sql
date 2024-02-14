--USE Portal_2_0
GO
IF object_id(N'budget_rel_conceptos_formulas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_conceptos_formulas]
      PRINT '<<< budget_rel_conceptos_formulas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_rel_conceptos_formulas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_conceptos_formulas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_cuenta_sap][int] NOT NULL, --fk
	[clave][char](1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[valor_defecto_mxn][float]NULL,
	[valor_defecto_usd][float]NULL,
	[valor_defecto_eur][float]NULL,	
	[valor_fijo][bit]NULL
 CONSTRAINT [PK_budget_rel_conceptos_formulas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agrega la columna para comentarios
ALTER TABLE [budget_rel_conceptos_formulas] 
ADD aplica_comentario bit NOT NULL DEFAULT 0;


-- restriccion de clave foranea
  alter table [budget_rel_conceptos_formulas]
 add constraint FK_budget_rel_conceptos_formulas_id_budget_cuenta_sap
  foreign key (id_budget_cuenta_sap)
  references budget_cuenta_sap(id);


SET IDENTITY_INSERT [budget_rel_conceptos_formulas] ON 

INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (1,2,'a','No. Días',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (2,2,'b','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (3,2,'c','Tarifa Nacional',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (4,2,'d','No. Días',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (5,2,'e','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (6,2,'f','Tarifa Extranjero',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (7,3,'a','No. Días',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (8,3,'b','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (9,3,'c','Tarifa Nacional por día',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (10,3,'d','No. Días',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (11,3,'e','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (12,3,'f','Tarifa Extranjero por día',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (13,4,'a','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (14,4,'b','Vuelo redondo nacional',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (15,4,'c','No. Personas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (16,4,'d','Vuelo redondo extranjero',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (17,10,'a','Litigation',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (18,10,'b','Business related legal services',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (19,10,'c','Debt Collection',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (20,10,'d','Labor and Employment Law',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (21,10,'e','Corporate Governance',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (22,10,'f','Regulatory Matters',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (23,11,'a','Auditoria A',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (24,11,'b','Auditoria B',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (25,11,'c','Auditoria C',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (26,11,'d','Otra',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (27,12,'a','Consultoria A',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (28,12,'b','Consultoria B',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (29,12,'c','Consultoria C',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (30,12,'d','Otra',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (31,15,'a','No teléfonos',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (32,15,'b','Tarifa por teléfono',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (33,15,'c','Servicio de internet/soporte de red',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (34,16,'a','No radios',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (35,16,'b','Precio por radio',123, null,null,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (36,16,'c','Accesorios comunicaciones',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (37,19,'a','Gensuite',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (38,19,'b','Plataforma de gestión mantto.',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (39,19,'c','Otros',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (40,20,'a','P60 Environment (DXC)',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (41,20,'b','Software One – Think Cell 5 licenses',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (42,20,'c','IM Projects- OpEX',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (43,20,'d','Office 365',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (44,20,'e','Email tkMM',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (45,20,'f','Azure',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (46,20,'g','Otros',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (47,23,'a','No equipos de cómputo',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (48,23,'b','Precio por equipo',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (49,23,'c','No impresoras',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (50,23,'d','Precio por impresora',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (51,23,'f','Otros equipos',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (52,26,'a','Transportes',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (53,26,'b','Multiple Empresarial',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (54,26,'c','Responsabilidad Civil General',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (55,26,'d','Responsabilidad Civil Directores y Funcionarios',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (56,26,'e','Autos en exceso',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (57,38,'a','Audiometrías',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (58,38,'b','Espirometrías',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (59,38,'c','Radiografías',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (60,38,'d','Check-ups',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (61,38,'e','Antidoping',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (62,38,'f','Microbiolóbicos',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (63,38,'g','Equipo médico',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (64,38,'h','Equipo de protección personal',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (65,38,'i','Medicamentos y material de curación',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (66,38,'j','Atención de ancidentes de trabajo',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (67,38,'k','Ambulancias',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (68,38,'l','Servicio de paramédicos',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (69,62,'a','Precio KW por hora',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (70,62,'b','Kilowatts',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (71,63,'a','No pipas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (72,63,'b','Precio por pipa',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (73,63,'c','No garrafones de agua',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (74,63,'d','Precio por garrafón',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (75,64,'a','Gas montacargas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (76,64,'b','Gas servicio de comedor',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (77,93,'a','Botas',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (78,93,'b','Cascos',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (79,93,'c','Equipo contra incendios',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (80,96,'a','No. De guardias',null, null,null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (81,96,'b','Costo por día por guardia',null, null,null,0)


SET IDENTITY_INSERT [budget_rel_conceptos_formulas] OFF
GO
 	  
IF object_id(N'budget_rel_conceptos_formulas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_conceptos_formulas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_conceptos_formulas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

