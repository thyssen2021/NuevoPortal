USE Portal_2_0
GO
IF object_id(N'budget_rel_anio_fiscal_centro',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_anio_fiscal_centro]
      PRINT '<<< budget_rel_anio_fiscal_centro en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_rel_anio_fiscal_centro
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_anio_fiscal_centro](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_anio_fiscal][int] NOT NULL,
	[id_centro_costo][int] NOT NULL,
	[tipo][varchar](20) NOT NULL,
	[estatus][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_budget_rel_anio_fiscal_centro] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_rel_anio_fiscal_centro]
 add constraint FK_budget_rel_anio_fiscal_centro_id_anio_fiscal
  foreign key (id_anio_fiscal)
  references budget_anio_fiscal(id);

  -- restriccion de clave foranea
  alter table [budget_rel_anio_fiscal_centro]
 add constraint FK_budget_rel_anio_fiscal_centro_id_centro_costo
  foreign key (id_centro_costo)
  references budget_centro_costo(id);

  -- restricion check
ALTER TABLE [budget_rel_anio_fiscal_centro] ADD CONSTRAINT CK_budget_anio_fiscal_tipo CHECK ([tipo] IN 
('ACTUAL','FORECAST','BUDGET')
)

 	  
IF object_id(N'budget_rel_anio_fiscal_centro',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_anio_fiscal_centro en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_anio_fiscal_centro  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
