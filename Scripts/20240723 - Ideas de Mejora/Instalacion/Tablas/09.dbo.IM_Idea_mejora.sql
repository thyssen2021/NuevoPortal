GO
IF object_id(N'IM_Idea_mejora',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[IM_Idea_mejora]
      PRINT '<<< IM_Idea_mejora en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los IM_cat_impacto
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE IM_Idea_mejora(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[folio] [varchar](10) NULL,
	[captura] [smalldatetime] NOT NULL,
	[titulo] [varchar](150) NOT NULL,
	[situacionActual] [varchar](2000) NOT NULL,
	[objetivo] [varchar](2000) NOT NULL,
	[descripcion] [varchar](2000) NOT NULL,
	[comiteAceptada] [tinyint]  NULL,    -- 1 = aceptada; 2 = SinEstatus; 0 = Rechazada;
	[ideaEnEquipo] [tinyint]  NULL,
	[clasificacionClave] [int]  NULL, --FK * covertir 1 en null
	[nivelImpactoClave] [int]  NULL,	 --FK*
	[enProcesoImplementacion] [tinyint]  NULL,
	[areaImplementacionClave] [int]  NULL,	--FK
	[ideaImplementada] [tinyint]  NULL,		
	[implementacionFecha] [smalldatetime]  NULL,
	[reconocimentoClave] [int]  NULL,	--FK *	
	[reconocimientoMonto] [decimal](9, 3)  NULL,
	[clave_planta] [int]  NULL,			--FK 
	[tipo_idea] [varchar](20) NOT NULL,
 CONSTRAINT [PK_IdeaMejora] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

  -- restriccion de clave foranea
alter table [dbo].IM_Idea_mejora
 add constraint FK_IM_Idea_mejora_clasificacionClave
  foreign key (clasificacionClave)
  references IM_cat_clasificacion(id);

    -- restriccion de clave foranea
alter table [dbo].IM_Idea_mejora
 add constraint FK_IM_Idea_mejora_nivelImpactoClave
  foreign key (nivelImpactoClave)
  references IM_cat_nivel_impacto(id);

      -- restriccion de clave foranea
alter table [dbo].IM_Idea_mejora
 add constraint FK_IM_Idea_mejora_areaImplementacionClave
  foreign key (areaImplementacionClave)
  references IM_cat_area(id);

        -- restriccion de clave foranea
alter table [dbo].IM_Idea_mejora
 add constraint FK_IM_Idea_mejora_reconocimentoClave
  foreign key (reconocimentoClave)
  references IM_cat_reconocimiento(id);

          -- restriccion de clave foranea
alter table [dbo].IM_Idea_mejora
 add constraint FK_IM_Idea_mejora_clave_planta
  foreign key (clave_planta)
  references plantas(clave);
GO

IF object_id(N'IM_Idea_mejora',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< IM_Idea_mejora en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla IM_Idea_mejora  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
