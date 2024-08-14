using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Infrastructure.Repositories.DTOs
{
    public class CsvDishDto
    {
        [Name("Dish (Unique ID)")]
        public string Name { get; set; }

        [Name("Carb")]
        public string Carbohydrates { get; set; }

        [Name("Protein")]
        public string Proteins { get; set; }

        [Name("Veg")]
        public string Vegetables { get; set; }

        [Name("Fruit")]
        public string Fruits { get; set; }
    }
}
