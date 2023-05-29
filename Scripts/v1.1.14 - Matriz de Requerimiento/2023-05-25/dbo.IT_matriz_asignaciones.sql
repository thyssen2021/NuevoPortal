USE Portal_2_0
GO
IF object_id(N'IT_matriz_asignaciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_matriz_asignaciones]
      PRINT '<<< IT_matriz_asignaciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_matriz_asignaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_matriz_asignaciones](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_matriz_requerimientos] [int] NOT NULL,
	[id_sistemas][int] NOT NULL,
	[fecha_asignacion] [datetime] NOT NULL,
	[comentario] [varchar](150) NOT NULL,
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IT_matriz_asignaciones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [IT_matriz_asignaciones]
 add constraint FK_IT_matriz_asignaciones_id_matriz_requerimientos
  foreign key (id_matriz_requerimientos)
  references IT_matriz_requerimientos(id);

  alter table [IT_matriz_asignaciones]
 add constraint FK_IT_matriz_asignaciones_id_sistema
  foreign key (id_sistemas)
  references empleados(id);

	  
IF object_id(N'IT_matriz_asignaciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_matriz_asignaciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_matriz_asignaciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
