USE Portal_2_0
GO
IF object_id(N'IT_equipos_checklist_categorias',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_equipos_checklist_categorias]
      PRINT '<<< IT_equipos_checklist_categorias en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_equipos_checklist_categorias
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_equipos_checklist_categorias](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,	
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IT_equipos_checklist_categorias] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

   	  
--inserta datos para pruebas
INSERT INTO IT_equipos_checklist_categorias (descripcion, activo) VALUES (N'Sistema Operativo',1)
INSERT INTO IT_equipos_checklist_categorias (descripcion, activo) VALUES (N'Portal tkMM', 1)

IF object_id(N'IT_equipos_checklist_categorias',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_equipos_checklist_categorias en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_equipos_checklist_categorias  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
