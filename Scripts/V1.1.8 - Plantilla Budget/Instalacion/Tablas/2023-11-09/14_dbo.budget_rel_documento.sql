
GO
IF object_id(N'budget_rel_documento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_documento]
      PRINT '<<< budget_rel_documento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_rel_documento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_documento](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_rel_fy_centro][int] NOT NULL,
	[id_cuenta_sap][int] NOT NULL,
	[id_documento] [int] NULL,
	[comentario] [varchar](180) NULL
	
 CONSTRAINT [PK_budget_rel_documento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_rel_documento]
 add constraint FK_budget_rel_documento_id_rel_fy_centro
  foreign key (id_budget_rel_fy_centro)
  references budget_rel_fy_centro(id);

  -- restriccion de clave foranea
  alter table [budget_rel_documento]
 add constraint FK_budget_rel_documento_id_cuenta_sap
  foreign key (id_cuenta_sap)
  references budget_cuenta_sap(id);

    -- restriccion de clave foranea
  alter table [budget_rel_documento]
 add constraint FK_budget_rel_documento_id_documento
  foreign key (id_documento)
  references biblioteca_digital(id);

-- restriccion unique
 alter table [budget_rel_documento]
  add constraint UQ_budget_rel_documento_anio_sap_mes
  unique (id_budget_rel_fy_centro,id_cuenta_sap);

GO
 	  
IF object_id(N'budget_rel_documento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_documento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_documento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
