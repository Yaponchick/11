private async Task<(int? userId, int? anonymousId)> GetUserIdAndAnonymousIdAsync()
{
    var userIdClaim = User.FindFirstValue(AuthOptions.UserIdClaimType);
    int? userId = null;
    int? anonymousId = null;

    if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
    {
        userId = parsedUserId;
        return (userId, anonymousId);
    }

    // Анонимный пользователь
    var sessionIdHeader = Request.Headers["X-Session-Id"].ToString();

    if (!string.IsNullOrEmpty(sessionIdHeader) && Guid.TryParse(sessionIdHeader, out Guid parsedSessionId))
    {
        var anonymousUser = await _context.Anonymous
            .FirstOrDefaultAsync(a => a.SessionId == parsedSessionId);

        if (anonymousUser != null)
        {
            anonymousId = anonymousUser.Id;
            return (userId, anonymousId);
        }
    }

    // Если SessionId не найден или невалиден — создаём нового анонимного пользователя
    var newSessionId = Guid.NewGuid();
    var newAnonymousUser = new AnonymousUser
    {
        SessionId = newSessionId,
        CreatedAt = DateTime.UtcNow
    };

    _context.Anonymous.Add(newAnonymousUser);
    await _context.SaveChangesAsync(); // Важно: сохранить, чтобы получить Id

    // Добавляем новый SessionId в заголовок ответа, чтобы клиент мог его запомнить
    Response.Headers["X-Session-Id"] = newSessionId.ToString();
    anonymousId = newAnonymousUser.Id;

    return (userId, anonymousId);
}
