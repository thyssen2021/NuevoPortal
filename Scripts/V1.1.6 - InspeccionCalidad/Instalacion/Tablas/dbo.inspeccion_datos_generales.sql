use[Portal_2_0]
GO
IF object_id(N'inspeccion_datos_generales',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[inspeccion_datos_generales]
      PRINT '<<< inspeccion_datos_generales en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos generales de los registros de inspeccion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[inspeccion_datos_generales](	
	[id_produccion_registro][int] NOT NULL,	
	[id_empleado_inspector][int] NOT NULL,
	[comentarios][varchar](255) NULL,
 CONSTRAINT [PK_inspeccion_datos_generales] PRIMARY KEY CLUSTERED 
(
	[id_produccion_registro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [inspeccion_datos_generales]
 add constraint FK_inspeccion_datos_generales_id_produccion_registro
  foreign key (id_produccion_registro)
  references produccion_registros(id);
GO

-- restriccion de clave foranea
alter table [inspeccion_datos_generales]
 add constraint FK_inspeccion_datos_generales_id_empleado_inspector
 foreign key (id_empleado_inspector)
  references empleados(id);
GO
 	  
IF object_id(N'inspeccion_datos_generales',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< inspeccion_datos_generales en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla inspeccion_datos_generales  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
