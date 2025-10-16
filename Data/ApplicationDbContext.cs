using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Models;
using ebillets_jo2024_API.Models;

namespace ebillets_jo2024_API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        internal readonly object Utilisateur;

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Offre> Offres { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierOffre> PaniersOffres { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Billet> Billets { get; set; }
        public DbSet<ScanBillet> ScansBillets { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; } // optionnel

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // clé composite PanierOffre
            modelBuilder.Entity<PanierOffre>()
                .HasKey(po => new { po.IdPanier, po.IdOffre });

            // PanierOffre relations
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

            // Reservation -> Billet (1 - N)
            modelBuilder.Entity<Billet>()
                .HasOne(b => b.Reservation)
                .WithMany(r => r.Billets)
                .HasForeignKey(b => b.IdReservation)
                .OnDelete(DeleteBehavior.Cascade);

            // Billet -> Offre (N - 1)
            modelBuilder.Entity<Billet>()
                .HasOne(b => b.Offre)
                .WithMany()
                .HasForeignKey(b => b.IdOffre)
                .OnDelete(DeleteBehavior.Cascade);

            // ScanBillet -> Billet
            modelBuilder.Entity<ScanBillet>()
                .HasOne(s => s.Billet)
                .WithMany(b => b.ScansBillets)
                .HasForeignKey(s => s.IdBillet)
                .OnDelete(DeleteBehavior.Cascade);

            // ScanBillet -> Employe (Utilisateur)
            modelBuilder.Entity<ScanBillet>()
                .HasOne(s => s.Employe)
                .WithMany()
                .HasForeignKey(s => s.IdEmploye)
                .OnDelete(DeleteBehavior.Restrict);

            // Paiement -> Reservation (1 - 1)
            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Reservation)
                .WithOne(r => r.Paiement)
                .HasForeignKey<Paiement>(p => p.IdReservation)
                .OnDelete(DeleteBehavior.Cascade);

            // Administrateur -> Utilisateur (optionnel)
            modelBuilder.Entity<Administrateur>()
                .HasOne(a => a.Utilisateur)
                .WithMany()
                .HasForeignKey(a => a.IdUtilisateur)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
