using AstroBoy.Models;

namespace AstroBoy.Utils;

public static class SessionUser
{
    public static User? Current { get; private set; }
    public static void Set(User user) => Current = user;
    public static void Clear() => Current = null;
}
