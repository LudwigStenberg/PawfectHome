public interface IUserRepository
{
    public Task<UserEntity> FetchUserAsync(string id);

    public Task DeleteUserAsync(string id);
}
