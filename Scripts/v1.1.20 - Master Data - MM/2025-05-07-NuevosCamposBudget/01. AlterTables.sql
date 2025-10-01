-- Inserta nuevas columnas de budget ---

-- SCDM_solicitud_rel_item_material
ALTER TABLE SCDM_solicitud_rel_item_material
ADD
    [Almacen_Norte] BIT NULL Default 0,
    [Tipo_de_Transporte] VARCHAR(10) NULL,
    [Tkmm_SOP] DATE NULL,
    [Tkmm_EOP] DATE NULL,
    [Pieces_Pac] FLOAT NULL,
    [Stacks_Pac] FLOAT NULL,
    [Type_of_Pallet] VARCHAR(20) NULL;

-- SCDM_solicitud_rel_creacion_referencia
ALTER TABLE SCDM_solicitud_rel_creacion_referencia
ADD
    [Almacen_Norte] BIT NULL Default 0,
    [Tipo_de_Transporte] VARCHAR(10) NULL,
    [Tkmm_SOP] DATE NULL,
    [Tkmm_EOP] DATE NULL,
    [Pieces_Pac] FLOAT NULL,
    [Stacks_Pac] FLOAT NULL,
    [Type_of_Pallet] VARCHAR(20) NULL;

-- SCDM_solicitud_rel_cambio_budget
ALTER TABLE SCDM_solicitud_rel_cambio_budget
ADD
    [Almacen_Norte] BIT NULL Default 0,
    [Tipo_de_Transporte] VARCHAR(10) NULL,
    [Tkmm_SOP] DATE NULL,
    [Tkmm_EOP] DATE NULL,
    [Pieces_Pac] FLOAT NULL,
    [Stacks_Pac] FLOAT NULL,
    [Type_of_Pallet] VARCHAR(20) NULL;

-- mm_v3
ALTER TABLE mm_v3
ADD
    [Almacen_Norte] BIT NULL Default 0,
    [Tipo_de_Transporte] VARCHAR(10) NULL,
    [Tkmm_SOP] DATE NULL,
    [Tkmm_EOP] DATE NULL,
    [Pieces_Pac] FLOAT NULL,
    [Stacks_Pac] FLOAT NULL,
    [Type_of_Pallet] VARCHAR(20) NULL;


GO

/****** Object:  Table [dbo].[SCDM_cat_tipo_transporte]    Script Date: 27/05/2025 04:25:38 p. m. ******/
--Ejecutar a partir de aqui ---

CREATE TABLE [dbo].[SCDM_cat_tipo_transporte](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave] [varchar](3) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,	
	[activo] [bit] NOT NULL,
 CONSTRAINT [PK_SCDM_cat_tipo_transporte_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

select * from [SCDM_cat_tipo_transporte]

Insert into [SCDM_cat_tipo_transporte] (clave, descripcion, activo) values ('01', 'Train', 1)
Insert into [SCDM_cat_tipo_transporte] (clave, descripcion, activo) values ('02', 'Truck', 1)
Insert into [SCDM_cat_tipo_transporte] (clave, descripcion, activo) values ('03', 'Ship', 1)
Insert into [SCDM_cat_tipo_transporte] (clave, descripcion, activo) values ('04', 'Airplane', 1)

----- Alter Propulsion System ------

ALTER TABLE SCDM_cat_ihs
ADD [Propulsion_System] VARCHAR(120) NULL;


ALTER TABLE SCDM_cat_ihs
ADD [Country] VARCHAR(3) NULL;

ALTER TABLE SCDM_cat_ihs
ADD [sop] date NULL;

ALTER TABLE SCDM_cat_ihs
ADD [eop] date NULL;

--- Alter Pais Origen --

-- SCDM_solicitud_rel_item_material
ALTER TABLE SCDM_solicitud_rel_item_material
ADD    
    [Country_IHS] VARCHAR(5) NULL;

-- SCDM_solicitud_rel_creacion_referencia
ALTER TABLE SCDM_solicitud_rel_creacion_referencia
ADD   
    [Country_IHS] VARCHAR(5) NULL;

-- SCDM_solicitud_rel_cambio_budget
ALTER TABLE SCDM_solicitud_rel_cambio_budget
ADD   
    [Country_IHS] VARCHAR(5) NULL;

/*** ACtualizar Views ***/

--================================================

/*********************************************************************************************************/
GO

/****** Object:  View [dbo].[view_SCDM_clientes_por_solictud]    Script Date: 04/06/2025 05:56:20 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER         VIEW [dbo].[view_SCDM_clientes_por_solictud] AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY f.id_solicitud), 0) as id,
f.* FROM (
-- creación de materiales
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_item_material cm	
join clientes as cl on '('+cl.claveSAP+') - '+cl.descripcion = cm.cliente 
-- creación de materiales con referencia
UNION
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_creacion_referencia mr
join clientes as cl on '('+cl.claveSAP+') - '+cl.descripcion = mr.cliente 
 -- cambio de ingenieria
UNION
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_cambio_ingenieria ci 
join clientes as cl on '('+cl.claveSAP+') - '+cl.descripcion = ci.cliente 
-- orden Compra
UNION
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_orden_compra oc  -- orden compra	
join class_v3 as c on c.Object = oc.num_material
JOIN clientes as cl on c.Customer = cl.claveSAP
-- activaciones
UNION
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_activaciones ac  	
join class_v3 as c on c.Object = ac.material
JOIN clientes as cl on c.Customer = cl.claveSAP
--extensiones
UNION
select distinct id_solicitud, cl.claveSAP as sap_cliente, cl.descripcion as nombre_cliente from SCDM_solicitud_rel_extension_usuario ex  	
join class_v3 as c on c.Object = ex.material
JOIN clientes as cl on c.Customer = cl.claveSAP

) as f
		
GO

/****************************************************/

/****** Object:  View [dbo].[view_SCDM_materiales_extension]    Script Date: 04/06/2025 05:57:33 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--select * from view_SCDM_materiales_extension
CREATE OR ALTER           VIEW [dbo].[view_SCDM_materiales_extension] AS

SELECT
  im.id_solicitud,
  im.numero_material,
  im.planta_sap,
  sl.clave,
  (SELECT TOP 1
    extension_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_item_material = im.id
  AND sl.id = id_cat_storage_location)
  AS [extension_ejecucion_correcta],
  (SELECT TOP 1
    extension_mensaje_sap
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_item_material = im.id
  AND sl.id = id_cat_storage_location)
  AS [mensaje_sap]
FROM SCDM_cat_storage_location AS sl
JOIN plantas AS p
  ON p.clave = sl.id_planta
CROSS JOIN view_SCDM_solicitud_rel_item_material AS im
WHERE im.planta_sap = p.codigoSap and sl.es_virtual = 0

UNION

SELECT
  cr.id_solicitud,
  cr.nuevo_material,
  p.codigoSap,
  sl.clave,
  (SELECT TOP 1
    extension_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_creacion_referencia = cr.id
  AND sl.id = id_cat_storage_location)
  AS [extension_ejecucion_correcta],
  (SELECT TOP 1
    extension_mensaje_sap
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_creacion_referencia = cr.id
  AND sl.id = id_cat_storage_location)
  AS [mensaje_sap]
FROM SCDM_cat_storage_location AS sl
JOIN plantas AS p
  ON p.clave = sl.id_planta
CROSS JOIN SCDM_solicitud_rel_creacion_referencia AS cr
WHERE p.clave = cr.id_planta and sl.es_virtual = 0

UNION

select eu.id_solicitud,
	eu.material,
	p.codigoSap,
	sl.clave,
	(SELECT TOP 1 r.extension_ejecucion_correcta
		FROM SCDM_solicitud_rel_extension AS r
		WHERE id_rel_solicitud_extension_usuario = eu.id
		AND sl.id = id_cat_storage_location)
  AS [extension_ejecucion_correcta],
  (SELECT TOP 1 r.extension_mensaje_sap
		FROM SCDM_solicitud_rel_extension AS r
		WHERE id_rel_solicitud_extension_usuario = eu.id
		AND sl.id = id_cat_storage_location)
  AS [mensaje_saps] 
from SCDM_cat_storage_location as sl
JOIN plantas AS p
  ON p.clave = sl.id_planta
  CROSS JOIN SCDM_solicitud_rel_extension_usuario AS eu  
WHERE (p.codigoSap = eu.planta_destino and es_virtual = 0) or (sl.es_virtual = 1 AND (SELECT count (*) from SCDM_rel_solicitud_extension_almacenes_virtuales as av where (av.id_solicitud = eu.id_solicitud AND almacen_virtual = sl.almacen))>0)

GO


/**********************************************************************/
/****** Object:  View [dbo].[view_SCDM_materiales_x_solicitud]    Script Date: 04/06/2025 05:58:18 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER         VIEW [dbo].[view_SCDM_materiales_x_solicitud] AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY t1.numero_solicitud), 0) as id,
t1.* FROM (
--Solicitudes de creacion de materiales y C&B
select x.id_solicitud as [numero_solicitud], ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, tm.descripcion as [tipo_material], x.numero_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_item_material as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_materiales_solicitud as tm on tm.id = x.id_tipo_material

union

--solicitudes de creación con referencia
select x.id_solicitud, ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, x.tipo_material_text as [tipo_material], x.nuevo_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_creacion_referencia as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante

union

--Cambio de ingenieria
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, x.tipo_material_text as [tipo_material], x.material_existente, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_cambio_ingenieria as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio

union

--Cambio en activaciones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, 'N/D' as [tipo_material], x.material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_activaciones as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio
--where tc.id = 1 or tc.id = 11

union

--extensiones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], NULL as tipo_cambio, 'N/D' as [tipo_material], x.material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_extension_usuario as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante

union
--ordenes de compra
--Cambio en activaciones
select x.id_solicitud, ts.descripcion as [tipo_solicitud], tc.descripcion as tipo_cambio, 'N/D' as [tipo_material], x.num_material, p.descripcion as [planta],
emp.nombre+' '+ISNULL(emp.apellido1,'')+' '+ISNULL(emp.apellido2,'') as solicitante, pr.descripcion as prioridad, s.fecha_creacion as fecha_solicitud
from SCDM_solicitud_rel_orden_compra as x
join SCDM_solicitud AS S ON s.id = x.id_solicitud
join SCDM_cat_tipo_solicitud as ts on ts.id = s.id_tipo_solicitud
join plantas as p on p.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
join empleados as emp on emp.id = s.id_solicitante
/*left*/ join SCDM_cat_tipo_cambio as tc on tc.id = s.id_tipo_cambio

)t1
GO


