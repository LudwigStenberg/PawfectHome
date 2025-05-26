using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;
    private readonly UserManager<UserEntity> userManager;

    public UserRepository(AppDbContext context, UserManager<UserEntity> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task<UserEntity?> FetchUserAsync(string id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}
