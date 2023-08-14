using System;
using System.Collections.Generic;
using BqsClinoTag.Models.BqsClinoTag.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace BqsClinoTag.Models
{
    public partial class CLINOTAGBQSContext : DbContext
    {
        public CLINOTAGBQSContext()
        {
        }

        public CLINOTAGBQSContext(DbContextOptions<CLINOTAGBQSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<GeolocAgent> GeolocAgents { get; set; } = null!;
        public virtual DbSet<Lieu> Lieus { get; set; } = null!;
        public virtual DbSet<Materiel> Materiels { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Passage> Passages { get; set; } = null!;
        public virtual DbSet<PassageTache> PassageTaches { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Tache> Taches { get; set; } = null!;
        public virtual DbSet<TacheLieu> TacheLieus { get; set; } = null!;
        public virtual DbSet<TachePlanifiee> TachePlanifiees { get; set; } = null!;
        public virtual DbSet<Uclient> Uclients { get; set; } = null!;
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
        public virtual DbSet<Utilisation> Utilisations { get; set; } = null!;
        public virtual DbSet<SatisfactionLog> SatisfactionLogs { get; set; } = null!;
        public virtual DbSet<SettingsModel> AdminSettings { get; set; } = null!;
        public virtual DbSet<Acknowledge> Acknowledges { get; set; } = null!;
        
        public virtual DbSet<AcknowledgeLog> AcknowledgeLogs { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //              optionsBuilder.UseSqlServer("Server=.;Initial Catalog=CLINOTAG-BQS;Integrated Security=True;trusted_connection=true;encrypt=false;");
                //              optionsBuilder.UseMySql("server=localhost;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=localhost;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True"));
                //              optionsBuilder.UseMySql("server=127.0.0.1;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=127.0.0.1;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True"));
                //              optionsBuilder.UseMySql("server=172.16.75.101;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=172.16.75.101;user=root;database=CLINOTAG-BQS;port=3306;password=000000;ConvertZeroDateTime=True"));
                //                optionsBuilder.UseMySql("server=172.16.75.101;user=clinotag;database=clinotag;port=3306;password=Cl1n0t@g:2023;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=172.16.75.101;user=clinotag;database=clinotag;port=3306;password=Cl1n0t@g:2023;ConvertZeroDateTime=True"));
                             optionsBuilder.UseMySql("server=localhost;user=root;database=CLINOTAG-BQS;port=3306;password=;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=localhost;user=root;database=CLINOTAG-BQS;port=3306;password=;ConvertZeroDateTime=True"));
            }
        }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SettingsModel>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(e => e.Code).IsFixedLength();
            });

            modelBuilder.Entity<GeolocAgent>(entity =>
            {
                entity.HasOne(d => d.IdAgentNavigation)
                    .WithMany(p => p.GeolocAgents)
                    .HasForeignKey(d => d.IdAgent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GEOLOC_AGENT_UTILISATEUR");
            });

            modelBuilder.Entity<Lieu>(entity =>
            {
                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Lieus)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LIEU_CLIENT");
            });

            modelBuilder.Entity<Materiel>(entity =>
            {
                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Materiels)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MATERIEL_CLIENT");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(d => d.IdUtilisationNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.IdUtilisation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NOTIFICATION_UTILISATION");
            });

            modelBuilder.Entity<Passage>(entity =>
            {
                entity.HasOne(d => d.IdAgentNavigation)
                    .WithMany(p => p.Passages)
                    .HasForeignKey(d => d.IdAgent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PASSAGE_PASSAGE");

                entity.HasOne(d => d.IdLieuNavigation)
                    .WithMany(p => p.Passages)
                    .HasForeignKey(d => d.IdLieu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PASSAGE_LIEU");
            });

            modelBuilder.Entity<PassageTache>(entity =>
            {
                entity.HasOne(d => d.IdPassageNavigation)
                    .WithMany(p => p.PassageTaches)
                    .HasForeignKey(d => d.IdPassage)
                    .HasConstraintName("FK_PASSAGE_TACHE_PASSAGE");

                entity.HasOne(d => d.IdTlNavigation)
                    .WithMany(p => p.PassageTaches)
                    .HasForeignKey(d => d.IdTl)
                    .HasConstraintName("FK_PASSAGE_TACHE_TACHE_LIEU");
            });

            modelBuilder.Entity<TacheLieu>(entity =>
            {
                entity.HasOne(d => d.IdLieuNavigation)
                    .WithMany(p => p.TacheLieus)
                    .HasForeignKey(d => d.IdLieu)
                    .HasConstraintName("FK_TACHE_LIEU_LIEU");

                entity.HasOne(d => d.IdTacheNavigation)
                    .WithMany(p => p.TacheLieus)
                    .HasForeignKey(d => d.IdTache)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TACHE_LIEU_TACHE");
            });

            modelBuilder.Entity<Uclient>(entity =>
            {
                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Uclients)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UCLIENT_CLIENT");

                entity.HasOne(d => d.IdUtilisateurNavigation)
                    .WithMany(p => p.Uclients)
                    .HasForeignKey(d => d.IdUtilisateur)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UCLIENT_UTILISATEUR");
            });

            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Utilisateurs)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UTILISATEUR_ROLE");
            });

            modelBuilder.Entity<Utilisation>(entity =>
            {
                entity.HasOne(d => d.IdAgentNavigation)
                    .WithMany(p => p.Utilisations)
                    .HasForeignKey(d => d.IdAgent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UTILISATION_AGENT");

                entity.HasOne(d => d.IdMaterielNavigation)
                    .WithMany(p => p.Utilisations)
                    .HasForeignKey(d => d.IdMateriel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UTILISATION_MATERIEL");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
