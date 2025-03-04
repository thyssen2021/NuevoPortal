--Agrega el mes a la tabla de tipo de cambio 
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
ADD [mes] INT NOT NULL DEFAULT (0);

--  Elimina la restricción única existente
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
DROP CONSTRAINT UQ_budget_rel_tipo_cambio_fy_anio_tipo_cambio;

--condicion para que no se repitan fy, mes y tipo de cambio
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy]
ADD CONSTRAINT UQ_tipo_cambio_fy_mes UNIQUE (id_budget_anio_fiscal, mes, id_tipo_cambio);

-- Inserta 12 registros nuevos por cada registro actual (con mes = 0)
INSERT INTO [dbo].[budget_rel_tipo_cambio_fy] (id_budget_anio_fiscal, id_tipo_cambio, cantidad, mes)
SELECT 
    t.id_budget_anio_fiscal,
    t.id_tipo_cambio,
    t.cantidad,
    v.mes
FROM [dbo].[budget_rel_tipo_cambio_fy] AS t
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) AS v(mes)
WHERE t.mes = 0 
  AND t.id_budget_anio_fiscal BETWEEN 1 AND 51;

-- Elimina los registros originales con mes = 0
DELETE FROM [dbo].[budget_rel_tipo_cambio_fy]
WHERE mes = 0
  AND id_budget_anio_fiscal BETWEEN 1 AND 51;

-- deja los TC vacios
update budget_rel_tipo_cambio_fy set cantidad = 0


-- ============= SP cambio de TC ========================
GO
IF OBJECT_ID('dbo.usp_ActualizarBudgetCantidad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_ActualizarBudgetCantidad;
GO

CREATE PROCEDURE dbo.usp_ActualizarBudgetCantidad
    @id_fy   INT,  -- Año fiscal
    @month   INT   -- Mes a procesar
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------------
    -- 1) Insertar fila local si NO existe y sí hay al menos 1 valor (sum > 0)
    ----------------------------------------------------------------------------
    INSERT INTO budget_cantidad (
        id_budget_rel_fy_centro,
        id_cuenta_sap,
        mes,
        currency_iso,
        cantidad,
        comentario,
        moneda_local_usd
    )
    SELECT 
        bc.id_budget_rel_fy_centro,
        bc.id_cuenta_sap,
        bc.mes,
        'USD' AS currency_iso,
        0     AS cantidad,
        NULL  AS comentario,
        1     AS moneda_local_usd
    FROM 
    (
        SELECT 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad c
        JOIN budget_rel_fy_centro fy ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            -- Sólo filas donde la moneda sea MXN, USD o EUR y sea "no local"
            AND c.currency_iso IN ('MXN','USD','EUR')
            AND c.moneda_local_usd = 0
        GROUP BY 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        HAVING SUM(c.cantidad) > 0  -- al menos uno con cantidad > 0
    ) AS bc
    WHERE NOT EXISTS (
        SELECT 1
        FROM budget_cantidad bc2
        WHERE bc2.id_budget_rel_fy_centro = bc.id_budget_rel_fy_centro
          AND bc2.id_cuenta_sap = bc.id_cuenta_sap
          AND bc2.mes = bc.mes
          AND bc2.moneda_local_usd = 1
    );

    ----------------------------------------------------------------------------
    -- 2) Eliminar fila local si NO hay cantidades en MXN, USD o EUR > 0
    ----------------------------------------------------------------------------
    DELETE bcLocal
    FROM budget_cantidad bcLocal
    JOIN budget_rel_fy_centro fy 
         ON fy.id = bcLocal.id_budget_rel_fy_centro
    WHERE 
        fy.id_anio_fiscal = @id_fy
        AND bcLocal.mes = @month
        AND bcLocal.moneda_local_usd = 1
        -- No existe ninguna fila con MXN, USD o EUR que tenga cantidad > 0
        AND NOT EXISTS (
            SELECT 1 
            FROM budget_cantidad bcNon
            WHERE bcNon.id_budget_rel_fy_centro = bcLocal.id_budget_rel_fy_centro
              AND bcNon.id_cuenta_sap = bcLocal.id_cuenta_sap
              AND bcNon.mes = bcLocal.mes
              AND bcNon.moneda_local_usd = 0
              AND bcNon.currency_iso IN ('MXN','USD','EUR')
              AND bcNon.cantidad > 0
        );

    ----------------------------------------------------------------------------
    -- 3) Actualizar fila local sumando (MXN→USD) + USD + (EUR→USD)
    ----------------------------------------------------------------------------

    ;WITH cteLocal AS (
        SELECT 
            c.id,
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad c
        JOIN budget_rel_fy_centro fy 
            ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            AND c.moneda_local_usd = 1
    ),
    cteTC AS (
        -- Obtenemos en una sola fila los tipos de cambio USD/MXN (id_tipo_cambio=1) y EUR/USD (id_tipo_cambio=2)
        SELECT
            @month AS mes,
            MAX(CASE WHEN id_tipo_cambio = 1 THEN cantidad END) AS usd_mxn,
            MAX(CASE WHEN id_tipo_cambio = 2 THEN cantidad END) AS eur_usd
        FROM budget_rel_tipo_cambio_fy
        WHERE 
            id_budget_anio_fiscal = @id_fy
            AND mes = @month
    )
    UPDATE bcLocal
    SET bcLocal.cantidad = CONVERT(DECIMAL(10,2),
          (ISNULL(bcMXN.cantidad, 0) / cteTC.usd_mxn)
        +  ISNULL(bcUSD.cantidad, 0)
        +  (ISNULL(bcEUR.cantidad, 0) * cteTC.eur_usd)
    )
    FROM cteLocal
    JOIN budget_cantidad bcLocal 
        ON bcLocal.id = cteLocal.id
    CROSS JOIN cteTC
    LEFT JOIN budget_cantidad bcMXN 
        ON bcMXN.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcMXN.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcMXN.mes = cteLocal.mes
       AND bcMXN.currency_iso = 'MXN'
       AND bcMXN.moneda_local_usd = 0
    LEFT JOIN budget_cantidad bcUSD
        ON bcUSD.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcUSD.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcUSD.mes = cteLocal.mes
       AND bcUSD.currency_iso = 'USD'
       AND bcUSD.moneda_local_usd = 0
    LEFT JOIN budget_cantidad bcEUR
        ON bcEUR.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcEUR.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcEUR.mes = cteLocal.mes
       AND bcEUR.currency_iso = 'EUR'
       AND bcEUR.moneda_local_usd = 0;

