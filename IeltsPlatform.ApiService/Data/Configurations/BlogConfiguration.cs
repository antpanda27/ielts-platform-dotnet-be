using IeltsPlatform.ApiService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IeltsPlatform.ApiService.Data.Configurations
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Blog_name)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(b => b.Blog_status)
                        .HasConversion<string>()
                        .HasColumnName("Blog_status")
                        .HasMaxLength(50);
            builder.Property(b => b.Blog_theme)
                        .HasConversion<string>()
                        .HasColumnName("Blog_theme")
                        .HasMaxLength(50);
        }
    }
}
