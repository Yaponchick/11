// Вывод полной информации по анкете

[HttpGet("{questionnaireId}")]
[Authorize]
public async Task<IActionResult> GetQuestionnaireById(int questionnaireId)
{
    Console.WriteLine($"Запрос на получение анкеты с ID: {questionnaireId}");

    var (userId, anonymousId) = await GetUserIdAndAnonymousIdAsync();
    if (userId == null && anonymousId == null)
    {
        Console.WriteLine("Ошибка: Не удалось получить ID пользователя.");
        return Unauthorized("Не удалось получить ID пользователя.");
    }

    // Загружаем все необходимые связанные данные одним запросом
    var questionnaire = await _context.Questionnaires
        .Include(q => q.Questions.OrderBy(qu => qu.Order)) // Сортируем вопросы
            .ThenInclude(qu => qu.Options.OrderBy(o => o.Order)) // Сортируем опции
                .ThenInclude(o => o.Answers) // Включаем ответы, связанные с опциями
                    .ThenInclude(a => a.User) // Включаем пользователя для этих ответов
        .Include(q => q.Questions)
            .ThenInclude(qu => qu.Answers) // Включаем ответы, связанные напрямую с вопросами
                .ThenInclude(a => a.User) // Включаем пользователя для этих ответов
        .FirstOrDefaultAsync(q => q.Id == questionnaireId);


    if (questionnaire == null)
    {
        Console.WriteLine($"Анкета с ID {questionnaireId} не найдена.");
        return NotFound(new { message = "Анкета не найдена." });
    }

    Console.WriteLine($"Анкета с ID {questionnaireId} успешно найдена.");

    // Проверка прав доступа (остается без изменений)
    var user = await _context.Users
        .Include(u => u.AccessLevel)
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        Console.WriteLine("Ошибка: Пользователь не найден.");
        return Unauthorized("Пользователь не найден.");
    }

    if (questionnaire.UserId != userId && user.AccessLevel?.LevelName != "admin") 
    {
        Console.WriteLine("Ошибка: У пользователя нет прав для просмотра этой анкеты.");
        return StatusCode(403, new { message = "У вас нет прав для просмотра этой анкеты." });
    }

    // Формируем данные для возврата с правильной структурой
    var result = new
    {
        Id = questionnaire.Id, 
        Title = questionnaire.Title,
        CreatedAt = questionnaire.CreatedAt,
        IsPublished = questionnaire.IsPublished,
        Questions = questionnaire.Questions.Select(q =>
        {
            // --- Объединяем ВСЕ ответы на этот вопрос в один список ---
            var allAnswersForQuestion = new List<object>();

            // 1. Добавляем ответы, связанные напрямую с вопросом (текст, шкала)
            allAnswersForQuestion.AddRange(q.Answers.Select(a => new
            {
                a.Id,
                Text = a.Text, // Текст ответа (для text, scale)
                SelectedOptionText = (string)null, // Для этих типов нет выбранной опции
                a.CreatedAt,
                UserId = a.UserId ?? anonymousId,
                UserName = a.User?.Username ?? "Аноним",
                IsAnonymous = a.UserId == null
            }));

            // 2. Добавляем ответы, связанные через опции (radio, checkbox, select)
            foreach (var option in q.Options)
            {
                allAnswersForQuestion.AddRange(option.Answers.Select(a => new
                {
                    a.Id,
                    Text = (string)null, // Для этих типов основной текст ответа не используется
                    SelectedOptionText = option.OptionText, // Текст ВЫБРАННОЙ опции
                    a.CreatedAt,
                    UserId = a.UserId ?? anonymousId,
                    UserName = a.User?.Username ?? "Аноним",
                    IsAnonymous = a.UserId == null
                }));
            }

            // Сортируем объединенные ответы по времени создания для консистентности
            allAnswersForQuestion = allAnswersForQuestion
               .OrderBy(a => ((dynamic)a).CreatedAt)
               .ToList();


            // --- Возвращаем данные вопроса с правильным типом и объединенными ответами ---
            return new
            {
                Id = q.Id,
                Text = q.Text,
                Type = MapQuestionTypeIdToString(q.QuestionTypeId), // <--- Преобразуем ID в строку
                                                                    // Не отправляем QuestionTypeId, если он не нужен фронтенду
                Options = q.Options.Select(o => new // Опции нужны фронтенду для отображения возможных вариантов
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    Order = o.Order
                    // Не включаем ответы здесь, т.к. они объединены ниже
                }).ToList(),
                Answers = allAnswersForQuestion // <--- Отправляем ЕДИНЫЙ список всех ответов
            };
        }).ToList()
    };

    return Ok(result);
}
