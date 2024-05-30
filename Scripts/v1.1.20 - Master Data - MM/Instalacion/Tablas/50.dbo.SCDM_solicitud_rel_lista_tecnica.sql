--USE Portal_2_0
GO
IF object_id(N'SCDM_solicitud_rel_lista_tecnica',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_solicitud_rel_lista_tecnica]
      PRINT '<<< SCDM_solicitud_rel_lista_tecnica en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_solicitud_rel_lista_tecnica
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/10/26
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_solicitud_rel_lista_tecnica](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_solicitud] [int] NOT NULL,
	[resultado][varchar](20) NULL,	
	[componente][varchar](20) NULL, 
	[sobrante][float] NULL,   --solo platinas
	[cantidad_platinas][int] NULL,
	[cantidad_cintas][int] NULL,
	[fecha_validez_reaplicacion][date] NULL,
 CONSTRAINT [PK_SCDM_solicitud_rel_lista_tecnica_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_solicitud_rel_lista_tecnica]
 add constraint FK_SCDM_solicitud_rel_lista_tecnica_id_solicitud
  foreign key (id_solicitud)
  references SCDM_solicitud(id);


	  
IF object_id(N'SCDM_solicitud_rel_lista_tecnica',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_solicitud_rel_lista_tecnica en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_solicitud_rel_lista_tecnica  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
