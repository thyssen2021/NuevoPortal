use[Portal_2_0]
GO
IF object_id(N'produccion_lotes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_lotes]
      PRINT '<<< produccion_lotes en Base de Datos:' + db_name() + 
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


CREATE TABLE [dbo].[produccion_lotes](	
	[id_produccion_registro][int] NOT NULL,		
	[numero_lote_izquierdo] [int] null,
	[numero_lote_derecho] [int] null,
	[piezas_paquete] int DEFAULT 0
 CONSTRAINT [PK_produccion_lotes] PRIMARY KEY CLUSTERED 
(
	[id_produccion_registro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_lotes]
 add constraint FK_produccion_lotes_id_produccion_registro
  foreign key (id_produccion_registro)
  references produccion_registros(id);

GO

 	  
IF object_id(N'produccion_lotes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_lotes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_lotes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
