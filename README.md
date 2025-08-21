// Смена роли пользователя
[HttpPut("{id}/role")]
public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
{
    // Проверяем, существует ли пользователь
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        return NotFound(new { message = "Пользователь не найден." });
    }

    // Проверяем, существует ли указанный уровень доступа
    var accessLevel = await _context.AccessLevels
        .FirstOrDefaultAsync(al => al.LevelName == request.Role);
    if (accessLevel == null)
    {
        return BadRequest(new { message = "Указанная роль не существует." });
    }

    // Обновляем роль
    user.AccessLevelId = accessLevel.Id;

    _context.Users.Update(user);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Роль успешно обновлена",
        role = accessLevel.LevelName
    });
}
