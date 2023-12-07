--Actualiza como posteado a todos los registros de planta puebla
update produccion_datos_entrada set posteado=1  where (select top 1 clave_planta from produccion_registros where id = produccion_datos_entrada.id_produccion_registro )= 1


--select * from produccion_datos_entrada as de where (select top 1 clave_planta from produccion_registros where id = de.id_produccion_registro )= 1

--select * from produccion_datos_entrada
