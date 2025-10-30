using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ModeloEF
{
    /// <summary>
    /// Contexto de Entity Framework configurado siguiendo el enfoque Database First.
    /// La clase reproduce el mapeo generado por el diseñador pero se mantiene en código
    /// para permitir su edición en este entorno sin herramientas gráficas.
    /// </summary>
    public class BiosMessengerContext : DbContext
    {
        static BiosMessengerContext()
        {
            Database.SetInitializer<BiosMessengerContext>(null);
        }

        /// <summary>
        /// Crea una nueva instancia utilizando la cadena de conexión configurada en el archivo de configuración.
        /// </summary>
        public BiosMessengerContext()
            : base("name=BiosMessengerContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public virtual DbSet<Entities.Usuario> Usuarios { get; set; }
        public virtual DbSet<Entities.Categoria> Categorias { get; set; }
        public virtual DbSet<Entities.Mensaje> Mensajes { get; set; }

        /// <summary>
        /// Configura el modelo según el esquema existente respetando las reglas de negocio exigidas.
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelo.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Entities.Usuario>()
                .HasKey(u => u.Username);

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.Username)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.Pass)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.NombreCompleto)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Entities.Categoria>()
                .HasKey(c => c.Codigo);

            modelBuilder.Entity<Entities.Categoria>()
                .Property(c => c.Codigo)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(3);

            modelBuilder.Entity<Entities.Categoria>()
                .Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Entities.Mensaje>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Entities.Mensaje>()
                .Property(m => m.Asunto)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Entities.Mensaje>()
                .Property(m => m.Texto)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Entities.Mensaje>()
                .Property(m => m.RemitenteUsername)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Entities.Mensaje>()
                .Property(m => m.CategoriaCod)
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(3);

            modelBuilder.Entity<Entities.Mensaje>()
                .HasRequired(m => m.Remitente)
                .WithMany(u => u.MensajesEnviados)
                .HasForeignKey(m => m.RemitenteUsername)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.Mensaje>()
                .HasRequired(m => m.Categoria)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.CategoriaCod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Entities.Usuario>()
                .HasMany(u => u.MensajesDestinados)
                .WithMany(m => m.Destinatarios)
                .Map(config =>
                {
                    config.ToTable("MensajeDestinatarios");
                    config.MapLeftKey("DestinoUsername");
                    config.MapRightKey("MensajeId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
