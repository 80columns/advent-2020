using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21 {
    class Program {
        static async Task Main(string[] args) {
            var foods = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Determine how many times the ingredients which cannot contain an allergen appear in the list of foods
             */
            var allergens_and_possible_ingredients = new Dictionary<string, HashSet<string>>();
            var ingredients_and_counts = new Dictionary<string, int>();

            foreach (var food in foods) {
                var food_items = food.Split('(');
                var food_ingredients = new HashSet<string>(food_items[0].Split(" ", StringSplitOptions.RemoveEmptyEntries));
                var food_allergens = new HashSet<string>(food_items[1].Replace("contains", string.Empty).Replace(")", string.Empty).TrimStart().Split(", ", StringSplitOptions.RemoveEmptyEntries));

                foreach (var food_allergen in food_allergens) {
                    if (!allergens_and_possible_ingredients.ContainsKey(food_allergen)) {
                        allergens_and_possible_ingredients.Add(food_allergen, new HashSet<string>(food_ingredients));
                    } else {
                        allergens_and_possible_ingredients[food_allergen].RemoveWhere(fa => !food_ingredients.Contains(fa));
                    }
                }

                foreach (var ingredient in food_ingredients) {
                    if (ingredients_and_counts.ContainsKey(ingredient)) {
                        ingredients_and_counts[ingredient] += 1;
                    } else {
                        ingredients_and_counts.Add(ingredient, 1);
                    }
                }
            }

            Console.WriteLine(ingredients_and_counts.Where(ingredient_and_count => allergens_and_possible_ingredients.Values.All(allergen_possible_ingredients => !allergen_possible_ingredients.Contains(ingredient_and_count.Key))).Select(ingredient_and_count => ingredient_and_count.Value).Sum());

            /*
             * Part 2
             *
             * Determine which ingredients contain an allergen, and sort them (ascending) alphabetically by the name of the corresponding allergen
             */
            var allergen_possible_ingredients_reduced = true;

            while (allergen_possible_ingredients_reduced) {
                allergen_possible_ingredients_reduced = false;

                var isolated_ingredients = allergens_and_possible_ingredients.Where(allergen_possible_ingredients => allergen_possible_ingredients.Value.Count == 1).Select(allergen_possible_ingredients => allergen_possible_ingredients.Value.First()).ToList();
                
                foreach (var allergen in allergens_and_possible_ingredients.Keys) {
                    if (
                        allergens_and_possible_ingredients[allergen].Count > 1
                     && allergens_and_possible_ingredients[allergen].Any(ingredient => isolated_ingredients.Contains(ingredient))
                    ) {
                        allergens_and_possible_ingredients[allergen].ExceptWith(isolated_ingredients);
                        allergen_possible_ingredients_reduced = true;
                    }
                }
            }

            Console.WriteLine(string.Join(",", allergens_and_possible_ingredients.OrderBy(allergen_possible_ingredients => allergen_possible_ingredients.Key).Select(allergen_possible_ingredients => allergen_possible_ingredients.Value.First())));
        }
    }
}