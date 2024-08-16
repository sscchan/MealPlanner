namespace MealPlanner.WebApplication.Dtos
{
    public class PostMealPlansResponseDto
    {
        public string Date { get; }
        public string DishNames { get; }
        public string Components { get; }

        public PostMealPlansResponseDto(DateOnly date, string dishNames, string components)
        {
            Date = $"{date.Year}-{date.Month}-{date.Day}";
            DishNames = dishNames;
            Components = components;
        }
    }
}
