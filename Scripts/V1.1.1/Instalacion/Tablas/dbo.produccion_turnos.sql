use[Portal_2_0]
GO
IF object_id(N'produccion_turnos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_turnos]
      PRINT '<<< produccion_turnos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los produccion_turnos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_turnos](
	--nuevos campos
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave_planta] [int]NOT NULL,
	[valor] [int] NOT NULL,
	[descripcion] [nvarchar](20) NOT NULL,
	[hora_inicio][time] NOT NULL,
	[hora_fin][time] NOT NULL,
	[activo] [bit] NULL
 CONSTRAINT [PK_produccion_turnos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_turnos]
 add constraint FK_produccion_turnos_planta_clave
  foreign key (clave_planta)
  references plantas(clave);

  -- restricción default
ALTER TABLE [produccion_turnos] ADD  CONSTRAINT [DF_produccion_turnos_activo]  DEFAULT (1) FOR [activo]
GO

--inserta los registros de los turnos
--puebla
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(1,1,'Primero',CAST ('7:00:00.0000000' AS TIME) ,CAST ('14:59:59.0000000' AS TIME),1);
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(1,2,'Segundo',CAST ('15:00:00.0000000' AS TIME) ,CAST ('22:29:59.0000000' AS TIME),1);
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(1,3,'Tercero',CAST ('22:30:00.0000000' AS TIME) ,CAST ('6:59:59.0000000' AS TIME),1);

--silao
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(2,1,'Primero',CAST ('7:00:00.0000000' AS TIME) ,CAST ('14:59:59.0000000' AS TIME),1);
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(2,2,'Segundo',CAST ('15:00:00.0000000' AS TIME) ,CAST ('22:29:59.0000000' AS TIME),1);
INSERT INTO [dbo].[produccion_turnos]([clave_planta],[valor],[descripcion],[hora_inicio],[hora_fin],[activo])VALUES(2,3,'Tercero',CAST ('22:30:00.0000000' AS TIME) ,CAST ('6:59:59.0000000' AS TIME),1);

IF object_id(N'produccion_turnos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_turnos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_turnos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
