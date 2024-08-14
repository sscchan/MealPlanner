using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Services
{
    public class MealPlannerService
    {
        private IMealPlanRepository _MealPlanRepository;

        public MealPlan ExistingMealPlan { get; }

        public MealPlan GenerateNewMealPlan()
        {
            return new MealPlan(new List<Meal>() { });
        }
    }
}
