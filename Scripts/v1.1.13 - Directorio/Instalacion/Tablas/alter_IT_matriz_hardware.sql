--USE Portal_2_0
GO
IF object_id(N'IT_matriz_hardware',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_matriz_hardware]
      PRINT '<<< IT_matriz_hardware en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

--elimina tabla que ya no será necesaria
IF object_id(N'IT_hardware_tipo',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_hardware_tipo]
      PRINT '<<< IT_hardware_tipo en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_matriz_hardware
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_matriz_hardware](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_matriz_requerimientos] [int] NOT NULL,
	[id_it_hardware] [int] NOT NULL,
	[id_it_asignacion_hardware][int] NULL,
	[descripcion] [varchar](90) NULL,	
	[comentario] [varchar](150) NULL,
	[completado][bit] NULL,
 CONSTRAINT [PK_IT_matriz_hardware] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [IT_matriz_hardware]
 add constraint FK_IT_matriz_hardware_id_it_hardware
  foreign key (id_it_hardware)
  references IT_inventory_hardware_type(id);

  -- restriccion de clave foranea
alter table [IT_matriz_hardware]
 add constraint FK_IT_matriz_hardware_id_it_asignacion_hardware
  foreign key (id_it_asignacion_hardware)
  references IT_asignacion_hardware(id);

  -- restriccion de clave foranea
alter table [IT_matriz_hardware]
 add constraint FK_IT_matriz_hardware_id_matriz_requerimientos
  foreign key (id_matriz_requerimientos)
  references IT_matriz_requerimientos(id);

-- restriccion unique
 alter table [IT_matriz_hardware]
  add constraint UQ_IT_matriz_hardware
  unique (id_matriz_requerimientos,id_it_hardware);
GO

	  
IF object_id(N'IT_matriz_hardware',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_matriz_hardware en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_matriz_hardware  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
