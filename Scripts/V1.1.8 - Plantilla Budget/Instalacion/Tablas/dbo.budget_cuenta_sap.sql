USE Portal_2_0
GO
IF object_id(N'budget_cuenta_sap',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[budget_cuenta_sap]
      PRINT '<<< budget_cuenta_sap en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos budget_cuenta_sap
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/02/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [budget_cuenta_sap](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_mapping][int] NOT NULL,
	[sap_account][varchar](8) NOT NULL,	
	[name][varchar](40) NOT NULL,	
 CONSTRAINT [PK_budget_cuenta_sap] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

-- restriccion de clave foranea
  alter table [budget_cuenta_sap]
 add constraint FK_budget_cuenta_sap_id_mapping
  foreign key (id_mapping)
  references budget_mapping(id);
 	  

	  
SET IDENTITY_INSERT [budget_cuenta_sap] ON 

--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (1, N'Supplier delay', 1)
--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (2, N'tkMM increase', 1)
--INSERT [budget_mapping] ([id], [descripcion], [activo]) VALUES (3, N'Customer increase', 1)

SET IDENTITY_INSERT [budget_cuenta_sap] OFF
GO

IF object_id(N'budget_cuenta_sap',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< budget_cuenta_sap en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla budget_cuenta_sap  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
