namespace User.Core.Exceptions;

/// <summary>
///     Класс для вывода сообщения об ошибке у пользователя
/// </summary>
/// <param name="msg">Сообщение ошибки</param>
public class UserException(string msg = "") :  Exception(msg);