using DataVault.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DataVault.API.Data;

public class UserDBContext: DbContext
{
    public UserDBContext(DbContextOptions options) : base(options) { }
    public DbSet<User> User { get; set; }
}