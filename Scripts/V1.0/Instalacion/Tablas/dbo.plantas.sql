GO
IF object_id(N'plantas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[plantas]
      PRINT '<<< plantas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los plantas más relaciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/09/27
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [plantas](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NOT NULL,
	[activo] [bit] NOT NULL,
	[codigoSap] [varchar](4) NOT NULL,
 CONSTRAINT [PK_Planta] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [plantas] ADD  CONSTRAINT [DF_plantas_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [plantas] ON 

INSERT [plantas] ([clave], [descripcion], [activo], [codigoSap]) VALUES (1, N'PUEBLA', 1, N'5190')
INSERT [plantas] ([clave], [descripcion], [activo], [codigoSap]) VALUES (2, N'SILAO', 1, N'5390')
INSERT [plantas] ([clave], [descripcion], [activo], [codigoSap]) VALUES (3, N'SALTILLO', 1, N'5490')
INSERT [plantas] ([clave], [descripcion], [activo], [codigoSap]) VALUES (4, N'C&B', 1, N'5490')
SET IDENTITY_INSERT [plantas] OFF
GO




 	  
IF object_id(N'plantas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< plantas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla plantas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
