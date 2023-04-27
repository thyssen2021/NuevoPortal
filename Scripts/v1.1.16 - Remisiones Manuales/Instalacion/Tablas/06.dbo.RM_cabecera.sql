--USE Portal_2_0
GO
IF object_id(N'RM_cabecera',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RM_cabecera]
      PRINT '<<< RM_cabecera en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RM_cabecera
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RM_cabecera](
	[clave] [int] IDENTITY(1,1) NOT NULL,
	[activo] [bit] NOT NULL,
	[remisionNumero] [varchar](50) NULL,
	[almacenClave] [int] NOT NULL,				--FK
	[transporteOtro] [varchar](50) NOT NULL,
	[transporteProveedorClave] [int]  NULL,  --FK
	[nombreChofer] [varchar](50) NOT NULL,
	[clienteClave] [int]  NULL,				--FK
	[clienteOtro] [varchar](50) NOT NULL,
	[placaTractor] [varchar](50) NOT NULL,
	[placaRemolque] [varchar](50) NOT NULL,
	[horarioDescarga] [varchar](50) NOT NULL,
	[enviadoAClave] [int]  NULL,			--FK
	[enviadoAOtro] [varchar](50) NOT NULL,
	[clienteOtroDireccion] [varchar](100) NOT NULL,
	[enviadoAOtroDireccion] [varchar](100) NOT NULL,
	[motivoClave] [int] NOT NULL,			--FK
	[motivoTexto] [varchar](1500) NOT NULL,
	[retornaMaterial] [bit] NOT NULL,
	[ultimoEstatus][int] NULL, ---FK 
 CONSTRAINT [PK_RemisionCabecera] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
GO


  -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_almacenClave
  foreign key (almacenClave)
  references RM_almacen(clave);

    -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_transporteProveedorClave
  foreign key (transporteProveedorClave)
  references RM_transporte_proveedor(clave);

     -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_clienteClave
  foreign key (clienteClave)
  references clientes(clave);

       -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_enviadoAClave
  foreign key (enviadoAClave)
  references clientes(clave);

    -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_motivoClave
  foreign key (motivoClave)
  references RM_remision_motivo(clave);

    -- restriccion de clave foranea
alter table [dbo].[RM_cabecera]
 add constraint FK_RM_cabecera_ultimoEstatus
  foreign key (ultimoEstatus)
  references RM_estatus(clave);


--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RM_cabecera] ON 

--Pendiente hasta sustituir los fk cero por null

SET IDENTITY_INSERT [dbo].[RM_cabecera] OFF
	  
IF object_id(N'RM_cabecera',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RM_cabecera en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RM_cabecera  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
