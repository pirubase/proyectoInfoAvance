using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;

namespace mecanico_plus.Data
{
    public class local : DbContext 
    {

        public local(DbContextOptions<local> options) : base(options)
        {
            
        }

        public DbSet<mecanico_plus.Data.t001_usuario> t001_usuario { get; set; }
        public DbSet<mecanico_plus.Data.t004_perfil> t004_perfil { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t005_menu> t005_menu { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t003_permisos> t003_permisos { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t002_empresa_o_persona_natural> t002_empresa_o_persona_natural { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t006_mecanico> t006_mecanico { get; set; } = default!;

        public DbSet<mecanico_plus.Data.t007_cliente> t007_cliente { get; set; } = default!;

        public DbSet<mecanico_plus.Data.t008_estados_usuario> t008_estados_usuario { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t009_cita> t009_cita { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t010_vehiculo> t010_vehiculo { get; set; } = default!;
  

         public DbSet<mecanico_plus.Data.t012_token> t012_token { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t013_documento> t013_documento { get; set; } = default!;
        public DbSet<mecanico_plus.Data.t014_servicio> t014_servicio { get; set; } = default!;
     


        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // ...existing code...
        //     modelBuilder.Entity<t012_token>()
        //         .HasOne(t => t.vObjCita) // Ajustar la propiedad de navegación si difiere
        //         .WithMany() // O .WithMany(c => c.t012_tokenCollection) si tienes colección
        //         .HasForeignKey(t => t.f012_rowid_cita)
        //         .OnDelete(DeleteBehavior.Cascade);
        //     // ...existing code...
        // }
    }
}
