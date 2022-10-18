USE Portal_2_0
GO
IF object_id(N'IT_equipos_checklist',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_equipos_checklist]
      PRINT '<<< IT_equipos_checklist en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_equipos_checklist
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/10/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_equipos_checklist](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_sistemas] [int] NOT NULL, 
	[nombre] [varchar](50) NOT NULL,
	[numero_serie] [varchar](50) NOT NULL,
	[modelo][varchar](120) NOT NULL,
	[sistema_operativo][varchar](50) NULL,
	[comentario_general] [varchar](200) NULL,
	[fecha][datetime] NOT NULL,
	[estatus][varchar](20) NOT NULL
 CONSTRAINT [PK_IT_equipos_checklist] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [IT_equipos_checklist]
 add constraint FK_equipos_checklist_id_sistemas
  foreign key (id_sistemas)
  references empleados(id);

  -- restricion check
ALTER TABLE [IT_equipos_checklist] ADD CONSTRAINT CK_IT_equipos_checklist_estatus CHECK ([estatus] IN 
('INICIADO','EN_PROCESO','FINALIZADO')
)

   	  
IF object_id(N'IT_equipos_checklist',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_equipos_checklist en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_equipos_checklist  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
