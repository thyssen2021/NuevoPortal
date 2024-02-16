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
	[valor_fijo][bit] NULL,
	[aplica_comentario] [bit] NOT NULL DEFAULT 0
 CONSTRAINT [PK_budget_rel_conceptos_formulas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [budget_rel_conceptos_formulas]
 add constraint FK_budget_rel_conceptos_formulas_id_budget_cuenta_sap
  foreign key (id_budget_cuenta_sap)
  references budget_cuenta_sap(id);

  SET IDENTITY_INSERT [dbo].[budget_rel_conceptos_formulas] ON 
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (1, 2, N'a', N'No. Días', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (2, 2, N'b', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (3, 2, N'c', N'Tarifa Nacional', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (4, 2, N'd', N'No. Días', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (5, 2, N'e', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (6, 2, N'f', N'Tarifa Extranjero', NULL, 12.3, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (7, 3, N'a', N'No. Días', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (8, 3, N'b', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (9, 3, N'c', N'Tarifa Nacional por día', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (10, 3, N'd', N'No. Días', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (11, 3, N'e', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (12, 3, N'f', N'Tarifa Extranjero por día', NULL, 12.3, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (13, 4, N'a', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (14, 4, N'b', N'Vuelo redondo nacional', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (15, 4, N'c', N'No. Personas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (16, 4, N'd', N'Vuelo redondo extranjero', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (17, 10, N'a', N'Litigation', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (18, 10, N'b', N'Business related legal services', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (19, 10, N'c', N'Debt Collection', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (20, 10, N'd', N'Labor and Employment Law', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (21, 10, N'e', N'Corporate Governance', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (22, 10, N'f', N'Regulatory Matters', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (23, 11, N'a', N'Auditoria A', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (24, 11, N'b', N'Auditoria B', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (25, 11, N'c', N'Auditoria C', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (26, 11, N'd', N'Otra', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (27, 12, N'a', N'Consultoria A', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (28, 12, N'b', N'Consultoria B', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (29, 12, N'c', N'Consultoria C', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (30, 12, N'd', N'Otra', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (31, 15, N'a', N'No teléfonos', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (32, 15, N'b', N'Tarifa por teléfono', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (33, 15, N'c', N'Servicio de internet/soporte de red', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (34, 16, N'a', N'No radios', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (35, 16, N'b', N'Precio por radio', 123, NULL, NULL, 1, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (36, 16, N'c', N'Accesorios comunicaciones', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (37, 19, N'a', N'Gensuite', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (38, 19, N'b', N'Plataforma de gestión mantto.', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (39, 19, N'c', N'Otros', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (40, 20, N'a', N'P60 Environment (DXC)', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (41, 20, N'b', N'Software One – Think Cell 5 licenses', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (42, 20, N'c', N'IM Projects- OpEX', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (43, 20, N'd', N'Office 365', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (44, 20, N'e', N'Email tkMM', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (45, 20, N'f', N'Azure', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (46, 20, N'g', N'Otros', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (47, 23, N'a', N'No equipos de cómputo', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (48, 23, N'b', N'Precio por equipo', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (49, 23, N'c', N'No impresoras', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (50, 23, N'd', N'Precio por impresora', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (51, 23, N'e', N'Otros equipos', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (52, 26, N'a', N'Transportes', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (53, 26, N'b', N'Multiple Empresarial', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (54, 26, N'c', N'Responsabilidad Civil General', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (55, 26, N'd', N'Responsabilidad Civil Directores y Funcionarios', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (56, 26, N'e', N'Autos en exceso', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (57, 38, N'a', N'Audiometrías', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (58, 38, N'b', N'Espirometrías', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (59, 38, N'c', N'Radiografías', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (60, 38, N'd', N'Check-ups', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (61, 38, N'e', N'Antidoping', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (62, 38, N'f', N'Microbiolóbicos', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (63, 38, N'g', N'Equipo médico', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (64, 38, N'h', N'Equipo de protección personal', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (65, 38, N'i', N'Medicamentos y material de curación', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (66, 38, N'j', N'Atención de ancidentes de trabajo', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (67, 38, N'k', N'Ambulancias', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (68, 38, N'l', N'Servicio de paramédicos', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (69, 62, N'a', N'Precio KW por hora', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (70, 62, N'b', N'Kilowatts', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (71, 63, N'a', N'No pipas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (72, 63, N'b', N'Precio por pipa', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (73, 63, N'c', N'No garrafones de agua', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (74, 63, N'd', N'Precio por garrafón', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (75, 64, N'a', N'Gas montacargas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (76, 64, N'b', N'Gas servicio de comedor', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (77, 93, N'a', N'Botas', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (78, 93, N'b', N'Cascos', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (79, 93, N'c', N'Equipo contra incendios', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (80, 96, N'a', N'No. De guardias', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (81, 96, N'b', N'Costo por día por guardia', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (82, 38, N'm', N'Otros', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (83, 14, N'a', N'Evento A (Núm Personas)', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (84, 14, N'b', N'Tarifa por Persona', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (85, 14, N'c', N'Evento B (Núm Personas)', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (86, 14, N'd', N'Tarifa por Persona', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (87, 14, N'e', N'Evento C (Núm Personas)', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (88, 14, N'f', N'Tarifa por Persona', NULL, NULL, NULL, 0, 0)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (89, 14, N'g', N'Otro (Núm Personas)', NULL, NULL, NULL, 0, 1)
GO
INSERT [dbo].[budget_rel_conceptos_formulas] ([id], [id_budget_cuenta_sap], [clave], [descripcion], [valor_defecto_mxn], [valor_defecto_usd], [valor_defecto_eur], [valor_fijo], [aplica_comentario]) VALUES (90, 14, N'h', N'Tarifa por Persona', NULL, NULL, NULL, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[budget_rel_conceptos_formulas] OFF
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

