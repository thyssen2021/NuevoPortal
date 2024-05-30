--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_motivo_creacion',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_motivo_creacion]
      PRINT '<<< SCDM_cat_motivo_creacion en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_motivo_creacion
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_motivo_creacion](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](80) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_motivo_creacion_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_creacion] ON 

GO

INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (1, 1,  N'Cambio de Calidad') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (2, 1,  N'Cambio de Número de Cliente') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (3, 1,  N'Cambio de Número de Molino') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (4, 1,  N'Cambio de Número MSA') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (5, 1,  N'Cambio en Ancho') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (6, 1,  N'Cambio en Avance') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (7, 1,  N'Cambio en Espesor') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (8, 1,  N'Cambio en Número de parte de cliente') 
INSERT [dbo].[SCDM_cat_motivo_creacion] ([id], [activo], [descripcion]) VALUES (9, 1,  N'Material de Prueba') 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_creacion] OFF
	  
IF object_id(N'SCDM_cat_motivo_creacion',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_motivo_creacion en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_motivo_creacion  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
