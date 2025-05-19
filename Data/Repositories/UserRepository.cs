using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext appDbContext;
    private readonly UserManager<UserEntity> userManager;

    public UserRepository(AppDbContext dbContext, UserManager<UserEntity> userManager)
    {
        appDbContext = dbContext;
        this.userManager = userManager;
    }

    public async Task<UserEntity> FetchUserAsync(string id)
    {
        return await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
