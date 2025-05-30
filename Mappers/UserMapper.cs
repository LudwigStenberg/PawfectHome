
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

    public static RegisterUserResponse ToRegisterResponse(UserEntity user)
    {
        return new RegisterUserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName!,
            LastName = user.LastName!,
        };
    }
}