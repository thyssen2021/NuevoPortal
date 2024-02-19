--USE Portal_2_0
GO
IF object_id(N'budget_tipo_cambio',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_tipo_cambio]
      PRINT '<<< budget_tipo_cambio en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_tipo_cambio
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/11/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [budget_tipo_cambio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipo] [varchar](10) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_budget_tipo_cambio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [budget_tipo_cambio] ADD  CONSTRAINT [DF_budget_tipo_cambio_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [budget_tipo_cambio] ON 

INSERT [budget_tipo_cambio] ([id], [tipo], [activo]) VALUES (1, N'USD/MXN',1)
INSERT [budget_tipo_cambio] ([id], [tipo], [activo]) VALUES (2, N'EUR/USD',1)

SET IDENTITY_INSERT [budget_tipo_cambio] OFF
GO




 	  
IF object_id(N'budget_tipo_cambio',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_tipo_cambio en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_tipo_cambio  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
