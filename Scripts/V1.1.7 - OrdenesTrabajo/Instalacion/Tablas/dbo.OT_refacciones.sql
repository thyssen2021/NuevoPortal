USE Portal_2_0
GO
IF object_id(N'OT_refacciones',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_refacciones]
      PRINT '<<< OT_refacciones en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los OT_refacciones
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/14
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_refacciones](
	[id][int] IDENTITY(1,1) NOT NULL,
	[id_orden_trabajo] [int] NOT NULL,
	[cantidad] [decimal](8,2) NOT NULL,
	[descripcion] [varchar](155) NOT NULL,
	[tipo][varchar](15) NOT NULL
 CONSTRAINT [PK_OT_refacciones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--agregar FK
-- restriccion de clave foranea
alter table [OT_refacciones]
 add constraint FK_OT_refacciones_id_orden_trabajo
  foreign key (id_orden_trabajo)
  references orden_trabajo(id);

  -- restricion check
ALTER TABLE [OT_refacciones] ADD CONSTRAINT CK_OT_refacciones_tipo CHECK ([tipo] IN 
('NECESARIA','FALTANTE')
)
GO

	  
IF object_id(N'OT_refacciones',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_refacciones en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_refacciones  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
