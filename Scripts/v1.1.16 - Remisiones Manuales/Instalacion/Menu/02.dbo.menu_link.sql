USE Portal_2_0
GO
IF object_id(N'menu_link',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[menu_link]
      PRINT '<<< menu_link en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los menu_link
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/05/15
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[menu_link](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_menu_item][int]NOT NULL,
	[link_externo] bit NOT NULL DEFAULT 0,
	[controller] [varchar](100) NULL,
	[action][varchar](100) NULL,
	[enlace][varchar](200) NULL,
	[text][varchar](100) NOT NULL,
	CONSTRAINT [PK_menu_link] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Foreign Keys
-- restriccion de clave foranea
alter table [dbo].[menu_link]
 add constraint FK_menu_link_id_menu_item
  foreign key (id_menu_item)
  references menu_item(id);

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[menu_link] ON 
--bitacoras produccion
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (1,1,0,N'ProduccionRegistros',N'Index',NULL,N'Acceder')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (2,1,0,N'ProduccionResultado',N'Index',NULL,N'Reporte')
--pfa
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (3,2,0,N'PremiumFreightApproval',N'Index',NULL,N'Crear')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (4,2,0,N'PremiumFreightApproval',N'AutorizadorPendientes',NULL,N'Aprobar')
--ideas de mejora
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (5,3,1,NULL,NULL,N'http://mejoracontinua.lagermex1.com.mx:8083/',N'Acceder')
--remisiones manuales
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (6,4,1,NULL,NULL,N'http://10.122.163.24:8080/',N'Acceder')
--piezas de descarte
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (7,5,0,N'InspeccionRegistros',N'BusquedaRegistro',NULL,N'Acceder')
--reporte de pesadas
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (8,6,0,N'ReportePesadas',N'Puebla',NULL,N'Puebla')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (9,6,0,N'ReportePesadas',N'Silao',NULL,N'Silao')
--directorio
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (10,7,0,N'Contacts',N'Index',NULL,N'Directorio')
--menu comedor
--directorio
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (11,8,0,N'RH_menu_comedor',N'DetailsMenu',NULL,N'Acceder')
--ciclo interno unidades
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (12,9,0,N'RU_registros',N'Index',NULL,N'Acceder')
--pólizas manuales
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (13,10,0,N'PolizaManual',N'CapturistaCreadas',NULL,N'Registro')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (14,10,0,N'PolizaManual',N'Reportes',NULL,N'Reportes')
--RH
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (15,11,0,N'Empleados',N'Index',NULL,N'Administración')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (16,11,0,N'IT_matriz_requerimientos',N'ListadoUsuarios',NULL,N'Matriz de Requerimientos')
--IT
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (17,12,0,N'IT_Inventory_items',N'Index',NULL,N'Inventarios')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (18,12,0,N'it_mantenimientos',N'Index',NULL,N'Mantenimientos')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (19,12,0,N'it_asignacion_hardware',N'Index',NULL,N'Asignaciones')
--Budget CeCo
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (20,13,0,N'ResponsableBudget',N'Centros',NULL,N'Mis Centros de Costo')
--Órdenes de trabajo
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (21,14,0,N'OrdenesTrabajo',N'Create',NULL,N'Solicitud')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (22,14,0,N'OrdenesTrabajo',N'ListadoAsignacionPendientes',NULL,N'Administración')
insert into menu_link (id,[id_menu_item],[link_externo],[controller],[action],[enlace],[text]) VALUES (23,14,0,N'OrdenesTrabajo',N'ReporteGeneral',NULL,N'Reportes')


SET IDENTITY_INSERT [dbo].[menu_link] OFF
	  
IF object_id(N'menu_link',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< menu_link en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla menu_link  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
