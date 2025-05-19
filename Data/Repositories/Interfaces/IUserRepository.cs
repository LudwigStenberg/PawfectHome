public interface IUserRepository
{
    public Task<UserEntity> FetchUserAsync(int id);
}
