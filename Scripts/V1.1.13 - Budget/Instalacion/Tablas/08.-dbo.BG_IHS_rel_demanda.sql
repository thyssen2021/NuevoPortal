--USE Portal_2_0
GO
IF object_id(N'BG_IHS_rel_demanda',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[BG_IHS_rel_demanda]
      PRINT '<<< BG_IHS_rel_demanda en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos BG_IHS_rel_demanda
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/09/16
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [BG_IHS_rel_demanda](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_ihs_item] [int] NOT NULL,	
	[cantidad] [int] NULL,
	[fecha] [datetime] NOT NULL,
	[fecha_carga] [datetime] NULL,
	[tipo][VARCHAR](15) NOT NULL,	
 CONSTRAINT [PK_BG_IHS_rel_demanda] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [BG_IHS_rel_demanda]
 add constraint FK_BG_IHS_rel_demanda_id_ihs_item
  foreign key (id_ihs_item)
  references BG_IHS_item(id);
 
-- restricion check
ALTER TABLE [BG_IHS_rel_demanda] ADD CONSTRAINT CK_BG_IHS_rel_demanda_tipo CHECK ([tipo] IN 
('ORIGINAL', 'CUSTOMER')
)
 	  
IF object_id(N'BG_IHS_rel_demanda',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< BG_IHS_rel_demanda en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla BG_IHS_rel_demanda  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
