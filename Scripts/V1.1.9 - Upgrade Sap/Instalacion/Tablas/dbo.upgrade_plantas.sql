USE Portal_2_0
GO
IF object_id(N'upgrade_plantas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[upgrade_plantas]
      PRINT '<<< upgrade_plantas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los upgrade_plantas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [upgrade_plantas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_upgrade_plantas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [upgrade_plantas] ADD  CONSTRAINT [DF_upgrade_plantas_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [upgrade_plantas] ON 

INSERT [upgrade_plantas] ([id], [descripcion], [activo]) VALUES (1, N'Puebla',1)
INSERT [upgrade_plantas] ([id], [descripcion],[activo]) VALUES (2, N'Silao',1)
INSERT [upgrade_plantas] ([id], [descripcion],[activo]) VALUES (3, N'Saltillo',1)

SET IDENTITY_INSERT [upgrade_plantas] OFF
GO




 	  
IF object_id(N'upgrade_plantas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< upgrade_plantas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla upgrade_plantas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