END
GO

-- ================ Table budget_target =====================
--- 
CREATE TABLE [dbo].[budget_target](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [id_cuenta_sap] [int] NOT NULL,       -- FK a budget_cuenta_sap
    [id_centro_costo] [int] NOT NULL,     -- FK a budget_centro_costo
    [id_anio_fiscal] [int] NOT NULL,      -- FK a budget_anio_fiscal
    [target] [float] NOT NULL,            -- Valor del target
    [activado] [bit] NOT NULL,            -- Indica si está activado o no
 CONSTRAINT [PK_budget_target] PRIMARY KEY CLUSTERED 
(
    [id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_cuenta_sap] FOREIGN KEY([id_cuenta_sap])
REFERENCES [dbo].[budget_cuenta_sap] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_cuenta_sap]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_centro_costo] FOREIGN KEY([id_centro_costo])
REFERENCES [dbo].[budget_centro_costo] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_centro_costo]
GO

ALTER TABLE [dbo].[budget_target] WITH CHECK ADD CONSTRAINT [FK_budget_target_anio_fiscal] FOREIGN KEY([id_anio_fiscal])
REFERENCES [dbo].[budget_anio_fiscal] ([id])
GO
ALTER TABLE [dbo].[budget_target] CHECK CONSTRAINT [FK_budget_target_anio_fiscal]
GO

--======= table budget_cantidad ==========
/**********************************************************************
    1) Eliminar la tabla budget_cantidad_forecast si ya existe
**********************************************************************/
IF OBJECT_ID('dbo.budget_cantidad_forecast', 'U') IS NOT NULL
    DROP TABLE dbo.budget_cantidad_forecast;
GO

