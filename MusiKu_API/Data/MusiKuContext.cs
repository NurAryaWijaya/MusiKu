using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusiKu_API.Models;  // Make sure to include your model namespaces

namespace MusiKu_API.Data
{
    // Inherit from IdentityDbContext with IdentityUser for user management
    public class MusiKuContext : IdentityDbContext<IdentityUser>
    {
        public MusiKuContext(DbContextOptions<MusiKuContext> options) : base(options) { }

        // DbSet for your custom entities
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<LikedSong> LikedSongs { get; set; }

        // Configure relationships and table settings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call base method to configure Identity tables

            // Define composite key for PlaylistSong many-to-many relationship
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany()
                .HasForeignKey(ps => ps.SongId);
        }

    }
}

