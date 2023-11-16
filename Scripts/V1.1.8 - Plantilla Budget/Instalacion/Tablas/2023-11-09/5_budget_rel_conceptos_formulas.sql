--USE Portal_2_0
GO
IF object_id(N'budget_rel_conceptos_formulas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_conceptos_formulas]
      PRINT '<<< budget_rel_conceptos_formulas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_rel_conceptos_formulas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_conceptos_formulas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_cuenta_sap][int] NOT NULL, --fk
	[clave][char](1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[valor_defecto_mxn][float]NULL,
	[valor_defecto_usd][float]NULL,
	[valor_defecto_eur][float]NULL,	
	[valor_fijo][bit]NULL
 CONSTRAINT [PK_budget_rel_conceptos_formulas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_rel_conceptos_formulas]
 add constraint FK_budget_rel_conceptos_formulas_id_budget_cuenta_sap
  foreign key (id_budget_cuenta_sap)
  references budget_cuenta_sap(id);


SET IDENTITY_INSERT [budget_rel_conceptos_formulas] ON 

INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (1,2,'a','No. Días',null, null, null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (2,2,'b','No. Personas',null, null, null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (3,2,'c','Tarifa Nacional', 1700, 100, 90,1)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (4,2,'d','No. Días',null, null, null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (5,2,'e','No. Personas',null, null, null,0)
INSERT [budget_rel_conceptos_formulas] ([id],[id_budget_cuenta_sap], clave, descripcion, [valor_defecto_mxn], valor_defecto_usd, valor_defecto_eur, valor_fijo) 
		VALUES (6,2,'f','Tarifa Extranjero', 2500, 150, 120,1)

SET IDENTITY_INSERT [budget_rel_conceptos_formulas] OFF
GO
 	  
IF object_id(N'budget_rel_conceptos_formulas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_conceptos_formulas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_conceptos_formulas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

