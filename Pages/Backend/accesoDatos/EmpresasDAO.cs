using mecanico_plus.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System;
using mecanico_plus.Pages.Backend.Conexion;
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Backend.accesoDatos
{
    public class EmpresasDAO
    {

        private local _context;

        private ConexionDB conexion;

        public IList<t002_empresa_o_persona_natural> t002_empresas { get; set; }

        private static String idusuario = String.Empty;

        public EmpresasDAO()
        {
            conexion = new ConexionDB();

            //creacion del contex
            var optionsBuilder = new DbContextOptionsBuilder<local>();
            optionsBuilder.UseNpgsql(conexion.ObtieneCadenaConexion());

            _context = new local(optionsBuilder.Options);
        }

        public async Task<bool> actualizaEmpresaSeleccionada(int empresaSeleccionada, String correoUsuario)
        {

            try
            {

                var sql = @"UPDATE t001_usuario
                            SET    f001_rowid_empresa_o_persona_natural=@empresaSeleccionada
                            WHERE  f001_correo_electronico = @correoUsuario";

                var db = conexion.conexionBdDirecta();
                var resultado = await db.ExecuteAsync(sql, new { empresaSeleccionada, correoUsuario });

                return resultado > 0;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> actualizaEmpresaSeleccionadaPorNit(string nit, String correoUsuario)
        {
            //String[] nitEmpresaAmbiente = nit.Split("_");
            try
            {
                t002_empresa_o_persona_natural t002_empresas = new t002_empresa_o_persona_natural();

                t002_empresas = await _context.t002_empresa_o_persona_natural
                                        .Where(x => x.f002_nit.Equals(nit))
                                        .FirstOrDefaultAsync();

                int rowidEmpresa = t002_empresas.f002_rowid;

                var sql = @"UPDATE t001_usuario
                            SET    f001_rowid_empresa_o_persona_natural=@rowidEmpresa
                            WHERE  f001_correo_electronico = @correoUsuario";

                var db = conexion.conexionBdDirecta();
                var resultado = await db.ExecuteAsync(sql, new { rowidEmpresa, correoUsuario });

                return resultado > 0;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<int> obtieneEmpresaSeleccionada(String usuario)
        {
            try
            {
                idusuario = usuario;

                IList<t001_usuario> t001_usuario = await _context.t001_usuario
                                                                    .Where(x => x.f001_correo_electronico == idusuario)
                                                                    .ToListAsync();
                return t001_usuario[0].f001_rowid_empresa_o_persona_natural;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<t002_empresa_o_persona_natural>> obtieneEmpresaPorNIT(string identificacionEmpresa)
        {
            //String[] nitEmpresaAmbiente = identificacionEmpresa.Split("_");
            try
            {
                IList<t002_empresa_o_persona_natural> t002_empresas = await _context.t002_empresa_o_persona_natural
                                                    .Where(x => x.f002_nit.Equals(identificacionEmpresa))
                                                    .ToListAsync();

                return t002_empresas;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
