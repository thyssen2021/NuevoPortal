USE Portal_2_0
GO
IF object_id(N'IT_mantenimientos_aplazamientos',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IT_mantenimientos_aplazamientos]
      PRINT '<<< IT_mantenimientos_aplazamientos en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IT_mantenimientos_aplazamientos
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/05/30
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [IT_mantenimientos_aplazamientos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_mantenimiento] [int] NOT NULL,  --FK
	[id_sistemas] [int] NOT NULL,		--FK
	[fecha_anterior] [datetime] NOT NULL,	
	[nueva_fecha] [datetime] NOT NULL,	
	[motivo] [varchar](300) NOT NULL,	
 CONSTRAINT [PK_IT_mantenimientos_aplazamientos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restriccion de clave foranea
alter table [IT_mantenimientos_aplazamientos]
 add constraint FK_IT_mantenimientos_aplazamientos_id_mantenimiento
  foreign key (id_mantenimiento)
  references IT_mantenimientos(id);

  alter table [IT_mantenimientos_aplazamientos]
 add constraint FK_IT_mantenimientos_aplazamientos_id_sistemas
  foreign key (id_sistemas)
  references empleados(id);


IF object_id(N'IT_mantenimientos_aplazamientos',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IT_mantenimientos_aplazamientos en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IT_mantenimientos_aplazamientos  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
