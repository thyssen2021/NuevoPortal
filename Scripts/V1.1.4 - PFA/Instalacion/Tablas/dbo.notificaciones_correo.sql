USE Portal_2_0
GO
IF object_id(N'notificaciones_correo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[notificaciones_correo]
      PRINT '<<< notificaciones_correo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los notificaciones_correo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [notificaciones_correo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NULL,
	[descripcion] [varchar](50) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_notificaciones_correo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [notificaciones_correo]
 add constraint FK_notificaciones_correo_empleado
  foreign key (id_empleado)
  references empleados(id);

-- restricción default
ALTER TABLE [notificaciones_correo] ADD  CONSTRAINT [DF_notificaciones_correo_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [notificaciones_correo] ON 
--Sólo para desarrollo
INSERT [notificaciones_correo] ([id],[id_empleado], [descripcion], [activo]) VALUES (1,438,N'PFA_MANAGER', 1)
INSERT [notificaciones_correo] ([id],[id_empleado], [descripcion], [activo]) VALUES (2,438,N'PFA_FINALIZADO', 1)

SET IDENTITY_INSERT [notificaciones_correo] OFF
GO
 	  
IF object_id(N'notificaciones_correo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< notificaciones_correo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla notificaciones_correo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
