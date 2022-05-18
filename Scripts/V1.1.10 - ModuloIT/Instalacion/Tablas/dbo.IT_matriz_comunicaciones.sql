USE Portal_2_0
GO
IF object_id(N'IT_matriz_comunicaciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_matriz_comunicaciones]
      PRINT '<<< IT_matriz_comunicaciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_matriz_comunicaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_matriz_comunicaciones](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_matriz_requerimientos] [int] NOT NULL,
	[id_it_comunicaciones] [int] NOT NULL,
	[descripcion] [varchar](90) NULL,
	[comentario] [varchar](90) NULL,
	[completado][bit] NULL,
 CONSTRAINT [PK_IT_matriz_comunicaciones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [IT_matriz_comunicaciones]
 add constraint FK_IT_matriz_comunicaciones_id_it_comunicaciones
  foreign key (id_it_comunicaciones)
  references IT_comunicaciones_tipo(id);

  -- restriccion de clave foranea
alter table [IT_matriz_comunicaciones]
 add constraint FK_IT_matriz_comunicaciones_id_matriz_requerimientos
  foreign key (id_matriz_requerimientos)
  references IT_matriz_requerimientos(id);

-- restriccion unique
 alter table [IT_matriz_comunicaciones]
  add constraint UQ_IT_matriz_comunicaciones
  unique (id_matriz_requerimientos,id_it_comunicaciones);
GO

	  
IF object_id(N'IT_matriz_comunicaciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_matriz_comunicaciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_matriz_comunicaciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
