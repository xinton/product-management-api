using apiB2e.Interfaces;

namespace apiB2e.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", HandleLogin)
           .WithName("Login")
           .WithOpenApi();
    }

    private static async Task<IResult> HandleLogin(LoginRequest request, IUserRepository userRepository)
    {
        var isValid = await userRepository.ValidateUserAsync(request.Login, request.Password);
        return !isValid ? Results.Unauthorized() : Results.Ok(new { message = "Login successful" });
    }
}