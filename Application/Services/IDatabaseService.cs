using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IDatabaseService
    {
        DbSet<UserModel> Users { get; set; }
        Task<bool> SaveAsync();
    }
}
