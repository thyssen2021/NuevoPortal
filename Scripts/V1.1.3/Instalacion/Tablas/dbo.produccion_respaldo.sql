use[Portal_2_0]
GO
IF object_id(N'produccion_respaldo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_respaldo]
      PRINT '<<< produccion_respaldo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los lotes de produccion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_respaldo](
	[id][int] IDENTITY(1,1) NOT NULL,
	[empleado_id][int] NULL, --empleado que sube la bitacora
	[fecha_carga][datetime] NOT NULL,  --fecha en la que se sube el respaldo
	[planta][varchar](50) NULL,
	[linea][varchar](50) NULL,
	[operador][varchar](100) NULL,
	[supervisor][varchar](100) NULL,
	[sap_platina][varchar](50) NULL,
	[tipo_material][varchar](50) NULL,
	[numero_parte_cliente][varchar](80) NULL,
	[sap_rollo][varchar](50) NULL,
	[material][varchar](50) NULL,
	[fecha][datetime] NULL,
	[turno][varchar](20) NULL,
	[hora][datetime] NULL,
	[orden_sap][varchar](25) NULL,
	[orden_sap_2][varchar](25) NULL,
	[pieza_por_golpe][float] NULL,
	[numero_rollo][varchar](30) NULL,
	[lote_rollo][varchar](30) NULL,
	[peso_etiqueta][float] NULL,
	[peso_regreso_rollo_real][float] NULL,
	[peso_rollo_usado][float] NULL,
	[peso_bascula_kgs][float] NULL,
	[piezas_por_paquete][float] NULL,
	[total_piezas][float] NULL,
	[peso_rollo_consumido][float] NULL,
	[numero_golpes][float] NULL,
	[peso_despunte_kgs][float] NULL,
	[peso_cola_kgs][float] NULL,
	[porcentaje_punta_y_colas][float] NULL,
	[total_piezas_ajuste][float] NULL,
	[peso_bruto_kgs][float] NULL,
	[peso_real_pieza_bruto][float] NULL,
	[peso_real_pieza_neto][float] NULL,
	[scrap_natural][float] NULL,
	[ordenes_por_pieza][float] NULL,
	[peso_rollo_usado_real_kgs][float] NULL,
	[peso_bruto_total_piezas_kgs][float] NULL,
	[peso_beto_total_piezas_kgs][float] NULL,
	[scrap_ingenieria_buenas_mas_ajuste][float] NULL,
	[peso_neto_total_piezas_ajuste][float] NULL,
	[peso_punta_y_colas_reales][float] NULL,
	[balance_scrap_real][float] NULL,
 CONSTRAINT [PK_produccion_respaldo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_respaldo]
 add constraint FK_produccion_respaldo_id_empleado
  foreign key (empleado_id)
  references empleados(id);

GO

 	  
IF object_id(N'produccion_respaldo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_respaldo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_respaldo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
