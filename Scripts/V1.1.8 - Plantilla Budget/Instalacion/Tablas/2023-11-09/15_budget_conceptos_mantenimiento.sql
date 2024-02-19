--USE Portal_2_0
GO
IF object_id(N'budget_conceptos_mantenimiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_conceptos_mantenimiento]
      PRINT '<<< budget_conceptos_mantenimiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_conceptos_mantenimiento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_conceptos_mantenimiento](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_rel_fy_cc][int] NOT NULL, --fk
	[id_cuenta_sap][int] NOT NULL, --fk
	[mes][int] NOT NULL,
	[moneda][varchar](3) NOT NULL,
	[gasto][float] NOT NULL default 0.0,
	[one_time][float] NULL,
	
 CONSTRAINT [PK_budget_conceptos_mantenimiento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_conceptos_mantenimiento]
 add constraint FK_budget_conceptos_mantenimiento_id_rel_fy_cc
  foreign key (id_rel_fy_cc)
  references budget_rel_fy_centro(id);

  -- restriccion de clave foranea
  alter table [budget_conceptos_mantenimiento]
 add constraint FK_budget_conceptos_mantenimiento_id_cuenta_sap
  foreign key (id_cuenta_sap)
  references budget_cuenta_sap(id);
 	  
IF object_id(N'budget_conceptos_mantenimiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_conceptos_mantenimiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_conceptos_mantenimiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

