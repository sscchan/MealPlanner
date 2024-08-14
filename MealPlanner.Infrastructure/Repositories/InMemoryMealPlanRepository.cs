using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Infrastructure.Repositories
{
    public class InMemoryMealPlanRepository : IMealPlanRepository
    {
        public Task<IEnumerable<MealPlan>> Get()
        {
            throw new NotImplementedException();
        }
    }
}
