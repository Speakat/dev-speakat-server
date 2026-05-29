namespace Speakat.Application.Users.Repositories;

public record CalendarDayData(DateOnly Date, int SessionCount);

public interface IUserCalendarRepository
{
    Task<IReadOnlyList<CalendarDayData>> GetMonthlyActivityAsync(long userId, int year, int month);
}
