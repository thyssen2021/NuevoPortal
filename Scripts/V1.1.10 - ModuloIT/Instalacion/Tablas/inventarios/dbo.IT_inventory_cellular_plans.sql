USE Portal_2_0
GO
IF object_id(N'IT_inventory_cellular_plans',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_cellular_plans]
      PRINT '<<< IT_inventory_cellular_plans en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_cellular_plans
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_cellular_plans](
	[id] [int] IDENTITY(1,1) NOT NULL,
	--[id_planta] [int] NULL,  --id_planta se obtendrá de quien esté asignado a la solicitud  
	[id_it_inventory_items] [int] NULL, --celular de inventario
	[razon_social] [varchar](120) NULL,
	[cuenta_padre][varchar](8) NULL,
	[cuenta_hija][varchar](8) NULL,
	[num_telefono][varchar](10) NOT NULL,
	[centro_costo][varchar](4) NULL,
	[fecha_corte][datetime] NULL,
	[numero_factura][varchar](15) NULL,
	[nombre_plan][varchar](120) NOT NULL,
	[costo_servicios_telecomunicaciones][decimal](7,2) NULL,
	[costo_servicios_y_suscripciones][decimal](7,2) NULL,
	[costo_servicios_y_suscripciones_terceros][decimal](7,2) NULL,
	[costo_equipo_celular][decimal](7,2) NULL,
	[costo_servicios_cobrados_terceros][decimal](7,2) NULL,
	[porcentaje_iva][decimal](4,2) NULL,
	[nombre_compania][varchar](80) NOT NULL,
	[comentarios][varchar](250) NULL,
	[activo] [bit] NOT NULL DEFAULT 1, 
 CONSTRAINT [PK_IT_inventory_cellular_plans] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_inventory_cellular_plans]
 add constraint FK_IT_id_it_inventory_items
  foreign key (id_it_inventory_items)
  references IT_inventory_items(id);  
GO

--UNIQUE
--restringir mediante código para que no se pueda insertar un registro numero celular repetido

 	  
IF object_id(N'IT_inventory_cellular_plans',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_cellular_plans en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_cellular_plans  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
