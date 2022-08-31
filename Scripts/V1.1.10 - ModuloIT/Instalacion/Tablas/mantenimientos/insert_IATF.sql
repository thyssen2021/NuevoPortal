select * from IATF_documentos
select * from IATF_revisiones

--Nuevos Documento
--(puebla)
INSERT INTO [dbo].[IATF_documentos]([id_planta],[clave],[nombre_documento],[proceso],[activo])
     VALUES (1,'ITF010',N'Formato de Hoja de Vida','IT_FORMATO_HOJA_DE_VIDA',1)
--(silao)
INSERT INTO [dbo].[IATF_documentos]([id_planta],[clave],[nombre_documento],[proceso],[activo])
     VALUES (2,'ITF010',N'Formato de Hoja de Vida','IT_FORMATO_HOJA_DE_VIDA',1)

	 ----------------REVISIONES-----------
	 --DBCC checkident ('IATF_revisiones', reseed,16);

--Revisiones (puebla)
INSERT INTO [dbo].[IATF_revisiones]([id_iatf_documento],[numero_revision],[responsable],[puesto_responsable],[fecha_revision],[descripcion])
     VALUES(7,1,'IT SISTEMAS',null,'2021-05-12','Alta del formato.')

INSERT INTO [dbo].[IATF_revisiones]([id_iatf_documento],[numero_revision],[responsable],[puesto_responsable],[fecha_revision],[descripcion])
     VALUES(7,2,'Alfredo Xochitemol',null,'2022-08-03','Se emite documento a través de portal web con clave ITF010-02')

--Revisiones (silao)
INSERT INTO [dbo].[IATF_revisiones]([id_iatf_documento],[numero_revision],[responsable],[puesto_responsable],[fecha_revision],[descripcion])
     VALUES(8,1,'IT SISTEMAS',null,'2021-05-12','Alta del formato.')

INSERT INTO [dbo].[IATF_revisiones]([id_iatf_documento],[numero_revision],[responsable],[puesto_responsable],[fecha_revision],[descripcion])
     VALUES(8,2,'Alfredo Xochitemol',null,'2022-08-03','Se emite documento a través de portal web con clave ITF010-02')


