USE Portal_2_0
GO
IF object_id(N'PFA_Destination_plant',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[PFA_Destination_plant]
      PRINT '<<< PFA_Destination_plant en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los PFA_Destination_plant
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/11/01
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [PFA_Destination_plant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](30) NOT NULL,
	[activo] [bit] NOT NULL	
 CONSTRAINT [PK_PFA_Destination_plant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- restricción default
ALTER TABLE [PFA_Destination_plant] ADD  CONSTRAINT [DF_PFA_Destination_plant_activo]  DEFAULT (1) FOR [activo]
GO


SET IDENTITY_INSERT [PFA_Destination_plant] ON 

INSERT [PFA_Destination_plant] ([id], [descripcion], [activo]) VALUES (1, N'Puebla', 1)
INSERT [PFA_Destination_plant] ([id], [descripcion], [activo]) VALUES (2, N'Silao', 1)
INSERT [PFA_Destination_plant] ([id], [descripcion], [activo]) VALUES (3, N'Saltillo', 1)
INSERT [PFA_Destination_plant] ([id], [descripcion], [activo]) VALUES (4, N'SLP', 1)
INSERT [PFA_Destination_plant] ([id], [descripcion], [activo]) VALUES (5, N'Other', 1)

SET IDENTITY_INSERT [PFA_Destination_plant] OFF
GO




 	  
IF object_id(N'PFA_Destination_plant',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< PFA_Destination_plant en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla PFA_Destination_plant  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
