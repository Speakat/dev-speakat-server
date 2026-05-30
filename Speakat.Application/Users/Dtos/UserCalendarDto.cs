namespace Speakat.Application.Users.Dtos;

public class UserCalendarDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public IReadOnlyList<CalendarDayDto> Days { get; init; } = [];
}

public class CalendarDayDto
{
    public DateOnly Date { get; init; }
    public int SessionCount { get; init; }
}
