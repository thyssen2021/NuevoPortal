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

Select * from CTZ_Material_Owner