/***********************************************************************/
/****** Object:  View [dbo].[view_SCDM_solicitud_cambios_ingenieria]    Script Date: 04/06/2025 05:58:57 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER           VIEW [dbo].[view_SCDM_solicitud_cambios_ingenieria] AS

select cr.*, cr.tipo_material_text as tipo_material, p.codigoSap,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad ) >=1
		THEN cast((select top 1 clave from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) AS varchar)	
	ELSE cr.grado_calidad
END AS clave_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) >=1
		THEN 1
	ELSE 0
END AS existe_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity ) >=1
		THEN cast((select top 1 clave from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) AS varchar)	
	ELSE cr.commodity
END AS clave_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) >=1
		THEN 1
	ELSE 0
END AS existe_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie ) >=1
		THEN cast((select top 1 clave from SCDM_cat_superficie where descripcion = cr.superficie) AS varchar)	
	ELSE cr.superficie
END AS clave_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie) >=1
		THEN 1
	ELSE 0
END AS existe_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial ) >=1
		THEN cast((select top 1 clave from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) AS varchar)	
	ELSE cr.tratamiento_superficial
END AS clave_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) >=1
		THEN 1
	ELSE 0
END AS existe_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento ) >=1
		THEN cast((select top 1 clave from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) AS varchar)	
	ELSE cr.peso_recubrimiento
END AS clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) >=1
		THEN 1
	ELSE 0
END AS existe_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino ) >=1
		THEN cast((select top 1 clave from SCDM_cat_molino where descripcion = cr.molino) AS varchar)	
	ELSE cr.molino
END AS clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino) >=1
		THEN 1
	ELSE 0
END AS existe_molino,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma ) >=1
		THEN cast((select top 1 clave from SCDM_cat_forma_material where descripcion = cr.forma) AS varchar)	
	ELSE cr.forma
END AS clave_forma,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma) >=1
		THEN 1
	ELSE 0
END AS existe_forma,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente ) >=1
		THEN cast((select top 1 claveSAP from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) AS varchar)	
	ELSE cr.cliente
END AS clave_cliente,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) >=1
		THEN 1
	ELSE 0
END AS existe_cliente
from SCDM_solicitud_rel_cambio_ingenieria as cr
--join SCDM_cat_tipo_materiales_solicitud as tm on tm.id = cr.id_tipo_material
join plantas as p on p.clave = cr.id_planta

GO


/*************************************************************************************************/


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER             VIEW [dbo].[view_SCDM_solicitud_creacion_referencia] AS

select cr.*, cr.tipo_material_text as tipo_material, tv.descripcion as tipo_venta, p.codigoSap,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad ) >=1
		THEN cast((select top 1 clave from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) AS varchar)	
	ELSE cr.grado_calidad
END AS clave_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) >=1
		THEN 1
	ELSE 0
END AS existe_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity ) >=1
		THEN cast((select top 1 clave from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) AS varchar)	
	ELSE cr.commodity
END AS clave_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) >=1
		THEN 1
	ELSE 0
END AS existe_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie ) >=1
		THEN cast((select top 1 clave from SCDM_cat_superficie where descripcion = cr.superficie) AS varchar)	
	ELSE cr.superficie
END AS clave_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie) >=1
		THEN 1
	ELSE 0
END AS existe_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial ) >=1
		THEN cast((select top 1 clave from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) AS varchar)	
	ELSE cr.tratamiento_superficial
END AS clave_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) >=1
		THEN 1
	ELSE 0
END AS existe_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento ) >=1
		THEN cast((select top 1 clave from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) AS varchar)	
	ELSE cr.peso_recubrimiento
END AS clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) >=1
		THEN 1
	ELSE 0
END AS existe_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino ) >=1
		THEN cast((select top 1 clave from SCDM_cat_molino where descripcion = cr.molino) AS varchar)	
	ELSE cr.molino
END AS clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino) >=1
		THEN 1
	ELSE 0
END AS existe_molino,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma ) >=1
		THEN cast((select top 1 clave from SCDM_cat_forma_material where descripcion = cr.forma) AS varchar)	
	ELSE cr.forma
END AS clave_forma,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma) >=1
		THEN 1
	ELSE 0
END AS existe_forma,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente ) >=1
		THEN cast((select top 1 claveSAP from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) AS varchar)	
	ELSE cr.cliente
END AS clave_cliente,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) >=1
		THEN 1
	ELSE 0
END AS existe_cliente
from SCDM_solicitud_rel_creacion_referencia as cr
--join SCDM_cat_tipo_materiales_solicitud as tm on tm.id = cr.id_tipo_material
join SCDM_cat_tipo_venta as tv on tv.id = cr.id_tipo_venta
join plantas as p on p.clave = cr.id_planta

GO


/*****************************************************************/

