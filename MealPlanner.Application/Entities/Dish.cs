using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Entities
{
    public class Dish(string name, IEnumerable<string> carbohydrateComponents, IEnumerable<string> proteinComponents, IEnumerable<string> vegetableComponents, IEnumerable<string> fruitComponents)
    {
        public string Name { get; } = name;
        public IEnumerable<string> CarbohydrateComponents { get; } = carbohydrateComponents;
        public IEnumerable<string> ProteinComponents { get; } = proteinComponents;
        public IEnumerable<string> VegetableComponents { get; } = vegetableComponents;
        public IEnumerable<string> FruitComponents { get; } = fruitComponents;
    }
}
