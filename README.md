[HttpPost("access/{accessLinkToken}/questions/{questionId}/answer")]
public async Task<IActionResult> SubmitAnswer(Guid accessLinkToken, int questionId, [FromBody] AnswerRequest request)
{
    // Проверка на null
    if (request == null)
    {
        return BadRequest("Тело запроса не может быть пустым.");
    }

    // Находим анкету по ID
    var questionnaire = await _context.Questionnaires
        .FirstOrDefaultAsync(q => q.AccessLinkToken == accessLinkToken);

    if (questionnaire == null)
    {
        return NotFound("Анкета не найдена.");
    }

    // Находим вопрос по ID из пути
    var question = await _context.Questions
        .Include(q => q.Options) // Подключаем варианты ответов
        .FirstOrDefaultAsync(q => q.Id == questionId && q.QuestionnaireId == questionnaire.Id);

    if (question == null)
    {
        return NotFound("Вопрос не найден в указанной анкете.");
    }

    // Проверяем права доступа
    var (userId, anonymousId) = await GetUserIdAndAnonymousIdAsync();

    // Обработка ответа в зависимости от типа вопроса
    switch (question.QuestionTypeId)
    {
        case 1: // Текстовый вопрос
            if (string.IsNullOrEmpty(request.AnswerText))
            {
                return BadRequest("Для текстового вопроса требуется поле 'AnswerText'.");
            }

            var textAnswer = new Answer
            {
                Text = request.AnswerText,
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now
            };

            await _context.Answers.AddAsync(textAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Текстовый ответ успешно отправлен.",
                answerId = textAnswer.Id
            });

        case 2: // Выбор одного варианта
            if (!request.AnswerClose.HasValue)
            {
                return BadRequest("Для выбора одного варианта требуется поле 'AnswerClose'.");
            }

            var singleOption = question.Options.FirstOrDefault(o => o.Order == request.AnswerClose.Value);
            if (singleOption == null)
            {
                return BadRequest($"Неверный вариант ответа: {request.AnswerClose.Value}");
            }

            var singleAnswer = new Answer
            {
                Text = null,
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now,
                SelectOption = singleOption.Id
            };

            await _context.Answers.AddAsync(singleAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответ успешно отправлен.",
                answerId = singleAnswer.Id
            });

        case 3: // Выбор нескольких вариантов
            if (request.AnswerMultiple == null || !request.AnswerMultiple.Any())
            {
                return BadRequest("Для выбора нескольких вариантов требуется поле 'AnswerMultiple'.");
            }

            foreach (var order in request.AnswerMultiple)
            {
                var option = question.Options.FirstOrDefault(o => o.Order == order);
                if (option == null)
                {
                    return BadRequest($"Неверный вариант ответа: {order}");
                }

                var multipleAnswer = new Answer
                {
                    Text = null,
                    QuestionId = questionId,
                    UserId = userId,
                    AnonymousId = anonymousId,
                    CreatedAt = DateTime.Now,
                    SelectOption = option.Id
                };

                await _context.Answers.AddAsync(multipleAnswer);
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответы успешно отправлены."
            });

        case 4: // Шкальный вопрос
            if (!request.AnswerScale.HasValue)
            {
                return BadRequest("Для шкального вопроса требуется поле 'AnswerScale'.");
            }

            if (request.AnswerScale < 1 || request.AnswerScale > 10)
            {
                return BadRequest("Значение шкалы должно быть между 1 и 10.");
            }

            var scaleAnswer = new Answer
            {
                Text = request.AnswerScale.ToString(),
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now
            };

            await _context.Answers.AddAsync(scaleAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Шкальный ответ успешно отправлен.",
                answerId = scaleAnswer.Id
            });
        case 5: // Выпадающий список (Dropdown)
                // Проверяем, пришел ли ID опции в поле AnswerClose
            if (!request.AnswerClose.HasValue)
            {
                // Сообщение можно уточнить, что ожидается ID
                return BadRequest("Для выпадающего списка требуется ID выбранного варианта (в поле AnswerClose).");
            }

            // Ищем опцию напрямую по ID, который пришел в AnswerClose
            var dropdownOption = question.Options.FirstOrDefault(o => o.Id == request.AnswerClose.Value); // <<< ИЗМЕНЕНИЕ ЗДЕСЬ: Ищем по o.Id

            if (dropdownOption == null)
            {
                // Если опция с таким ID не найдена
                return BadRequest($"Неверный ID варианта ответа: {request.AnswerClose.Value}");
            }

            // Создаем ответ, сохраняя ID найденной опции
            var dropdownAnswer = new Answer
            {
                Text = null, // Текст не нужен для выбора из списка
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now,
                SelectOption = dropdownOption.Id // Сохраняем ID выбранной опции
            };

            await _context.Answers.AddAsync(dropdownAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответ (выпадающий список) успешно отправлен.",
                answerId = dropdownAnswer.Id
            });
        default:
            return BadRequest("Неизвестный тип вопроса.");
    }
}


private async Task<(int? userId, int? anonymousId)> GetUserIdAndAnonymousIdAsync()
{
    var userIdClaim = User.FindFirstValue(AuthOptions.UserIdClaimType);
    int? userId = null;
    int? anonymousId = null;

    if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
    {
        userId = parsedUserId; // Авторизованный пользователь
    }
    else
    {
        var sessionIdHeader = Request.Headers["X-Session-Id"].ToString();
        if (!string.IsNullOrEmpty(sessionIdHeader) && Guid.TryParse(sessionIdHeader, out Guid parsedSessionId))
        {
            var anonymousUser = await _context.Anonymous.FirstOrDefaultAsync(a => a.SessionId == parsedSessionId);
            if (anonymousUser == null)
            {
                throw new UnauthorizedAccessException("Неверный или потерянный SessionId для анонимного пользователя.");
            }
            anonymousId = anonymousUser.Id;
        }
    }

    if (userId == null && anonymousId == null)
    {
        throw new UnauthorizedAccessException("Отсутствует проверка подлинности или действительный идентификатор сеанса.");
    }

    return (userId, anonymousId);
}
