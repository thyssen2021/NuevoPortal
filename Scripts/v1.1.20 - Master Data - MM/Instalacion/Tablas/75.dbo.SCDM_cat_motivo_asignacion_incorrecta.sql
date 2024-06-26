--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_motivo_asignacion_incorrecta',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_motivo_asignacion_incorrecta]
      PRINT '<<< SCDM_cat_motivo_asignacion_incorrecta en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_motivo_asignacion_incorrecta
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_motivo_asignacion_incorrecta](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](80) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_motivo_asignacion_incorrecta_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_asignacion_incorrecta] ON 

GO

INSERT [dbo].[SCDM_cat_motivo_asignacion_incorrecta] ([id], [activo], [descripcion]) VALUES (1, 1,  N'Sin actividades por parte del departamento') 
INSERT [dbo].[SCDM_cat_motivo_asignacion_incorrecta] ([id], [activo], [descripcion]) VALUES (2, 1,  N'Asignar a otro compañero del departamento') 
INSERT [dbo].[SCDM_cat_motivo_asignacion_incorrecta] ([id], [activo], [descripcion]) VALUES (3, 1,  N'Otros') 
SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_asignacion_incorrecta] OFF
GO


--ALTER TABLE solicitud
ALTER TABLE dbo.SCDM_solicitud_asignaciones 
ADD id_motivo_asignacion_incorrecta INT NULL;

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones ]
 add constraint FK_SCDM_solicitud_id_motivo_asignacion_incorrecta
  foreign key (id_motivo_asignacion_incorrecta)
  references SCDM_cat_motivo_asignacion_incorrecta(id);

ALTER TABLE dbo.SCDM_solicitud_asignaciones 
ADD comentario_asignacion_incorrecta varchar(255) NULL;

  
	  
IF object_id(N'SCDM_cat_motivo_asignacion_incorrecta',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_motivo_asignacion_incorrecta en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_motivo_asignacion_incorrecta  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