/****** Object:  View [dbo].[view_SCDM_solicitud_rechazos_tiempos]    Script Date: 04/06/2025 06:00:08 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER   VIEW [dbo].[view_SCDM_solicitud_rechazos_tiempos] AS
select 
id_solicitud, pr.descripcion as prioridad, dep.descripcion as departamento, (e.nombre+' '+e.apellido1+' '+e.apellido2) as nombre_rechazo,
pl.descripcion as planta, sa.comentario_rechazo,  fecha_asignacion, fecha_rechazo,  dbo.WorkTime (sa.fecha_asignacion, sa.fecha_rechazo) as minutos, ROUND(CAST(dbo.WorkTime(sa.fecha_asignacion, sa.fecha_rechazo) AS float)/60, 2) as horas
from SCDM_solicitud_asignaciones sa
join SCDM_cat_departamentos_asignacion as dep on sa.id_departamento_asignacion = dep.id
join empleados as e on e.id = sa.id_rechazo
join SCDM_solicitud as s on s.id = sa.id_solicitud
join plantas as pl on pl.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
WHERE sa.descripcion <> 'Asignación inicial'
GO

/*******************************************************************/

/****** Object:  View [dbo].[view_SCDM_solicitud_rel_facturacion]    Script Date: 04/06/2025 06:00:46 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--select * from view_SCDM_solicitud_rel_facturacion
CREATE OR ALTER     VIEW [dbo].[view_SCDM_solicitud_rel_facturacion] AS

SELECT im.id_solicitud, numero_material as material, planta_sap as planta,
(SELECT TOP 1 unidad_medida
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id
  )as unidad_medida,
  (SELECT TOP 1 clave_producto_servicio
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id
  ) clave_producto_servicio,
    (SELECT TOP 1 cliente
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) cliente,
    (SELECT TOP 1 descripcion_en
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) descripcion_en,
  (SELECT TOP 1 uso_CFDI_01
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_01,
	(SELECT TOP 1 uso_CFDI_02
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_02,
(SELECT TOP 1 uso_CFDI_03
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_03,
(SELECT TOP 1 uso_CFDI_04
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_04,
(SELECT TOP 1 uso_CFDI_05
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_05,
(SELECT TOP 1 uso_CFDI_06
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_06,
(SELECT TOP 1 uso_CFDI_07
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_07,
(SELECT TOP 1 uso_CFDI_08
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_08,
(SELECT TOP 1 uso_CFDI_09
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_09,
  (SELECT TOP 1 uso_CFDI_10
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_10,
   (SELECT TOP 1 mensaje_sap
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) mensaje_sap,
    (SELECT TOP 1 ejecucion_correcta
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) ejecucion_correcta
from view_SCDM_solicitud_rel_item_material AS im

UNION

SELECT cr.id_solicitud, nuevo_material as material, codigoSap as planta,
(SELECT TOP 1 unidad_medida
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as unidad_medida,
  (SELECT TOP 1 clave_producto_servicio
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as clave_producto_servicio,
  (SELECT TOP 1 cliente
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as cliente,
(SELECT TOP 1 descripcion_en
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as descripcion_en,
  (SELECT TOP 1 uso_CFDI_01
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_01,
  (SELECT TOP 1 uso_CFDI_02
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_02,
(SELECT TOP 1 uso_CFDI_03
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_03,
(SELECT TOP 1 uso_CFDI_04
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_04,
(SELECT TOP 1 uso_CFDI_05
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_05,
(SELECT TOP 1 uso_CFDI_06
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_06,
(SELECT TOP 1 uso_CFDI_07
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_07,
(SELECT TOP 1 uso_CFDI_08
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_08,
(SELECT TOP 1 uso_CFDI_09
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_09,
(SELECT TOP 1 uso_CFDI_10
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_10,
(SELECT TOP 1 mensaje_sap
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as mensaje_sap,
(SELECT TOP 1 ejecucion_correcta
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as ejecucion_correcta
from view_SCDM_solicitud_creacion_referencia AS cr
GO


/*****************************************************************/

