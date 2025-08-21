[HttpPut("{questionnaireId}/questions/order")]
public async Task<IActionResult> UpdateQuestionOrder(string questionnaireId, [FromBody] List<Guid> questionIds)
{
    try
    {
        // Проверка авторизации и прав доступа
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        // Проверка, что анкета принадлежит пользователю
        var questionnaire = await _context.Questionnaires
            .FirstOrDefaultAsync(q => q.Id == Guid.Parse(questionnaireId) && q.UserId == Guid.Parse(userId));
        
        if (questionnaire == null)
            return NotFound("Анкета не найдена или у вас нет прав на её редактирование");
        
        // Обновление порядка вопросов
        for (int i = 0; i < questionIds.Count; i++)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionIds[i] && q.QuestionnaireId == Guid.Parse(questionnaireId));
            
            if (question != null)
            {
                question.Order = i + 1; // или другой логический порядок
            }
        }
        
        await _context.SaveChangesAsync();
        return Ok();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при обновлении порядка вопросов");
        return StatusCode(500, "Произошла ошибка при обновлении порядка");
    }
}
