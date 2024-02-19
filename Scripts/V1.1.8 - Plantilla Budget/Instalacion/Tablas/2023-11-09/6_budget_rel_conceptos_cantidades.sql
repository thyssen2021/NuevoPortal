--USE Portal_2_0
GO
IF object_id(N'budget_rel_conceptos_cantidades',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_conceptos_cantidades]
      PRINT '<<< budget_rel_conceptos_cantidades en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_rel_conceptos_cantidades
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_conceptos_cantidades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_cantidad][int] NOT NULL, --fk
	[id_rel_conceptos][int] NOT NULL, --fk
	[cantidad][float] NULL,
	[comentario][varchar](80) NULL
	
 CONSTRAINT [PK_budget_rel_conceptos_cantidades] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_rel_conceptos_cantidades]
 add constraint FK_budget_rel_conceptos_cantidades_id_budget_cantidad
  foreign key (id_budget_cantidad)
  references budget_cantidad(id);

  -- restriccion de clave foranea
  alter table [budget_rel_conceptos_cantidades]
 add constraint FK_budget_rel_conceptos_cantidades_id_rel_conceptos
  foreign key (id_rel_conceptos)
  references budget_rel_conceptos_formulas(id);

  
SET IDENTITY_INSERT [budget_rel_conceptos_cantidades] ON 


SET IDENTITY_INSERT [budget_rel_conceptos_cantidades] OFF
GO
 	  
IF object_id(N'budget_rel_conceptos_cantidades',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_conceptos_cantidades en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_conceptos_cantidades  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

