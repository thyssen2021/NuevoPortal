USE Portal_2_0
GO
IF object_id(N'IT_mantenimientos_checklist_categorias',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_mantenimientos_checklist_categorias]
      PRINT '<<< IT_mantenimientos_checklist_categorias en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_mantenimientos_checklist_categorias
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/02
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_mantenimientos_checklist_categorias](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_mantenimientos_checklist_categorias] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_mantenimientos_checklist_categorias] ADD  CONSTRAINT [DF_IT_mantenimientos_checklist_categorias_activo]  DEFAULT (1) FOR [activo]
GO
		

SET IDENTITY_INSERT [IT_mantenimientos_checklist_categorias] ON 

INSERT [IT_mantenimientos_checklist_categorias] ([id], [descripcion], [activo]) VALUES (1, N'Sistema Operativo',1)
INSERT [IT_mantenimientos_checklist_categorias] ([id], [descripcion], [activo]) VALUES (2, N'Navegadores',1)
INSERT [IT_mantenimientos_checklist_categorias] ([id], [descripcion], [activo]) VALUES (3, N'Hardware',1)

SET IDENTITY_INSERT [IT_mantenimientos_checklist_categorias] OFF


GO


 	  
IF object_id(N'IT_mantenimientos_checklist_categorias',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_mantenimientos_checklist_categorias en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_mantenimientos_checklist_categorias  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
