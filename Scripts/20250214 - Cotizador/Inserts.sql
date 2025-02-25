--====== INSERT CTZ_Clients  =====

/* copia de la BD de SCDM */
--select * from clientes
--select * from CTZ_Clients
--DELETE FROM dbo.CTZ_Clients;
--DBCC CHECKIDENT ('dbo.CTZ_Clients', RESEED, 0);

INSERT INTO dbo.CTZ_Clients (Client_Name, Clave_SAP, Pais, Direccion, Ciudad, Codigo_Postal, Calle, Estado, Automotriz, Active)
SELECT    
    descripcion,                -- Mapeo de descripcion a Client_Name
    claveSAP,                   -- Mapeo de claveSAP a Clave_SAP
    pais,                       -- Mapeo de pais a Pais
    direccion,                  -- Mapeo de direccion a Direccion
    ciudad,                     -- Mapeo de ciudad a Ciudad
    codigo_postal,              -- Mapeo de codigo_postal a Codigo_Postal
    calle,                      -- Mapeo de calle a Calle
    estado,                     -- Mapeo de estado a Estado
    automotriz,                 -- Mapeo de automotriz a Automotriz
    activo                      -- Mapeo de activo a Active
FROM dbo.clientes
WHERE CAST(claveSAP AS INT) IN (
    1956, 1997, 1280, 2410, 2200, 1728, 2125, 954, 946, 1561,
	2400, 2421, 2370, 2365, 2420, 2395, 2195, 1200, 1494, 1123,
	1293, 2021, 989, 2010, 2209, 1408, 2415, 1374, 2063, 1413,
	1953, 980, 2356, 2416, 2407, 2397, 2312, 1339, 2413, 2284,
	2350, 2342, 1546, 2343, 2221, 2274, 2386, 2407, 2313, 2328,
	2189, 1430, 1189, 1472, 2406, 2129, 2027, 2028, 2345, 1959, 
	1744, 1970, 1296, 2325, 1801, 1283, 1595, 2055, 2026, 1421, 
	2330, 2266, 2265, 1522, 945, 2327
);


--====== INSERT OEMClients  =====

--DELETE FROM dbo.CTZ_OEMClients;  -- Elimina todos los registros
--DBCC CHECKIDENT ('dbo.CTZ_OEMClients', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('BMW', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('BYD', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Chery', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('COMPAS', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Ford', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Geely', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('General Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Giant Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Honda', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Hyundai', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Lucid Motors', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mazda', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mazda Toyota', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Mercedes-Benz', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Multimatic', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Navistar', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Nissan', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Oshkosh', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Otros', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Rivian', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('SIA', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Stellantis', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Tesla', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Toyota', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Volkswagen', 1,1)
INSERT INTO [dbo].[CTZ_OEMClients]([Client_Name],[Active],[Automotriz])VALUES('Zoox', 1,1)

--select * from [CTZ_OEMClients]

--====== INSERT [CTZ_plants]  =====
--DELETE FROM dbo.CTZ_plants;  -- Elimina todos los registros
--DBCC CHECKIDENT ('dbo.CTZ_plants', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Puebla', '5190',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Silao', '5390',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('Saltillo', '5490',1)
INSERT INTO [dbo].[CTZ_plants]([Description],[Codigo_SAP],[Active])VALUES('San Luis Potosí', '5890',1)

--select * from CTZ_plants

--====== INSERT [CTZ_Project_Status] =====

INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('25','Quotes')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('50','Carry Over')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('75','Casi Casi')
INSERT INTO [dbo].[CTZ_Project_Status]([Status_Percent],[Description])VALUES('100','POH')

--select * from [CTZ_Project_Status]

--====== INSERT [CTZ_Material_Owner] =====

INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('OWN', 'Propiedad tkMM',1)
INSERT INTO [dbo].[CTZ_Material_Owner]([Owner_Key],[Description],[Active])VALUES('CM', 'Propiedad del Cliente',1)

--Select * from CTZ_Material_Owner

--======= INSERT CTZ_Temp_IHS =========

DELETE FROM dbo.CTZ_Temp_IHS;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_Temp_IHS] 
    (Vehicle, Program, SOP, EOP, Max_Production, Mnemonic_Vehicle_plant, Production_Plant)
SELECT 
    i.vehicle,
    i.program,
    CAST(i.sop_start_of_production AS DATE) AS SOP,
    CAST(i.eop_end_of_production AS DATE) AS EOP,
    d.MaxProduction,
    i.mnemonic_vehicle_plant,
    i.production_plant
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item] i
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones] v 
    ON i.id_ihs_version = v.id
CROSS APPLY (
    SELECT TOP 1 
           SUM(r.cantidad) AS MaxProduction
    FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda] r
    WHERE r.id_ihs_item = i.id
      AND r.tipo = 'ORIGINAL'
    GROUP BY YEAR(r.fecha)
    ORDER BY SUM(r.cantidad) DESC
) d
WHERE 
    v.periodo = (SELECT MAX(periodo) FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones])
    AND i.origen = 'IHS';


	select * from CTZ_Temp_IHS

