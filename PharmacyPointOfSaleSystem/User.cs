using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Spectre.Console;
using static PharmacyPointOfSaleSystem.Interfaces;

namespace PharmacyPointOfSaleSystem
{
    public abstract class User : IViewInventory
    {
        public string productListPath = "C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\ProductList.csv";
        public static string ordersLogPath = "C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\Orders.txt";
        public static string directoryPath = "C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\OrderQueue\\";

        public virtual void ShowProductList()
        {
            // Check if the file exists
            if (File.Exists(productListPath))
            {
                var productList = new List<Product>();

                var lines = File.ReadAllLines(productListPath);

                // Skip the first line (header) and process the rest of the lines
                foreach (var line in lines.Skip(1)) // This skips the first line (header)
                {
                    var columns = line.Split(',');

                    // Ensure there are enough columns to parse the product
                    if (columns.Length >= 6)
                    {
                        var product = new Product
                        {
                            code = columns[0].Trim(),
                            name = columns[1].Trim(),
                            Brand = columns[2].Trim(), // Assuming the 3rd column is brand
                            quantity = int.TryParse(columns[3].Trim(), out var qty) ? qty : 0,
                            price = double.TryParse(columns[4].Trim(), out var prc) ? prc : 0.00,
                            isPrescription = bool.TryParse(columns[5].Trim(), out var isPrescription) ? isPrescription : false,
                            info = columns.Length > 6 ? columns[6].Trim() : "N/A"
                        };

                        productList.Add(product);
                    }
                }

                // Create a table using Spectre.Console
                var table = new Table();

                // Add columns to the table
                var codeColumn = table.AddColumn("Code");
                var nameColumn = table.AddColumn("Name");
                var brandColumn = table.AddColumn("Brand");
                var quantityColumn = table.AddColumn("Quantity");
                var priceColumn = table.AddColumn("Price (PHP)");
                var prescriptionColumn = table.AddColumn("Prescription?");
                var infoColumn = table.AddColumn("Info");

                // Loop through the products and add them to the table
                foreach (var product in productList)
                {
                    table.AddRow(
                        product.code,
                        product.name,
                        product.Brand,
                        product.quantity.ToString(),
                        product.price.ToString(),
                        (product.isPrescription ? "Yes" : " "),
                        product.info);
                }

                // Display the table
                AnsiConsole.Write(table);
            }
            else
            {
                Console.WriteLine("The file does not exist.");
            }
        }

        public virtual string[] SearchProduct(string key)
        {
            // Check if the file exists
            if (!File.Exists(productListPath))
            {
                Console.WriteLine("The product list file does not exist.");
                return null;
            }

            // Read all lines from the file
            var lines = File.ReadAllLines(productListPath);

            // Parse the CSV into a list of products
            var products = lines.Skip(1) // Skip the header
                                .Select(line => line.Split(','))
                                .ToList();

            // Perform an exact match search on Code or Name
            var exactMatches = products.Where(product =>
                product[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase) ||
                product[1].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (exactMatches.Count == 1)
            {
                // If exactly one match is found, display the product in a table
                var match = exactMatches[0];
                //DisplayProductInTable(match);

                // Return the match as an array of strings (trimmed, without commas)
                return match.Select(column => column.Trim()).ToArray();
            }

            // If no exact match is found, attempt a wildcard search
            var wildcardMatches = products.Where(product =>
                product[0].Trim().StartsWith(key, StringComparison.OrdinalIgnoreCase) ||
                product[1].Trim().StartsWith(key, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (wildcardMatches.Count == 1)
            {
                // If exactly one wildcard match is found, display the product in a table
                var match = wildcardMatches[0];

                // Return the match as an array of strings (trimmed, without commas)
                return match.Select(column => column.Trim()).ToArray();
            }

            if (wildcardMatches.Count > 1)
            {
                // If there are multiple wildcard matches, display them in a table
                DisplayProductsInTable(wildcardMatches);
                return null;
            }

            // If no results are found, display "N/A" in the table
            //DisplayNoResults();
            return null;
        }


        // Method to display a single product in a table
        public virtual void DisplayProductInTable(string[] product)
        {
            var table = new Table();
            table.AddColumn("Code");
            table.AddColumn("Name");
            table.AddColumn("Brand");
            table.AddColumn("Quantity");
            table.AddColumn("Price (PHP)");
            table.AddColumn("Prescription?");
            table.AddColumn("Info");

            table.AddRow(
                product[0].Trim(),
                product[1].Trim(),
                product[2].Trim(),
                product[3].Trim(),
                product[4].Trim(),
                string.IsNullOrWhiteSpace(product[5]) ? "No" : "Yes",
                product[6].Trim()
            );

            AnsiConsole.Write(table);
        }

        // Method to display multiple products in a table
        public virtual void DisplayProductsInTable(IEnumerable<string[]> products)
        {
            var table = new Table();
            table.AddColumn("Code");
            table.AddColumn("Name");
            table.AddColumn("Brand");
            table.AddColumn("Quantity");
            table.AddColumn("Price (PHP)");
            table.AddColumn("Prescription?");
            table.AddColumn("Info");

            foreach (var product in products)
            {
                table.AddRow(
                    product[0].Trim(),
                    product[1].Trim(),
                    product[2].Trim(),
                    product[3].Trim(),
                    product[4].Trim(),
                    string.IsNullOrWhiteSpace(product[5]) ? " " : "Yes",
                    product[6].Trim()
                );
            }

            Console.WriteLine();
            AnsiConsole.Write(table); // Display spectre table
            Console.WriteLine();
            Console.WriteLine("Multiple results found: " + products.Count() + " products.");
        }

        // Method to display "N/A" if no results are found
        public virtual void DisplayNoResults()
        {
            var table = new Table();
            table.AddColumn("Code");
            table.AddColumn("Name");
            table.AddColumn("Brand");
            table.AddColumn("Quantity");
            table.AddColumn("Price (PHP)");
            table.AddColumn("Prescription?");
            table.AddColumn("Info");

            table.AddRow("N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A");

            Console.WriteLine();
            AnsiConsole.Write(table);
        }
    }
}