use[Portal_2_0]
GO
IF object_id(N'inspeccion_pieza_descarte_produccion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[inspeccion_pieza_descarte_produccion]
      PRINT '<<< inspeccion_pieza_descarte_produccion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las piezas de descarte asociadas a un registro
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/01/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[inspeccion_pieza_descarte_produccion](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_produccion_registro][int] NOT NULL,		
	[id_falla] [int] NOT NULL,
	[cantidad] [int] NOT NULL DEFAULT 0
 CONSTRAINT [PK_inspeccion_pieza_descarte_produccion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [inspeccion_pieza_descarte_produccion]
 add constraint FK_inspeccion_pieza_descarte_produccion_id_produccion_registro
  foreign key (id_produccion_registro)
  references produccion_registros(id);
GO

-- restriccion de clave foranea
alter table [inspeccion_pieza_descarte_produccion]
 add constraint FK_inspeccion_pieza_descarte_produccion_id_falla
  foreign key (id_falla)
  references inspeccion_fallas(id);
GO

 	  
IF object_id(N'inspeccion_pieza_descarte_produccion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< inspeccion_pieza_descarte_produccion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla inspeccion_pieza_descarte_produccion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
