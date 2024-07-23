--USE Portal_2_0
GO
IF object_id(N'SCDM_cat_rel_gerentes_clientes',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[SCDM_cat_rel_gerentes_clientes]
      PRINT '<<< SCDM_cat_rel_gerentes_clientes en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los SCDM_cat_rel_gerentes_clientes
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[SCDM_cat_rel_gerentes_clientes](
	[id] [int] IDENTITY(1,1) NOT NULL,	
	[id_empleado] [int] NOT NULL,
	[id_cliente] [int] NOT NULL,
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_rel_gerentes_clientes_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

  -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_rel_gerentes_clientes]
 add constraint FK_SCDM_cat_rel_gerentes_clientes_id_empleado
  foreign key (id_empleado)
  references empleados(id);

    -- restriccion de clave foranea
alter table [dbo].[SCDM_cat_rel_gerentes_clientes]
 add constraint FK_SCDM_cat_rel_gerentes_clientes_id_cliente
  foreign key (id_cliente)
  references clientes(clave);

--Inserta valores

SET IDENTITY_INSERT [dbo].[SCDM_cat_rel_gerentes_clientes] ON 

GO



SET IDENTITY_INSERT [dbo].[SCDM_cat_rel_gerentes_clientes] OFF
GO


	  
IF object_id(N'SCDM_cat_rel_gerentes_clientes',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< SCDM_cat_rel_gerentes_clientes en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla SCDM_cat_rel_gerentes_clientes  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
