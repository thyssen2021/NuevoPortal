USE Portal_2_0
GO
IF object_id(N'IT_matriz_requerimientos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_matriz_requerimientos]
      PRINT '<<< IT_matriz_requerimientos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_matriz_requerimientos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_matriz_requerimientos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado] [int] NOT NULL,
	[id_jefe_directo] [int] NOT NULL,
	[id_internet_tipo] [int] NOT NULL,
	[fecha_solicitud] [datetime] NOT NULL,
	[fecha_aprobacion_jefe] [datetime] NULL,
	[fecha_cierre] [datetime] NULL,
	[estatus] [varchar](20) NOT NULL,
	[comentario] [varchar](350) NULL,
	[comentario_rechazo] [varchar](350) NULL,
 CONSTRAINT [PK_IT_matriz_requerimientos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [IT_matriz_requerimientos] ADD  CONSTRAINT [DF_IT_matriz_requerimientos_fecha_solicitud]  DEFAULT (GETDATE()) FOR [fecha_solicitud]
GO

-- restriccion de clave foranea
alter table [IT_matriz_requerimientos]
 add constraint FK_IT_mIT_matriz_requerimientos_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [IT_matriz_requerimientos]
 add constraint FK_IT_mIT_matriz_requerimientos_id_jefe
  foreign key (id_jefe_directo)
  references empleados(id);


-- restricion check
ALTER TABLE [IT_matriz_requerimientos] ADD CONSTRAINT CK_it_matriz_requerimientos_Estatus CHECK ([estatus] IN 
('CREADO', 'ENVIADO_A_JEFE','ENVIADO_A_IT','RECHAZADO', 'FINALIZADO')
)
GO


 	  
IF object_id(N'IT_matriz_requerimientos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_matriz_requerimientos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_matriz_requerimientos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
