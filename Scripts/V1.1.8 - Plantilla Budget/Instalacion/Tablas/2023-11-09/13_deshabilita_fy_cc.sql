/*****************************************
* Se dehabilita la edici�n de todos los centros de costo, se habilitar�n
* el a�o fiscal seg�n las indicaciones de Carmen
******************************************/

--Habilita CeCo
update budget_rel_fy_centro set estatus=1
--Deshabilita CeCo
update budget_rel_fy_centro set estatus=0
