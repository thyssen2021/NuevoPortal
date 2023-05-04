--USE Portal_2_0
GO
IF object_id(N'RM_cambio_estatus',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_cambio_estatus]
      PRINT '<<< RM_cambio_estatus en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_cambio_estatus
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_cambio_estatus](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[capturaFecha] [datetime] NOT NULL,
	[id_empleado] int NULL,		--FK
	[remisionCabeceraClave] [int] NOT NULL,  --FK
	[catalogoEstatusClave] [int] NOT NULL,		--FK
	[nombre_usuario_old][varchar](50)  NULL,
	[texto] [varchar](1200) NOT NULL,	
 CONSTRAINT [PK_RemisionEstatus] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[RM_cambio_estatus]
 add constraint FK_RM_cambio_estatus_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [dbo].[RM_cambio_estatus]
 add constraint FK_RM_cambio_remisionCabeceraClaveo
  foreign key (remisionCabeceraClave)
  references RM_cabecera(clave);
    
  -- restriccion de clave foranea
alter table [dbo].[RM_cambio_estatus]
 add constraint FK_RM_cambio_catalogoEstatusClave
  foreign key (catalogoEstatusClave)
  references RM_estatus(clave);

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RM_cambio_estatus] ON 

--Pendiente la inserccion de datos hasta cambiar clave usario por id_empleado

SET IDENTITY_INSERT [dbo].[RM_cambio_estatus] OFF
	  
IF object_id(N'RM_cambio_estatus',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_cambio_estatus en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_cambio_estatus  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
