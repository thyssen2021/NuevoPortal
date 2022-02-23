USE Portal_2_0
GO
IF object_id(N'budget_mapping',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_mapping]
      PRINT '<<< budget_mapping en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los budget_mapping
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_mapping](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_mapping_bridge][int] NOT NULL,
	[descripcion] [varchar](40) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_budget_mapping] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
  alter table [budget_mapping]
 add constraint FK_budget_mapping_id_mapping_bridge
  foreign key (id_mapping_bridge)
  references budget_mapping_bridge(id);

-- restricción default
ALTER TABLE [budget_mapping] ADD  CONSTRAINT [DF_budget_mapping_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [budget_mapping] ON 

--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (1, N'Supplier delay', 1)
--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (2, N'tkMM increase', 1)
--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (3, N'Customer increase', 1)

SET IDENTITY_INSERT [budget_mapping] OFF
GO
 	  
IF object_id(N'budget_mapping',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_mapping en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_mapping  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
