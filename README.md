[HttpPut("{questionnaireId}/questions/order")]
public async Task<IActionResult> UpdateQuestionOrder(int questionnaireId, [FromBody] List<int> questionIds)
{
    try
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();
        
        // Проверка владения анкетой
        var questionnaireExists = await _context.Questionnaires
            .AnyAsync(q => q.Id == questionnaireId && q.UserId == userId);
        
        if (!questionnaireExists)
            return NotFound("Анкета не найдена");
        
        var existingQuestions = await _context.Questions
            .Where(q => q.QuestionnaireId == questionnaireId)
            .ToListAsync();
        
        foreach (var question in existingQuestions)
        {
            var newOrder = questionIds.IndexOf(question.Id) + 1; // Теперь работает!
            if (newOrder > 0)
            {
                question.Order = newOrder;
                question.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        await _context.SaveChangesAsync();
        return Ok(new { message = "Порядок обновлен" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при обновлении порядка");
        return StatusCode(500, "Ошибка сервера");
    }
}
