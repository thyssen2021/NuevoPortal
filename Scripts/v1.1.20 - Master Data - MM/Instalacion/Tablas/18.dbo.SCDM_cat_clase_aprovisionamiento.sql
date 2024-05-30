--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_clase_aprovisionamiento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_clase_aprovisionamiento]
      PRINT '<<< SCDM_cat_clase_aprovisionamiento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_clase_aprovisionamiento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_clase_aprovisionamiento](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_clase_aprovisionamiento_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_clase_aprovisionamiento] ON 

GO

INSERT [dbo].[SCDM_cat_clase_aprovisionamiento] ([id], [activo], [descripcion]) VALUES (1, 1,  N'Acopio Externo') 
INSERT [dbo].[SCDM_cat_clase_aprovisionamiento] ([id], [activo], [descripcion]) VALUES (2, 1,  N'Fabricación Propia') 

GO

SET IDENTITY_INSERT [dbo].[SCDM_cat_clase_aprovisionamiento] OFF
	  
IF object_id(N'SCDM_cat_clase_aprovisionamiento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_clase_aprovisionamiento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_clase_aprovisionamiento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
