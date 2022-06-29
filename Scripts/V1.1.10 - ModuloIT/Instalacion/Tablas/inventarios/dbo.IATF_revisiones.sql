USE Portal_2_0
GO
IF object_id(N'IATF_revisiones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IATF_revisiones]
      PRINT '<<< IATF_revisiones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IATF_revisiones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IATF_revisiones](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_iatf_documento] [int] NOT NULL, 	
	[numero_revision] [int]NOT NULL,
	[responsable] [varchar](120) NOT NULL,	
	[puesto_responsable] [varchar](120) NULL,	
	[fecha_revision] [datetime] NOT NULL,	
	[descripcion] [varchar](250) NOT NULL,	
 CONSTRAINT [PK_IATF_revisiones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IATF_revisiones]
 add constraint FK_id_iatf_documento
  foreign key (id_iatf_documento)
  references IATF_documentos(id);

  -- restriccion unique
 alter table [IATF_revisiones]
  add constraint UQ_IATF_revisiones
  unique (id_iatf_documento,numero_revision);
   	  
--inserta documento para pruebas
INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable,fecha_revision,descripcion) VALUES (1,1,'IT SISTEMAS',null, '2019-12-01', 'Emisión del Formato.')
INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable, fecha_revision,descripcion) VALUES (1,2,'IT SISTEMAS',null, '2020-10-21', 'Se quita el no. De revisión de la política interna de TkMM y solo se mantiene el código ITE001')
INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable,fecha_revision,descripcion) VALUES (1,3,'Alfredo Xochitemol Cruz',null, '2022-06-22', 'Se emite documento a través de portal web con la clave ITF007-03.')

INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable,fecha_revision,descripcion) VALUES (2,1,'IT SISTEMAS',null, '2019-12-01', 'Emisión del Formato.')
INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable, fecha_revision,descripcion) VALUES (2,2,'IT SISTEMAS',null, '2020-10-21', 'Se quita el no. De revisión de la política interna de TkMM y solo se mantiene el código ITE001')
INSERT INTO IATF_revisiones (id_iatf_documento,numero_revision,responsable, puesto_responsable,fecha_revision,descripcion) VALUES (2,3,'Alfredo Xochitemol Cruz',null, '2022-06-22', 'Se emite documento a través de portal web con la clave ITF007-03.')

IF object_id(N'IATF_revisiones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IATF_revisiones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IATF_revisiones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