/**********************************************************************
    2) Crear la tabla con la misma estructura de columnas
**********************************************************************/
CREATE TABLE [dbo].[budget_cantidad_forecast]
(
    [id]                INT IDENTITY(1,1) NOT NULL,
    [id_budget_rel_fy_centro] INT NOT NULL,
    [id_cuenta_sap]     INT NOT NULL,
    [mes]               INT NOT NULL,
    [currency_iso]      VARCHAR(3) NOT NULL,
    [cantidad]          DECIMAL(14,2) NOT NULL,
    [comentario]        VARCHAR(150) NULL,
    [moneda_local_usd]  BIT NOT NULL,

    CONSTRAINT [PK_budget_cantidad_forecast] 
        PRIMARY KEY CLUSTERED ([id] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
) ON [PRIMARY];
GO

/**********************************************************************
    3) Agregar la UNIQUE constraint (análoga a UQ_budget_cantidad_anio_sap_mes)
**********************************************************************/
ALTER TABLE [dbo].[budget_cantidad_forecast]
ADD CONSTRAINT [UQ_budget_cantidad_forecast_anio_sap_mes] UNIQUE NONCLUSTERED
(
    [id_budget_rel_fy_centro], 
    [id_cuenta_sap], 
    [mes], 
    [currency_iso], 
    [moneda_local_usd]
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
ON [PRIMARY];
GO

/**********************************************************************
    4) Establecer el DEFAULT para [moneda_local_usd], 
       tal como existe en budget_cantidad
**********************************************************************/
ALTER TABLE [dbo].[budget_cantidad_forecast]
ADD DEFAULT((1)) FOR [moneda_local_usd];
GO

/* Replicar las FOREIGN KEY si lo necesitas: */

       ALTER TABLE [dbo].[budget_cantidad_forecast]
       ADD CONSTRAINT [FK_budget_cantidad_forecast_id_cuenta_sap]
           FOREIGN KEY([id_cuenta_sap])
           REFERENCES [dbo].[budget_cuenta_sap]([id]);
    
       ALTER TABLE [dbo].[budget_cantidad_forecast]
       ADD CONSTRAINT [FK_budget_cantidad_forecast_id_rel_fy_centro]
           FOREIGN KEY([id_budget_rel_fy_centro])
           REFERENCES [dbo].[budget_rel_fy_centro]([id]);
    
       ALTER TABLE [dbo].[budget_cantidad_forecast]
       ADD CONSTRAINT [FK_budget_cantidad_forecast_currency_iso]
           FOREIGN KEY([currency_iso])
           REFERENCES [dbo].[currency]([CurrencyISO]);

/**********************************************************************
    5) Copiar los datos desde la tabla budget_cantidad
**********************************************************************/
SET IDENTITY_INSERT [dbo].budget_cantidad_forecast ON;

INSERT INTO [dbo].[budget_cantidad_forecast]
(
	[id],
    [id_budget_rel_fy_centro],
    [id_cuenta_sap],
    [mes],
    [currency_iso],
    [cantidad],
    [comentario],
    [moneda_local_usd]
)
SELECT
	[id],
    [id_budget_rel_fy_centro],
    [id_cuenta_sap],
    [mes],
    [currency_iso],
    [cantidad],
    [comentario],
    [moneda_local_usd]
FROM [dbo].[budget_cantidad];

SET IDENTITY_INSERT [dbo].budget_cantidad_forecast OFF;
GO
GO

-- ==========  budget_rel_conceptos_cantidades_forecast ==========

----------------------------------------
-- 1. Eliminar tabla si ya existe
----------------------------------------
IF OBJECT_ID(N'[dbo].[budget_rel_conceptos_cantidades_forecast]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[budget_rel_conceptos_cantidades_forecast];
END
GO

----------------------------------------
-- 2. Crear la nueva tabla (misma estructura)
--    NOTA: Se omite la creación de los FKs para crearlos después de cargar datos
----------------------------------------
CREATE TABLE [dbo].[budget_rel_conceptos_cantidades_forecast](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_budget_cantidad_forecast] [int] NOT NULL,
	[id_rel_conceptos] [int] NOT NULL,
	[cantidad] [float] NULL,
	[comentario] [varchar](80) NULL,
    CONSTRAINT [PK_budget_rel_conceptos_cantidades_forecast] PRIMARY KEY CLUSTERED 
    (
	    [id] ASC
    ) WITH
    (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        IGNORE_DUP_KEY = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON,
        OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
) ON [PRIMARY];
GO

----------------------------------------
-- 3. Copiar datos desde la tabla original,
--    preservando los mismos valores de IDENTITY
----------------------------------------
SET IDENTITY_INSERT [dbo].[budget_rel_conceptos_cantidades_forecast] ON;

INSERT INTO [dbo].[budget_rel_conceptos_cantidades_forecast]
       ([id], [id_budget_cantidad_forecast], [id_rel_conceptos], [cantidad], [comentario])
SELECT [id], [id_budget_cantidad], [id_rel_conceptos], [cantidad], [comentario]
FROM [dbo].[budget_rel_conceptos_cantidades];

SET IDENTITY_INSERT [dbo].[budget_rel_conceptos_cantidades_forecast] OFF;
GO

----------------------------------------
-- 4. Crear las mismas claves foráneas, 
--    pero con nombres diferentes para no chocar 
--    con las FKs de la tabla original
----------------------------------------

ALTER TABLE [dbo].[budget_rel_conceptos_cantidades_forecast]
  WITH CHECK ADD  CONSTRAINT [FK_brc_cant_new_id_budget_cantidad]
  FOREIGN KEY([id_budget_cantidad_forecast])
  REFERENCES [dbo].[budget_cantidad_forecast] ([id]);
GO

ALTER TABLE [dbo].[budget_rel_conceptos_cantidades_forecast]
  CHECK CONSTRAINT [FK_brc_cant_new_id_budget_cantidad];
GO

ALTER TABLE [dbo].[budget_rel_conceptos_cantidades_forecast]
  WITH CHECK ADD  CONSTRAINT [FK_brc_cant_new_id_rel_conceptos]
  FOREIGN KEY([id_rel_conceptos])
  REFERENCES [dbo].[budget_rel_conceptos_formulas] ([id]);
GO

ALTER TABLE [dbo].[budget_rel_conceptos_cantidades_forecast]
  CHECK CONSTRAINT [FK_brc_cant_new_id_rel_conceptos];
GO

-- ========== view_valores_forecast ==============
GO
CREATE VIEW [dbo].[view_valores_forecast] AS

   -------------------------------------------------------------------------
    -- Primera parte: Registros que sí existen en budget_cantidad_forecast, con suma de montos
    -------------------------------------------------------------------------
    SELECT 
        ROW_NUMBER() OVER (ORDER BY fyc.id_centro_costo, v.id_cuenta_sap) AS id,
        fyc.id AS [id_budget_rel_fy_centro],
        fyc.id_anio_fiscal,
        fyc.id_centro_costo,
        v.id_cuenta_sap,
        cs.sap_account,
        cs.[name],
        cc.num_centro_costo AS cost_center,
        cc.descripcion       AS department,
        cc.class_1,
        cc.class_2,
        -- Responsable: construimos el string con FOR XML
        (
          SELECT SUBSTRING(
                 (
                   SELECT '/' + ISNULL(e.nombre,'') + ' ' + ISNULL(e.apellido1,'') + ' ' + ISNULL(e.apellido2,'')
                   FROM budget_responsables r
                   JOIN empleados e ON e.id = r.id_responsable
                   WHERE r.id_budget_centro_costo = fyc.id_centro_costo
                   FOR XML PATH('')
                 ), 2, 9999
          )
        ) AS responsable,
        bp.codigo_sap,
        bp.id               AS id_budget_plant,
        bm.descripcion      AS mapping,
        bmb.descripcion     AS [mapping_bridge],
        'USD'               AS [currency_iso],

        -- USD
        SUM(CASE WHEN v.[Mes] = 1  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Enero],
        SUM(CASE WHEN v.[Mes] = 2  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Febrero],
        SUM(CASE WHEN v.[Mes] = 3  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Marzo],
        SUM(CASE WHEN v.[Mes] = 4  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Abril],
        SUM(CASE WHEN v.[Mes] = 5  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Mayo],
        SUM(CASE WHEN v.[Mes] = 6  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Junio],
        SUM(CASE WHEN v.[Mes] = 7  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Julio],
        SUM(CASE WHEN v.[Mes] = 8  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Agosto],
        SUM(CASE WHEN v.[Mes] = 9  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Septiembre],
        SUM(CASE WHEN v.[Mes] = 10 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Octubre],
        SUM(CASE WHEN v.[Mes] = 11 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Noviembre],
        SUM(CASE WHEN v.[Mes] = 12 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Diciembre],

        -- MXN
        SUM(CASE WHEN v.[Mes] = 1  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Enero_MXN],
        SUM(CASE WHEN v.[Mes] = 2  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Febrero_MXN],
        SUM(CASE WHEN v.[Mes] = 3  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Marzo_MXN],
        SUM(CASE WHEN v.[Mes] = 4  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Abril_MXN],
        SUM(CASE WHEN v.[Mes] = 5  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Mayo_MXN],
        SUM(CASE WHEN v.[Mes] = 6  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Junio_MXN],
        SUM(CASE WHEN v.[Mes] = 7  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Julio_MXN],
        SUM(CASE WHEN v.[Mes] = 8  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Agosto_MXN],
        SUM(CASE WHEN v.[Mes] = 9  AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Septiembre_MXN],
        SUM(CASE WHEN v.[Mes] = 10 AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Octubre_MXN],
        SUM(CASE WHEN v.[Mes] = 11 AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Noviembre_MXN],
        SUM(CASE WHEN v.[Mes] = 12 AND v.currency_iso = 'MXN' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Diciembre_MXN],

        -- EUR
        SUM(CASE WHEN v.[Mes] = 1  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Enero_EUR],
        SUM(CASE WHEN v.[Mes] = 2  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Febrero_EUR],
        SUM(CASE WHEN v.[Mes] = 3  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Marzo_EUR],
        SUM(CASE WHEN v.[Mes] = 4  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Abril_EUR],
        SUM(CASE WHEN v.[Mes] = 5  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Mayo_EUR],
        SUM(CASE WHEN v.[Mes] = 6  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Junio_EUR],
        SUM(CASE WHEN v.[Mes] = 7  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Julio_EUR],
        SUM(CASE WHEN v.[Mes] = 8  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Agosto_EUR],
        SUM(CASE WHEN v.[Mes] = 9  AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Septiembre_EUR],
        SUM(CASE WHEN v.[Mes] = 10 AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Octubre_EUR],
        SUM(CASE WHEN v.[Mes] = 11 AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Noviembre_EUR],
        SUM(CASE WHEN v.[Mes] = 12 AND v.currency_iso = 'EUR' AND v.moneda_local_usd = 0 THEN v.[Cantidad] END) AS [Diciembre_EUR],

        -- USD LOCAL
        SUM(CASE WHEN v.[Mes] = 1  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Enero_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 2  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Febrero_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 3  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Marzo_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 4  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Abril_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 5  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Mayo_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 6  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Junio_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 7  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Julio_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 8  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Agosto_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 9  AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Septiembre_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 10 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Octubre_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 11 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Noviembre_USD_LOCAL],
        SUM(CASE WHEN v.[Mes] = 12 AND v.currency_iso = 'USD' AND v.moneda_local_usd = 1 THEN v.[Cantidad] END) AS [Diciembre_USD_LOCAL],

        -- Comentarios: en lugar del subselect TOP(1), usamos LEFT JOIN para extraer directamente
        c.comentarios AS [Comentario]
    FROM budget_cantidad_forecast          AS v
    JOIN budget_rel_fy_centro     AS fyc ON fyc.id = v.id_budget_rel_fy_centro
    JOIN budget_centro_costo      AS cc  ON cc.id  = fyc.id_centro_costo
    JOIN budget_departamentos     AS bd  ON bd.id  = cc.id_budget_departamento
    JOIN budget_plantas           AS bp  ON bp.id  = bd.id_budget_planta
    JOIN budget_cuenta_sap        AS cs  ON cs.id  = v.id_cuenta_sap
    JOIN budget_mapping           AS bm  ON bm.id  = cs.id_mapping
    JOIN budget_mapping_bridge    AS bmb ON bmb.id = bm.id_mapping_bridge
    LEFT JOIN budget_rel_comentarios AS c 
           ON c.id_budget_rel_fy_centro = v.id_budget_rel_fy_centro
          AND c.id_cuenta_sap          = v.id_cuenta_sap
    GROUP BY
         fyc.id,
         fyc.id_anio_fiscal,
         fyc.id_centro_costo,
         v.id_cuenta_sap,
         cs.sap_account,
         cs.name,
         cc.num_centro_costo,
         cc.descripcion,
         cc.class_1,
         cc.class_2,
         bp.codigo_sap,
         bp.id,
         bm.descripcion,
         bmb.descripcion,
         c.comentarios

    -------------------------------------------------------------------------
    -- Segunda parte: Registros que sólo existen en budget_rel_comentarios 
    -- (NO tienen datos en budget_cantidad_forecast), por ello se incluyen con UNION ALL.
    -------------------------------------------------------------------------
    UNION ALL

    SELECT
        ROW_NUMBER() OVER (ORDER BY cm.id_budget_rel_fy_centro, cm.id_cuenta_sap) AS id,
        cm.id_budget_rel_fy_centro,
        fyc.id_anio_fiscal,
        fyc.id_centro_costo,
        cm.id_cuenta_sap,
        cs.sap_account,
        cs.[name],
        cc.num_centro_costo AS cost_center,
        cc.descripcion       AS department,
        cc.class_1,
        cc.class_2,
        (
          SELECT SUBSTRING(
                 (
                   SELECT '/' + ISNULL(e.nombre,'') + ' ' + ISNULL(e.apellido1,'') + ' ' + ISNULL(e.apellido2,'')
                   FROM budget_responsables r
                   JOIN empleados e ON e.id = r.id_responsable
                   WHERE r.id_budget_centro_costo = fyc.id_centro_costo
                   FOR XML PATH('')
                 ), 2, 9999
          )
        ) AS responsable,
        bp.codigo_sap,
        bp.id              AS id_budget_plant,
        bm.descripcion     AS mapping,
        bmb.descripcion    AS [mapping_bridge],
        'USD'             AS [currency_iso],   -- Se respeta la literal 'USD' del original
        /* Todos los montos NULL para "simular" que no hay movimientos en budget_cantidad_forecast */
        NULL AS [Enero],     NULL AS [Febrero],     NULL AS [Marzo],
        NULL AS [Abril],     NULL AS [Mayo],        NULL AS [Junio],
        NULL AS [Julio],     NULL AS [Agosto],      NULL AS [Septiembre],
        NULL AS [Octubre],   NULL AS [Noviembre],   NULL AS [Diciembre],
        NULL AS [Enero_MXN], NULL AS [Febrero_MXN], NULL AS [Marzo_MXN],
        NULL AS [Abril_MXN], NULL AS [Mayo_MXN],    NULL AS [Junio_MXN],
        NULL AS [Julio_MXN], NULL AS [Agosto_MXN],  NULL AS [Septiembre_MXN],
        NULL AS [Octubre_MXN],   NULL AS [Noviembre_MXN],   NULL AS [Diciembre_MXN],
        NULL AS [Enero_EUR],    NULL AS [Febrero_EUR],    NULL AS [Marzo_EUR],
        NULL AS [Abril_EUR],    NULL AS [Mayo_EUR],       NULL AS [Junio_EUR],
        NULL AS [Julio_EUR],    NULL AS [Agosto_EUR],     NULL AS [Septiembre_EUR],
        NULL AS [Octubre_EUR],  NULL AS [Noviembre_EUR],  NULL AS [Diciembre_EUR],
        NULL AS [Enero_USD_LOCAL],   NULL AS [Febrero_USD_LOCAL],   NULL AS [Marzo_USD_LOCAL],
        NULL AS [Abril_USD_LOCAL],   NULL AS [Mayo_USD_LOCAL],      NULL AS [Junio_USD_LOCAL],
        NULL AS [Julio_USD_LOCAL],   NULL AS [Agosto_USD_LOCAL],    NULL AS [Septiembre_USD_LOCAL],
        NULL AS [Octubre_USD_LOCAL], NULL AS [Noviembre_USD_LOCAL], NULL AS [Diciembre_USD_LOCAL],
        cm.comentarios
    FROM budget_rel_comentarios   AS cm
    JOIN budget_rel_fy_centro     AS fyc ON fyc.id = cm.id_budget_rel_fy_centro
    JOIN budget_centro_costo      AS cc  ON cc.id  = fyc.id_centro_costo
    JOIN budget_departamentos     AS bd  ON bd.id  = cc.id_budget_departamento
    JOIN budget_plantas           AS bp  ON bp.id  = bd.id_budget_planta
    JOIN budget_cuenta_sap        AS cs  ON cs.id  = cm.id_cuenta_sap
    JOIN budget_mapping           AS bm  ON bm.id  = cs.id_mapping
    JOIN budget_mapping_bridge    AS bmb ON bmb.id = bm.id_mapping_bridge
    WHERE NOT EXISTS (
        SELECT 1 
        FROM budget_cantidad_forecast bc 
        WHERE bc.id_budget_rel_fy_centro = cm.id_budget_rel_fy_centro
          AND bc.id_cuenta_sap = cm.id_cuenta_sap
    );
GO

-- =========================================================================
-- Crear tabla con budget_rel_tipo_cambio_fy_forecast
-- =========================================================================
CREATE TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast](
    [id]                  INT IDENTITY(1,1) NOT NULL,
    [id_budget_anio_fiscal] INT NOT NULL,
    [id_tipo_cambio]     INT NOT NULL,
    [cantidad]           FLOAT NOT NULL,
    [mes]                INT NOT NULL,
    CONSTRAINT [PK_budget_rel_tipo_cambio_fy_forecast] 
        PRIMARY KEY CLUSTERED ([id] ASC)
        WITH (
            PAD_INDEX = OFF, 
            STATISTICS_NORECOMPUTE = OFF, 
            IGNORE_DUP_KEY = OFF, 
            ALLOW_ROW_LOCKS = ON, 
            ALLOW_PAGE_LOCKS = ON, 
            OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
        ) ON [PRIMARY],
    CONSTRAINT [UQ_tipo_cambio_fy_mes_forecast] 
        UNIQUE NONCLUSTERED (
            [id_budget_anio_fiscal] ASC,
            [mes] ASC,
            [id_tipo_cambio] ASC
        )
        WITH (
            PAD_INDEX = OFF, 
            STATISTICS_NORECOMPUTE = OFF, 
            IGNORE_DUP_KEY = OFF, 
            ALLOW_ROW_LOCKS = ON, 
            ALLOW_PAGE_LOCKS = ON, 
            OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
        ) ON [PRIMARY]
) ON [PRIMARY];
GO

-- Agregar DEFAULT a la columna [mes]
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast]
    ADD CONSTRAINT [DF_budget_rel_tipo_cambio_fy_forecast_mes] 
        DEFAULT ((0)) FOR [mes];
GO

-- =========================================================================
-- Llaves foráneas
-- =========================================================================

-- FK hacia budget_anio_fiscal
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast]  
    WITH CHECK ADD  CONSTRAINT [FK_budget_rel_tipo_cambio_fy_forecast_id_budget_anio_fiscal] 
    FOREIGN KEY([id_budget_anio_fiscal])
    REFERENCES [dbo].[budget_anio_fiscal] ([id]);
