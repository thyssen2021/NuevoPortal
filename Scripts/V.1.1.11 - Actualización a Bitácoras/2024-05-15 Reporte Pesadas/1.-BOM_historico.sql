-- Code 1.0 
CREATE TABLE [dbo].bom_pesos(
	[id] [int] IDENTITY(1,1) NOT NULL,
	plant Varchar(4) NOT NULL,
	material VARCHAR(10) NOT NULL, 
	gross_weight float NOT NULL,
	net_weight float NOT NULL,
	CreatedIn datetime2(2) NOT NULL DEFAULT GETDATE()
 CONSTRAINT [PK_bom_pesos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


-- !!!!!!! valores prueba reemplazar por script que recorra la tabla de materiales
-- !!!!! Ejecutar script de copia de pesos de bom desde mm


--Agrega campos necesario para el versionado
-- Code 1.1
 
ALTER TABLE DBO.bom_pesos
ADD SYSSTART DATETIME2(0) GENERATED ALWAYS 
 AS ROW START NOT NULL
 CONSTRAINT DFT_PRODUCTS_SYSSTART 
 DEFAULT('19000101'), 
SYSEND DATETIME2(0) GENERATED ALWAYS 
  AS ROW END NOT NULL
  CONSTRAINT DFT_PRODUCTS_SYSEND 
  DEFAULT('99991231 23:59:59'), 
PERIOD FOR SYSTEM_TIME(SYSSTART, SYSEND);

-- Code 1.2
-- Habilita el versionado
 
ALTER TABLE DBO.bom_pesos
SET(SYSTEM_VERSIONING = ON
(HISTORY_TABLE = DBO.BOM_PESOS_HISTORY));
GO
----------------------------------------
--Aplica algun cambio
--delete bom_pesos where id = 1
--update bom_pesos set net_weight = 80 where id = 2

select * from bom_pesos
select * from BOM_PESOS_HISTORY

-------- Consulta-------------------------
 /*Add offset of the local time zone to current time*/

DECLARE @fecha datetime = '20240515 16:08';  
SET @fecha = DATEADD(HH, 6,@fecha)

SELECT
    plant
    , material
    , gross_weight
    , net_weight
    , CreatedIn    
FROM bom_pesos
    FOR SYSTEM_TIME AS OF @fecha where id=2;

-----------------------------------------------------------------------------
