using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClinoTag.Models
{
    public partial class CLINOTAGContext : DbContext
    {
        public CLINOTAGContext()
        {
        }

        public CLINOTAGContext(DbContextOptions<CLINOTAGContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Lieu> Lieus { get; set; } = null!;
        public virtual DbSet<Passage> Passages { get; set; } = null!;
        public virtual DbSet<PassageTache> PassageTaches { get; set; } = null!;
        public virtual DbSet<Tache> Taches { get; set; } = null!;
        public virtual DbSet<TacheLieu> TacheLieus { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=.;Initial Catalog=CLINOTAG;Integrated Security=True;trusted_connection=true;encrypt=false;");
                optionsBuilder.UseMySql("server=localhost;user=root;database=CLINOTAG;port=3306;password=", ServerVersion.AutoDetect("server=localhost;user=root;database=CLINOTAG;port=3306;password="));

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(e => e.Code).IsFixedLength();
            });

            modelBuilder.Entity<Lieu>(entity =>
            {
                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Lieus)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LIEU_CLIENT");
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
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PASSAGE_TACHE_PASSAGE");

                entity.HasOne(d => d.IdTlNavigation)
                    .WithMany(p => p.PassageTaches)
                    .HasForeignKey(d => d.IdTl)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PASSAGE_TACHE_TACHE_LIEU");
            });

            modelBuilder.Entity<TacheLieu>(entity =>
            {
                entity.HasOne(d => d.IdLieuNavigation)
                    .WithMany(p => p.TacheLieus)
                    .HasForeignKey(d => d.IdLieu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TACHE_LIEU_LIEU");

                entity.HasOne(d => d.IdTacheNavigation)
                    .WithMany(p => p.TacheLieus)
                    .HasForeignKey(d => d.IdTache)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TACHE_LIEU_TACHE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
