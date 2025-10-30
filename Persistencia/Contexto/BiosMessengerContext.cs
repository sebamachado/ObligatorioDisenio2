using System.Data.Entity;
using EntidadesCompartidas.Entidades;

namespace Persistencia.Contexto
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
        public virtual DbSet<MensajeDestinatario> MensajeDestinatarios { get; set; }

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
                .Property(m => m.RemitenteUsername)
                .HasColumnName("RemitenteUsername")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Mensaje>()
                .Property(m => m.CategoriaCod)
                .HasColumnName("CategoriaCod")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(3);

            modelBuilder.Entity<Mensaje>()
                .HasRequired(m => m.Remitente)
                .WithMany(u => u.MensajesEnviados)
                .HasForeignKey(m => m.RemitenteUsername)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mensaje>()
                .HasRequired(m => m.Categoria)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.CategoriaCod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mensaje>()
                .HasMany(m => m.DestinatariosInternos)
                .WithRequired(md => md.Mensaje)
                .HasForeignKey(md => md.MensajeId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MensajeDestinatario>()
                .ToTable("MensajeDestinatarios")
                .HasKey(md => new { md.MensajeId, md.DestinoUsername });

            modelBuilder.Entity<MensajeDestinatario>()
                .Property(md => md.DestinoUsername)
                .HasColumnName("DestinoUsername")
                .IsFixedLength()
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<MensajeDestinatario>()
                .HasRequired(md => md.Destinatario)
                .WithMany(u => u.MensajesRecibidos)
                .HasForeignKey(md => md.DestinoUsername)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
