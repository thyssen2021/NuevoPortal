USE Portal_2_0
GO
IF object_id(N'IT_asignacion_hardware',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_asignacion_hardware]
      PRINT '<<< IT_asignacion_hardware en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_asignacion_hardware
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_asignacion_hardware](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_it_inventory_item] [int] NOT NULL,
	[id_iatf_version] [int] NOT NULL,
	[id_empleado] [int] NOT NULL,
	[id_sistemas] [int] NOT NULL,
	[id_biblioteca_digital] [int] NULL,
	[fecha_asignacion] [datetime] NOT NULL,
	[fecha_desasignacion] [datetime] NULL,
	[es_asignacion_actual] [bit] NOT NULL,
	[id_responsable_principal][int] NULL,
 CONSTRAINT [PK_IT_asignacion_hardware] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] ADD  CONSTRAINT [DF_IT_asignacion_hardware_actual]  DEFAULT ((1)) FOR [es_asignacion_actual]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_biblioteca_digital] FOREIGN KEY([id_biblioteca_digital])
REFERENCES [dbo].[biblioteca_digital] ([Id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] CHECK CONSTRAINT [FK_id_biblioteca_digital]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_empleado] FOREIGN KEY([id_empleado])
REFERENCES [dbo].[empleados] ([id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] CHECK CONSTRAINT [FK_id_empleado]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_iatf_version] FOREIGN KEY([id_iatf_version])
REFERENCES [dbo].[IATF_revisiones] ([id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] CHECK CONSTRAINT [FK_id_iatf_version]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_it_inventory_item] FOREIGN KEY([id_it_inventory_item])
REFERENCES [dbo].[IT_inventory_items] ([id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] CHECK CONSTRAINT [FK_id_it_inventory_item]
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_sistemas] FOREIGN KEY([id_sistemas])
REFERENCES [dbo].[empleados] ([id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware]  WITH CHECK ADD  CONSTRAINT [FK_id_responsable_principal] FOREIGN KEY([id_responsable_principal])
REFERENCES [dbo].[empleados] ([id])
GO

ALTER TABLE [dbo].[IT_asignacion_hardware] CHECK CONSTRAINT [FK_id_sistemas]
GO


IF object_id(N'IT_asignacion_hardware',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_asignacion_hardware en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_asignacion_hardware  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END