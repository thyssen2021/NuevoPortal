--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_storage_location',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_storage_location]
      PRINT '<<< SCDM_cat_storage_location en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_storage_location
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2024/01/03
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/



CREATE TABLE [dbo].[SCDM_cat_storage_location](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_planta][int] NOT NULL,
	[clave] [varchar](4) NOT NULL,
	[sloc_name] [varchar](20) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_storage_location_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_storage_location] ON 

GO

INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (1, 1, 'PU01',  N'Almacén Sur', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (2, 1, 'PU02',  N'Almacén Norte', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (3, 1, 'PU03',  N'Puertos', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (4, 2, 'SI01',  N'Almacén Silao', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (5, 2, 'SI03',  N'Puertos Silao', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (6, 3, 'SA01',  N'Almacén Saltillo', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (7, 3, 'SA03',  N'Puertos Saltillo', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (8, 4, 'CB01',  N'Almacén C&B', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (9, 4, 'CB03',  N'Puertos C&B', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (10, 5, 'SL01',  N'Almacén SLP', 1) 
INSERT [dbo].[SCDM_cat_storage_location] ([id], [id_planta], [clave],[sloc_name], [activo]) VALUES (11, 5, 'SL03',  N'Puertos SLP', 1) 


GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_storage_location] OFF
	  
IF object_id(N'SCDM_cat_storage_location',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_storage_location en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_storage_location  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
