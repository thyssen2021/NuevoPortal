USE Portal_2_0
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
	[id_solicitud_rel_material_item][int] NULL,
	[id_solicitud][int] NULL, --en caso de que no aplique el campo anterior
	[id_oc_existente][int] NULL,
	[id_centro_recibo][int] NULL,
	[id_proveedor][int] NULL,
	[id_incoterm][int] NULL,
	[id_frontera_puesto_planta] [int] NULL,
	[id_condiciones_pago][int] NULL,
	[id_transporte_1][int] NULL,
	[id_transporte_2][int] NULL,
	[id_molino][int] NULL,
	[id_clave_pais_origen] [int] NULL,
	[numero_orden_compra][varchar](10) NULL,
	[numero_dias_para_entrega][int] NULL,
	[cantidad_estandar][varchar](20) NULL,
	[cantidad_minima][varchar](20) NULL,
	[cantidad_maxima][varchar](20) NULL,
	[proveedor_otro][varchar](120) NULL,
	[nombre_fiscal_proveedor][varchar](150) NULL,
	[aplica_iva][bit] NULL,
	[vigencia_precio][datetime] NULL,
	[precio][varchar](20) NULL,
	[moneda][varchar](3) NULL,
	[cantidad_estimada_compra][int] NULL,
	[contacto][varchar](120) NULL,
	[telefono][varchar](20) NULL,
	[correo_electronico][varchar](120) NULL,
	[requerimientos_especificos][varchar](250) NULL,	
 CONSTRAINT [PK_SCDM_solicitud_rel_orden_compra_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_solicitud_rel_material_item
  foreign key (id_solicitud_rel_material_item)
  references SCDM_solicitud_rel_item_material(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);

      -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_oc_existente
  foreign key (id_oc_existente)
  references SCDM_cat_po_existente(id);

        -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_centro_recibo
  foreign key (id_centro_recibo)
  references SCDM_cat_centro_recibo(id);

         -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_proveedor
  foreign key (id_proveedor)
  references proveedores(clave);

           -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_incoterm
  foreign key (id_incoterm)
  references SCDM_cat_incoterm(id);

-- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_frontera_puesto_planta
  foreign key (id_frontera_puesto_planta)
  references SCDM_cat_frontera_puerto_planta(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_condiciones_pago
  foreign key (id_condiciones_pago)
  references SCDM_cat_po_condiciones_pago(id);

   -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_transporte_1
  foreign key (id_transporte_1)
  references SCDM_cat_po_transporte(id);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_transporte_2
  foreign key (id_transporte_2)
  references SCDM_cat_po_transporte(id);

     -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_molino
  foreign key (id_molino)
  references SCDM_cat_molino(id);

       -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_orden_compra]
 add constraint FK_SCDM_solicitud_rel_orden_compra_id_clave_pais_origen
  foreign key (id_clave_pais_origen)
  references SCDM_cat_clave_paises(id);
  

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
