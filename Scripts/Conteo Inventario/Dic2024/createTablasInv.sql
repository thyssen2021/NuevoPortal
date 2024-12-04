-- Validar y eliminar tablas existentes
IF OBJECT_ID('dbo.Inv_HistorialCaptura', 'U') IS NOT NULL DROP TABLE dbo.Inv_HistorialCaptura;
IF OBJECT_ID('dbo.Inv_GrupoLoteDetalle', 'U') IS NOT NULL DROP TABLE dbo.Inv_GrupoLoteDetalle;
IF OBJECT_ID('dbo.Inv_LoteGrupo', 'U') IS NOT NULL DROP TABLE dbo.Inv_LoteGrupo;
IF OBJECT_ID('dbo.Inv_Lote', 'U') IS NOT NULL DROP TABLE dbo.Inv_Lote;
IF OBJECT_ID('dbo.Inv_Material', 'U') IS NOT NULL DROP TABLE dbo.Inv_Material;

-- Tabla Inv_Material
CREATE TABLE Inv_Material (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    NumeroMaterial NVARCHAR(50) NOT NULL UNIQUE,
	NumeroAntiguo NVARCHAR(50) NULL,
    Descripcion NVARCHAR(255),
	Espesor DECIMAL(11,3),
    EspesorMin DECIMAL(11,3),
    EspesorMax DECIMAL(11,3)
);

-- Tabla Inv_Lote
CREATE TABLE Inv_Lote (
    LoteID INT PRIMARY KEY IDENTITY(1,1),
    MaterialID INT NOT NULL,
    Batch NVARCHAR(50) NOT NULL,
    StorageBin NVARCHAR(50) NOT NULL,
	StorageLocation NVARCHAR(50) NOT NULL,
    PlantClave INT NOT NULL,
    Pieces INT,
    Unrestricted INT,
    Blocked INT,
    QualityInspection INT,
    AlturaCalculada DECIMAL(10,2),
	EspesorUsuario DECIMAL(10,2),
	UbicacionFisica NVARCHAR(128),  --por defecto debe ser igual al storage bin
    FOREIGN KEY (MaterialID) REFERENCES Inv_Material(MaterialID),
    FOREIGN KEY (PlantClave) REFERENCES plantas(clave)
);

ALTER TABLE Inv_Lote
ADD RowVersion rowversion NOT NULL;

-- Tabla Inv_LoteGrupo
CREATE TABLE Inv_LoteGrupo (
    GrupoID INT PRIMARY KEY IDENTITY(1,1),
    NumeroMaterial NVARCHAR(50) NOT NULL,
    AlturaTotal DECIMAL(10,2) NOT NULL,
    UsuarioCaptura NVARCHAR(128) NOT NULL,
    FechaCaptura DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioCaptura) REFERENCES AspNetUsers(Id)
);

-- Tabla Inv_GrupoLoteDetalle
CREATE TABLE Inv_GrupoLoteDetalle (
    DetalleID INT PRIMARY KEY IDENTITY(1,1),
    GrupoID INT NOT NULL,
    LoteID INT NOT NULL,
    FOREIGN KEY (GrupoID) REFERENCES Inv_LoteGrupo(GrupoID),
    FOREIGN KEY (LoteID) REFERENCES Inv_Lote(LoteID)
);

-- Tabla Inv_HistorialCaptura
CREATE TABLE Inv_HistorialCaptura (
    CapturaID INT PRIMARY KEY IDENTITY(1,1),
    LoteID INT,
    GrupoID INT,
    AlturaUsuario DECIMAL(10,2),
	EspesorUsuario DECIMAL(10,2),
	UbicacionFisica NVARCHAR(128),  --por defecto debe ser igual al storage bin
    PiezasCalculadas INT,
    DiferenciaPiezas INT,
    Advertencia NVARCHAR(255),
    UsuarioCaptura NVARCHAR(128) NOT NULL,
    FechaCaptura DATETIME DEFAULT GETDATE(),
    Comentarios NVARCHAR(MAX),
    Sincronizado BIT DEFAULT 0,
    FOREIGN KEY (LoteID) REFERENCES Inv_Lote(LoteID),
    FOREIGN KEY (GrupoID) REFERENCES Inv_LoteGrupo(GrupoID),
    FOREIGN KEY (UsuarioCaptura) REFERENCES AspNetUsers(Id)
);

-- Índices
CREATE NONCLUSTERED INDEX IX_Inv_Material_NumeroMaterial ON Inv_Material (NumeroMaterial);
CREATE NONCLUSTERED INDEX IX_Inv_Lote_Batch_StorageBin ON Inv_Lote (Batch, StorageBin);
CREATE NONCLUSTERED INDEX IX_Inv_GrupoLoteDetalle_GrupoID ON Inv_GrupoLoteDetalle (GrupoID);
