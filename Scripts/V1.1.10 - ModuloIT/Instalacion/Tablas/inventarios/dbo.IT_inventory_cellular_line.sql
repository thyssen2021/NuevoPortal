USE Portal_2_0
GO
IF object_id(N'IT_inventory_cellular_line',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_cellular_line]
      PRINT '<<< IT_inventory_cellular_line en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_cellular_line
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/07/05
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_cellular_line](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_inventory_celullar_plan][int]NOT NULL,
	[id_planta][int]NOT NULL,
	[numero_celular][varchar](15) NOT NULL,
	[fecha_corte][datetime] NULL,
	[activo] [bit] NOT NULL DEFAULT 1, 
 CONSTRAINT [PK_IT_inventory_cellular_line] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_inventory_cellular_line]
 add constraint FK_IATF_documentos_id_inventory_celullar_plan
  foreign key (id_inventory_celullar_plan)
  references IT_inventory_cellular_plans(id);

  -- restriccion de clave foranea
alter table [IT_inventory_cellular_line]
 add constraint FK_IT_inventory_cellular_line_id_planta
  foreign key (id_planta)
  references plantas(clave);

--UNIQUE
  -- restriccion unique
 alter table [IT_inventory_cellular_line]
  add constraint UQ_IT_inventory_cellular_line_num_cel
  unique (numero_celular);

 	  
IF object_id(N'IT_inventory_cellular_line',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_cellular_line en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_cellular_line  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
