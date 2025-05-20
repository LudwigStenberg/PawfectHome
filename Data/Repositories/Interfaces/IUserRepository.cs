public interface IUserRepository
{
    public Task<UserEntity> FetchUserAsync(string id);
}
