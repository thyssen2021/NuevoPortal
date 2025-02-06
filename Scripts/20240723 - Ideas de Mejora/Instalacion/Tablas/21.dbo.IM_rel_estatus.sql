GO
IF object_id(N'IM_rel_estatus',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_rel_estatus]
      PRINT '<<< IM_rel_estatus en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/
CREATE TABLE IM_rel_estatus(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[captura] [smalldatetime] NOT NULL,
	[ideaMejoraClave] [int] NOT NULL,
	[catalogoIdeaMejoraEstatusClave] [int] NOT NULL,
	[id_empleado][int] NULL,		--FK
 CONSTRAINT [PK_Estatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


--Agrega FK
  -- restriccion de clave foranea
alter table [dbo].IM_rel_estatus
 add constraint FK_IM_rel_estatus_ideaMejoraClave
  foreign key (ideaMejoraClave)
  references IM_idea_mejora(id);

alter table [dbo].IM_rel_estatus
 add constraint FK_IM_rel_estatus_catalogoIdeaMejoraEstatusClave
  foreign key (catalogoIdeaMejoraEstatusClave)
  references IM_cat_estatus(id);

  alter table [dbo].IM_rel_estatus
 add constraint FK_IM_rel_estatus_id_empleado
  foreign key (id_empleado)
  references empleados(id);

 GO


IF object_id(N'IM_rel_estatus',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_rel_estatus en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_rel_estatus  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
