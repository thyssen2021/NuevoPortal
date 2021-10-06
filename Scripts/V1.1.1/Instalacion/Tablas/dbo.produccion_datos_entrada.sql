use[Portal_2_0]
GO
IF object_id(N'produccion_datos_entrada',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_datos_entrada]
      PRINT '<<< produccion_datos_entrada en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos de entrada de producción
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_datos_entrada](	
	[id_produccion_registro][int] NOT NULL,		
	[orden_sap][varchar](15) NULL,
	[orden_sap_2][varchar](15) NULL,
	[piezas_por_golpe][int] DEFAULT 0,
	[numero_rollo][varchar](30) NULL,
	[lote_rollo][varchar](10) NULL,
	[peso_etiqueta] float DEFAULT 0,
	[peso_regreso_rollo_real] float DEFAULT 0,
	[peso_bascula_kgs] float DEFAULT 0,
	[peso_despunte_kgs] float DEFAULT 0,
	[peso_cola_kgs] float DEFAULT 0,
	[total_piezas_ajuste] int DEFAULT 0,
	[ordenes_por_pieza] int DEFAULT 0
 CONSTRAINT [PK_produccion_datos_entrada] PRIMARY KEY CLUSTERED 
(
	[id_produccion_registro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_datos_entrada]
 add constraint FK_produccion_datos_entrada_id_produccion_registro
  foreign key (id_produccion_registro)
  references produccion_registros(id);

GO

 	  
IF object_id(N'produccion_datos_entrada',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_datos_entrada en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_datos_entrada  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
