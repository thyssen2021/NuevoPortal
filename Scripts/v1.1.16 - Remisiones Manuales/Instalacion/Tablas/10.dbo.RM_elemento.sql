--USE Portal_2_0
GO
IF object_id(N'RM_elemento',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_elemento]
      PRINT '<<< RM_elemento en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_elemento
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_elemento](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NULL,
	--[usuarioClave] [int] NOT NULL,
	[remisionCabeceraClave] [int] NOT NULL,  --FK
	[capturaFecha] [datetime] NOT NULL,
	[numeroParteCliente] [varchar](50) NOT NULL,
	[numeroMaterial] [varchar](50) NOT NULL,
	[numeroLote] [varchar](50) NOT NULL,
	[numeroRollo] [varchar](50) NOT NULL,
	[cantidad] [float] NOT NULL,
	--[unidadMedidaCantidadClave] [int] NULL,
	[peso] [float] NOT NULL,
	--[unidadMedidaPesoClave] [int] NULL,
	[remisionSap] [varchar](20) NULL,
 CONSTRAINT [PK_RemisionElemento] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[RM_elemento]
 add constraint FK_RM_elemento_remisionCabeceraClave
  foreign key (remisionCabeceraClave)
  references RM_cabecera(clave);

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RM_elemento] ON 

--Pendiente la inserccion de datos hasta cambiar clave usario por id_empleado

SET IDENTITY_INSERT [dbo].[RM_elemento] OFF
	  
IF object_id(N'RM_elemento',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_elemento en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_elemento  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
