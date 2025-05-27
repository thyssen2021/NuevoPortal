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