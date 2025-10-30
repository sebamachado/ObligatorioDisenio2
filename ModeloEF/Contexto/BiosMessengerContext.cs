using System.Data.Entity;
using System.Data.SqlClient;
using ModeloEF.Entidades;

namespace ModeloEF
{
    public class BiosMessengerContext : DbContext
    {
        public BiosMessengerContext()
            : base("name=BiosMessenger")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public BiosMessengerContext(string connectionString)
            : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Mensaje> Mensajes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios")
                .HasKey(u => u.Username);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Username)
                .HasColumnName("Username")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Pass)
                .HasColumnName("Pass")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.NombreCompleto)
                .HasColumnName("NombreCompleto")
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.FechaNacimiento)
                .HasColumnName("FechaNacimiento")
                .IsRequired();

            modelBuilder.Entity<Categoria>()
                .ToTable("Categorias")
                .HasKey(c => c.Codigo);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Codigo)
                .HasColumnName("Codigo")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(3);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .HasColumnName("Nombre")
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Mensaje>()
                .ToTable("Mensajes")
                .HasKey(m => m.Id);

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.Asunto)
                .HasColumnName("Asunto")
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.Texto)
                .HasColumnName("Texto")
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.FechaEnvio)
                .HasColumnName("FechaEnvio")
                .IsRequired();

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.FechaCaducidad)
                .HasColumnName("FechaCaducidad")
                .IsRequired();

            modelBuilder.Entity<Mensaje>()
                .HasRequired(m => m.Remitente)
                .WithMany(u => u.MensajesEnviados)
                .Map(cfg => cfg.MapKey("RemitenteUsername"))
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mensaje>()
                .HasRequired(m => m.Categoria)
                .WithMany(c => c.Mensajes)
                .Map(cfg => cfg.MapKey("CategoriaCod"))
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mensaje>()
                .HasMany(m => m.Destinatarios)
                .WithMany(u => u.MensajesRecibidos)
                .Map(cfg =>
                {
                    cfg.ToTable("MensajeDestinatarios");
                    cfg.MapLeftKey("MensajeId");
                    cfg.MapRightKey("DestinoUsername");
                });

            base.OnModelCreating(modelBuilder);
        }

        public int EjecutarAltaMensaje(Mensaje mensaje)
        {
            var idParam = new SqlParameter("@Id", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            var asuntoParam = new SqlParameter("@Asunto", mensaje.Asunto);
            var textoParam = new SqlParameter("@Texto", mensaje.Texto);
            var categoriaParam = new SqlParameter("@CategoriaCod", mensaje.Categoria.Codigo.Trim());
            var remitenteParam = new SqlParameter("@Remitente", mensaje.Remitente.Username.Trim());
            var caducidadParam = new SqlParameter("@FechaCaducidad", mensaje.FechaCaducidad);

            Database.ExecuteSqlCommand(
                "EXEC spMensaje_Alta @Asunto, @Texto, @CategoriaCod, @Remitente, @FechaCaducidad, @Id OUTPUT",
                asuntoParam,
                textoParam,
                categoriaParam,
                remitenteParam,
                caducidadParam,
                idParam);

            return (int)idParam.Value;
        }

        public void EjecutarAltaDestinatario(int mensajeId, string destinoUsername)
        {
            var idParam = new SqlParameter("@IdMsg", mensajeId);
            var destinoParam = new SqlParameter("@Destino", destinoUsername);

            Database.ExecuteSqlCommand("EXEC spMensaje_AddDestinatario @IdMsg, @Destino", idParam, destinoParam);
        }

        public void EjecutarBajaUsuario(string username)
        {
            var usuarioParam = new SqlParameter("@Username", username);
            Database.ExecuteSqlCommand("EXEC spUsuario_Baja @Username", usuarioParam);
        }
    }
}
