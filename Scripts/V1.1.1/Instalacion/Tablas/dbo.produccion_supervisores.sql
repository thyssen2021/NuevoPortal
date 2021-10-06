use[Portal_2_0]
GO
IF object_id(N'produccion_supervisores',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[produccion_supervisores]
      PRINT '<<< produccion_supervisores en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los operadores de producción
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/10/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [dbo].[produccion_supervisores](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_empleado][int] NOT NULL,
	[clave_planta] [int] NOT NULL,	
	[activo] [bit] NULL
 CONSTRAINT [PK_produccion_supervisores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restriccion de clave foranea
alter table [produccion_supervisores]
 add constraint FK_produccion_supervisores_id_empleado
  foreign key (id_empleado)
  references empleados(id);

  -- restriccion de clave foranea
alter table [produccion_supervisores]
 add constraint FK_produccion_supervisores_clave_planta
  foreign key (clave_planta)
  references plantas(clave);

  -- restricción default
ALTER TABLE [produccion_supervisores] ADD  CONSTRAINT [DF_produccion_supervisores_activo]  DEFAULT (1) FOR [activo]
GO

--inserta los registros de los turnos
--puebla
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='20048'),1,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='20023'),1,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='20183'),1,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='20336'),1,1);

--silao
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='80164'),2,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='80227'),2,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='80308'),2,1);
INSERT INTO [dbo].[produccion_supervisores]([id_empleado],[clave_planta],[activo])VALUES((SELECT id FROM empleados where numeroEmpleado='80349'),2,1);

IF object_id(N'produccion_supervisores',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< produccion_supervisores en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla produccion_supervisores  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
