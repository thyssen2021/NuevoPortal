--USE Portal_2_0
GO
IF object_id(N'SCDM_versiones_herramienta_excel',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_versiones_herramienta_excel]
      PRINT '<<< SCDM_versiones_herramienta_excel en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_versiones_herramienta_excel
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_versiones_herramienta_excel](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_archivo] [int]  NOT NULL,	
	[id_responsable] [int] NOT NULL,
	[version] varchar(20) Not null,
	[cambio] varchar(150) not null,
	[fecha_liberacion] [date] NOT NULL,
	
 CONSTRAINT [PK_SCDM_versiones_herramienta_excel_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_versiones_herramienta_excel]
 add constraint FK_SCDM_versiones_herramienta_excel_id_responsable
  foreign key (id_responsable)
  references empleados(id);

  -- restriccion de clave foranea
alter table [dbo].[SCDM_versiones_herramienta_excel]
 add constraint FK_SSCDM_versiones_herramienta_excel_id_archivo
  foreign key (id_archivo)
  references biblioteca_digital(id);

  -- el numero de la version debe ser unico
  alter table [SCDM_versiones_herramienta_excel]
 add constraint UQ_SCDM_versiones_herramienta_excel_version
 unique ([version]);

	  
IF object_id(N'SCDM_versiones_herramienta_excel',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_versiones_herramienta_excel en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_versiones_herramienta_excel  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
