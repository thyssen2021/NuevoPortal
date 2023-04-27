use Portal_2_0;
-- DROP TABLE #stats_ddl

DECLARE @count INT;

CREATE TABLE #stats_ddl(
	[clave] [int] NULL,
	[capturaFecha] [smalldatetime] NULL,
	[usuarioClave] [int] NULL,
	[remisionCabeceraClave] [int] NULL,
	[catalogoEstatusClave] [int] NULL,
	[texto] [varchar](1000) NULL,	
	);

INSERT INTO #stats_ddl 
SELECT clave,capturaFecha, usuarioClave,remisionCabeceraClave,catalogoEstatusClave,texto FROM Portalthyssenkrupp.remision.RemisionEstatus order by clave asc

SET IDENTITY_INSERT [dbo].[RM_cambio_estatus] ON 

SELECT @count = COUNT(*) FROM #stats_ddl;

WHILE @count > 0
BEGIN

DECLARE @id_user INT = (SELECT TOP(1) usuarioClave FROM #stats_ddl);
DECLARE @user_old varchar(50) = (SELECT TOP(1) nombre FROM Portalthyssenkrupp.remision.Usuario WHERE clave=@id_user);
DECLARE @id_empleado int = NULL;

SET @id_empleado =  (SELECT CASE @id_user
					-- 1 no encontrado Obsoleto (ISRAEL VERA VERA)-No Usar
					When 2 Then  149 --JULIO CESAR CUAUTLE FLORES
					When 3 Then  163 --JESUS ANDRES LOPEZ
					When 4 Then  67 -- RUBEN VEGA
					When 5 Then  76 -- MARCO ANTONIO COYOTZI 
					When 6 Then  103 --ERICK DIAZ REYES
					When 7 Then 161 --JOSE ARMANDO CORDOVA
					When 8 Then  159 -- SERGIO FLORES SÁNCHEZ
					-- 9 no encontrado	ROBERTO ALVAREZ MARROQUIN
					-- 10 no encontrado	ROBERTO GONZALEZ MORALES
					When 11 Then  190 --ALEJANDRO ISMAEL HERRERA RODRIGUEZ
					When 12 Then  191 --ANDRES MARQUEZ JIMENEZ
					When 13 Then  192 --JUAN DORANTES GOMEZ
					When 14 Then 309  --BALTAZAR CHAGOYA AGUILAR 
					-- 15 no encontrado		GERARDO SERRANO RODRIGUEZ 
					When 16 Then 335  --VICTOR MANUEL HERNANDEZ RUIZ 
					--17 borrado de BD
					When 18 Then 377  --LINO ALCANTAR CADENA 
					When 19 Then 313  --ALEJANDRO HERNANDEZ VELÁZQUEZ 
					When 20 Then  320 --ALEJANDRO CANO RANGEL 
					When 21 Then 322  --FRANCISCO EDUARDO HERNANDEZ PEREZ 
					--22 al 38 borrado de BD
					When 39 Then  425 --JORGE GUADALUPE SALINAS GALVAN
					When 40 Then 427  --JUAN ANTONIO HUERTA YEVERINO
					When 41 Then  428 --ERICK FABIAN VILLEGAS DE LA PEÑA
					-- 42 no encontrado	KARLA ESTRELLITA ALVARADO  AGUILAR
					When 43 Then 426  --LUIS ARMANDO FERNANDEZ RANGEL
					When 44 Then  326 --PEDRO DANIEL SORIA GONZALEZ
					-- 45 no encontrado	SARAI MAGALLAN GUTIERREZ
					When 46 Then 407   --ALVARO IRVIN COLEOTE GARCIA
					--47 borrado de BD
					When 48 Then 405  --VIVIANA ELIZABETH LLANAS ALFARO
					When 49 Then  408 --ELIZABETH LANDOIS
					--50 borrado de BD
					When 51 Then 406  --ELEAZAR MORENO
					--52 borrado de BD
					--53 no encontrado	HECTOR NOLASCO
					When 54 Then 429  --LILIA VANESSA LIRA GARCIA
					When 55 Then 176  --USIEL OTAÑEZ NARVAEZ
					When 2055 Then 430  --ROBERTO MUZQUIZ DURAN
					When 2056 Then 255  --PABLO SANCHEZ
					When 2057 Then  152 -- ALFONSO MIRON LOPEZ

					ELSE @id_empleado
	END)

INSERT INTO [dbo].[RM_cambio_estatus]
           (
		   [clave]
		   ,[capturaFecha]
           ,[id_empleado]
		   ,[nombre_usuario_old]
           ,[remisionCabeceraClave]
           ,[catalogoEstatusClave]
           ,[texto])
     VALUES
           ((SELECT TOP(1) clave FROM #stats_ddl)
		   ,(SELECT TOP(1) capturaFecha FROM #stats_ddl)
           ,@id_empleado
           ,@user_old
		   ,(SELECT TOP(1) remisionCabeceraClave FROM #stats_ddl)
           ,(SELECT TOP(1) catalogoEstatusClave FROM #stats_ddl)
           ,(SELECT TOP(1) texto FROM #stats_ddl)
		   )
    DELETE TOP (1) FROM #stats_ddl
    SELECT @count = COUNT(*) FROM #stats_ddl 
END

SET IDENTITY_INSERT [dbo].[RM_cambio_estatus] OFF
DROP TABLE #stats_ddl;
