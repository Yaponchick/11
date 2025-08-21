[HttpPut("{questionnaireId}/questions/order")]
public async Task<IActionResult> UpdateQuestionOrder(Guid questionnaireId, [FromBody] List<Guid> questionIds)
{
    try
    {
        // Проверка авторизации
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        // Проверяем, что пользователь владеет анкетой
        var canEdit = await _context.Questionnaires
            .AnyAsync(q => q.Id == questionnaireId && q.UserId == Guid.Parse(userId));
        
        if (!canEdit)
            return Forbid();
        
        // Массовое обновление порядка
        var questionsToUpdate = await _context.Questions
            .Where(q => q.QuestionnaireId == questionnaireId && questionIds.Contains(q.Id))
            .ToListAsync();
        
        foreach (var question in questionsToUpdate)
        {
            question.Order = questionIds.IndexOf(question.Id) + 1;
            question.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        return Ok();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при обновлении порядка");
        return StatusCode(500, "Internal server error");
    }
}
