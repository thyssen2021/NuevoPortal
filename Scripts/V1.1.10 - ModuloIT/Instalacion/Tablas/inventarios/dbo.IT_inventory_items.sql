USE Portal_2_0
GO
IF object_id(N'IT_inventory_items',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_items]
      PRINT '<<< IT_inventory_items en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_items
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/05/10
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_inventory_items](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_planta] [int] NOT NULL,
	[id_inventory_type] [int] NOT NULL,
	[active][bit] NOT NULL,
	[purchase_date][datetime] NULL,
	[inactive_date][datetime] NULL,
	[comments][varchar](250) NULL,
	[hostname][varchar](50) NULL,
	[brand][varchar](80) NULL,
	[model][varchar](80) NULL,
	[serial_number][varchar](60) NULL,
	--[warranty][bit] NULL,
	--[start_warranty][datetime] NULL,
	[end_warranty][datetime] NULL,
	[mac_lan][varchar](17) NULL,
	[mac_wlan][varchar](17) NULL,
	[processor][varchar](50) NULL,
	[total_physical_memory_mb][decimal](8,2) NULL, --RAM
	[maintenance_period_months][int] NULL,
	--[last_maintenance][datetime] NULL,
	--[physical_status][varchar](150) NULL,
	--[is_in_operation][bit] NULL,
	[cpu_speed_mhz][int] NULL,
	[operation_system][varchar](50) NULL,
	[bits_operation_system][int] NULL,
	[number_of_cpus][int] NULL,
	[printer_ubication][varchar](50) NULL,
	[ip_adress][varchar](25) NULL,
	[cost_center][varchar](8) NULL,
	[movil_device_storage_mb][int] NULL,
	[inches][decimal](4,2) NULL,
	[imei_1][varchar](15) NULL,
	[imei_2][varchar](15) NULL,
	[code][varchar](20) NULL,
	[accessories][varchar](50) NULL,

 CONSTRAINT [PK_IT_inventory_items] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_inventory_items]
 add constraint FK_IT_inventory_items_id_planta
  foreign key (id_planta)
  references plantas(clave);

  -- restriccion de clave foranea
alter table [IT_inventory_items]
 add constraint FK_IT_inventory_items_id_inventory_type
  foreign key (id_inventory_type)
  references IT_inventory_hardware_type(id);

   	  
IF object_id(N'IT_inventory_items',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_items en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_items  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
