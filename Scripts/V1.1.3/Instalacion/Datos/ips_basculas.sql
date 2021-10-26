use Portal_2_0

--actualiza las ip de las lineas de produccion

UPDATE produccion_lineas set ip='10.122.162.190' where id = 1; --blk 1 puebla
UPDATE produccion_lineas set ip='10.122.162.191' where id = 2; --blk 2 puebla
UPDATE produccion_lineas set ip='10.122.162.192' where id = 3; --blk 3 puebla
UPDATE produccion_lineas set ip='10.121.24.190' where id = 7; --blk 1 silao
UPDATE produccion_lineas set ip='10.121.24.182' where id = 8; --blk 2 silao
UPDATE produccion_lineas set ip='10.121.24.192' where id = 9; --blk 3 silao