/****** Object:  View [dbo].[view_SCDM_solicitud_rel_item_budget]    Script Date: 04/06/2025 06:01:24 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER                 VIEW [dbo].[view_SCDM_solicitud_rel_item_budget] AS
--Creación de Materiales
select t.descripcion As tipo_material, i.id, i.id_solicitud, i.id_tipo_material, i.metal, i.tipo_venta, i.posicion_rollo, i.ihs_1, i.ihs_2, i.ihs_3, i.ihs_4, i.ihs_5,
i.modelo_negocio, i.numero_material, CAST (i.espesor_mm AS varchar) AS espesor_mm, CAST (i.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (i.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (i.ancho_mm AS varchar) AS ancho_mm,
CAST (i.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (i.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
CAST (i.peso_min_kg AS VARCHAR) as peso_min_kg, CAST (i.requiere_consiliacion_puntas_colar AS VARCHAR) AS requiere_consiliacion_puntas_colar, CAST (i.scrap_permitido_puntas_colas as VARCHAR) AS scrap_permitido_puntas_colas, 
CAST (i.avance_mm AS varchar) AS avance_mm, CAST (i.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (i.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
CAST (i.piezas_por_golpe AS varchar) as piezas_por_golpe, CAST (i.piezas_por_paquete AS varchar) as piezas_por_paquete, 
CAST (i.peso_bruto_real_bascula AS varchar) as [peso_real_bruto], 
CAST (i.peso_neto_real_bascula AS varchar) as [peso_real_neto], 
CAST (i.piezas_por_auto AS varchar) AS piezas_por_auto, 
CAST (i.peso_inicial AS varchar) as peso_inicial, 
CAST (i.porcentaje_scrap_puntas_colas as varchar) as porcentaje_scrap_puntas_colas,
CAST (i.conciliacion_scrap_ingenieria AS varchar) as conciliacion_scrap_ingenieria, CAST (i.angulo_a as varchar) as angulo_a, cast(i.angulo_b as varchar) as angulo_b
,CASE
	WHEN i.tipo_venta = 'Reaplicación'
	THEN 1
	WHEN i.reaplicacion is null
		THEN '0'
	ELSE i.reaplicacion
END AS reaplicacion,
 d.budget_ejecucion_correcta, d.budget_mensaje_sap,
CAST(i.espesor_mm + i.espesor_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.espesor_mm + i.espesor_tolerancia_positiva_mm AS varchar) as espesor_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
	ELSE CAST(i.ancho_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
END AS ancho_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN cast (i.ancho_entrega_cinta_mm as varchar) 
	ELSE cast (i.ancho_mm as varchar)
END AS ancho_budget,
CAST(i.avance_mm + i.avance_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.avance_mm + i.avance_tolerancia_positiva_mm AS varchar) as avance_tolerancias,
CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta 
		THEN CAST (i.peso_max_kg As varchar)
	WHEN i.id_tipo_material in (3,4,5) AND i.peso_max_kg is null --platina, platina soldada, shearing
		THEN CAST (i.peso_neto AS varchar)
	WHEN i.id_tipo_material in (3,4,5) AND i.peso_max_kg is not null --platina, platina soldada, shearing
		THEN CAST (i.peso_max_kg AS varchar) 
	ELSE NULL
END  as peso_maximo_budget
, CAST (i.peso_maximo_tolerancia_negativa AS varchar) as peso_maximo_tolerancia_negativa
, CAST (i.peso_maximo_tolerancia_positiva AS varchar) as peso_maximo_tolerancia_positiva
, CASE
	WHEN i.peso_max_kg is not null AND NOT ((i.peso_maximo_tolerancia_negativa = 0 OR i.peso_maximo_tolerancia_negativa is null) AND (i.peso_maximo_tolerancia_positiva = 0 OR i.peso_maximo_tolerancia_positiva is null))
		THEN CAST(i.peso_max_kg + isnull(i.peso_maximo_tolerancia_negativa,0) AS varchar)+' - '+CAST(i.peso_max_kg + isnull(i.peso_maximo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN i.peso_max_kg is not null AND ((i.peso_maximo_tolerancia_negativa = 0 OR i.peso_maximo_tolerancia_negativa is null) AND (i.peso_maximo_tolerancia_positiva = 0 OR i.peso_maximo_tolerancia_positiva is null))
		THEN CAST(i.peso_max_kg AS varchar)
	ELSE NULL
END AS peso_maximo_tolerancias
, CAST (i.peso_minimo_tolerancia_negativa AS varchar) as peso_minimo_tolerancia_negativa
, CAST (i.peso_minimo_tolerancia_positiva AS varchar)as peso_minimo_tolerancia_positiva
, CASE
	WHEN i.peso_min_kg is not null AND NOT ((i.peso_minimo_tolerancia_negativa = 0 OR i.peso_minimo_tolerancia_negativa is null) AND (i.peso_minimo_tolerancia_positiva = 0 OR i.peso_minimo_tolerancia_positiva is null))
		THEN CAST(i.peso_min_kg + isnull(i.peso_minimo_tolerancia_negativa,0) AS varchar)+' - '+CAST(i.peso_min_kg + isnull(i.peso_minimo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN i.peso_min_kg is not null AND ((i.peso_minimo_tolerancia_negativa = 0 OR i.peso_minimo_tolerancia_negativa is null) AND (i.peso_minimo_tolerancia_positiva = 0 OR i.peso_minimo_tolerancia_positiva is null))
		THEN CAST(i.peso_min_kg AS varchar)
	ELSE NULL
END AS peso_minimo_tolerancias
, i.pieza_doble AS pieza_doble
, CAST (i.Almacen_Norte AS varchar) AS almacen_norte
, tt.clave AS tipo_transporte --> Aqui cambiar por clave
, CAST (i.Stacks_Pac AS varchar) AS stacks_paquete
, CAST (i.Type_of_Pallet AS varchar) As type_pallet
, CONVERT(varchar(7), i.Tkmm_SOP, 23) AS tkmm_sop    --estilo 23 = yyyy-MM-dd (sólo toma los primeros caracteres {yyyy-MM})
, CONVERT(varchar(7), i.Tkmm_EOP, 23) AS tkmm_eop 
from SCDM_solicitud_rel_item_material As i 
join SCDM_cat_tipo_materiales_solicitud As t On t.id = i.id_tipo_material
left join SCDM_solicitud_item_material_datos_sap as d on d.id_scdm_solicitud_rel_item_material = i.id
LEFT JOIN dbo.SCDM_cat_tipo_transporte AS tt ON tt.descripcion = i.Tipo_de_Transporte  -- coincide descripción → devuelve clave

UNION
-- cambios de budget
SELECT rc.tipo_material, rc.id, rc.id_solicitud, 1 as id_tipo_material,rc.tipo_metal as metal, rc.tipo_venta, 
rc.posicion_rollo, rc.IHS_num_1 as ihs_1, rc.IHS_num_2 as ihs_2, rc.IHS_num_3 as ihs_3, rc.IHS_num_4 as ihs_4, rc.IHS_num_5 as ihs_5,
rc.modelo_negocio, rc.material_existente as numero_material, '\' as espesor_mm, '\' as espesor_tolerancia_negativa_mm, '\' as espesor_tolerancia_positiva_mm,
'\' as ancho_mm, '\' as ancho_tolerancia_negativa_mm, '\' as ancho_tolerancia_positiva_mm, CAST (rc.peso_minimo as varchar) as peso_min_kg,
CAST(rc.conciliacion_puntas_colas as VARCHAR) AS requiere_consiliacion_puntas_colar, CAST (rc.scrap_permitido_puntas_colas AS varchar) as scrap_permitido_puntas_colas,
'\' as avance_mm, '\' as avance_tolerancia_negativa_mm, '\' as avance_tolerancia_positiva_mm, CAST (rc.piezas_por_golpe AS varchar) AS piezas_por_golpe, CAST (rc.piezas_por_paquete AS VARCHAR) AS piezas_por_paquete,
cast (rc.peso_bruto_real_bascula as varchar) as peso_real_bruto, cast (rc.peso_neto_real_bascula as varchar) as peso_real_neto, CAST (rc.piezas_por_auto AS VARCHAR) AS piezas_por_auto, CAST(rc.peso_inicial AS varchar) AS peso_inicial,
CAST (rc.scrap_permitido_puntas_colas AS varchar) as porcentaje_scrap_puntas_colas, CAST (rc.conciliacion_scrap_ingenieria AS VARCHAR) as conciliacion_scrap_ingenieria, 
CAST (rc.angulo_a AS VARCHAR) AS angulo_a, CAST (rc.angulo_b AS varchar) AS angulo_b, CAST(rc.reaplicacion AS VARCHAR) AS reaplicacion, rc.ejecucion_correcta as budget_ejecucion_correcta, rc.resultado as budget_mensaje_sap, 
'\' as espesor_tolerancias, '\' as ancho_tolerancias, '\' as ancho_budget, '\' as avance_tolerancias, 
CAST (peso_maximo AS varchar) as peso_maximo_budget, CAST(rc.peso_maximo_tolerancia_negativa AS varchar) AS peso_maximo_tolerancia_negativa, CAST (rc.peso_maximo_tolerancia_positiva AS varchar) AS peso_maximo_tolerancia_positiva,
CASE
	WHEN rc.peso_maximo is not null AND NOT ((rc.peso_maximo_tolerancia_negativa = 0 OR rc.peso_maximo_tolerancia_negativa is null) AND (rc.peso_maximo_tolerancia_positiva = 0 OR rc.peso_maximo_tolerancia_positiva is null))
		THEN CAST(rc.peso_maximo + isnull(rc.peso_maximo_tolerancia_negativa,0) AS varchar)+' - '+CAST(rc.peso_maximo + isnull(rc.peso_maximo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN rc.peso_maximo is not null AND ((rc.peso_maximo_tolerancia_negativa = 0 OR rc.peso_maximo_tolerancia_negativa is null) AND (rc.peso_maximo_tolerancia_positiva = 0 OR rc.peso_maximo_tolerancia_positiva is null))
		THEN CAST(rc.peso_maximo AS varchar)
	ELSE NULL
END AS peso_maximo_tolerancias,
CAST(rc.peso_minimo_tolerancia_negativa AS varchar) AS peso_minimo_tolerancia_negativa, CAST (rc.peso_minimo_tolerancia_positiva AS varchar) AS peso_minimo_tolerancia_positiva,
CASE
	WHEN rc.peso_minimo is not null AND NOT ((rc.peso_minimo_tolerancia_negativa = 0 OR rc.peso_minimo_tolerancia_negativa is null) AND (rc.peso_minimo_tolerancia_positiva = 0 OR rc.peso_minimo_tolerancia_positiva is null))
		THEN CAST(rc.peso_minimo + isnull(rc.peso_minimo_tolerancia_negativa,0) AS varchar)+' - '+CAST(rc.peso_minimo + isnull(rc.peso_minimo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN rc.peso_minimo is not null AND ((rc.peso_minimo_tolerancia_negativa = 0 OR rc.peso_minimo_tolerancia_negativa is null) AND (rc.peso_minimo_tolerancia_positiva = 0 OR rc.peso_minimo_tolerancia_positiva is null))
		THEN CAST(rc.peso_minimo AS varchar)
	ELSE NULL
END AS peso_minimo_tolerancias,
CAST(rc.pieza_doble AS varchar) AS pieza_doble
, CAST (rc.Almacen_Norte AS varchar) AS almacen_norte
, tt2.clave AS tipo_transporte --> Aqui cambiar por clave
, CAST (rc.Stacks_Pac AS varchar) AS stacks_paquete
, CAST (rc.Type_of_Pallet AS varchar) As type_pallet
, CONVERT(varchar(7), rc.Tkmm_SOP, 23) AS tkmm_sop    --estilo 23 = yyyy-MM-dd (sólo toma los primeros caracteres {yyyy-MM})
, CONVERT(varchar(7), rc.Tkmm_EOP, 23) AS tkmm_eop 
from SCDM_solicitud_rel_cambio_budget as rc
LEFT JOIN dbo.SCDM_cat_tipo_transporte AS tt2 ON tt2.descripcion = rc.Tipo_de_Transporte

UNION
--Creación con referencia
SELECT cr.tipo_material_text as tipo_material, cr.id, cr.id_solicitud, 1 As id_tipo_material,
tipo_metal as metal, tv.descripcion as tipo_venta, 
cr.posicion_rollo as posicion_rollo, 
cr.IHS_num_1 as ihs_1, 
cr.IHS_num_2 as ihs_2, 
cr.IHS_num_3 as ihs_3, 
cr.IHS_num_4 as ihs_4, 
cr.IHS_num_5 as ihs_5,
cr.modelo_negocio as modelo_negocio, 
nuevo_material as numero_material, CAST (cr.espesor_mm AS varchar) AS espesor_mm, CAST (cr.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (cr.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (cr.ancho_mm AS varchar) AS ancho_mm,
CAST (cr.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (cr.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
CAST (cr.peso_minimo AS varchar) as peso_min_kg, 
CAST (cr.conciliacion_puntas_colas AS VARCHAR) AS requiera_consiliacion_puntas_colar, 
CAST (cr.scrap_permitido_puntas_colas AS VARCHAR) scrap_permitido_puntas_colas, 
CAST (cr.avance_mm AS varchar) AS avance_mm, CAST (cr.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (cr.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
CAST (cr.piezas_por_golpe AS varchar) as piezas_por_golpe, 
CAST (cr.piezas_por_paquete AS varchar) as piezas_por_paquete, 
CAST (cr.peso_bruto_real_bascula as varchar) as peso_real_bruto, 
CAST (cr.peso_neto_real_bascula as varchar) as peso_real_neto,
CAST (cr.piezas_por_auto AS varchar) as  piezas_por_auto, 
CAST (cr.peso_inicial AS varchar) as peso_inicial, 
CAST (cr.scrap_permitido_puntas_colas as varchar) as porcentaje_scrap_puntas_colas,
CAST (cr.conciliacion_scrap_ingenieria AS varchar) coinciliacion_scrap_ingenieria, 
CAST (cr.angulo_a as varchar) as angulo_a, 
CAST (cr.angulo_b as varchar) as angulo_b, 
CASE
	WHEN tv.id = 3 --Reaplicación
		THEN '1'
	WHEN cr.reaplicacion is null
		THEN '0'
	ELSE cr.reaplicacion
END AS reaplicacion,
cr.ejecucion_correcta_budget as budget_ejecucion_correcta, cr.resultado_budget as budget_mensaje_sap,
CASE
	WHEN cr.espesor_mm is not null AND NOT ((cr.espesor_tolerancia_negativa_mm = 0 OR cr.espesor_tolerancia_negativa_mm is null) AND (cr.espesor_tolerancia_positiva_mm = 0 OR cr.espesor_tolerancia_positiva_mm is null))
		THEN CAST(cr.espesor_mm + isnull(cr.espesor_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.espesor_mm  + isnull(cr.espesor_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.espesor_mm is not null AND ((cr.espesor_tolerancia_negativa_mm = 0 OR cr.espesor_tolerancia_negativa_mm is null) AND (cr.espesor_tolerancia_positiva_mm = 0 OR cr.espesor_tolerancia_positiva_mm is null))
		THEN CAST(cr.espesor_mm AS varchar)
	ELSE NULL
END AS espesor_tolerancias,
CASE
	WHEN cr.ancho_mm is not null AND NOT ((cr.ancho_tolerancia_negativa_mm = 0 OR cr.ancho_tolerancia_negativa_mm is null) AND (cr.ancho_tolerancia_positiva_mm = 0 OR cr.ancho_tolerancia_positiva_mm is null))
		THEN CAST(cr.ancho_mm + isnull(cr.ancho_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.ancho_mm  + isnull(cr.ancho_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.ancho_mm is not null AND ((cr.ancho_tolerancia_negativa_mm = 0 OR cr.ancho_tolerancia_negativa_mm is null) AND (cr.ancho_tolerancia_positiva_mm = 0 OR cr.ancho_tolerancia_positiva_mm is null))
		THEN CAST(cr.ancho_mm AS varchar)
	ELSE NULL
END AS ancho_tolerancias,
CAST(cr.ancho_mm AS varchar) as ancho_budget,
CASE
	WHEN cr.avance_mm is not null AND NOT ((cr.avance_tolerancia_negativa_mm = 0 OR cr.avance_tolerancia_negativa_mm is null) AND (cr.avance_tolerancia_positiva_mm = 0 OR cr.avance_tolerancia_positiva_mm is null))
		THEN CAST(cr.avance_mm + isnull(cr.avance_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.avance_mm  + isnull(cr.avance_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.avance_mm is not null AND ((cr.avance_tolerancia_negativa_mm = 0 OR cr.avance_tolerancia_negativa_mm is null) AND (cr.avance_tolerancia_positiva_mm = 0 OR cr.avance_tolerancia_positiva_mm is null))
		THEN CAST(cr.avance_mm AS varchar)
	ELSE NULL
END AS avance_tolerancias,
CAST (cr.peso_maximo AS varchar) as peso_maximo_budget, 
CAST (cr.peso_maximo_tolerancia_negativa AS varchar) as peso_maximo_tolerancia_negativa, 
CAST (cr.peso_maximo_tolerancia_positiva AS varchar) as peso_maximo_tolerancia_positiva,
CASE
	WHEN cr.peso_maximo is not null AND NOT ((cr.peso_maximo_tolerancia_negativa = 0 OR cr.peso_maximo_tolerancia_negativa is null) AND (cr.peso_maximo_tolerancia_positiva = 0 OR cr.peso_maximo_tolerancia_positiva is null))
		THEN CAST(cr.peso_maximo + isnull(cr.peso_maximo_tolerancia_negativa,0) AS varchar)+' - '+CAST(cr.peso_maximo + isnull(cr.peso_maximo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN cr.peso_maximo is not null AND ((cr.peso_maximo_tolerancia_negativa = 0 OR cr.peso_maximo_tolerancia_negativa is null) AND (cr.peso_maximo_tolerancia_positiva = 0 OR cr.peso_maximo_tolerancia_positiva is null))
		THEN CAST(cr.peso_maximo AS varchar)
	ELSE NULL
END AS peso_maximo_tolerancias, 
CAST (cr.peso_minimo_tolerancia_negativa AS varchar) as peso_minimo_tolerancia_negativa,
CAST (cr.peso_minimo_tolerancia_positiva AS varchar) as peso_minimo_tolerancia_positiva, 
CASE
	WHEN cr.peso_minimo is not null AND NOT ((cr.peso_minimo_tolerancia_negativa = 0 OR cr.peso_minimo_tolerancia_negativa is null) AND (cr.peso_minimo_tolerancia_positiva = 0 OR cr.peso_minimo_tolerancia_positiva is null))
		THEN CAST(cr.peso_minimo + isnull(cr.peso_minimo_tolerancia_negativa,0) AS varchar)+' - '+CAST(cr.peso_minimo + isnull(cr.peso_minimo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN cr.peso_minimo is not null AND ((cr.peso_minimo_tolerancia_negativa = 0 OR cr.peso_minimo_tolerancia_negativa is null) AND (cr.peso_minimo_tolerancia_positiva = 0 OR cr.peso_minimo_tolerancia_positiva is null))
		THEN CAST(cr.peso_minimo AS varchar)
	ELSE NULL
END AS peso_minimo_tolerancias, 
cr.pieza_doble as pieza_doble
, CAST (cr.Almacen_Norte AS varchar) AS almacen_norte
, tt3.clave AS tipo_transporte  --> Aqui cambiar por clave
, CAST (cr.Stacks_Pac AS varchar) AS stacks_paquete
, CAST (cr.Type_of_Pallet AS varchar) As type_pallet
, CONVERT(varchar(7), cr.Tkmm_SOP, 23) AS tkmm_sop    --estilo 23 = yyyy-MM-dd (sólo toma los primeros caracteres {yyyy-MM})
, CONVERT(varchar(7), cr.Tkmm_EOP, 23) AS tkmm_eop 

FROM SCDM_solicitud_rel_creacion_referencia as cr
join SCDM_cat_tipo_venta as tv on tv.id = cr.id_tipo_venta
LEFT JOIN dbo.SCDM_cat_tipo_transporte AS tt3
         ON tt3.descripcion = cr.Tipo_de_Transporte

UNION
--Cambio de Ingenieria
SELECT ci.tipo_material_text as tipo_material, ci.id, ci.id_solicitud, 1 as id_tipo_material, ci.tipo_metal as metal,
ci.tipo_venta, '\' posicion_rollo,'\' ihs_1, '\' ihs_2, '\' ihs_3, '\' ihs_4, '\' ihs_5,
'\' modelo_negocio, material_existente as numero_material,
CAST (ci.espesor_mm AS varchar) AS espesor_mm, CAST (ci.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (ci.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (ci.ancho_mm AS varchar) AS ancho_mm,
CAST (ci.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (ci.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
'\' as peso_min_kg, '\' requiera_consiliacion_puntas_colar, '\' scrap_permitido_puntas_colas, 
CAST (ci.avance_mm AS varchar) AS avance_mm, CAST (ci.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (ci.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
'\' piezas_por_golpe, '\' piezas_por_paquete, '\' peso_real_bruto, '\' peso_real_neto, '\' piezas_por_auto, '\' peso_inicial, '\' porcentaje_scrap_puntas_colas,
'\' coinciliacion_scrap_ingenieria, '\' angulo_a, '\' angulo_b,
CASE
	WHEN ci.tipo_venta = 'Reaplicación' --Reaplicación
	THEN '1'
	ELSE '0'
END AS reaplicacion,
ci.ejecucion_correcta_budget as budget_ejecucion_correcta, ci.resultado_budget as budget_mensaje_sap,
CASE
	WHEN ci.espesor_mm is not null AND NOT ((ci.espesor_tolerancia_negativa_mm = 0 OR ci.espesor_tolerancia_negativa_mm is null) AND (ci.espesor_tolerancia_positiva_mm = 0 OR ci.espesor_tolerancia_positiva_mm is null))
		THEN CAST(ci.espesor_mm + isnull(ci.espesor_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.espesor_mm  + isnull(ci.espesor_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.espesor_mm is not null AND ((ci.espesor_tolerancia_negativa_mm = 0 OR ci.espesor_tolerancia_negativa_mm is null) AND (ci.espesor_tolerancia_positiva_mm = 0 OR ci.espesor_tolerancia_positiva_mm is null))
		THEN CAST(ci.espesor_mm AS varchar)
	ELSE NULL
END AS espesor_tolerancias,
CASE
	WHEN ci.ancho_mm is not null AND NOT ((ci.ancho_tolerancia_negativa_mm = 0 OR ci.ancho_tolerancia_negativa_mm is null) AND (ci.ancho_tolerancia_positiva_mm = 0 OR ci.ancho_tolerancia_positiva_mm is null))
		THEN CAST(ci.ancho_mm + isnull(ci.ancho_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.ancho_mm  + isnull(ci.ancho_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.ancho_mm is not null AND ((ci.ancho_tolerancia_negativa_mm = 0 OR ci.ancho_tolerancia_negativa_mm is null) AND (ci.ancho_tolerancia_positiva_mm = 0 OR ci.ancho_tolerancia_positiva_mm is null))
		THEN CAST(ci.ancho_mm AS varchar)
	ELSE NULL
END AS ancho_tolerancias,
CAST(ci.ancho_mm AS varchar) as ancho_budget,
CASE
	WHEN ci.avance_mm is not null AND NOT ((ci.avance_tolerancia_negativa_mm = 0 OR ci.avance_tolerancia_negativa_mm is null) AND (ci.avance_tolerancia_positiva_mm = 0 OR ci.avance_tolerancia_positiva_mm is null))
		THEN CAST(ci.avance_mm + isnull(ci.avance_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.avance_mm  + isnull(ci.avance_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.avance_mm is not null AND ((ci.avance_tolerancia_negativa_mm = 0 OR ci.avance_tolerancia_negativa_mm is null) AND (ci.avance_tolerancia_positiva_mm = 0 OR ci.avance_tolerancia_positiva_mm is null))
		THEN CAST(ci.avance_mm AS varchar)
	ELSE NULL
END AS avance_tolerancias,
'\' peso_maximo_budget, '\' peso_maximo_tolerancia_negativa, '\' peso_maximo_tolerancia_positiva, '\' peso_maximo_tolerancias, '\' peso_minimo_tolerancia_negativa,
'\' peso_minimo_tolerancia_positiva, '\' peso_minimo_tolerancias, '\' pieza_doble
, '\' AS almacen_norte
, '\' AS tipo_transporte
, '\' AS stacks_paquete
, '\' As type_pallet
, '\' AS tkmm_sop
, '\' AS tkmm_eop
from SCDM_solicitud_rel_cambio_ingenieria as ci
GO


/*******************************************************************/
/****** Object:  View [dbo].[view_SCDM_solicitud_rel_item_material]    Script Date: 04/06/2025 06:02:11 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--select * from view_SCDM_solicitud_rel_item_material
CREATE OR ALTER               VIEW [dbo].[view_SCDM_solicitud_rel_item_material] AS

select t.descripcion As tipo_material, i.*, 
(select top 1 codigoSap from plantas where clave = (select top 1 id_planta from SCDM_rel_solicitud_plantas where id_solicitud=i.id_solicitud)) as planta_sap,
CASE
	WHEN i.id_tipo_material =1 --Rollo
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_mm AS varchar) 
	WHEN i.id_tipo_material = 2 --Cinta
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_entrega_cinta_mm AS varchar) 
	WHEN i.id_tipo_material = 3 OR i.id_tipo_material = 4  OR i.id_tipo_material = 5 --Platina, Shearing, PlatinaSoldada
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_mm AS varchar)+'X'+CAST(i.avance_mm AS varchar) 
	ELSE null
END  as dimensiones,
CASE
	WHEN i.id_tipo_material =1 --Rollo
		THEN CAST(i.espesor_mm AS varchar) + '(+'+CAST( espesor_tolerancia_positiva_mm as varchar)+'/'+
			CASE 
				WHEN espesor_tolerancia_negativa_mm = 0
				THEN '-'
				else ''
			END
		+CAST( espesor_tolerancia_negativa_mm as varchar)+') X ' +
		CAST(i.ancho_mm AS varchar) + '(+'+CAST( ancho_tolerancia_positiva_mm as varchar)+'/'+
			CASE 
				WHEN ancho_tolerancia_negativa_mm = 0
				THEN '-'
				else ''
			END
		+CAST( ancho_tolerancia_negativa_mm as varchar)+')'
	ELSE null
END  as dimensiones_tolerancia,
CASE
	WHEN clase_aprovisionamiento = 'Acopio Externo'
		THEN 'F'
	ELSE 'E'
END  as procurement_type,
(select 
top 1 material_sap from SCDM_cat_material_referencia where clave = 
'R-'+
			CASE
				WHEN clase_aprovisionamiento = 'Acopio Externo'
					THEN 'E'
				ELSE 'I'
			END +'-'+ SUBSTRING(i.tipo_recubrimiento,3,2)+'-'+i.unidad_medida_inventario+'-'+
			CASE
				WHEN id_tipo_material = 4 OR id_tipo_material = 5
					THEN 'Platina'
				ELSE t.descripcion
			END
 ) as material_referencia,
 d.*,
 CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1.005'
	ELSE CAST(i.peso_bruto AS varchar)
END  as peso_bruto_class,
 CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1.000'
	ELSE CAST(i.peso_neto AS varchar)
END  as peso_neto_class,
CASE
	WHEN SUBSTRING(i.tipo_recubrimiento,3,2) ='CM'
		THEN NULL
	ELSE SUBSTRING(i.tipo_recubrimiento,3,2)
END as commodity_class,
CASE 
	WHEN i.espesor_tolerancia_negativa_mm = i.espesor_tolerancia_positiva_mm
		THEN CAST(i.espesor_mm AS varchar)
	ELSE CAST(i.espesor_mm + i.espesor_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.espesor_mm + i.espesor_tolerancia_positiva_mm AS varchar)
END  as espesor_tolerancias,
CASE
	WHEN i.ancho_tolerancia_negativa_mm = ancho_tolerancia_positiva_mm AND i.id_tipo_material = 2 --cinta
		THEN CAST(i.ancho_entrega_cinta_mm AS varchar)
	WHEN i.ancho_tolerancia_negativa_mm = ancho_tolerancia_positiva_mm AND i.id_tipo_material <> 2 -- diferente de cinta
		THEN CAST(i.ancho_mm AS varchar)
	WHEN i.id_tipo_material = 2 --cinta
		THEN CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
	ELSE CAST(i.ancho_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
END AS ancho_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN i.ancho_entrega_cinta_mm
	ELSE i.ancho_mm
END AS ancho_budget,

CASE 
	WHEN i.avance_tolerancia_negativa_mm = i.avance_tolerancia_positiva_mm
		THEN CAST(i.avance_mm AS varchar)
	ELSE CAST(i.avance_mm + i.avance_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.avance_mm + i.avance_tolerancia_positiva_mm AS varchar)
END  as avance_tolerancias,
CASE
	WHEN i.planicidad_mm is NOT null
		THEN i.planicidad_mm
	--espesor >=0 && espesor <.7
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 4	
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 4.5
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1500
		THEN 4.5
		--espesor >=.7 && espesor <1.2
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 3
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 4
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1500
		THEN 5
		--espesor >=1.2 
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 2.5
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 3
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1500
		THEN 4.5
	ELSE null
END as planicidad_class,
CASE
	WHEN i.parte_interior_exterior = 'Exterior'
		THEN 1
	ELSE 3
END as superficie_class,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad ) >=1
		THEN cast((select top 1 clave from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad) AS varchar)	
	ELSE i.grado_calidad
END AS clave_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad) >=1
		THEN 1
	ELSE 0
END AS existe_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) 
	ELSE i.peso_recubrimiento
END AS clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) >=1
		THEN 1
	ELSE 0
END AS existe_clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_molino where i.molino = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_molino where i.molino = descripcion) 
	ELSE i.molino
END AS clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_molino where i.molino = descripcion) >=1
		THEN 1
	ELSE 0
END AS existe_clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where i.forma = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_forma_material where i.forma = descripcion) 
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1'
	ELSE CAST (i.forma as varchar)
END AS clave_forma,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where i.forma = descripcion) >=1 OR i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN 1
	ELSE 0
END AS existe_clave_forma,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = i.cliente ) >=1
		THEN cast((select top 1 claveSAP from clientes where '('+claveSAP+') - '+descripcion = i.cliente) AS varchar)	
	ELSE i.cliente
END AS clave_cliente,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = i.cliente ) >=1
		THEN 1
	ELSE 0
END AS existe_clave_cliente,
CASE
	WHEN (select count(*) from proveedores where '('+claveSAP+') - '+descripcion = i.proveedor ) >=1
		THEN cast((select top 1 claveSAP from proveedores where '('+claveSAP+') - '+descripcion = i.proveedor) AS varchar)	
	ELSE i.proveedor
END AS clave_proveedor,
CASE
	WHEN (select count(*) from proveedores where '('+claveSAP+') - '+descripcion = i.proveedor ) >=1
		THEN 1
	ELSE 0
END AS existe_clave_proveedor,
CASE
	WHEN (select count(*) from SCDM_cat_incoterm where '('+codigo+') '+descripcion = i.incoterm ) >=1
		THEN cast((select top 1 codigo from SCDM_cat_incoterm where '('+codigo+') '+descripcion = i.incoterm) AS varchar)	
	ELSE i.incoterm
END AS clave_incoterm,
CASE
	WHEN (select count(*) from SCDM_cat_incoterm where '('+codigo+') '+descripcion = i.incoterm ) >=1
		THEN 1
	ELSE 0
END AS existe_clave_incoterm,
CASE
	WHEN (select count(*) from SCDM_cat_terminos_pago where clave + ' - '+descripcion = i.terminos_pago ) >=1
		THEN cast((select top 1 clave from SCDM_cat_terminos_pago where clave + ' - '+descripcion = i.terminos_pago) AS varchar)	
	ELSE i.incoterm
END AS clave_termino_pago,
CASE
	WHEN (select count(*) from SCDM_cat_terminos_pago where clave + ' - '+descripcion = i.terminos_pago ) >=1
		THEN 1
	ELSE 0
END AS existe_clave_termino_pago,


CASE
	WHEN i.id_tipo_material = 1 --Rollo 
		THEN i.diametro_interior
	WHEN i.id_tipo_material = 2 --Cinta
		THEN i.diametro_interior_salida
	ELSE NULL
END  as diametro_interior_class,
CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta 
		THEN i.peso_max_kg
	WHEN i.id_tipo_material in (3,4,5) --platina, platina soldada, shearing
		THEN i.peso_neto
	ELSE NULL
END  as peso_maximo_budget

from SCDM_solicitud_rel_item_material As i 
join SCDM_cat_tipo_materiales_solicitud As t On t.id = i.id_tipo_material
left join SCDM_solicitud_item_material_datos_sap as d on d.id_scdm_solicitud_rel_item_material = i.id

GO


/****************************************************************************/
/****** Object:  View [dbo].[view_SCDM_solicitud_tiempos]    Script Date: 04/06/2025 06:03:01 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   VIEW [dbo].[view_SCDM_solicitud_tiempos] AS

select 
id_solicitud, pr.descripcion as prioridad, dep.descripcion as departamento, (e.nombre+' '+e.apellido1+' '+e.apellido2) as nombre_cierre,
pl.descripcion as planta, fecha_asignacion, fecha_cierre,  dbo.WorkTime (sa.fecha_asignacion, sa.fecha_cierre) as minutos, ROUND(CAST(dbo.WorkTime(sa.fecha_asignacion, sa.fecha_cierre) AS float)/60, 2) as horas
from SCDM_solicitud_asignaciones sa
join SCDM_cat_departamentos_asignacion as dep on sa.id_departamento_asignacion = dep.id
join empleados as e on e.id = sa.id_cierre
join SCDM_solicitud as s on s.id = sa.id_solicitud
join plantas as pl on pl.clave = s.planta_solicitud
join SCDM_cat_prioridad as pr on pr.id = s.id_prioridad
WHERE sa.descripcion <> 'Asignación inicial'

GO

/*****************************************************************************************************/
/****** Object:  View [dbo].[view_SCDM_solicitudes]    Script Date: 04/06/2025 06:03:52 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--select * from SCDM_solicitud_asignaciones

CREATE OR ALTER               VIEW [dbo].[view_SCDM_solicitudes] AS
SELECT TOP(5000)
    s.id,
    t.descripcion AS [tipo_solicitud],
    tc.descripcion AS [tipo_cambio],
    pr.descripcion AS [prioridad],
    -- Concatenar todas las plantas separadas por coma:
    STUFF((
      SELECT ',' + plt2.codigoSap
      FROM SCDM_rel_solicitud_plantas rp2
      INNER JOIN plantas plt2 ON plt2.clave = rp2.id_planta
      WHERE rp2.id_solicitud = s.id
      FOR XML PATH(''), TYPE
    ).value('.', 'varchar(max)'), 1, 1, '') AS [planta_sap],
    STUFF((
      SELECT ',' + plt2.descripcion
      FROM SCDM_rel_solicitud_plantas rp2
      INNER JOIN plantas plt2 ON plt2.clave = rp2.id_planta
      WHERE rp2.id_solicitud = s.id
      FOR XML PATH(''), TYPE
    ).value('.', 'varchar(max)'), 1, 1, '') AS [planta],   
    (e.nombre+' '+e.apellido1+' '+ISNULL(e.apellido2,'')) AS [solicitante],
	 -- Concatenar todos los departamentos del solicitante
    STUFF((
      SELECT ',' + da2.descripcion
      FROM SCDM_cat_rel_usuarios_departamentos ud2
      INNER JOIN SCDM_cat_departamentos_asignacion da2 ON da2.id = ud2.id_departamento
      WHERE ud2.id_empleado = s.id_solicitante
      FOR XML PATH(''), TYPE
    ).value('.', 'varchar(max)'), 1, 1, '') AS [departamento],
    s.descripcion,
    s.justificacion,
    s.fecha_creacion,
    s.on_hold,
    s.activo,
    CASE
        WHEN (SELECT COUNT(*) 
              FROM SCDM_solicitud_asignaciones 
              WHERE id_solicitud = s.id) = 0  
            THEN 'Creada'
        WHEN (SELECT COUNT(*) 
              FROM SCDM_solicitud_asignaciones 
              WHERE id_solicitud = s.id 
                AND id_departamento_asignacion = 9 
                AND fecha_cierre IS NULL 
                AND fecha_rechazo IS NULL) > 0  
            THEN 'Asignada a SCDM'
        WHEN (SELECT COUNT(*) 
              FROM SCDM_solicitud_asignaciones 
              WHERE id_solicitud = s.id 
                AND id_departamento_asignacion <> 9 
                AND fecha_cierre IS NULL 
                AND fecha_rechazo IS NULL) > 0  
            THEN 'Asignada a Departamentos'
        WHEN (SELECT COUNT(*) 
              FROM SCDM_solicitud_asignaciones 
              WHERE id_solicitud = s.id 
                AND fecha_cierre IS NULL 
                AND fecha_rechazo IS NULL) = 0
             AND (SELECT COUNT(*) 
                  FROM SCDM_solicitud_asignaciones 
                  WHERE id_solicitud = s.id) > 0  
            THEN 'Finalizada'
        ELSE 'En proceso'
    END AS estatus
FROM SCDM_solicitud AS s
LEFT JOIN SCDM_cat_tipo_solicitud AS t
  ON s.id_tipo_solicitud = t.id
LEFT JOIN SCDM_cat_tipo_cambio AS tc
  ON tc.id = s.id_tipo_cambio
LEFT JOIN SCDM_cat_prioridad AS pr
  ON pr.id = s.id_prioridad
LEFT JOIN empleados AS e
  ON e.id = s.id_solicitante
ORDER BY s.id DESC;

GO




/*********************************************************************************************/
/****** Object:  View [dbo].[view_SCDM_solicitudes_lista_tecnica]    Script Date: 04/06/2025 06:04:35 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER     VIEW [dbo].[view_SCDM_solicitudes_lista_tecnica] AS

select lt.id_solicitud, lt.resultado, imr.tipo_venta, tmr.descripcion as [tipo_material_resultado], imr.peso_bruto, imr.peso_neto, imr.unidad_medida_inventario,
lt.sobrante, lt.componente, tmc.descripcion as [tipo_material_componente], lt.cantidad_platinas, cantidad_cintas, lt.fecha_validez_reaplicacion
 from SCDM_solicitud_rel_lista_tecnica as lt
left join SCDM_solicitud_rel_item_material as imr on imr.id = (select top 1 id from SCDM_solicitud_rel_item_material where numero_material = lt.resultado AND id_solicitud=lt.id_solicitud)
left join SCDM_solicitud_rel_item_material as imc on imc.id = (select top 1 id from SCDM_solicitud_rel_item_material where numero_material = lt.componente AND id_solicitud=lt.id_solicitud)
left join SCDM_cat_tipo_materiales_solicitud as tmr on tmr.id = imr.id_tipo_material
left join SCDM_cat_tipo_materiales_solicitud as tmc on tmc.id = imc.id_tipo_material
GO

/**************************************************************************/
/****** Object:  View [dbo].[view_SCDM_materiales_extension_almacenes]    Script Date: 04/06/2025 06:05:19 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--select * from view_SCDM_materiales_extension_almacenes
CREATE OR ALTER             VIEW [dbo].[view_SCDM_materiales_extension_almacenes] AS

SELECT
  im.id_solicitud,
  im.numero_material,
  im.planta_sap,
  al.warehouse,
  al.storage_type,
  --  ubicacion
  (SELECT TOP 1 ubicacion
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as ubicacion,
  --ejecucion_correcta
   (SELECT TOP 1 almacen_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as almacen_ejecucion_correcta,
  --mensaje_sap
   (SELECT TOP 1 almacen_mensaje_sap
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as almacen_mensaje_sap,
  'CreacionMateriales' AS tipo_fuente
FROM SCDM_cat_almacenes AS al
JOIN plantas AS p
  ON p.clave = al.id_planta
CROSS JOIN view_SCDM_solicitud_rel_item_material AS im
WHERE im.planta_sap = p.codigoSap and al.es_virtual = 0

UNION

SELECT
  cr.id_solicitud,
  cr.nuevo_material,
  cr.codigoSap,
  al.warehouse,
  al.storage_type,
  --  ubicacion
  (SELECT TOP 1 ubicacion
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as ubicacion,
  --ejecucion_correcta
   (SELECT TOP 1 almacen_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as almacen_ejecucion_correcta,
  --mensaje_sap
   (SELECT TOP 1 almacen_mensaje_sap
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as almacen_mensaje_sap,
'CreacionReferencia' AS tipo_fuente
FROM SCDM_cat_almacenes AS al
JOIN plantas AS p
  ON p.clave = al.id_planta
CROSS JOIN view_SCDM_solicitud_creacion_referencia AS cr
WHERE cr.codigoSap = p.codigoSap and al.es_virtual = 0

UNION

SELECT
  eu.id_solicitud,
  eu.material,
  p.codigoSap,
  al.warehouse,
  al.storage_type,
  --  ubicacion
  (SELECT TOP 1 ubicacion
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_rel_solicitud_extension_usuario = eu.id
  AND al.id = id_cat_almacenes
  )as ubicacion,
  --ejecucion_correcta
   (SELECT TOP 1 almacen_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_rel_solicitud_extension_usuario = eu.id
  AND al.id = id_cat_almacenes
  )as almacen_ejecucion_correcta,
  --mensaje_sap
   (SELECT TOP 1 almacen_mensaje_sap
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_rel_solicitud_extension_usuario = eu.id
  AND al.id = id_cat_almacenes
  )as almacen_mensaje_sap,
'ExtensionUsuario' AS tipo_fuente
FROM SCDM_cat_almacenes AS al
JOIN plantas AS p ON p.clave = al.id_planta
CROSS JOIN SCDM_solicitud_rel_extension_usuario AS eu
WHERE ((eu.planta_destino = p.codigoSap and al.es_virtual = 0) or (al.es_virtual = 1 AND (SELECT count (*) from SCDM_rel_solicitud_extension_almacenes_virtuales as av where (av.id_solicitud = eu.id_solicitud AND almacen_virtual = al.warehouse))>0))


/**************************************************************************************************/
