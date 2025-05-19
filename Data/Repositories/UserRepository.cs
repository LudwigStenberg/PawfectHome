using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext appDbContext;
    private readonly UserManager<UserEntity> userManager;

    public UserRepository(AppDbContext dbContext)
    {
        appDbContext = dbContext;
    }

    public async Task<UserEntity> FetchUserAsync(int id)
    {
        return await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