GO

ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast] 
    CHECK CONSTRAINT [FK_budget_rel_tipo_cambio_fy_forecast_id_budget_anio_fiscal];
GO

-- FK hacia budget_tipo_cambio
ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast]  
    WITH CHECK ADD  CONSTRAINT [FK_budget_rel_tipo_cambio_fy_forecast_id_tipo_cambio] 
    FOREIGN KEY([id_tipo_cambio])
    REFERENCES [dbo].[budget_tipo_cambio] ([id]);
GO

ALTER TABLE [dbo].[budget_rel_tipo_cambio_fy_forecast] 
    CHECK CONSTRAINT [FK_budget_rel_tipo_cambio_fy_forecast_id_tipo_cambio];
GO

-- ===============  INSERTA DATOS ==========================
-- 1) Permitir la inserción manual de valores en la columna IDENTITY
SET IDENTITY_INSERT [dbo].[budget_rel_tipo_cambio_fy_forecast] ON;

-- 2) Insertar incluyendo la columna [id]
INSERT INTO [dbo].[budget_rel_tipo_cambio_fy_forecast] 
    ([id], 
     [id_budget_anio_fiscal], 
     [id_tipo_cambio], 
     [cantidad], 
     [mes])
SELECT 
    [id],
    [id_budget_anio_fiscal], 
    [id_tipo_cambio],
    [cantidad], 
    [mes]
