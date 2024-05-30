--USE [Portal_2_0_scdm]
GO

ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] DROP CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_tipo_material]
GO


ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] DROP CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_solicitud]
GO


--ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] DROP CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_aleacion]
--GO

/****** Object:  Table [dbo].[SCDM_solicitud_rel_item_material]    Script Date: 08/12/2023 09:41:50 a.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SCDM_solicitud_rel_item_material]') AND type in (N'U'))
DROP TABLE [dbo].[SCDM_solicitud_rel_item_material]
GO

/****** Object:  Table [dbo].[SCDM_solicitud_rel_item_material]    Script Date: 08/12/2023 09:41:50 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SCDM_solicitud_rel_item_material](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_solicitud] [int] NOT NULL,
	[id_tipo_material] [int] NOT NULL,
	[tipo_venta] [varchar](50) NULL,  --FK 
	[cliente] [varchar](120) NULL,
	[proveedor] [varchar](150) NULL,
	[molino] [varchar](60) NULL,
	[metal] [varchar](60) NULL,
	[unidad_medida_inventario] [varchar](5) NULL,
	[tipo_recubrimiento] [varchar](60) NULL,
	[clase_aprovisionamiento][varchar](60) NULL,
	[diametro_interior] [int] NULL,
	[peso_recubrimiento] [varchar](60) NULL,
	[parte_interior_exterior] [varchar](60) NULL,
	[posicion_rollo] [varchar](60) NULL,
	[ihs_1] [varchar](250) NULL,
	[ihs_2] [varchar](250) NULL,
	[ihs_3] [varchar](250) NULL,
	[ihs_4] [varchar](250) NULL,
	[ihs_5] [varchar](250) NULL,
	[modelo_negocio] [varchar](50) NULL,
	[tipo_transito] [varchar](30) NULL,
	[procesador_externo_nombre][varchar](120) NULL,
	[disponente] [varchar](120) NULL,
	[material_del_cliente] [bit] NULL,
	[numero_odc_cliente] [varchar](15) NULL,
	[requiere_ppaps] [bit] NULL,
	[requiere_imds] [bit] NULL,
	[numero_material] [varchar](20) NULL,
	[descripcion_material_es] [varchar](120) NULL,
	[descripcion_material_en] [varchar](120) NULL,
	[grado_calidad] [varchar](50) NULL,
	[numero_parte] [varchar](60) NULL,
	[descripcion_numero_parte] [varchar](80) NULL,
	[norma_referencia] [varchar](60) NULL,
	[espesor_mm] [float] NULL,
	[espesor_tolerancia_negativa_mm] [float] NULL,
	[espesor_tolerancia_positiva_mm] [float] NULL,
	[ancho_mm] [float] NULL,
	[ancho_tolerancia_negativa_mm] [float] NULL,
	[ancho_tolerancia_positiva_mm] [float] NULL,
	[diametro_exterior_maximo_mm] [float] NULL,
	[peso_min_kg] [float] NULL,
	[peso_max_kg] [float] NULL,
	[aplica_procesador_externo] [bit] NULL,
	[numero_antiguo_material] [varchar](50) NULL,
	[planicidad_mm] [float] NULL,
	[msa_hoda] [varchar](50) NULL,
	[requiere_consiliacion_puntas_colar] [bit] NULL,
	[scrap_permitido_puntas_colas] [float] NULL,
	[fecha_validez] [datetime] NULL,
	[numero_cintas_resultantes] [int] NULL,
	[diametro_interior_salida] [int] NULL,
	[ancho_entrega_cinta_mm] [float] NULL,
	[avance_mm] [float] NULL,
	[avance_tolerancia_negativa_mm] [float] NULL,
	[avance_tolerancia_positiva_mm] [float] NULL,
	[forma] [varchar](20) NULL,
	[piezas_por_golpe] [int] NULL,
	[piezas_por_paquete] [int] NULL,
	[peso_bruto] [float] NULL,
	[peso_neto] [float] NULL,
	[piezas_por_auto] [int] NULL,
	[peso_inicial] [float] NULL,
	[porcentaje_scrap_puntas_colas] [float] NULL,
	[conciliacion_scrap_ingenieria] [bit] NULL,
	[angulo_a] [int] NULL,
	[angulo_b] [int] NULL,
	[material_compra_tkmm] [bit] NULL,
	[materia_prima_producto_terminado] [varchar](30) NULL,
	[tipo_metal_cb] [varchar] (40)NULL,
	[aleacion] [varchar](15) NULL,
	[precio] [float] NULL,
	[moneda] [varchar](22) NULL,
	[incoterm] [varchar](55) NULL,
	[terminos_pago][varchar](108) NULL,
	[aplica_tasa_iva] [bit] NULL,
 CONSTRAINT [PK_SCDM_solicitud_rel_item_material_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material]  WITH CHECK ADD  CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_aleacion] FOREIGN KEY([id_aleacion])
--REFERENCES [dbo].[SCDM_cat_aleacion] ([id])
--GO

--ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] CHECK CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_aleacion]
--GO


ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material]  WITH CHECK ADD  CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_solicitud] FOREIGN KEY([id_solicitud])
REFERENCES [dbo].[SCDM_solicitud] ([id])
GO

ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] CHECK CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_solicitud]
GO


ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material]  WITH CHECK ADD  CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_tipo_material] FOREIGN KEY([id_tipo_material])
REFERENCES [dbo].[SCDM_cat_tipo_materiales_solicitud] ([id])
GO

ALTER TABLE [dbo].[SCDM_solicitud_rel_item_material] CHECK CONSTRAINT [FK_SCDM_solicitud_rel_item_material_id_tipo_material]
GO
