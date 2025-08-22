[HttpGet("access/{accessLinkToken}/check-submission")]
public async Task<IActionResult> CheckSubmission(Guid accessLinkToken)
{
    var questionnaire = await _context.Questionnaires
        .FirstOrDefaultAsync(q => q.AccessLinkToken == accessLinkToken);

    if (questionnaire == null) return NotFound();

    var (userId, anonymousId) = await GetUserIdAndAnonymousIdAsync();

    var hasSubmitted = await _context.Answers.AnyAsync(a =>
        a.Question.QuestionnaireId == questionnaire.Id &&
        ((userId.HasValue && a.UserId == userId) ||
         (anonymousId.HasValue && a.AnonymousId == anonymousId)));

    return Ok(new { hasSubmitted });
}
