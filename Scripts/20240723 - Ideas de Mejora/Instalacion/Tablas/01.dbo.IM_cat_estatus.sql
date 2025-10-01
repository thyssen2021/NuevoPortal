GO
IF object_id(N'IM_cat_estatus',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_cat_estatus]
      PRINT '<<< IM_cat_estatus en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_estatus
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE IM_cat_estatus(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ideaMejoraEstatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


--Inserta valores
SET IDENTITY_INSERT IM_cat_estatus ON 
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (1, 1, N'Recibida')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (2, 1, N'Falta informacion / hay dudas')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (3, 1, N'No aceptada / no cumple con los lineamientos')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (4, 1, N'Rechazada por comite')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (6, 1, N'En proceso de implementacion')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (7, 1, N'Finalizada sin implementacion')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (8, 1, N'Implementada')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (9, 1, N'Creada')
GO
INSERT IM_cat_estatus ([id], [activo], [descripcion]) VALUES (10, 1, N'Aceptada por comite')
GO
SET IDENTITY_INSERT IM_cat_estatus OFF


IF object_id(N'IM_cat_estatus',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_cat_estatus en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_cat_estatus  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO

