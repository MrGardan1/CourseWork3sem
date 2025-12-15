using CtfApp.Data;

namespace CtfApp;

public static class Session
{
    // запись пользователя после успешного входа
    public static User? CurrentUser { get; set; }
}