FROM [dbo].[budget_rel_tipo_cambio_fy];

-- 3) Desactivar la inserción manual en la columna IDENTITY
SET IDENTITY_INSERT [dbo].[budget_rel_tipo_cambio_fy_forecast] OFF;

--- =*=*=*=*=*=*=*=*==*=*=*=*==*=*=*=*==*=*=*=*==*=*=*=*==*=*=*=*==*=*=*=*==
-- =========================================================================
-- SP usp_ActualizarBudgetCantidadForecast
-- =========================================================================
GO
IF OBJECT_ID('dbo.usp_ActualizarBudgetCantidadForecast', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_ActualizarBudgetCantidadForecast;
GO

CREATE PROCEDURE dbo.usp_ActualizarBudgetCantidadForecast
    @id_fy   INT,  -- Año fiscal
    @month   INT   -- Mes a procesar
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------------
    -- 1) Insertar fila local si NO existe y sí hay al menos 1 valor (sum > 0)
    ----------------------------------------------------------------------------
    INSERT INTO budget_cantidad_forecast (
        id_budget_rel_fy_centro,
        id_cuenta_sap,
        mes,
        currency_iso,
        cantidad,
        comentario,
        moneda_local_usd
    )
    SELECT 
        bc.id_budget_rel_fy_centro,
        bc.id_cuenta_sap,
        bc.mes,
        'USD' AS currency_iso,
        0     AS cantidad,
        NULL  AS comentario,
        1     AS moneda_local_usd
    FROM 
    (
        SELECT 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad_forecast c
        JOIN budget_rel_fy_centro fy ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            -- Sólo filas donde la moneda sea MXN, USD o EUR y sea "no local"
            AND c.currency_iso IN ('MXN','USD','EUR')
            AND c.moneda_local_usd = 0
        GROUP BY 
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        HAVING SUM(c.cantidad) > 0  -- al menos uno con cantidad > 0
    ) AS bc
    WHERE NOT EXISTS (
        SELECT 1
        FROM budget_cantidad_forecast bc2
        WHERE bc2.id_budget_rel_fy_centro = bc.id_budget_rel_fy_centro
          AND bc2.id_cuenta_sap = bc.id_cuenta_sap
          AND bc2.mes = bc.mes
          AND bc2.moneda_local_usd = 1
    );

    ----------------------------------------------------------------------------
    -- 2) Eliminar fila local si NO hay cantidades en MXN, USD o EUR > 0
    ----------------------------------------------------------------------------
    DELETE bcLocal
    FROM budget_cantidad_forecast bcLocal
    JOIN budget_rel_fy_centro fy 
         ON fy.id = bcLocal.id_budget_rel_fy_centro
    WHERE 
        fy.id_anio_fiscal = @id_fy
        AND bcLocal.mes = @month
        AND bcLocal.moneda_local_usd = 1
        -- No existe ninguna fila con MXN, USD o EUR que tenga cantidad > 0
        AND NOT EXISTS (
            SELECT 1 
            FROM budget_cantidad_forecast bcNon
            WHERE bcNon.id_budget_rel_fy_centro = bcLocal.id_budget_rel_fy_centro
              AND bcNon.id_cuenta_sap = bcLocal.id_cuenta_sap
              AND bcNon.mes = bcLocal.mes
              AND bcNon.moneda_local_usd = 0
              AND bcNon.currency_iso IN ('MXN','USD','EUR')
              AND bcNon.cantidad > 0
        );

    ----------------------------------------------------------------------------
    -- 3) Actualizar fila local sumando (MXN→USD) + USD + (EUR→USD)
    ----------------------------------------------------------------------------

    ;WITH cteLocal AS (
        SELECT 
            c.id,
            c.id_budget_rel_fy_centro,
            c.id_cuenta_sap,
            c.mes
        FROM budget_cantidad_forecast c
        JOIN budget_rel_fy_centro fy 
            ON fy.id = c.id_budget_rel_fy_centro
        WHERE 
            fy.id_anio_fiscal = @id_fy
            AND c.mes = @month
            AND c.moneda_local_usd = 1
    ),
    cteTC AS (
        -- Obtenemos en una sola fila los tipos de cambio USD/MXN (id_tipo_cambio=1) y EUR/USD (id_tipo_cambio=2)
        SELECT
            @month AS mes,
            MAX(CASE WHEN id_tipo_cambio = 1 THEN cantidad END) AS usd_mxn,
            MAX(CASE WHEN id_tipo_cambio = 2 THEN cantidad END) AS eur_usd
        FROM budget_rel_tipo_cambio_fy_forecast
        WHERE 
            id_budget_anio_fiscal = @id_fy
            AND mes = @month
    )
    UPDATE bcLocal
    SET bcLocal.cantidad = CONVERT(DECIMAL(10,2),
          (ISNULL(bcMXN.cantidad, 0) / cteTC.usd_mxn)
        +  ISNULL(bcUSD.cantidad, 0)
        +  (ISNULL(bcEUR.cantidad, 0) * cteTC.eur_usd)
    )
    FROM cteLocal
    JOIN budget_cantidad_forecast bcLocal 
        ON bcLocal.id = cteLocal.id
    CROSS JOIN cteTC
    LEFT JOIN budget_cantidad_forecast bcMXN 
        ON bcMXN.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcMXN.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcMXN.mes = cteLocal.mes
       AND bcMXN.currency_iso = 'MXN'
       AND bcMXN.moneda_local_usd = 0
    LEFT JOIN budget_cantidad_forecast bcUSD
        ON bcUSD.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcUSD.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcUSD.mes = cteLocal.mes
       AND bcUSD.currency_iso = 'USD'
       AND bcUSD.moneda_local_usd = 0
    LEFT JOIN budget_cantidad_forecast bcEUR
        ON bcEUR.id_budget_rel_fy_centro = cteLocal.id_budget_rel_fy_centro
       AND bcEUR.id_cuenta_sap = cteLocal.id_cuenta_sap
       AND bcEUR.mes = cteLocal.mes
       AND bcEUR.currency_iso = 'EUR'
       AND bcEUR.moneda_local_usd = 0;

