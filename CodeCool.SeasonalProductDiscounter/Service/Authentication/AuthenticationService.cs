using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Users;

namespace CodeCool.SeasonalProductDiscounter.Service.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public bool Authenticate(User user)
    {
        var loginUser = _userRepository.Get(user.UserName);
        if (loginUser is null) return false;
        
        return loginUser.UserName == user.UserName && loginUser.Password == user.Password; 
    }
}
