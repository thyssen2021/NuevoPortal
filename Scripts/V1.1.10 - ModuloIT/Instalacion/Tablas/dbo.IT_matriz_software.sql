USE Portal_2_0
GO
IF object_id(N'IT_matriz_software',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_matriz_software]
      PRINT '<<< IT_matriz_software en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_matriz_software
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_matriz_software](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_matriz_requerimientos] [int] NOT NULL,
	[id_it_software] [int] NOT NULL,
	[descripcion] [varchar](50) NULL
 CONSTRAINT [PK_IT_matriz_software] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [IT_matriz_software]
 add constraint FK_IT_matriz_software_id_it_software
  foreign key (id_it_software)
  references IT_software_tipo(id);

  -- restriccion de clave foranea
alter table [IT_matriz_software]
 add constraint FK_IT_matriz_software_id_matriz_requerimientos
  foreign key (id_matriz_requerimientos)
  references IT_matriz_requerimientos(id);

-- restriccion unique
 alter table [IT_matriz_software]
  add constraint UQ_IT_matriz_software
  unique (id_matriz_requerimientos,id_it_software);
GO

	  
IF object_id(N'IT_matriz_software',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_matriz_software en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_matriz_software  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
