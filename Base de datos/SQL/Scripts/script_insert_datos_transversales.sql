-- script_insert_datos_transversales.sql

-- Estados de Usuario
INSERT INTO t008_estados_usuario (f008_id, f008_nombre_estado)
VALUES 
    (1, 'Activo'),
    (2, 'Inactivo');

-- Perfiles
INSERT INTO t004_perfil (f004_ts, f004_id, f004_nombre, f004_descripcion)
VALUES 
    (current_date, 1, 'Administrador', 'Perfil con acceso completo'),

    (current_date, 2, 'Cliente', 'Perfil para gestion cliente');


-- Menus
INSERT INTO t005_menu (f005_ts, f005_id, f005_nombre, f005_descripcion)
VALUES 
    (current_date, 1, 'usuarios', 'Gestion de usuarios'),
    (current_date, 2, 'perfiles', 'Gestion de perfiles'),
    (current_date, 3, 'permisos', 'Gestion de permisos'),
    (current_date, 4, 'mecanicos', 'Gestion de mecanicos'),
    (current_date, 5, 'menus', 'Gestion de menus'),
    (current_date, 6, 'clientes', 'Gestion de clientes'),
    (current_date, 7, 'vehiculos', 'Gestion de vehiculos'),
    (current_date, 8, 'citas', 'Gestion de citas'),
    (current_date, 9, 'calendarios', 'Gestion de calendarios'),
    (current_date, 10, 'servicios', 'Gestion de servicios'),
    (current_date, 11, 'calendario mecanicos', 'Gestion de calendario mecanicos');

    
    
    