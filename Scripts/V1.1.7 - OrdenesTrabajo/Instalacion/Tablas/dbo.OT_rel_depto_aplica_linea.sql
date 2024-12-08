USE Portal_2_0
GO
IF object_id(N'OT_rel_depto_aplica_linea',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_rel_depto_aplica_linea]
      PRINT '<<< OT_rel_depto_aplica_linea en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos OT_rel_depto_aplica_linea
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/04/28
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_rel_depto_aplica_linea](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_area][int] NOT NULL,
 CONSTRAINT [PK_OT_rel_depto_aplica_linea] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
  alter table [OT_rel_depto_aplica_linea]
 add constraint FK_OT_rel_depto_aplica_linea_id_area
  foreign key (id_area)
  references Area(clave);

 	  
IF object_id(N'OT_rel_depto_aplica_linea',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_rel_depto_aplica_linea en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_rel_depto_aplica_linea  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
