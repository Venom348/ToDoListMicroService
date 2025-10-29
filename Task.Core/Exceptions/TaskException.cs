namespace Task.Core.Exceptions;

/// <summary>
///     Класс для вывода об ошибке для задачи
/// </summary>
/// <param name="msg">Сообщение ошибки</param>
public class TaskException(string msg = "") :  Exception(msg);