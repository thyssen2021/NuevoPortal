USE Portal_2_0
GO
IF object_id(N'OT_zona_falla',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[OT_zona_falla]
      PRINT '<<< OT_zona_falla en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los datos OT_zona_falla
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2022/08/23
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [OT_zona_falla](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_linea][int] NOT NULL,
	[zona_falla][varchar](50) NOT NULL,
	[activo][bit] NOT NULL DEFAULT 1
 CONSTRAINT [PK_OT_zona_falla] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--agregar FK

  -- restriccion de clave foranea
  alter table [OT_zona_falla]
 add constraint FK_OT_zona_falla_id_linea
  foreign key (id_linea)
  references produccion_lineas(id);


  --Inserts
GO
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'LAVADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESAS DE TROQUEL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (7,N'OTRO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESAS DE TROQUEL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (8,N'OTRO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'RODILLO SUCIO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'PREENDEREZADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'LAVADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'CHILLER',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (9,N'OTRO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'LAVADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESAS DE TROQUEL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'RODILLO SUCIO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'PREENDEREZADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'CHILLER',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (1,N'OTRO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'LAVADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESAS DE TROQUEL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'RODILLO SUCIO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'PREENDEREZADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'CHILLER',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (2,N'OTRO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'ALMACÉN MÓVIL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'DESENRROLLADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESA DE ENHEBRADO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'RODILLO DEFLECTOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESA DE INTERCONEXIÓN',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'CIZALLA DE DESPUNTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'RODILLO ALIMENTADOR',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'LAVADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'APLANADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESAS DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'FOSA DE BUCLE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'RODILLOS CICLICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESA DE COLAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESAS DE TROQUEL',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'CINTAS TELESCÓPICAS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'APILADORES',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'MESA DE DESCARTE',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'G. H. LINEA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'G. H. PRENSA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'TABLEROS ELÉCTRICOS',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'LIMPIEZA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'RODILLO SUCIO',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'PREENDEREZADORA',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'CHILLER',1)
  INSERT into [OT_zona_falla] (id_linea, zona_falla, activo) VALUES (3,N'OTRO',1)

GO

 	  
IF object_id(N'OT_zona_falla',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< OT_zona_falla en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla OT_zona_falla  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
