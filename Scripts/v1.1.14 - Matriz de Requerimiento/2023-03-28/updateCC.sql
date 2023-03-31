-- 1.- Actualiza los centros de costo
Update area set numero_centro_costo='4403' WHERE clave =2
Update area set numero_centro_costo='4401' WHERE clave =3
Update area set numero_centro_costo='4800' WHERE clave =4
Update area set numero_centro_costo='4402' WHERE clave =5
Update area set numero_centro_costo='4400' WHERE clave =6
Update area set numero_centro_costo='4700' WHERE clave =7
Update area set numero_centro_costo='4404' WHERE clave =8
Update area set numero_centro_costo='4300' WHERE clave =10
Update area set numero_centro_costo='4890' WHERE clave =11
Update area set numero_centro_costo='4500' WHERE clave =12
Update area set numero_centro_costo='4900' WHERE clave =13
Update area set numero_centro_costo='4600' WHERE clave =14
Update area set numero_centro_costo='4211' WHERE clave =17
Update area set numero_centro_costo='4000' WHERE clave =19
Update area set numero_centro_costo='4230' WHERE clave =20
Update area set numero_centro_costo='4310' WHERE clave =24
Update area set numero_centro_costo='4130' WHERE clave =25
Update area set numero_centro_costo='4212' WHERE clave =26
Update area set numero_centro_costo='4120' WHERE clave =27
Update area set numero_centro_costo='6130' WHERE clave =29
Update area set numero_centro_costo='6300' WHERE clave =30
Update area set numero_centro_costo='6400' WHERE clave =31
Update area set numero_centro_costo='6401' WHERE clave =32
Update area set numero_centro_costo='6800' WHERE clave =33
Update area set numero_centro_costo='6403' WHERE clave =34
Update area set numero_centro_costo='6402' WHERE clave =35
Update area set numero_centro_costo='6404' WHERE clave =36
Update area set numero_centro_costo='6600' WHERE clave =37
Update area set numero_centro_costo='6000' WHERE clave =38
Update area set numero_centro_costo='6500' WHERE clave =39
Update area set numero_centro_costo='6700' WHERE clave =40
Update area set numero_centro_costo='6230' WHERE clave =41
Update area set numero_centro_costo='6890' WHERE clave =42
Update area set numero_centro_costo='6211' WHERE clave =43
Update area set numero_centro_costo='6310' WHERE clave =44
Update area set numero_centro_costo='6120' WHERE clave =45
Update area set numero_centro_costo='8500' WHERE clave =46
Update area set numero_centro_costo='8000' WHERE clave =47
Update area set numero_centro_costo='8700' WHERE clave =48
Update area set numero_centro_costo='7130' WHERE clave =50

--2. Actualiza los deptos Shared services
Update area set numero_centro_costo='3100', shared_services='1' WHERE clave =1
Update area set numero_centro_costo='3110', shared_services='1' WHERE clave =9
Update area set numero_centro_costo='3240', shared_services='1' WHERE clave =15
Update area set numero_centro_costo='3320', shared_services='1' WHERE clave =16
Update area set numero_centro_costo='3140', shared_services='1' WHERE clave =18
Update area set numero_centro_costo='3200', shared_services='1' WHERE clave =21
Update area set numero_centro_costo='3220', shared_services='1' WHERE clave =22
Update area set numero_centro_costo='3300', shared_services='1' WHERE clave =23
Update area set numero_centro_costo='8000', shared_services='1' WHERE clave =28
Update area set numero_centro_costo='7300', shared_services='0' WHERE clave =49
Update area set numero_centro_costo='9100', shared_services='1' WHERE clave =51

--3. elimina duplicado de diana
--20383
delete from empleados where id=503
update empleados set numeroEmpleado='20383', [8ID]='10714153', sexo='F' where id=490

-- 4.- Actualiza los jefes directos
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10256438') where [8ID]='10383874'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where id='410'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10520023') where [8ID]='10509751'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10520023') where [8ID]='10509753'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10681847') where [8ID]='10512933'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10518413') where [8ID]='10520023'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10619223'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10631420'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10660784'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10440477') where [8ID]='10681014'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10477806') where [8ID]='10681837'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681879'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681880'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681881'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681882'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681883'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681886'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681887'
UPDATE empleados set id_jefe_directo=(SELECT TOP 1 id from empleados where [8ID]='10351408') where [8ID]='10681888'
