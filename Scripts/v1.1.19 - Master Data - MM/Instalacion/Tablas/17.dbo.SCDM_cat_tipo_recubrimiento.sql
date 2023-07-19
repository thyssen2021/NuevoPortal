--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_tipo_recubrimiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_tipo_recubrimiento]
      PRINT '<<< SCDM_cat_tipo_recubrimiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_tipo_recubrimiento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_tipo_recubrimiento](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[codigo][varchar](4) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_tipo_recubrimiento_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ON 

GO

INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (1, 1, N'Z_AL',  N'Aluminizado') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (2, 1, N'Z_CB', N'Copper & Brass') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (3, 1, N'Z_CM', N'Customer Material') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (4, 1, N'Z_CR', N'Cold Rolled') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (5, 1, N'Z_EG', N'Electro-Galvanizado') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (6, 1, N'Z_GN', N'Galvanizado') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (7, 1, N'Z_GO', N'Grain Oriented') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (8, 1, N'Z_HD', N'Hot Dipped') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (9, 1, N'Z_HP', N'Hot Rolled P&O') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (10, 1, N'Z_HR', N'Hot Rolled Back') 
INSERT [dbo].[SCDM_cat_tipo_recubrimiento] ([id], [activo], [codigo], [descripcion]) VALUES (11, 1, N'Z_SM', N'Servicio de Maquila ') 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_tipo_recubrimiento] OFF
	  
IF object_id(N'SCDM_cat_tipo_recubrimiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_tipo_recubrimiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_tipo_recubrimiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
