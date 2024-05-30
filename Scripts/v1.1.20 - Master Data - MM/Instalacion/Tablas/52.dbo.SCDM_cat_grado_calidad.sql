--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_grado_calidad',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_grado_calidad]
      PRINT '<<< SCDM_cat_grado_calidad en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_grado_calidad
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_grado_calidad](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave] [varchar](4) NOT NULL,	
	[grado_calidad] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_grado_calidad_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_grado_calidad] ON 

GO
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (1, '1',N'CQ', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (2, '10',N'HFT-340', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (3, '100',N'XAR 650 green', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (4, '101',N'XAR 550', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (5, '102',N'TP-347', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (6, '103',N'Secure 600', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (7, '104',N'S355J2G3', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (8, '105',N'S470CS', 1)
INSERT [dbo].[SCDM_cat_grado_calidad] ([id],[clave], [grado_calidad], [activo]) VALUES (9, '106',N'035SK', 1)



SET IDENTITY_INSERT [dbo].[SCDM_cat_grado_calidad] OFF
GO


	  
IF object_id(N'SCDM_cat_grado_calidad',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_grado_calidad en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_grado_calidad  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