END
GO

----- =========== InsertBudgetTarget ================
IF OBJECT_ID('dbo.InsertBudgetTarget', 'P') IS NOT NULL
    DROP PROCEDURE dbo.InsertBudgetTarget;
GO

CREATE PROCEDURE dbo.InsertBudgetTarget
    @SapAccount      VARCHAR(8),
    @NumCentroCosto  VARCHAR(6),
    @idAnioFiscal    INT,
    @Cantidad        FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[budget_target]
           ([id_cuenta_sap],
            [id_centro_costo],
            [id_anio_fiscal],
            [target],
            [activado])
    SELECT 
         cs.id,                -- Id de budget_cuenta_sap obtenido mediante el sap_account
         cc.id,                -- Id de budget_centro_costo obtenido mediante el num_centro_costo
         @idAnioFiscal,        -- Id del año fiscal proporcionado
         @Cantidad,            -- Cantidad a insertar en target
         1                     -- Activado (TRUE)
    FROM [dbo].[budget_cuenta_sap] AS cs
    JOIN [dbo].[budget_centro_costo] AS cc 
        ON cc.num_centro_costo = @NumCentroCosto
    WHERE cs.sap_account = @SapAccount;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No se encontró coincidencia para los parámetros proporcionados.', 16, 1);
    END
END
GO

--ejemplo de call
--EXEC dbo.InsertBudgetTarget @SapAccount = '50750000', @NumCentroCosto = 'CC002', @idAnioFiscal = 2, @Cantidad = 500.75;