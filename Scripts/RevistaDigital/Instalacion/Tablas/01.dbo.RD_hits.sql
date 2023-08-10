USE Portal_2_0
GO
IF object_id(N'RD_hits',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[RD_hits]
      PRINT '<<< RD_hits en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los RD_hits
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2023/02/13
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/

CREATE TABLE [dbo].[RD_hits](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fecha][datetime] NOT NULL,
	[usuario] [varchar](50) NULL,
	[tipo][char](1) NOT NULL,
 CONSTRAINT [PK_RD_hits] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
GO

-- restricion check
ALTER TABLE [RD_hits] ADD CONSTRAINT CK_RD_hits_tipo CHECK ([tipo] IN 
(
'H'  -- Hit, visita 
,'L',  -- Like
'D'		--Dislike
))

--inserta datos
GO
SET IDENTITY_INSERT [dbo].[RD_hits] ON 



SET IDENTITY_INSERT [dbo].[RD_hits] OFF
	  
IF object_id(N'RD_hits',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< RD_hits en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla RD_hits  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
