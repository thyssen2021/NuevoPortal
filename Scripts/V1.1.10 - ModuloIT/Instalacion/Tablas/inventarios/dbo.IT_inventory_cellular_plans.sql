USE Portal_2_0
GO
IF object_id(N'IT_inventory_cellular_plans',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_inventory_cellular_plans]
      PRINT '<<< IT_inventory_cellular_plans en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_inventory_cellular_plans
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/06/06
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [IT_inventory_cellular_plans](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[nombre_plan][varchar](120) NOT NULL,
	[nombre_compania][varchar](80) NOT NULL,
	[precio][decimal](7,2) NULL,
	[comentarios][varchar](250) NULL,
	[activo] [bit] NOT NULL DEFAULT 1, 
 CONSTRAINT [PK_IT_inventory_cellular_plans] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



--Datos demo

delete from dbo.[IT_inventory_cellular_plans]; DBCC checkident ('IT_inventory_cellular_plans', reseed,0);
Select * from [IT_inventory_cellular_plans]

INSERT INTO IT_inventory_cellular_plans (nombre_plan, nombre_compania, precio, activo) values (N'TM SIN LIMITE EMPRESA 2000','TELCEL',257.76,1);
INSERT INTO IT_inventory_cellular_plans (nombre_plan, nombre_compania, precio, activo) values (N'TM SIN LIMITE EMPRESA 5000','TELCEL',430.17,1);
INSERT INTO IT_inventory_cellular_plans (nombre_plan, nombre_compania, precio, activo) values (N'MAX SL EMPRESA 7000 4G','TELCEL',690.47,1);
 	  
IF object_id(N'IT_inventory_cellular_plans',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_inventory_cellular_plans en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_inventory_cellular_plans  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
