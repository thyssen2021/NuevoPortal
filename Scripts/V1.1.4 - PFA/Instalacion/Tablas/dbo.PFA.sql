USE Portal_2_0
GO
IF object_id(N'PFA',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PFA]
      PRINT '<<< PFA en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PFA
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PFA](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_solicitante][int] NOT NULL,
	[id_PFA_Department][int] NOT NULL,
	[id_PFA_volume][int] NOT NULL,
	[id_PFA_border_port][int] NOT NULL,
	[id_PFA_destination_plant][int] NOT NULL,
	[id_PFA_reason][int] NOT NULL,
	[id_PFA_type_shipment][int] NOT NULL,
	[id_PFA_responsible_cost][int] NOT NULL,
	[id_PFA_recovered_cost][int] NOT NULL,
	[id_PFA_autorizador][int] NOT NULL,
	[date_request][datetime] NOT NULL,
	[sap_part_number][varchar](15) NOT NULL,
	[customer_part_number][varchar](15) NOT NULL,
	[volume][decimal](11,2) NOT NULL,
	[mill][varchar](35) NOT NULL,
	[customer][varchar](40) NOT NULL,
	[total_cost][decimal](11,2) NOT NULL,
	[total_pf_cost][decimal](11,2) NOT NULL,
	[promise_recovering_date][datetime] NOT NULL,
	[comentarios][varchar](350)  NULL,
	[razon_rechazo][varchar](350)  NULL,	
	[estatus][varchar](12) NOT NULL ,
	[fecha_aprobacion][datetime] NULL,	
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PFA] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK
-- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_solicitante
  foreign key (id_solicitante)
  references empleados(id);

    -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_Department
  foreign key (id_PFA_Department)
  references PFA_Department(id);

  -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_volume
  foreign key (id_PFA_volume)
  references PFA_Volume(id);

  -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_border_port
  foreign key (id_PFA_border_port)
  references PFA_Border_port(id);

    -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_destination_plant
  foreign key (id_PFA_destination_plant)
  references PFA_Destination_plant(id);

   -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_reason
  foreign key (id_PFA_reason)
  references PFA_Reason(id);

     -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_type_shipment
  foreign key (id_PFA_type_shipment)
  references PFA_type_shipment(id);

     -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_responsible_cost
  foreign key (id_PFA_responsible_cost)
  references PFA_responsible_cost(id);

       -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_id_PFA_recovered_cost
  foreign key (id_PFA_recovered_cost)
  references PFA_recovered_cost(id);

  -- restriccion de clave foranea
alter table [PFA]
 add constraint FK_PFA_autorizador
  foreign key (id_PFA_autorizador)
  references empleados(id);
  
-- restricción default
ALTER TABLE [PFA] ADD  CONSTRAINT [DF_PFA_activo]  DEFAULT (1) FOR [activo]

-- restricción default
ALTER TABLE [PFA] ADD  CONSTRAINT [DF_PFA_date_request]  DEFAULT (GETDATE()) FOR [date_request]
GO

--retricion check
ALTER TABLE [PFA] ADD CONSTRAINT CK_PFA_Estatus CHECK ([estatus] IN ('CREADO','ENVIADO', 'APROBADO', 'RECHAZADO','FINALIZADO'))
GO

 	  
IF object_id(N'PFA',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PFA en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PFA  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
