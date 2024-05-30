GO
IF object_id(N'SCDM_solicitud_rel_orden_compra',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_orden_compra]
      PRINT '<<< SCDM_solicitud_rel_orden_compra en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_orden_compra
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_orden_compra](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud][int] NULL,  --FK
	[oc_existente][varchar](20) NULL,	
	[numero_orden_compra_nueva_linea][varchar](10) NULL,
	[aplica_procesador_externo][bit] NULL,
	[num_proveedor_procesador_externo][varchar](120) NULL,
	[centro_recibo][varchar](4) NULL,
	[numero_dias_para_entrega][int] NULL,
	[cantidad_estandar][varchar](20) NULL,
	[cantidad_minima][varchar](20) NULL,
	[cantidad_maxima][varchar](20) NULL,
	[num_proveedor_material][varchar](120) NULL,
	[nombre_fiscal_proveedor][varchar](150) NULL,
	[aplica_iva][bit] NULL,
	[vigencia_precio][varchar](80) NULL,
	[incoterm][varchar](60) NULL,
	[frontera_puerto][varchar](30) NULL,
	[condiciones_pago][varchar](110) NULL,
	[transporte_1][varchar](30) NULL,
	[transporte_2][varchar](30) NULL,
	[num_material][varchar](25) NULL,
	[numero_parte_cliente][varchar](60) NULL,
	[dimensiones][varchar](80) NULL,
	[precio][varchar](25) NULL,
	[moneda][varchar](3) NULL,
	[unidad_medida][varchar](10) NULL,
	[cantidad_estimada_compra][varchar](25) NULL,
	[descripcion][varchar](50) NULL,
	[peso_minimo][varchar](20) NULL,
	[peso_maximo][varchar](20) NULL,
	[tipo_compra][varchar](30) NULL,
	[contacto][varchar](120) NULL,
	[telefono][varchar](20) NULL,
	[correo_electronico][varchar](120) NULL,
	[requerimientos_especificos][varchar](250) NULL,	
	[molino][varchar](60) NULL,
	[pais_origen][varchar](60) NULL,
 CONSTRAINT [PK_SCDM_solicitud_rel_orden_compra_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


    -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);


--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_solicitud_rel_orden_compra] ON 

SET IDENTITY_INSERT [dbo].[SCDM_solicitud_rel_orden_compra] OFF
	  
IF object_id(N'SCDM_solicitud_rel_orden_compra',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_orden_compra en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_orden_compra  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
