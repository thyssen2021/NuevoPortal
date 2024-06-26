--USE [Portal_2_0]
GO

/****** Object:  Table [dbo].[SCDM_rel_solicitud_extension_almacenes_virtuales]    Script Date: 25/06/2024 05:10:49 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SCDM_rel_solicitud_extension_almacenes_virtuales](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_solicitud] [int] NOT NULL,
	[almacen_virtual] varchar(3) NOT NULL,
 CONSTRAINT [PK_SCDM_rel_solicitud_extension_almacenes_virtuales_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SCDM_rel_solicitud_extension_almacenes_virtuales]  WITH CHECK ADD  CONSTRAINT [FK_SCDM_rel_solicitud_extension_almacenes_virtuales_id_solicitud] FOREIGN KEY([id_solicitud])
REFERENCES [dbo].[SCDM_solicitud] ([id])
GO

ALTER TABLE [dbo].[SCDM_rel_solicitud_extension_almacenes_virtuales] CHECK CONSTRAINT [FK_SCDM_rel_solicitud_extension_almacenes_virtuales_id_solicitud]
GO