--======== CTZ CTZ_Temp_IHS_Production =========

DELETE FROM dbo.CTZ_Temp_IHS_Production;  -- Elimina todos los registros
DBCC CHECKIDENT ('dbo.CTZ_Temp_IHS_Production', RESEED, 0); -- Reinicia el índice IDENTITY a 1

INSERT INTO [dbo].[CTZ_Temp_IHS_Production] (ID_IHS, Production_Year, Production_Sum)
SELECT 
    t.ID_IHS,
    YEAR(r.fecha) AS Production_Year,
    SUM(r.cantidad) AS Production_Sum
FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_rel_demanda] r
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_item] i 
    ON r.id_ihs_item = i.id
INNER JOIN [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones] v
    ON i.id_ihs_version = v.id
INNER JOIN [dbo].[CTZ_Temp_IHS] t
    ON i.vehicle = t.Vehicle  -- Se asume que 'Vehicle' es único en CTZ_Temp_IHS
WHERE 
    v.periodo = (SELECT MAX(periodo) FROM [Portal_2_0_budget_desarrollo].[dbo].[BG_IHS_versiones])
    AND i.origen = 'IHS'
    AND r.tipo = 'ORIGINAL'
GROUP BY t.ID_IHS, YEAR(r.fecha);



select * from CTZ_Temp_IHS  where ID_IHS = 1
select * from CTZ_Temp_IHS_Production where ID_IHS = 1

--======== CTZ Route =========

select * from CTZ_Route
select * from CTZ_Material_Owner

INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RP', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + RPLTZ', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + SH', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'BLK + WLD', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'COIL TO COIL', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'REWINDED', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT + BLK', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'SLT + BLK + WLD', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WAREHOUSING', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WAREHOUSING / REPALLETIZING', 1)
INSERT INTO [dbo].[CTZ_Route]([ID_Material_Owner],[Route_Name],[Active])VALUES(1,'WEIGHT DIVISION', 1)

--======== CTZ_Vehicle_types ===========
Select * from CTZ_Vehicle_types

SET IDENTITY_INSERT CTZ_Vehicle_Types ON;
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (1,'Automotriz',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (2,'No automotriz',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (3,'Camiones',1)
INSERT INTO [dbo].[CTZ_Vehicle_Types]([ID_VehicleType],[VehicleType_Name],[Active])VALUES (4,'Construcción',1)
SET IDENTITY_INSERT CTZ_Vehicle_Types OFF;

--======== CTZ_Production_Lines ===========
select * from ctz_plants
Select * From CTZ_Production_Lines

SET IDENTITY_INSERT CTZ_Production_Lines ON;
-- Puebla
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(1,1,'Blanking 1', 'Puebla - Blanking 1',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(2,1,'Blanking 2', 'Puebla - Blanking 2',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(3,1,'Blanking 3', 'Puebla - Blanking 3',0,1,1)

-- Silao
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(4,2,'Blanking 1', 'Silao - Blanking 1',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(5,2,'Blanking 2', 'Silao - Blanking 2',0,1,1)
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(6,2,'Blanking 3', 'Silao - Blanking 3',1,1,1)

--SLP
INSERT INTO [dbo].[CTZ_Production_Lines]([ID_Line], ID_Plant,[Line_Name],[Description],[Aluminum],[Steel],[Active])
     VALUES(7,4,'Blanking 1', 'SLP - Blanking 1',0,1,1)

SET IDENTITY_INSERT CTZ_Production_Lines OFF;

--======== CTZ_Material_Type ===========
Select * From CTZ_Material_Type





