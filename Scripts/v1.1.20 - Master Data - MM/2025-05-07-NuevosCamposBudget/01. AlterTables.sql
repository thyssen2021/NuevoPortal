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