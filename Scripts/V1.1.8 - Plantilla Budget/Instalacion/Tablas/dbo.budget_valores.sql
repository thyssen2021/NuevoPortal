USE Portal_2_0
GO
IF object_id(N'budget_valores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_valores]
      PRINT '<<< budget_valores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_valores
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_valores](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_rel_anio_centro][int] NOT NULL,
	[id_cuenta_sap][int] NOT NULL,
	[mes][int] NOT NULL,
	[currency_iso][varchar](3) NOT NULL,
	[cantidad] [decimal](14,2) NOT NULL,
	[comentario] [varchar](100) NOT NULL,
	
 CONSTRAINT [PK_budget_valores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_valores]
 add constraint FK_budget_valores_id_rel_anio_centro
  foreign key (id_rel_anio_centro)
  references budget_rel_anio_fiscal_centro(id);

  -- restriccion de clave foranea
  alter table [budget_valores]
 add constraint FK_budget_valores_id_cuenta_sap
  foreign key (id_cuenta_sap)
  references budget_cuenta_sap(id);
  
  -- restriccion fk
  alter table [budget_valores]
 add constraint FK_budget_valores_currency_iso
  foreign key (currency_iso)
  references currency(CurrencyISO);

-- restriccion unique
 alter table [budget_valores]
  add constraint UQ_budget_valores_anio_sap_mes
  unique (id_rel_anio_centro,id_cuenta_sap,mes);


GO

 	  
IF object_id(N'budget_valores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_valores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_valores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
