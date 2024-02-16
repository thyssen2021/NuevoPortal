--use[Portal_2_0]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************
*  Tipo de objeto:     Alter table
*  Autor :  Alfredo Xochitemol
*  Fecha de Creación: 
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  09/11/2023	Alfredo Xochitemol		 Se agrega indicador de si es suma
******************************************************************************/

--BEGIN TRANSACTION
		IF object_id(N'budget_cuenta_sap','U') IS NOT NULL
		BEGIN			
			
			ALTER TABLE budget_cuenta_sap ADD aplica_formula bit NULL
			PRINT 'Se ha creado la columna aplica_formula en la tabla budget_cuenta_sap'		

			ALTER TABLE budget_cuenta_sap ADD formula [varchar](30) NULL
			PRINT 'Se ha creado la columna formula en la tabla budget_cuenta_sap'					
			
			ALTER TABLE budget_cuenta_sap ADD aplica_mxn bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'							

			ALTER TABLE budget_cuenta_sap ADD aplica_usd bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'

			ALTER TABLE budget_cuenta_sap ADD aplica_eur bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'

			ALTER TABLE budget_cuenta_sap ADD aplica_gastos_mantenimiento bit NOT NULL default 0
			PRINT 'Se ha creado la columna aplica_mxn en la tabla budget_cuenta_sap'


		END
		ELSE
		BEGIN
			PRINT 'La tabla budget_cuenta_sap NO EXISTE, no se puede crear las columnas'
		END

--COMMIT TRANSACTION


