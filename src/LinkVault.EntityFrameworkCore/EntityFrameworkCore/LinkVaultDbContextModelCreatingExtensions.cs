using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using LinkVault.Links;
using LinkVault.Collections;
using LinkVault.Tags;

namespace LinkVault.EntityFrameworkCore;

/// <summary>
/// Extension methods for configuring LinkVault entities in EF Core.
/// </summary>
public static class LinkVaultDbContextModelCreatingExtensions
{
    public static void ConfigureLinkVault(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure Link entity */
        builder.Entity<Link>(b =>
        {
            b.ToTable(LinkVaultConsts.DbTablePrefix + "Links", LinkVaultConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Url).IsRequired().HasMaxLength(LinkConsts.MaxUrlLength);
            b.Property(x => x.Title).IsRequired().HasMaxLength(LinkConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(LinkConsts.MaxDescriptionLength);
            b.Property(x => x.Favicon).HasMaxLength(LinkConsts.MaxFaviconLength);
            b.Property(x => x.Domain).IsRequired().HasMaxLength(LinkConsts.MaxDomainLength);
            b.Property(x => x.IsFavorite).HasDefaultValue(false);
            b.Property(x => x.VisitCount).HasDefaultValue(0);

            // Indexes for efficient querying
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.Url }).IsUnique().HasFilter("\"IsDeleted\" = false");
            b.HasIndex(x => new { x.UserId, x.IsFavorite });
            b.HasIndex(x => new { x.UserId, x.Domain });
            b.HasIndex(x => new { x.UserId, x.CreationTime });
            b.HasIndex(x => new { x.UserId, x.VisitCount });

            // Relationships
            b.HasOne(x => x.Collection)
                .WithMany(x => x.Links)
                .HasForeignKey(x => x.CollectionId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.LinkTags)
                .WithOne(x => x.Link)
                .HasForeignKey(x => x.LinkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* Configure LinkTag entity (junction table) */
        builder.Entity<LinkTag>(b =>
        {
            b.ToTable(LinkVaultConsts.DbTablePrefix + "LinkTags", LinkVaultConsts.DbSchema);

            b.HasKey(x => new { x.LinkId, x.TagId });

            b.HasOne(x => x.Link)
                .WithMany(x => x.LinkTags)
                .HasForeignKey(x => x.LinkId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Tag)
                .WithMany(x => x.LinkTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.TagId);
        });

        /* Configure Collection entity */
        builder.Entity<Collection>(b =>
        {
            b.ToTable(LinkVaultConsts.DbTablePrefix + "Collections", LinkVaultConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(CollectionConsts.MaxNameLength);
            b.Property(x => x.Color).HasMaxLength(CollectionConsts.MaxColorLength).HasDefaultValue(CollectionConsts.DefaultColor);
            b.Property(x => x.Icon).HasMaxLength(CollectionConsts.MaxIconLength);
            b.Property(x => x.Order).HasDefaultValue(0);

            // Indexes
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.ParentId });
            b.HasIndex(x => new { x.UserId, x.Name });

            // Self-referencing relationship for nested collections
            b.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* Configure Tag entity */
        builder.Entity<Tag>(b =>
        {
            b.ToTable(LinkVaultConsts.DbTablePrefix + "Tags", LinkVaultConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(TagConsts.MaxNameLength);
            b.Property(x => x.Color).HasMaxLength(TagConsts.MaxColorLength).HasDefaultValue(TagConsts.DefaultColor);

            // Indexes
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.Name }).IsUnique();

            b.HasMany(x => x.LinkTags)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
