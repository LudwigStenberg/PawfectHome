
public static class UserMapper
{
    public static UserEntity ToEntity(RegisterUserRequest user)
    {
        return new UserEntity
        {
            UserName = user.Email,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}