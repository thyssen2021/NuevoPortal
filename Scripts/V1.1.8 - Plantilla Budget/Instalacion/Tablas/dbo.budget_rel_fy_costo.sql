USE Portal_2_0
GO
IF object_id(N'budget_rel_fy_centro',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_rel_fy_centro]
      PRINT '<<< budget_rel_fy_centro en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_rel_fy_centro
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_rel_fy_centro](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_anio_fiscal][int] NOT NULL,
	[id_centro_costo][int] NOT NULL,
	[estatus][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_budget_rel_fy_centro] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_rel_fy_centro]
 add constraint FK_budget_rel_fy_centro_id_anio_fiscal
  foreign key (id_anio_fiscal)
  references budget_anio_fiscal(id);

  -- restriccion de clave foranea
  alter table [budget_rel_fy_centro]
 add constraint FK_budget_rel_fy_centro_id_centro_costo
  foreign key (id_centro_costo)
  references budget_centro_costo(id);

  -- restriccion unique
 alter table [budget_rel_fy_centro]
  add constraint UQ_bbudget_rel_fy_centro_anio_centro
  unique (id_anio_fiscal,id_centro_costo);


 	  
IF object_id(N'budget_rel_fy_centro',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_rel_fy_centro en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_rel_fy_centro  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
