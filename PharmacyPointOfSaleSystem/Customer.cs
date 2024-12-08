using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PharmacyPointOfSaleSystem.Interfaces;

namespace PharmacyPointOfSaleSystem
{
    public class Customer : User, ICreateOrder
    {
        private static string orderFilePath;
        private static string tempPath = $"C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\TMP.csv";
        private static string latestOrderPath = $"C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\LatestOrder.txt";

        private static int orderNumber = 0;
        public Customer()
        {
            // track latest order number
            orderNumber = GetLatestOrderNumber() + 1;
            orderFilePath = $"C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\OrderQueue\\OR{orderNumber}.csv";
        }
        public void PlaceOrder()
        {
            orderFilePath = $"C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\OrderQueue\\OR{orderNumber}.csv";
            try
            {

                // Read all lines from the order CSV file
                var lines = File.ReadAllLines(tempPath);

                if (lines.Length <= 1)
                {
                    Console.WriteLine("No items in this order.\n");
                    return;
                }
                else
                {
                    // Append the order number to Orders.txt before incrementing
                    using (var writer = new StreamWriter(ordersLogPath, append: true))
                    {
                        writer.WriteLine($"OR{orderNumber}");
                    }

                    Console.WriteLine($"Order OR{orderNumber} placed on " + DateTime.Now);
                    Console.WriteLine($"Please pay {CalculateSubTotal():F2} to the Cashier.");
                    File.Move(tempPath, orderFilePath);
                    Console.WriteLine();
                    orderNumber++; // update order number;

                    // Write to the file, overwriting any existing content
                    File.WriteAllText(latestOrderPath, orderNumber.ToString());

                    Console.WriteLine("File has been overwritten.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("No items in this order.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("No items in this order.\n");
            }
        }

        public void ClearCart()
        {
            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                    Console.WriteLine("Cart cleared successfully.");
                }
                else
                {
                    Console.WriteLine("Error clearing cart. Empty.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Error clearing cart. Empty.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error clearing cart. Empty.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing cart. Empty.");
            }
        }

        public void ViewCart()
        {
            const int LineWidth = 85; // Adjust as needed for consistent width in separators

            try
            {
                if (!File.Exists(tempPath))
                {
                    Console.WriteLine("No order details to show.\n");
                    return;
                }

                // Read all lines from the order CSV file
                var lines = File.ReadAllLines(tempPath);

                if (lines.Length <= 1)
                {
                    Console.WriteLine("No items in this order.\n");
                    return;
                }

                // Split header line to get column names
                var headers = lines[0].Split(',');

                // Print header with proper formatting
                Console.WriteLine();
                Console.WriteLine($"Order Details: OR{orderNumber} \t" + DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine(new string('-', LineWidth));
                Console.WriteLine($"{headers[0],-10} {headers[1],-20} {headers[2],-10} {headers[3],-10} {headers[4],-15}");
                Console.WriteLine(new string('-', LineWidth));

                // Print each order item with alignment
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(',');

                    // Check that the line has the right number of columns
                    if (values.Length == headers.Length)
                    {
                        Console.WriteLine($"{values[0],-10} {values[1],-20} {values[2],-10} {values[3],-10} {values[4],-15}");
                    }
                }

                Console.WriteLine(new string('-', LineWidth)); // Bottom border line
                // Print Subtotal
                Console.WriteLine($"Subtotal: {CalculateSubTotal():F2}");
                Console.WriteLine();
                Console.WriteLine("End of order details."); // Indicate end of order list
                Console.WriteLine();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while accessing the order details: {ex.Message}");
                Console.WriteLine("Please contact support.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine("Please contact support.");
            }
        }

        public void AddToCart(string name)
        {
            // Get the entry line of the product in the file
            string[] productLine = FindProductByCode(name);
            string prescriptionImageLink = " ";

            // Check if the order file exists, create it only if it doesn't exist
            try
            {
                // Check if the order file exists, create it only if it doesn't exist
                if (!File.Exists(tempPath))
                {
                    using (var writer = new StreamWriter(tempPath))
                    {
                        writer.WriteLine("Code,Name,Quantity,Price,Prescription (if applicable)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Optionally log the exception or handle it further
            }


            // Step 1: Check Prescription
            bool isPrescription = productLine[5].ToLower() == "true";
            if (isPrescription)
            {
                Console.Write("This product requires a prescription. Do you have one? [y/n]: ");
                string hasPrescription = Console.ReadLine().ToLower();
                if (hasPrescription != "y")
                {
                    Console.WriteLine("Cannot proceed without a prescription for this product.");
                    Console.WriteLine("Please secure a prescription and try again.");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                    return; // Skip adding this product and ask for another product
                }
                Console.Write("Enter the link of the prescription image: ");
                prescriptionImageLink = Console.ReadLine();
                // Optionally: Save or validate the link if needed
            }

            // Step 2: Prompt for quantity
            int quantity = 0;
            bool validQuantity = false;
            byte choice = 0;

            while (!validQuantity)
            {
                Console.Write("Enter quantity: ");
                string quantityInput = Console.ReadLine();
                validQuantity = int.TryParse(quantityInput, out quantity) && quantity >= 0;

                if (!validQuantity)
                {
                    Console.WriteLine("Invalid quantity. Please enter a positive integer.");
                }
                else if(quantity > int.Parse(productLine[3]))
                {
                    Console.WriteLine("Not enough stock. Would you like to continue with the remaining stock or skip this product?");
                    Console.WriteLine("[y] - Continue   [n] - Skip");
                    while (choice != 121 && choice != 110) { choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar); }   // check for 'y' or 'n' keypress
                    Console.WriteLine((char)choice);

                    if ((char)choice == 'y')
                    {
                        quantity = int.Parse(productLine[3]);
                        break;
                    } 
                    else
                    {
                        Console.WriteLine($"Skipping this product [{productLine[1]}]...");
                        Console.Write("Press 'Enter' to continue...");
                        Console.ReadLine();
                        return;
                    }

                } else if (quantity == 0)
                {
                    Console.WriteLine($"Skipping this product [{productLine[1]}]...");
                    Console.Write("Press 'Enter' to continue...");
                    Console.ReadLine();
                    return;
                }
            }

            // Step 6: Append product to order file
            using (var writer = new StreamWriter(tempPath, append: true))
            {
                writer.WriteLine($"{productLine[0]},{productLine[1]},{quantity},{productLine[4]}, {prescriptionImageLink}");
            }

            Console.WriteLine("Product added to cart.");

        }

        // Method to get latest order number
        public static int GetLatestOrderNumber()
        {
            try
            {
                // Check if the file exists
                if (File.Exists(latestOrderPath))
                {
                    // Read the single line from the file
                    string numberString = File.ReadAllText(latestOrderPath);

                    // Try to parse the number
                    if (int.TryParse(numberString, out int number))
                    {
                        return number;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    Console.WriteLine("Error: The file does not exist.");

                    return 0;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Access to the file is denied.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }


            return 0;
        }
    

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
                            name = columns[1].Trim(),
                            Brand = columns[2].Trim(), // Assuming the 3rd column is brand
                            quantity = int.TryParse(columns[3].Trim(), out var qty) ? qty : 0,
                            price = double.TryParse(columns[4].Trim(), out var prc) ? prc : 0.00,
                            isPrescription = bool.TryParse(columns[5].Trim(), out var isPrescription) ? isPrescription : false,
                            info = columns.Length > 6 ? columns[6].Trim() : "N/A"
                        };

                        if (product.quantity == 0)
                        {
                            product = null; // reset product
                            continue;    // skip if the product is out of stock
                        }

                        productList.Add(product);
                    }
                }

                // Create a table using Spectre.Console
                var table = new Table();

                // Add columns to the table
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

        public override string[] SearchProduct(string key)
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

            // Perform an exact match search on Name
            var exactMatches = products.Where(product =>
                product[1].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (exactMatches.Count == 1)
            {
                // If exactly one match is found, display the product in a table
                var match = exactMatches[0];
                DisplayProductInTable(match);

                // Return the match as an array of strings (trimmed, without commas)
                return match.Select(column => column.Trim()).ToArray();
            }

            // If no exact match is found, attempt a wildcard search
            var wildcardMatches = products.Where(product =>
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
            table.AddColumn("Name");
            table.AddColumn("Brand");
            table.AddColumn("Quantity");
            table.AddColumn("Price (PHP)");
            table.AddColumn("Prescription?");
            table.AddColumn("Info");

            table.AddRow(
                product[1].Trim(),
                product[2].Trim(),
                product[3].Trim(),
                product[4].Trim(),
                string.IsNullOrWhiteSpace(product[5]) ? "No" : "Yes",
                product[6].Trim()
            );

            Console.Clear();
            AnsiConsole.Write(table);
        }

        // Method to display multiple products in a table
        public virtual void DisplayProductsInTable(IEnumerable<string[]> products)
        {
            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Brand");
            table.AddColumn("Quantity");
            table.AddColumn("Price (PHP)");
            table.AddColumn("Prescription?");
            table.AddColumn("Info");

            foreach (var product in products)
            {
                table.AddRow(
                    product[1].Trim(),
                    product[2].Trim(),
                    product[3].Trim(),
                    product[4].Trim(),
                    string.IsNullOrWhiteSpace(product[5]) ? " " : "Yes",
                    product[6].Trim()
                );
            }

            Console.Clear();
            AnsiConsole.Write(table); // Display spectre table
            Console.WriteLine();
            Console.WriteLine("Multiple results found: " + products.Count() + " products.");
        }

        // Method to calculate subtotal of items in cart
        public double CalculateSubTotal()
        {
            try
            {
                if (File.Exists(tempPath))
                {
                    // Read all lines from the CSV
                    var lines = File.ReadAllLines(tempPath);

                    // Skip the header line and parse the CSV
                    var entries = lines.Skip(1) // Skip the header
                                       .Select(line => line.Split(',')) // Split each line by commas
                                       .Select(columns => new
                                       {
                                           Quantity = int.TryParse(columns[2].Trim(), out var qty) ? qty : 0,
                                           Price = double.TryParse(columns[3].Trim(), out var prc) ? prc : 0.0
                                       });

                    // Calculate the subtotal
                    double subtotal = entries.Sum(entry => entry.Quantity * entry.Price);
                    return subtotal;
                }
                else
                {
                    Console.WriteLine("The CSV file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return 0;
        }

        // Method to find a product by its code in the product list CSV
        private string[] FindProductByCode(string code)
        {
            var lines = File.ReadAllLines(productListPath);
            for (int i = 1; i < lines.Length; i++) // Skipping header row
            {
                var product = lines[i].Split(',');
                if (product[0] == code) // Assuming first column is Code
                {
                    return product;
                }
            }
            return null; // Product not found
        }
    }
}
