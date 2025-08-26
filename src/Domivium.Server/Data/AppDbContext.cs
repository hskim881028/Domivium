using Domivium.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Domivium.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();
}