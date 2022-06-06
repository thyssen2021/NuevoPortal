USE Portal_2_0
GO
IF object_id(N'IT_solicitud_usuarios',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_solicitud_usuarios]
      PRINT '<<< IT_solicitud_usuarios en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos IT_solicitud_usuarios
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/30
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_solicitud_usuarios](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NULL,
	[id_planta][int] NULL,
	[fecha_solicitud][datetime] NOT NULL,
	[fecha_cierre][datetime] NULL,
	[nombre][varchar](50) NULL,
	[apellido1][varchar](50) NULL,
	[apellido2][varchar](50) NULL,
	[area][varchar](50) NULL,
	[puesto][varchar](50) NULL,
	[8ID][varchar](8) NULL,
	[numero_empleado][varchar](8) NULL,
	[correo][varchar](100) NULL,
	[comentario][varchar](250) NULL,
	[estatus][varchar](25) NULL,
 CONSTRAINT [PK_IT_solicitud_usuarios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [IT_solicitud_usuarios]
 add constraint FK_IT_solicitud_usuarios_id_empleado
  foreign key (id_empleado)
  references empleados(id);

    -- restriccion de clave foranea
  alter table [IT_solicitud_usuarios]
 add constraint FK_IT_solicitud_usuarios_id_planta
  foreign key (id_planta)
  references plantas(clave);

-- restricción default
ALTER TABLE [IT_solicitud_usuarios] ADD  CONSTRAINT [DF_IT_solicitud_usuarios_fecha_solicitud]  DEFAULT (GETDATE()) FOR [fecha_solicitud]

-- restricion check
ALTER TABLE [IT_solicitud_usuarios] ADD CONSTRAINT CK_IT_solicitud_usuarios_Estatus CHECK ([estatus] IN 
('CREADO','CERRADO')
)



 	  
IF object_id(N'IT_solicitud_usuarios',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_solicitud_usuarios en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_solicitud_usuarios  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
