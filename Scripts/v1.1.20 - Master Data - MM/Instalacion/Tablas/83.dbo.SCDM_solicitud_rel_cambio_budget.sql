--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_cambio_budget',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_cambio_budget]
      PRINT '<<< SCDM_solicitud_rel_cambio_budget en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_cambio_budget
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_cambio_budget](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,	
	[id_planta] [int]  NULL,
	[material_existente] [varchar](12) NULL,
    [peso_bruto_real_bascula] [float] NULL,	
	[peso_neto_real_bascula] [float] NULL,	
	[angulo_a] [float] NULL,	
	[angulo_b] [float] NULL,	
	[scrap_permitido_puntas_colas] [float] NULL,	
	[pieza_doble][varchar](2) NULL,
	[reaplicacion][bit] NULL,
	[conciliacion_puntas_colas][bit] NULL,
	[conciliacion_scrap_ingenieria][bit] NULL,
	[tipo_metal] [varchar](80) NULL,	
	[tipo_material] [varchar](80) NULL,	
	[tipo_venta] [varchar](80) NULL,	
	[modelo_negocio] [varchar](80) NULL,	
	[posicion_rollo] [varchar](80) NULL,	
	[IHS_num_1] [varchar](120) NULL,	
	[IHS_num_2] [varchar](120) NULL,	
	[IHS_num_3] [varchar](120) NULL,	
	[IHS_num_4] [varchar](120) NULL,	
	[IHS_num_5] [varchar](120) NULL,	
	[piezas_por_auto] [float] NULL,	
	[piezas_por_golpe] [float] NULL,	
	[piezas_por_paquete] [float] NULL,	
	[peso_inicial] [float] NULL,	
	[peso_maximo] [float] NULL,	
	[peso_maximo_tolerancia_positiva] [float] NULL,	
	[peso_maximo_tolerancia_negativa] [float] NULL,	
	[peso_minimo] [float] NULL,	
	[peso_minimo_tolerancia_positiva] [float] NULL,	
	[peso_minimo_tolerancia_negativa] [float] NULL,		
	[comentarios] [varchar](120) NULL,
	[ejecucion_correcta] [varchar](120) NULL,
	[resultado] [varchar](120) NULL, --Mensaje SAP
 CONSTRAINT [PK_SCDM_solicitud_rel_cambio_budget_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_cambio_budget]
 add constraint FK_SCDM_solicitud_rel_cambio_budget_id_planta
  foreign key (id_planta)
  references plantas(clave);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_cambio_budget]
 add constraint FK_SCDM_solicitud_rel_cambio_budget_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);


IF object_id(N'SCDM_solicitud_rel_cambio_budget',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_cambio_budget en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_cambio_budget  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
