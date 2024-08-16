using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MealPlanner.WebApplication.Dtos
{
    public class PostMealPlansRequestDto
    {
        /// <summary>
        /// The number of days in the new meal plan.
        /// </summary>
        [Required]
        [DefaultValue(7)]
        public int NumberOfDaysInPlan { get; set; }

        /// <summary>
        /// The number of days before an dish component can be used again.
        /// </summary>
        [Required]
        [DefaultValue(3)]
        public int NumberOfDaysBeforeDishComponentsCanBeUsedAgain { get; set; }

        /// <summary>
        /// A list of dish components that will be prioritized during meal plan creation.
        /// </summary>
        public IList<string> PriorityDishComponents { get; set; }

        /// <summary>
        /// A list of dish components that will not be included in the new meal plan.
        /// </summary>
        public IList<string> ProhibitedDishComponents { get; set; }

        /// <summary>
        /// A list of dishes that will not be included in the new meal plan.
        /// </summary>
        public IList<string> ProhibitedDishes { get; set; }

    }
}
