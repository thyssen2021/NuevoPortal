--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_motivo_rechazo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_motivo_rechazo]
      PRINT '<<< SCDM_cat_motivo_rechazo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_motivo_rechazo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_motivo_rechazo](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[descripcion] [varchar](80) NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_motivo_rechazo_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_rechazo] ON 

GO

INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (1, 1,  N'Descripcion incorrecta') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (2, 1,  N'Dimensiones incorrectas') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (3, 1,  N'Dimensiones incorrectas/clasificacion') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (4, 1,  N'El material se creo con costo estandar') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (5, 1,  N'El material se creo con costo variable') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (6, 1,  N'El material se creo con costo variable y estandar') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (7, 1,  N'Error en categoria de valoracion') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (8, 1,  N'Error en relevancia de costo dentro de BOM') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (9, 1,  N'Falta autorizacion del cliente (Desviaciones)') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (10, 1,  N'Falta dato en el formato de alta') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (11, 1,  N'Falta formato de orden de compra') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (12, 1,  N'No tiene BOM') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (13, 1,  N'No tiene RUTA') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (14, 1,  N'No tiene Orden de Compra el material') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (15, 1,  N'Numero de parte incorrecto') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (16, 1,  N'Peso incorrecto en BOM') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (17, 1,  N'Peso incorrecto en MM') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (18, 1,  N'Sin factibilidad tecnica') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (19, 1,  N'Sin informacion tecnica') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (20, 1,  N'Solicitud con numeros SAP incorrectos') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (21, 1,  N'Solicitud sin numeros SAP') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (22, 1,  N'uMB incorrecta') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (23, 1,  N'Asignacion incorrecta') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (24, 1,  N'No se hicieron los cambios solicitados') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (25, 1,  N'No se cambio en todos los campos') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (26, 1,  N'Falta informacion en la Lista Tecnica') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (27, 1,  N'Archivo dañado, no se puede abrir') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (28, 1,  N'Descripcion completa HONDA (Tipo de Mat + Grado + Peso del recubrimiento)') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (29, 1,  N'No hay informacion del proyecto/solicitud') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (30, 1,  N'Descripcion mayor a 40 caracteres') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (31, 1,  N'Textos de Ventas incorrectos') 
INSERT [dbo].[SCDM_cat_motivo_rechazo] ([id], [activo], [descripcion]) VALUES (32, 1,  N'Otros') 
SET IDENTITY_INSERT [dbo].[SCDM_cat_motivo_rechazo] OFF
GO

--ALTER TABLE solicitud
ALTER TABLE dbo.SCDM_solicitud_asignaciones 
ADD id_motivo_rechazo INT NULL ;

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones ]
 add constraint FK_SCDM_solicitud_id_motivo_rechazo
  foreign key (id_motivo_rechazo)
  references SCDM_cat_motivo_rechazo(id);

  --agrega id_cierre y id_rechazo
--ALTER TABLE solicitud
ALTER TABLE dbo.SCDM_solicitud_asignaciones 
ADD id_cierre INT NULL ;

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones ]
 add constraint FK_SCDM_solicitud_id_cierre
  foreign key (id_cierre)
  references empleados(id);

    --agrega id_cierre y id_rechazo
--ALTER TABLE solicitud
ALTER TABLE dbo.SCDM_solicitud_asignaciones 
ADD id_rechazo INT NULL ;

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_asignaciones ]
 add constraint FK_SCDM_solicitud_id_rechazo
  foreign key (id_rechazo)
  references empleados(id);
  
	  
IF object_id(N'SCDM_cat_motivo_rechazo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_motivo_rechazo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_motivo_rechazo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
