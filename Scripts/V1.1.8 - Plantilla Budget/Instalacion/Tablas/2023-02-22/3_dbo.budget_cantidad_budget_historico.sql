USE Portal_2_0
GO
IF object_id(N'budget_cantidad_budget_historico',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_cantidad_budget_historico]
      PRINT '<<< budget_cantidad_budget_historico en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_cantidad_budget_historico
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_cantidad_budget_historico](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_rel_fy_centro][int] NOT NULL,
	[id_cuenta_sap][int] NOT NULL,
	[mes][int] NOT NULL,
	[currency_iso][varchar](3) NOT NULL,
	[cantidad] [decimal](14,2) NOT NULL,
	[comentario][varchar](150) NULL,
	
 CONSTRAINT [PK_budget_cantidad_budget_historico] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_cantidad_budget_historico]
 add constraint FK_budget_cantidad_budget_historico_id_rel_fy_centro
  foreign key (id_budget_rel_fy_centro)
  references budget_rel_fy_centro(id);


  -- restriccion de clave foranea
  alter table [budget_cantidad_budget_historico]
 add constraint FK_budget_cantidad_budget_historico_id_cuenta_sap
  foreign key (id_cuenta_sap)
  references budget_cuenta_sap(id);
  
  -- restriccion fk
  alter table [budget_cantidad_budget_historico]
 add constraint FK_budget_cantidad_budget_historico_currency_iso
  foreign key (currency_iso)
  references currency(CurrencyISO);

-- restriccion unique
 alter table [budget_cantidad_budget_historico]
  add constraint UQ_budget_cantidad_budget_historico_anio_sap_mes
  unique (id_budget_rel_fy_centro,id_cuenta_sap,mes);

GO
 	  
IF object_id(N'budget_cantidad_budget_historico',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_cantidad_budget_historico en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_cantidad_budget_historico  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO