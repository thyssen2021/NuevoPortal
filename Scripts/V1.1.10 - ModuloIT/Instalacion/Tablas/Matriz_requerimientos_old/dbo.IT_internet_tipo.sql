USE Portal_2_0
GO
IF object_id(N'IT_internet_tipo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_internet_tipo]
      PRINT '<<< IT_internet_tipo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_internet_tipo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_internet_tipo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_IT_internet_tipo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_internet_tipo] ADD  CONSTRAINT [DF_IT_internet_tipo_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [IT_internet_tipo] ON 

INSERT [IT_internet_tipo] ([id], [descripcion], [activo]) VALUES (1, N'Nulo', 1)
INSERT [IT_internet_tipo] ([id], [descripcion], [activo]) VALUES (2, N'Bajo (Intranet)', 1)
INSERT [IT_internet_tipo] ([id], [descripcion], [activo]) VALUES (3, N'Medio (Accesos restringidos)', 1)
INSERT [IT_internet_tipo] ([id], [descripcion], [activo]) VALUES (4, N'Alto (Direccion)', 1)

SET IDENTITY_INSERT [IT_internet_tipo] OFF
GO




 	  
IF object_id(N'IT_internet_tipo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_internet_tipo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_internet_tipo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
