USE Portal_2_0
GO
IF object_id(N'currency',N'U') IS NOT NULL
	BEGIN
    DROP TABLE [dbo].[currency]
      PRINT '<<< currency en Base de Datos:' + db_name() + 
	  ' en el Servidor:' + @@servername + ' se ha ELIMINADO! >>>'   
	END
GO

/*****************************************************************************
*  Tipo de objeto:      Table
*  Funcion: Crea una tabla para guardar los currency
*  Autor :  Alfredo Xochitemol	Cruz
*  Fecha de Creación: 2021/12/09
*  Log de Mantenimiento: 
*  Date         Modified By       Description            
*  ----------   ---------------   -------------------------------------
******************************************************************************/


CREATE TABLE [currency](
	[CurrencyISO] VARCHAR(3)  DEFAULT NULL,
  [CurrencyName] VARCHAR(35)  DEFAULT NULL,
  [Money] VARCHAR(30)  DEFAULT NULL,
  [Symbol] VARCHAR(3) DEFAULT NULL,
  [LimitePoliza] DECIMAL(14,2) NULL,
  [activo] bit NOT NULL
 CONSTRAINT [PK_currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyISO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- restricción default
ALTER TABLE [currency] ADD  CONSTRAINT [DF_currency_activo]  DEFAULT (1) FOR [activo]
GO

--Inserta los datos
INSERT INTO [dbo].[currency]
           ([CurrencyISO]
           ,[CurrencyName]
           ,[Money]
           ,[Symbol]
		   )
     VALUES 
   ('AFN','Afghani', 'افغانۍ', '؋'),
    ('THB','Baht', 'บาทไทย', '฿'),
    ('PAB','Balboa', 'Balboa', 'B'),
    ('ETB','Birr Etiopía', 'Birr', 'B'),
    ('VEF','Bolivar Fuerte', 'Bolívar', 'B'),
    ('BOB','Boliviano', 'Boliviano', 'B'),
    ('BND','Brunei Dólar', 'Ringgit', 'B'),
    ('GHS','Cedi', 'Cedi', 'G'),
    ('CRC','Colón Costa Rican', 'Colón', '₡'),
    ('SVC','Colón Salvadoreño', 'Colón', '₡'),
    ('NIO','Cordoba Oro', 'Córdoba', 'C'),
    ('DKK','Corona Danesa', 'Corona', 'K'),
    ('ISK','Corona Islandia', 'Corona', 'k'),
    ('NOK','Corona Noruega', 'Corona', 'k'),
    ('SEK','Corona Suiza', 'Corona', 'k'),
    ('GMD','Dalasi', 'Dalasi', 'D'),
    ('MKD','Dinar', 'Денар', 'д'),
    ('DZD','Dinar Algeria', 'دينار', 'د'),
    ('BHD','Dinar Bahrainí', 'دينار', '.'),
    ('LYD','Dinar de Libia', 'دينار', 'ل'),
    ('RSD','Dinar de Serbian', 'Динар', 'д'),
    ('TND','Dinar de Tunisia', 'دينار', 'د'),
    ('IQD','Dinar Iraqi', 'دينار', 'ع'),
    ('JOD','Dinar Jordano', 'دينار', 'د'),
    ('KWD','Dinar Kuwaití', 'دينار', 'د'),
    ('MAD','Dirham de Moroco', 'درهم', 'د'),
    ('DJF','Djibouti Franco', 'الفرنك', 'F'),
    ('STD','Dobra', 'Dobra', 'D'),
    ('USD','Dólar Americano', 'Dólar', '$'),
    ('AUD','Dólar Australiano', 'Dólar', '$'),
    ('BSD','Dólar Bahamas', 'Dólar', 'B'),
    ('BBD','Dólar Barbados', 'Dólar', 'B'),
    ('BZD','Dólar Belize', 'Dólar', 'B'),
    ('BMD','Dólar Bremuda', 'Dólar', 'B'),
    ('CAD','Dólar Canadiense', 'Dólar', '$'),
    ('GYD','Dólar de Guyana', 'Dólar', 'G'),
    ('NAD','Dólar de Namibia', 'Dólar', 'N'),
    ('ZWL','Dólar de Zimbabwe', 'Dólar', '$'),
    ('XCD','Dólar del Este Caribeño', 'Dólar', 'E'),
    ('FJD','Dólar Fiji', 'Dólar', 'F'),
    ('HKD','Dólar Hong Kong', '香港圓', 'H'),
    ('KYD','Dólar Islas Caimán', 'Dólar', '$'),
    ('SBD','Dólar Islas Salomón', 'Dólar', 'S'),
    ('JMD','Dólar Jamaiquino', 'Dólar', '$'),
    ('LRD','Dólar Liberiano', 'Dólar', 'L'),
    ('NZD','Dólar Nueva Zelanda', 'Dólar', '$'),
    ('SGD','Dólar Singapur', '新加坡元', 'S'),
    ('SRD','Dólar Surinam', 'Dólar', '$'),
    ('TWD','Dólar Taiwanés', '新臺幣', '$'),
    ('TTD','Dólar Trinidad y Tobago', 'Dólar', '$'),
    ('VND','Dong', 'đồng', '₫'),
    ('AMD','Dram Armenio', 'Դրամ', 'դ'),
    ('CVE','Escudo Cabo Verde', 'Escudo', 'E'),
    ('EUR','Euro', 'Euro', '€'),
    ('HUF','Florín Húgaro', 'Forín', 'F'),
    ('XAF','Franco CFA', 'Franco', 'C'),
    ('XOF','Franco CFA', 'Franco', 'C'),
    ('XPF','Franco CFA', 'Franco', 'F'),
    ('CDF','Franco Congolés', 'Franco', 'F'),
    ('KMF','Franco de Comoro', 'Franco', 'F'),
    ('GNF','Franco de Guinea', 'Franco', 'F'),
    ('RWF','Franco Ruandés', 'Franco', 'R'),
    ('CHF','Franco Suizo', 'Franco', 'F'),
    ('BIF','Francoo de Burundi', 'Franco', 'F'),
    ('HTG','Gourde', 'Gourde', 'G'),
    ('PYG','Guarani', 'Guaraní', '₲'),
    ('ANG','Guilder Antillas Nerlandesas', 'Gulden', 'N'),
    ('AWG','Guilder de Aruba', 'Florijn', 'A'),
    ('UAH','Hryvnia', 'Гривня', '₴'),
    ('PGK','Kina', 'Kina', 'K'),
    ('LAK','Kip', 'ກີບ', '₭'),
    ('CZK','Koruna Checa', 'Koruna', 'K'),
    ('EEK','Kroon', 'Kroon', 'K'),
    ('HRK','Kuna Croatí', 'Kuna', 'k'),
    ('MWK','Kwacha', 'Kwacha', 'M'),
    ('ZMK','Kwacha de Zambia', 'Kwacha', 'Z'),
    ('AOA','Kwanza', 'Kwanza', 'K'),
    ('MMK','Kyat', 'Kyat', 'K'),
    ('GEL','Lari', 'ლარი', 'l'),
    ('LVL','Latvian Lats', 'Lats', 'L'),
    ('ALL','Lek', 'Leku', 'L'),
    ('HNL','Lempira', 'Lempira', 'L'),
    ('SLL','Leone', 'Leone', 'L'),
    ('MDL','Leu de Moldovia', 'Leu', 'l'),
    ('BGN','Lev Búlgaro', 'лев', 'л'),
    ('SHP','Libra de Santa Eelena', 'Libra', '£'),
    ('SDG','Libra de Sudán', 'جنيه', '£'),
    ('EGP','Libra Egipcia', 'الجنيه', '£'),
    ('GIP','Libra Gibraltar', 'Libra', '£'),
    ('FKP','Libra Islas Falkland', 'Libra', '£'),
    ('LBP','Libra Libanesa', 'ليرة,', 'ل'),
    ('SYP','Libra Siria', 'الليرة', 'S'),
    ('GBP','Libra Sterling', 'Libra', '£'),
    ('SZL','Lilangeni', 'Lilangeni', 'L'),
    ('TRY','Lira Turca', 'Lira', 'T'),
    ('LTL','Litas Lutianesa', 'Litas', 'L'),
    ('LSL','Loti', 'Loti', 'L'),
    ('MGA','Malagasy Ariary', 'Ariary', 'F'),
    ('TMT','Manat', 'Манат', 'm'),
    ('AZN','Manat Azerbaijanian', 'Manatı', 'м'),
    ('BAM','Marcos Convertibles', 'Marka', 'K'),
    ('MZN','Metical', 'Metical', 'M'),
    ('NGN','Naira', 'Naira', '₦'),
    ('ERN','Nakfa', 'Nakfa', 'N'),
    ('BTN','Ngultrum', 'དངུལ་ཀྲམ', 'N'),
    ('RON','Nuevo Leu', 'Leu', 'L'),
    ('PEN','Nuevo Sol', 'Sol', 'S'),
    ('MRO','Ouguiya', 'أوقية', 'U'),
    ('TOP','Paanga', 'Paanga', 'T'),
    ('MOP','Pataca', '澳門圓', 'M'),
    ('ARS','Peso Argentino', 'Peso', '$'),
    ('CLP','Peso Chileno', 'Peso', '$'),
    ('COP','Peso Colombiano', 'Peso', 'C'),
    ('CUP','Peso Cubano', 'Peso', '$'),
    ('DOP','Peso Dominicano', 'Peso', 'R'),
    ('PHP','Peso Filipino', 'Piso', '₱'),
    ('GWP','Peso Guinea-Bissau', 'Peso', '$'),
    ('MXN','Peso Mexicano', 'Peso', '$'),
    ('UYU','Peso Uruguayo', 'Peso', '$'),
    ('BWP','Pula', 'Pula', 'P'),
    ('QAR','Qatari Rial', 'ريال', 'ر'),
    ('GTQ','Quetzal', 'Quetzal', 'Q'),
    ('ZAR','Rand', 'Rand', 'R'),
    ('BRL','Real Brasileño', 'Real', 'R'),
    ('IRR','Rial Iraní', '﷼', '﷼'),
    ('OMR','Rial Omani', 'ريال', 'ر'),
    ('YER','Rial Yemení', 'ريال', 'ر'),
    ('KHR','Riel', 'Riel', 'r'),
    ('MYR','Ringgit de Malasia', 'ريڠڬيت', 'R'),
    ('SAR','Riyal Saudí', 'ريال', 'ر'),
    ('BYR','Rublo Bieloruso', 'Рубель', 'B'),
    ('RUB','Rublo Ruso', 'Рубль', 'р'),
    ('MVR','Rufiyaa', 'Rufiyaa', 'R'),
    ('IDR','Rupia', 'Rupia', 'R'),
    ('SCR','Rupia de Seychelles', 'Rupia', 'S'),
    ('INR','Rupia Indú', 'Rupia', 'R'),
    ('MUR','Rupia Mauritius', 'Rupia', 'R'),
    ('NPR','Rupia Nepalesa', 'Rupia', 'N'),
    ('PKR','Rupia Pakistaní', 'Rupia', 'R'),
    ('LKR','Rupia Sri Lanka', 'Rupia', 'R'),
    ('ILS','Sheqel Israeli', 'שקל', '₪'),
    ('TZS','Shilling de Tanzania', 'Shilingi', '/'),
    ('UGX','Shilling de Uganda', 'Shilling', 'U'),
    ('KES','Shilling Kenya', 'Shilling', 'K'),
    ('SOS','Shilling Somalí', 'Shilin', 'S'),
    ('KGS','Som', 'сом', 'с'),
    ('TJS','Somoni', 'Сомонӣ', 'С'),
    ('UZS','Sum Uzbekistán', 'Сўм', 'с'),
    ('BDT','Taka', 'টাকা', '৳'),
    ('WST','Tala', 'Tālā', 'W'),
    ('KZT','Tenge', 'Теңгесі', '〒'),
    ('MNT','Tugrik', 'Төгрөг', '₮'),
    ('AED','UAE Dirham', 'درهم', 'د'),
    ('VUV','Vatu', 'Vatu', 'V'),
    ('KRW','Won', '원', '₩'),
    ('KPW','Won Nor-Coreano', '원', '₩'),
    ('JPY','Yen Japonés', '日本円', '¥'),
    ('CNY','Yuan Chino', '人民币', '¥'),
    ('PLN','Zloty Polaco', 'Złoty', 'z')
	;

	--establece los limites de las pólizas
	UPDATE [dbo].[currency] SET LimitePoliza= 100000.00 WHERE CurrencyISO='USD';
	UPDATE [dbo].[currency] SET LimitePoliza= 90000.00 WHERE CurrencyISO='EUR';
	UPDATE [dbo].[currency] SET LimitePoliza= 2000000.00 WHERE CurrencyISO='MXN';
 	  
IF object_id(N'currency',N'U') IS NOT NULL
	BEGIN
		PRINT '<<< currency en Base de Datos:' + db_name() + 
		' en el Servidor:' + @@servername + ' se ha creado con exito! >>>'
	END
ELSE
	BEGIN
	      PRINT '<<< No ha sido posible crear la tabla currency  ' +
		  'en la Base de Datos: ' + db_name() + ' en el Servidor: ' + @@servername + '  >>>'
	END
	
SET ANSI_PADDING OFF
GO
