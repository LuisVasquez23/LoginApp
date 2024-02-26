using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuracion
{
    public class UserConfiguration
    {
        public UserConfiguration(EntityTypeBuilder<UserModel> entityBuilder) {
            
            entityBuilder
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
