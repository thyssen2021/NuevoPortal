--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_moneda',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_moneda]
      PRINT '<<< SCDM_cat_moneda en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_moneda
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_moneda](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave_iso][varchar](3) NOT NULL,
	[descripcion] [varchar](15) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_moneda_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_moneda] ON 

GO
INSERT [dbo].[SCDM_cat_moneda] ([id],[clave_iso], [descripcion], [activo]) VALUES (1, N'MXN', N'Pesos mexicanos', 1)
INSERT [dbo].[SCDM_cat_moneda] ([id],[clave_iso], [descripcion], [activo]) VALUES (2, N'USD', N'Dólares', 1)
INSERT [dbo].[SCDM_cat_moneda] ([id],[clave_iso], [descripcion], [activo]) VALUES (3, N'EUR', N'Euros', 1)

SET IDENTITY_INSERT [dbo].[SCDM_cat_moneda] OFF
GO


	  
IF object_id(N'SCDM_cat_moneda',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_moneda en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_moneda  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
