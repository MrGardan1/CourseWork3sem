using CtfApp.Data;

namespace CtfApp;

public static class Session
{
    // Запись пользователя после успешного входа
    public static User? CurrentUser { get; set; }
}
