[HttpPut("{questionnaireId}/questions/order")]
public async Task<IActionResult> UpdateQuestionOrder(
    string questionnaireId, 
    [FromBody] List<QuestionOrderDto> questionOrders)
{
    try
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Проверяем права доступа
        var questionnaireExists = await _context.Questionnaires
            .AnyAsync(q => q.Id == Guid.Parse(questionnaireId) && q.UserId == Guid.Parse(userId));
        
        if (!questionnaireExists)
            return NotFound("Анкета не найдена");

        // Обновляем порядок вопросов
        foreach (var order in questionOrders)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => 
                    q.Id == order.QuestionId && 
                    q.QuestionnaireId == Guid.Parse(questionnaireId));
            
            if (question != null)
            {
                question.Order = order.Order;
                question.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return Ok("Порядок вопросов обновлен");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при обновлении порядка вопросов");
        return StatusCode(500, "Ошибка при обновлении порядка");
    }
}
