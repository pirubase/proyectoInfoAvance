-- script_inserta_datos_base_by_company.sql

DO $$
DECLARE
    rowid_empresa INT;
    rowid_perfil_generado INT;
    rowid_usuario_generado INT;

    -- Variables de Empresa
    razon_social TEXT := 'Company1';
    nit TEXT := '9016437836';
    alcance TEXT := 'Privada';
    cantidad_empleados INT := 5;
   

    -- Variables de Usuario
    id_usuario TEXT := 'user1';
    clave TEXT := 'sefNlhNLsEpzFE0UUmrW7Q==';
    nombres TEXT := 'prueba';
    apellidos TEXT := 'mecanico plus';
    correo_usuario TEXT := 'sebastian_arcila2010@hotmail.com';
    numero_celular TEXT := '123456789';
    estado_activo INT := 1;

BEGIN
    -- Insercion en t002_empresa_o_persona_natural
    INSERT INTO t002_empresa_o_persona_natural (
        f002_ts, f002_razon_social, f002_nit, f002_alcance, f002_cantidad_empleados
    )
    VALUES (
        current_date, razon_social, nit, alcance, cantidad_empleados
    )
    RETURNING f002_rowid INTO rowid_empresa;

    -- Verificar si el perfil "Administrador" ya existe
    SELECT f004_rowid INTO rowid_perfil_generado
    FROM t004_perfil
    WHERE f004_nombre = 'Administrador';

    -- Si no existe, insertar el perfil
    IF NOT FOUND THEN
        INSERT INTO t004_perfil (
            f004_ts, f004_id, f004_nombre, f004_descripcion
        )
        VALUES (
            current_date, 1, 'Administrador', 'Perfil Administrador'
        )
        RETURNING f004_rowid INTO rowid_perfil_generado;
    END IF;

    -- Insercion en t001_usuario
    INSERT INTO t001_usuario (
        f001_ts, f001_id_usuario, f001_clave, f001_nombres, f001_apellidos, f001_correo_electronico, f001_numero_celular, f001_rowid_estado, f001_rowid_perfil, f001_rowid_empresa_o_persona_natural
    )
    VALUES (
        current_date, id_usuario, clave, nombres, apellidos, correo_usuario, numero_celular, estado_activo, rowid_perfil_generado, rowid_empresa
    )
    RETURNING f001_rowid INTO rowid_usuario_generado;

    -- Permisos del Usuario
    INSERT INTO t003_permisos (
        f003_ts, f003_id, f003_permiso_consultar, f003_permiso_crear, f003_permiso_editar, f003_permiso_detalle, f003_permiso_eliminar, f003_permiso_uso_menu, f003_rowid_menu, f003_rowid_perfil, f003_rowid_empresa_o_persona_natural
    )
    VALUES 
    (current_date, 1, true, true, true, true, true, true, 1, rowid_perfil_generado, rowid_empresa),
    (current_date, 2, true, true, true, true, true, true, 2, rowid_perfil_generado, rowid_empresa),
    (current_date, 3, true, true, true, true, true, true, 3, rowid_perfil_generado, rowid_empresa),
    (current_date, 4, true, true, true, true, true, true, 4, rowid_perfil_generado, rowid_empresa),
    (current_date, 5, true, true, true, true, true, true, 5, rowid_perfil_generado, rowid_empresa),
    (current_date, 6, true, true, true, true, true, true, 6, rowid_perfil_generado, rowid_empresa),
    (current_date, 7, true, true, true, true, true, true, 7, rowid_perfil_generado, rowid_empresa),
    (current_date, 8, true, true, true, true, true, true, 8, rowid_perfil_generado, rowid_empresa),
    (current_date, 9, true, true, true, true, true, true, 9, rowid_perfil_generado, rowid_empresa),
    (current_date, 10, true, true, true, true, true, true, 10, rowid_perfil_generado, rowid_empresa);

    INSERT INTO t014_servicio (f014_ts, f014_id, f014_nombre, f014_valor,f014_rowid_empresa_o_persona_natural)
    VALUES 
    (current_date, 1, 'Cambio de aceite', 25000,rowid_empresa),
    (current_date, 2, 'Alineación', 500000,rowid_empresa),
    (current_date, 3, 'Pintura', 600000,rowid_empresa),
    (current_date, 4, 'Reparación de frenos', 410000,rowid_empresa),
    (current_date, 5, 'Cambio de neumáticos', 300000,rowid_empresa),
    (current_date, 6, 'Revisión general', 150000,rowid_empresa),
    (current_date, 7, 'Reparación de motor', 120000,rowid_empresa),
    (current_date, 8, 'Reparación de transmisión',1500000,rowid_empresa),
    (current_date, 9, 'Reparación de suspensión', 400000,rowid_empresa),
    (current_date, 10, 'Diagnóstico electrónico',350000,rowid_empresa),
    (current_date, 11, 'Cambio de batería', 600000,rowid_empresa),
    (current_date, 12, 'Reparación de aire acondicionado', 550000,rowid_empresa),
    (current_date, 13, 'Reparación de dirección', 120000,rowid_empresa),
    (current_date, 14, 'Reparación de escape',110000,rowid_empresa),
    (current_date, 15, 'Reparación de radiador', 1500000,rowid_empresa);

END $$;