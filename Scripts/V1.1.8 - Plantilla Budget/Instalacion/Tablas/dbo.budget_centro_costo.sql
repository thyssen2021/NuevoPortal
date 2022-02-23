USE Portal_2_0
GO
IF object_id(N'budget_centro_costo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_centro_costo]
      PRINT '<<< budget_centro_costo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_centro_costo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creaci�n: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_centro_costo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_area][int] NOT NULL,
	[id_responsable][int] NOT NULL,	
	[num_centro_costo][varchar](6) NOT NULL,
 CONSTRAINT [PK_budget_centro_costo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_centro_costo]
 add constraint FK_budget_centro_costo_id_area
  foreign key (id_area)
  references Area(clave);

  -- restriccion de clave foranea
  alter table [budget_centro_costo]
 add constraint FK_budget_centro_costo_id_responsable
  foreign key (id_responsable)
  references empleados(id);

 	  
IF object_id(N'budget_centro_costo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_centro_costo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_centro_costo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
