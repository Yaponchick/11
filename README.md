[HttpPut("{questionnaireId}/questions/order")]
public async Task<IActionResult> UpdateQuestionOrder(string questionnaireId, [FromBody] List<string> questionIds) // Исправлено на string
{
    try
    {
        // Проверка авторизации и прав доступа
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        // Проверка, что анкета принадлежит пользователю
        var questionnaire = await _context.Questionnaires
            .FirstOrDefaultAsync(q => q.Id.ToString() == questionnaireId && q.UserId.ToString() == userId); // Исправлено сравнение
        
        if (questionnaire == null)
            return NotFound("Анкета не найдена или у вас нет прав на её редактирование");
        
        // Обновление порядка вопросов
        for (int i = 0; i < questionIds.Count; i++)
        {
            var questionId = questionIds[i];
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id.ToString() == questionId && q.QuestionnaireId.ToString() == questionnaireId); // Исправлено сравнение
            
            if (question != null)
            {
                question.Order = i + 1;
                question.UpdatedAt = DateTime.UtcNow; // Обновляем время изменения
            }
        }
        
        await _context.SaveChangesAsync();
        return Ok(new { message = "Порядок вопросов успешно обновлен" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при обновлении порядка вопросов");
        return StatusCode(500, "Произошла ошибка при обновлении порядка");
    }
}
