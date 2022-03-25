USE Portal_2_0
GO
IF object_id(N'budget_plantas',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_plantas]
      PRINT '<<< budget_plantas en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_plantas
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/03/25
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [budget_plantas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_budget_plantas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [budget_plantas] ADD  CONSTRAINT [DF_budget_plantas_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [budget_plantas] ON 

INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (1, N'Shared Services',1)
INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (2, N'Puebla',1)
INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (3, N'Silao',1)
INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (4, N'Saltillo',1)
INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (5, N'SLP',1)
INSERT [budget_plantas] ([id], [descripcion], [activo]) VALUES (6, N'C&B',1)

SET IDENTITY_INSERT [budget_plantas] OFF
GO




 	  
IF object_id(N'budget_plantas',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_plantas en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_plantas  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
