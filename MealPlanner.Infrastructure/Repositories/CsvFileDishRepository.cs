using CsvHelper;
using CsvHelper.Configuration;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;
using MealPlanner.Infrastructure.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Infrastructure.Repositories
{
    public class CsvFileDishRepository : IDishRepository
    {
        private string _csvDataFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "Repositories", "Data", "Dishes.csv"
            );

        public CsvFileDishRepository(string csvDataFilePathOverride)
        {
            if (!string.IsNullOrWhiteSpace(csvDataFilePathOverride))
            {
                _csvDataFilePath = csvDataFilePathOverride;
            }
        }

        public async IAsyncEnumerable<Dish> GetAll()
        {
            var csvReaderConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            using var streamReader = new StreamReader(_csvDataFilePath);
            using var csvReader = new CsvReader(streamReader, csvReaderConfiguration);
            var dishDtos = csvReader.GetRecordsAsync<CsvDishDto>();
            await foreach (var dishDto in dishDtos)
            {
                yield return toDish(dishDto);
            }
        }

        private Dish toDish(CsvDishDto csvDishDto)
        {
            string[] dishcomponentSeparatorString = { "," };
            var dishcomponentStringSplitOptions = StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries;
            Func<string, IEnumerable<string>> splitComponentString = (
                cs => cs
                    .Split(dishcomponentSeparatorString, dishcomponentStringSplitOptions)
                    .Where(c => c != "#N/A")
                    .ToList());

            var dish = new Dish(
                name: csvDishDto.Name,
                carbohydrateComponents: splitComponentString(csvDishDto.Carbohydrates),
                proteinComponents: splitComponentString(csvDishDto.Proteins),
                vegetableComponents: splitComponentString(csvDishDto.Vegetables),
                fruitComponents: splitComponentString(csvDishDto.Fruits)
            );

            return dish;
        }
    }
}