--update cuentas sap
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610010', name = 'Gastos de viaje - No Deducibles', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=1
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610030', name = 'Gastos de viaje - hospedaje', activo=1, aplica_formula=1, formula = '(a*b*c) + (d*e*f)', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=2
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610040', name = 'Gastos de viaje - comidas', activo=1, aplica_formula=1, formula = '(a*b*c) + (d*e*f)', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=3
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610070', name = 'Gastos de viaje - vuelos', activo=1, aplica_formula=1, formula = '(a*b)+(c*d)', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=4
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610071', name = 'Gastos de viaje - transporte terrestre', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=5
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610072', name = 'Gastos de viaje - renta de automovil', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=6
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610073', name = 'Gastos de Viaje - Casetas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=7
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610074', name = 'Gastos de Viaje - Combustible', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=8
UPDATE budget_cuenta_sap set id_mapping =1, sap_account= '610080', name = 'Gastos de viaje - varios', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=9
UPDATE budget_cuenta_sap set id_mapping =2, sap_account= '650030', name = 'Honorarios legales & profesionales', activo=1, aplica_formula=1, formula = 'a+b+c+d+e+f', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=10
UPDATE budget_cuenta_sap set id_mapping =3, sap_account= '650031', name = 'Honorarios de auditoria', activo=1, aplica_formula=1, formula = 'a+b+c+d', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=11
UPDATE budget_cuenta_sap set id_mapping =4, sap_account= '650032', name = 'Honorarios de consultoría', activo=1, aplica_formula=1, formula = 'a+b+c+d', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=12
UPDATE budget_cuenta_sap set id_mapping =5, sap_account= '610090', name = 'Capacitación', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=13
UPDATE budget_cuenta_sap set id_mapping =5, sap_account= '610091', name = 'Festejos', activo=1, aplica_formula=1, formula = '(a*b)+(c*d)+(e*f)+(g*h)', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=14
UPDATE budget_cuenta_sap set id_mapping =6, sap_account= '630060', name = 'Renta de telefono', activo=1, aplica_formula=1, formula = '(a*b)+c', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=15
UPDATE budget_cuenta_sap set id_mapping =6, sap_account= '630065', name = 'Comunicaciones', activo=1, aplica_formula=1, formula = '(a*b)+c', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=16
UPDATE budget_cuenta_sap set id_mapping =6, sap_account= '630066', name = 'Mantenimiento de sistema de telefono', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=17
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '630067', name = 'Otros gastos - EDI', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=18
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '650011', name = 'Software - IT', activo=1, aplica_formula=1, formula = 'a+b+c', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=19
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '650012', name = 'Software - tk', activo=1, aplica_formula=1, formula = 'a+b+c+d+e+f+g', aplica_mxn = 0, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=0 where id=20
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '650019', name = 'Redes', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=21
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '652000', name = 'Accesorios', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=22
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '652100', name = 'Hardware', activo=1, aplica_formula=1, formula = '(a*b)+(c*d)+e', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=23
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '652200', name = 'Mantenimiento IT', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=24
UPDATE budget_cuenta_sap set id_mapping =7, sap_account= '653000', name = 'Renta - IT', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=25
UPDATE budget_cuenta_sap set id_mapping =8, sap_account= '630040', name = 'Seguro', activo=1, aplica_formula=1, formula = 'a+b+c+d+e', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=26
UPDATE budget_cuenta_sap set id_mapping =8, sap_account= '706020', name = 'Gastos de automóvil - seguros', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=27
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '630050', name = 'Renta de oficina y edificio', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=28
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '650001', name = 'Mensajería', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=29
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '651000', name = 'Gastos de oficina', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=30
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '651010', name = 'Mobiliario de Oficina', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=31
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '651020', name = 'Mantenimiento de Oficinas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=32
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '651100', name = 'Gastos Administrativos RHQ Allocations', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=33
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '705040', name = 'Gastos de café', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=34
UPDATE budget_cuenta_sap set id_mapping =9, sap_account= '705050', name = 'Gastos de renta de equipo de la oficina', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=35
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '650018', name = 'TKMNA Consult  Exp', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=36
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '650002', name = 'Servicios de almacenamiento', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=37
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '612010', name = 'Gastos medicos', activo=1, aplica_formula=1, formula = 'a+b+c+d+e+f+g+h+i+j+k+l+m', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=38
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '650025', name = 'Infraestructura - IT', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=39
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '650040', name = 'Administrative charges C&B - Debit', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=40
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '650041', name = 'Administrative Charges C&B - Credit', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=41
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '651101', name = 'Servicio de administración de personal', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=42
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '660000', name = 'Otros gastos generales', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=43
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '660010', name = 'Suscripciones / libros', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=44
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '660011', name = 'Uniformes', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=45
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '660012', name = 'Servicio de Transporte', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=46
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '660020', name = 'Consumibles', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=47
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '690100', name = 'Derechos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=48
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700000', name = 'Cargos del banco', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=49
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700300', name = 'Recargos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=50
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700600', name = 'Interéses recibidos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=51
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700840', name = 'Gastos No Deducibles', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=52
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '704090', name = 'Gastos de cafetería', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=53
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '705020', name = 'Gastos de la oficina - Limpieza', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=54
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '706000', name = 'Gastos de automóvil - general', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=55
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '706010', name = 'Gastos de automóvil - reparaciones', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=56
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '704080', name = 'Gastos de cocina', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=57
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '704100', name = 'Gastos misceláneos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=58
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700310', name = 'Actualizaciones Y Multas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=59
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '704020', name = 'Obsequios de la empresa', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=60
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '700910', name = 'Predial - Detroit / Wayne County', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=61
UPDATE budget_cuenta_sap set id_mapping =11, sap_account= '630010', name = 'Electricidad', activo=1, aplica_formula=1, formula = 'a*b', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=62
UPDATE budget_cuenta_sap set id_mapping =11, sap_account= '630020', name = 'Agua', activo=1, aplica_formula=1, formula = '(a*b)+(c*d)', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=63
UPDATE budget_cuenta_sap set id_mapping =11, sap_account= '630025', name = 'Gas', activo=1, aplica_formula=1, formula = 'a+b', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=64
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '704110', name = 'Prueba del laboratorio & análisis', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=65
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '704200', name = 'Compra de Equipo de Medicion', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=66
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '704220', name = 'Calibraciones', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=67
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707000', name = 'Mantenimiento de edificios', activo=1, aplica_formula=1, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=1 where id=68
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707010', name = 'Mantenimiento de grúas', activo=1, aplica_formula=1, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=1 where id=69
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707020', name = 'Mantenimiento de montacargas', activo=1, aplica_formula=1, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=1 where id=70
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707021', name = 'Arrendamiento de montacargas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=71
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707030', name = 'Mantenimiento de maquinas', activo=1, aplica_formula=1, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=1 where id=72
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707040', name = 'Mantenimiento Varios', activo=1, aplica_formula=1, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=1 where id=73
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707050', name = 'Mantenimiento Troqueles', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=0 where id=74
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707062', name = 'Electrotecnica, Electronica', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=0 where id=75
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707063', name = 'Materiales De Instalacion Electrica', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=76
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707064', name = 'Cojinetes-Bearings', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=0 where id=77
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707065', name = 'Compresores', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=78
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707066', name = 'Reparacion de Rack''s Metalicos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=79
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707067', name = 'Rasquetas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=80
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707068', name = 'Reparacion de Bandas Transportadoras', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=81
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707100', name = 'Programas de Seguridad', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=82
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707110', name = 'Programas de Productividad', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=83
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707120', name = 'Prueba & análisis industriales', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=84
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707130', name = 'Equipo de Medición', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=85
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707140', name = 'Calibraciones', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=86
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708000', name = 'Artículos para el almacen - varios', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=87
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708010', name = 'Artículos para el almacen - formatos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=88
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708011', name = 'Consumibles para producción', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=89
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708020', name = 'Lubricantes, Aceites, Grasas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=90
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708021', name = 'Hidraulica Y Neumatica', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=1, aplica_gastos_mantenimiento=0 where id=91
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708022', name = 'Productos Textiles', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=92
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708040', name = 'Equipo de Protección Personal', activo=1, aplica_formula=1, formula = 'a+b+c', aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=93
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708050', name = 'Artículos para el almacen - herramientas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=94
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708060', name = 'Gastos del almacen - Seguridad', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=95
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '708070', name = 'Vigilancia', activo=1, aplica_formula=1, formula = 'a*b', aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=96
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708080', name = 'Gastos del almacen - Residuos Peligrosos', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=97
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708210', name = 'Gastos del almacen - limpieza', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=98
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708251', name = 'Herramientas de Oxicorte', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=99
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708252', name = 'Flejadoras', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=100
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708253', name = 'Sorteo de Material', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=101
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '709010', name = 'Señaletica', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=102
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '630030', name = 'Otros gastos de mantenimiento', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=103
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '708200', name = 'Almacén - Herramientas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=104
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707061', name = 'Elementos De Union', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=105
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707060', name = 'Mantenimiento a Tuberías', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=106
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '611020', name = 'Programas de Seguridad', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 0, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=107
UPDATE budget_cuenta_sap set id_mapping =13, sap_account= '708120', name = 'Flete - Traspaso entre Plantas', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=108
UPDATE budget_cuenta_sap set id_mapping =13, sap_account= '708160', name = 'Flete Salida Puebla', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=109
UPDATE budget_cuenta_sap set id_mapping =13, sap_account= '708161', name = 'Flete Salida Silao', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=110
UPDATE budget_cuenta_sap set id_mapping =13, sap_account= '708162', name = 'Flete Salida Saltillo', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=111
UPDATE budget_cuenta_sap set id_mapping =14, sap_account= '708170', name = 'Compra de Polines y Racks de Madrera', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=112
UPDATE budget_cuenta_sap set id_mapping =14, sap_account= '708180', name = 'Gastos del almacen - fleje', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=113
UPDATE budget_cuenta_sap set id_mapping =14, sap_account= '708190', name = 'Materiales Para Embalaje', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 1, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=114
UPDATE budget_cuenta_sap set id_mapping =12, sap_account= '707001', name = 'Rent Maintenance', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=115
UPDATE budget_cuenta_sap set id_mapping =10, sap_account= '612029', name = 'Gastos sindicales', activo=1, aplica_formula=NULL, formula = NULL, aplica_mxn = 1, aplica_usd= 0, aplica_eur=0, aplica_gastos_mantenimiento=0 where id=116

