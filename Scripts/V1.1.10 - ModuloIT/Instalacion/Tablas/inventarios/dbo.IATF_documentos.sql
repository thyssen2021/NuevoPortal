USE Portal_2_0
GO
IF object_id(N'IATF_documentos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IATF_documentos]
      PRINT '<<< IATF_documentos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IATF_documentos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IATF_documentos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_planta] [int] NULL, --null aplica para todas	
	[clave] [varchar](10) NOT NULL,
	[nombre_documento] [varchar](120) NOT NULL,	
	[proceso] [varchar](50) NOT NULL,	
	[activo] [bit] NOT NULL,	
 CONSTRAINT [PK_IATF_documentos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IATF_documentos]
 add constraint FK_IATF_documentos_id_planta
  foreign key (id_planta)
  references plantas(clave);

  -- restriccion unique
 alter table [IATF_documentos]
  add constraint UQ_IATF_documentos
  unique (id_planta,clave);
   	  
--inserta documento para pruebas

INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (1,'ITF007', N'Responsiva de Equipo', N'RESPONSIVA_LAPTOP',1);
INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (2,'ITF007', N'Responsiva de Equipo',N'RESPONSIVA_LAPTOP',1);
-----------------------------------------------------------------------------------
INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (1,'ITF008', N'Responsiva de Celular', N'RESPONSIVA_CELULAR',1);
INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (2,'ITF008', N'Responsiva de Celular', N'RESPONSIVA_CELULAR',1);

INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (1,'ITF009', N'Responsiva de Accesorios', N'RESPONSIVA_ACCESORIOS',1);
INSERT INTO IATF_documentos (id_planta,clave, nombre_documento, proceso, activo) VALUES (2,'ITF009', N'Responsiva de Accesorios', N'RESPONSIVA_ACCESORIOS',1);

IF object_id(N'IATF_documentos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IATF_documentos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IATF_documentos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
