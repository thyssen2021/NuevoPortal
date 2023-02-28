--USE Portal_2_0
GO
IF object_id(N'IT_epo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_epo]
      PRINT '<<< IT_epo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar las IT_epo
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/22
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[IT_epo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[system_name][varchar](50) NULL, --hostname
	[username] [varchar](200) NULL,
	[operating_system] [varchar](200) NULL,
	[is_laptop][bit] NULL,
	[group_name][varchar](80) NULL,
	[os_type][varchar](80) NULL,
	[mac_address][varchar](30) NULL,
	[numbers_of_cpus][int] NULL,
	[cpu_speed][int] NULL,
	[system_manufacturer] [varchar](80) NULL,
	[system_model] [varchar](150) NULL,
	[system_serial_number] [varchar](150) NULL,
	[total_disk_space_mb][int] NULL,
	[total_c_drive_space_mb][int] NULL,
	[last_communication][varchar](50) NULL,
	[total_physical_memory_mb][float] NULL,
	[assigment_path][varchar](150) NULL,
 CONSTRAINT [PK_IT_epo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF object_id(N'IT_epo',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_epo en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_epo  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END