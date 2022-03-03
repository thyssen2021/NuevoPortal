USE Portal_2_0
GO
IF object_id(N'budget_comentarios_rel_anio_cuenta',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_comentarios_rel_anio_cuenta]
      PRINT '<<< budget_comentarios_rel_anio_cuenta en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_comentarios_rel_anio_cuenta
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_comentarios_rel_anio_cuenta](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_anio_fiscal][int] NOT NULL,
	[id_centro_costo][int] NOT NULL,
	[id_cuenta_sap][int] NOT NULL,	
	[comentario] [varchar](150)  NULL,
	
 CONSTRAINT [PK_budget_comentarios_rel_anio_cuenta] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
  alter table [budget_comentarios_rel_anio_cuenta]
 add constraint FK_budget_comentarios_rel_anio_cuenta_id_rel_anio_centro
  foreign key (id_rel_anio_centro)
  references budget_rel_anio_fiscal_centro(id);

  -- restriccion de clave foranea
  alter table [budget_comentarios_rel_anio_cuenta]
 add constraint FK_budget_comentarios_rel_anio_cuenta_id_cuenta_sap
  foreign key (id_cuenta_sap)
  references budget_cuenta_sap(id);

-- restriccion unique
 alter table [budget_comentarios_rel_anio_cuenta]
  add constraint UQ_budget_comentarios_rel_anio_cuenta_anio_sap_mes
  unique (id_rel_anio_centro,id_cuenta_sap);
  
GO

 	  
IF object_id(N'budget_comentarios_rel_anio_cuenta',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_comentarios_rel_anio_cuenta en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_comentarios_rel_anio_cuenta  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
