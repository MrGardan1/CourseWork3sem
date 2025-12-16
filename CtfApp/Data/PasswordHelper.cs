namespace CtfApp.Data;

public static class PasswordHelper
{
    /// <summary>
    /// Хеширует пароль
    /// </summary>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    /// <summary>
    /// Проверяет пароль с хешем
    /// </summary>
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
