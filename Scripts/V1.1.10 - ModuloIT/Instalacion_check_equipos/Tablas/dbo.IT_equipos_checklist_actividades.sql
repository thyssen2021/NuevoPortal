USE Portal_2_0
GO
IF object_id(N'IT_equipos_checklist_actividades',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_equipos_checklist_actividades]
      PRINT '<<< IT_equipos_checklist_actividades en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_equipos_checklist_actividades
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_equipos_checklist_actividades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_categoria_ck] [int] NOT NULL, 
	[descripcion] [varchar](120) NOT NULL,
	[activo][bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_IT_equipos_checklist_actividades] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



  -- restriccion de clave foranea
alter table [IT_equipos_checklist_actividades]
 add constraint FK_equipos_checklist_id_categoria_ck
  foreign key (id_categoria_ck)
  references IT_equipos_checklist_categorias(id);


--inserta documento para pruebas
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Actualizar OS (Windows Update)',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Actualizar Antivirus McAfee y mover a grupo',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Agregar a dominio y mover a carpeta AD',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Agregar conexiones SAP (PROD, QA)',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Configurar correo Outlook',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Instalación de Webex',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Configuración VNC',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Agregar aplicaciones no default (OOE, TRESS, COFIDI, MP9)',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Revisión actualizaciones WSUS',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (1,N'Agregar MAC en filtro WIFI y conectar a red',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (2,N'Agregar a Inventario',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (2,N'Asignar a usuario',1)
INSERT INTO [IT_equipos_checklist_actividades] (id_categoria_ck,descripcion, activo) VALUES (2,N'Imprimir, firmar y subir responsivas',1)

   	  
IF object_id(N'IT_equipos_checklist_actividades',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_equipos_checklist_actividades en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_equipos_checklist_actividades  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
