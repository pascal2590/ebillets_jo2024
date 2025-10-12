using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Models;

namespace ebillets_jo2024.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // === Tables principales ===
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Offre> Offres { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierOffre> PaniersOffres { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<ScanBillet> ScansBillets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Table PanierOffre : clé composite ===
            modelBuilder.Entity<PanierOffre>()
                .HasKey(po => new { po.IdPanier, po.IdOffre });

            modelBuilder.Entity<PanierOffre>()
                .HasOne(po => po.Panier)
                .WithMany(p => p.PaniersOffres)
                .HasForeignKey(po => po.IdPanier)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PanierOffre>()
                .HasOne(po => po.Offre)
                .WithMany(o => o.PaniersOffres)
                .HasForeignKey(po => po.IdOffre)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation Utilisateur → Panier (1-N)
            modelBuilder.Entity<Panier>()
                .HasOne(p => p.Utilisateur)
                .WithMany(u => u.Paniers)
                .HasForeignKey(p => p.IdUtilisateur)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation Utilisateur → Reservation (1-N)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Utilisateur)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.IdUtilisateur)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation Offre → Reservation (1-N)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Offre)
                .WithMany(o => o.Reservations)
                .HasForeignKey(r => r.IdOffre)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation Reservation → Paiement (1-1)
            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Reservation)
                .WithOne(r => r.Paiement)
                .HasForeignKey<Paiement>(p => p.IdReservation)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation ScanBillet → Reservation (1-N)
            modelBuilder.Entity<ScanBillet>()
                .HasOne(s => s.Reservation)
                .WithMany()
                .HasForeignKey(s => s.IdReservation)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relation ScanBillet → Employé (1-N, auto-référence à Utilisateur)
            modelBuilder.Entity<ScanBillet>()
                .HasOne(s => s.Employe)
                .WithMany()
                .HasForeignKey(s => s.IdEmploye)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
