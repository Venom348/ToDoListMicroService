namespace Auth.Core.Exceptions;

/// <summary>
///     Класс для вывода сообщения об ошибке при аутентификации
/// </summary>
/// <param name="msg">Сообщение ошибки</param>
public class AuthException(string msg = "") :  Exception(msg);