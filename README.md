private async Task<(int? userId, int? anonymousId)> GetUserIdAndAnonymousIdAsync()
{
    var userIdClaim = User.FindFirstValue(AuthOptions.UserIdClaimType);
    if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
    {
        return (parsedUserId, null);
    }

    // Получаем SessionId из заголовка или куки
    var sessionIdStr = await GetSessionIdAsync();
    if (string.IsNullOrEmpty(sessionIdStr) || !Guid.TryParse(sessionIdStr, out Guid sessionId))
    {
        // Только если нет вообще — создаём новый
        sessionId = Guid.NewGuid();
        var newAnon = new AnonymousUser
        {
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow
        };
        _context.Anonymous.Add(newAnon);
        await _context.SaveChangesAsync();

        Response.Headers["X-Session-Id"] = sessionId.ToString();
        return (null, newAnon.Id);
    }

    // Пытаемся найти существующего пользователя по SessionId
    var existingAnon = await _context.Anonymous
        .FirstOrDefaultAsync(a => a.SessionId == sessionId);

    if (existingAnon != null)
    {
        return (null, existingAnon.Id);
    }

    // ❌ Если SessionId пришёл, но его нет в БД — это подозрительно
    // Но лучше создать нового, чем блокировать
    var freshAnon = new AnonymousUser
    {
        SessionId = sessionId, // ← используем тот же ID, что прислал клиент
        CreatedAt = DateTime.UtcNow
    };

    _context.Anonymous.Add(freshAnon);
    await _context.SaveChangesAsync();

    Response.Headers["X-Session-Id"] = sessionId.ToString(); // ← тот же
    return (null, freshAnon.Id);
}
