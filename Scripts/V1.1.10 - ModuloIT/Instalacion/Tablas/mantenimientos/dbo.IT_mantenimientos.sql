USE Portal_2_0
GO
IF object_id(N'IT_mantenimientos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_mantenimientos]
      PRINT '<<< IT_mantenimientos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_mantenimientos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_mantenimientos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_it_inventory_item] [int] NOT NULL,
	[id_empleado_responsable] [int] NULL,
	[id_empleado_sistemas] [int] NULL,
	[id_iatf_version] [int] NULL,
	[id_biblioteca_digital] [int] NULL,
	[fecha_programada] [datetime] NOT NULL,   --con las fechas se obtiene el estatus = PROXIMO, PENDIENTE FINALIZADO
	[fecha_realizacion] [datetime] NULL,
	[comentarios][varchar](300)  NULL,
 CONSTRAINT [PK_IT_mantenimientos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_mantenimientos]
 add constraint FK_IT_mantenimientos_id_it_inventory_item
  foreign key (id_it_inventory_item)
  references IT_inventory_items(id);

  -- restriccion de clave foranea
alter table [IT_mantenimientos]
 add constraint FK_IT_mantenimientos_id_empleado_responsable
  foreign key (id_empleado_responsable)
  references empleados(id);

    -- restriccion de clave foranea
alter table [IT_mantenimientos]
 add constraint FK_IT_mantenimientos_id_empleado_sistemas
  foreign key (id_empleado_sistemas)
  references empleados(id);

    -- restriccion de clave foranea
  ALTER TABLE [dbo].[IT_mantenimientos]  WITH CHECK ADD  CONSTRAINT [FK_IT_mantenimientos_id_iatf_version] FOREIGN KEY([id_iatf_version])
REFERENCES [dbo].[IATF_revisiones] ([id])

    -- restriccion de clave foranea
alter table [IT_mantenimientos]
 add constraint FK_IT_mantenimientos_id_biblioteca_digital
  foreign key (id_biblioteca_digital)
  references biblioteca_digital(id);


IF object_id(N'IT_mantenimientos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_mantenimientos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_mantenimientos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END