--USE Portal_2_0
GO
IF object_id(N'BG_IHS_rel_cuartos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_rel_cuartos]
      PRINT '<<< BG_IHS_rel_cuartos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_rel_cuartos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/03
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_rel_cuartos](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_ihs_item] [int] NOT NULL,	
	[cuarto] [int] NOT NULL,
	[anio] [int] NOT NULL,
	[cantidad] [int] NULL,
	[fecha_carga] [datetime] NULL
 CONSTRAINT [PK_BG_IHS_rel_cuartos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [BG_IHS_rel_cuartos]
 add constraint FK_BG_IHS_rel_cuartos_id_ihs_item
  foreign key (id_ihs_item)
  references BG_IHS_item(id);

IF object_id(N'BG_IHS_rel_cuartos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_rel_cuartos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_rel_cuartos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
