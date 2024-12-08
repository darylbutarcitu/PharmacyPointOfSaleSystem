using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PharmacyPointOfSaleSystem.Interfaces;

namespace PharmacyPointOfSaleSystem
{
    public class StorageManager : User, ICreateProduct, IModifyProduct
    {
        private int emptied;
        public StorageManager()
        {
            // Read file of product list
            // count how many products are emptied, qty == 0
        }

        // Unique method to show out of stock
        public void ShowZeroProducts()
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

                        if (product.quantity != 0)
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

        public void AddProduct(Product product)
        {
            string record = product.code.ToUpper() + "," + product.name + "," + product.Brand + "," +
                            product.quantity + "," + product.price + "," +
                            (product.isPrescription ? "true" : " ") + "," + product.info;

            try
            {
                // Check if the file exists
                if (!File.Exists(productListPath))
                {
                    // If the file doesn't exist, create it and write the header
                    using (StreamWriter writer = new StreamWriter(productListPath, false))
                    {
                        writer.NewLine = "\r\n"; // Ensure Windows-style line endings
                        writer.WriteLine("Code,Name,Brand,Quantity,Price,isPrescription,Info"); // Header line
                        writer.WriteLine(record); // Add the product record as the first entry
                    }
                    Console.WriteLine("Product file created with header and product added.");
                }
                else
                {
                    // If the file exists, just append the product record
                    using (StreamWriter writer = File.AppendText(productListPath))
                    {
                        writer.NewLine = "\r\n"; // Ensure Windows-style line endings
                        writer.WriteLine(record); // Add the product record to the file
                    }
                    Console.WriteLine("Product successfully added to inventory.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product to file: {ex.Message}");
            }
        }

        public int RemoveProduct(string code)
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(productListPath))
                {
                    Console.WriteLine("The product list file does not exist.");
                    return -1;
                }

                // Read all lines from the CSV file
                var lines = File.ReadAllLines(productListPath).ToList();

                // Check if the file has more than just the header
                if (lines.Count <= 1)
                {
                    Console.WriteLine("No products available to remove.");
                    return -1;
                }

                // Find the product with the matching code
                bool productFound = false;
                string temp = " ";
                for (int i = 1; i < lines.Count; i++)  // Start from 1 to skip the header
                {
                    var values = lines[i].Split(',');

                    if (values[0].Trim() == code)  // Match the product code
                    {
                        lines.RemoveAt(i);  // Remove the product entry
                        temp = values[0] + " " + values[1];
                        productFound = true;
                        break;  // Exit the loop once the product is found and removed
                    }
                }

                if (productFound)
                {
                    // Write the updated data back to the file
                    File.WriteAllLines(productListPath, lines);
                    Console.WriteLine(temp + " removed successfully.");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Product not found in the list.");
                    return -1;
                }
            }
            catch (IOException ex)
            {
                // Handle file access issues (e.g., file being used by another process)
                Console.WriteLine($"An error occurred while accessing the file: {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                // Catch any other general errors
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return -1;
            }
        }

        public void UpdateProduct(string code)
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(productListPath))
                {
                    Console.WriteLine("The product list file does not exist.");
                    return;
                }

                // Read all lines from the CSV file
                var lines = File.ReadAllLines(productListPath).ToList();

                // Check if the file has more than just the header
                if (lines.Count <= 1)
                {
                    Console.WriteLine("No products available to update.");
                    return;
                }

                // Find the product with the matching code
                bool productFound = false;
                int productIndex = -1;
                string[] currentProduct = null;

                for (int i = 1; i < lines.Count; i++)  // Start from 1 to skip the header
                {
                    var values = lines[i].Split(',');

                    if (values[0].Trim() == code)  // Match the product code
                    {
                        productFound = true;
                        productIndex = i;
                        currentProduct = values;
                        break;  // Exit the loop once the product is found
                    }
                }

                if (!productFound)
                {
                    Console.WriteLine("Product not found in the list.");
                    return;
                }

                // Display current product details in a neat, aligned table format
                Console.WriteLine("\nCurrent product details:");
                Console.WriteLine("---------------------------------------");
                Console.WriteLine(" {0,-15}  {1,-20} ", "Field", "Value");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine(" {0,-15}  {1,-20} ", "Code", currentProduct[0]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "Name", currentProduct[1]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "Brand", currentProduct[2]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "Quantity", currentProduct[3]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "Price", currentProduct[4]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "IsPrescription", currentProduct[5]);
                Console.WriteLine(" {0,-15}  {1,-20} ", "Info", currentProduct[6]);
                Console.WriteLine("---------------------------------------");

                // Ask user to update fields, allowing empty input to keep current values
                Console.WriteLine("\nUpdate fields (leave empty to keep current value):");

                // Update Code
                Console.Write("Update Code: ");
                string prodCode = Console.ReadLine().ToUpper();
                currentProduct[0] = string.IsNullOrEmpty(prodCode) ? currentProduct[0] : prodCode;

                // Update Name
                Console.Write("Update Name: ");
                string name = Console.ReadLine();
                currentProduct[1] = string.IsNullOrEmpty(name) ? currentProduct[1] : name;

                // Update Brand
                Console.Write("Update Brand: ");
                string brand = Console.ReadLine();
                currentProduct[2] = string.IsNullOrEmpty(brand) ? currentProduct[2] : brand;

                // Update Quantity
                Console.Write("Update Quantity: ");
                string quantityInput = Console.ReadLine();
                currentProduct[3] = string.IsNullOrEmpty(quantityInput) ? currentProduct[3] : quantityInput;

                // Update Price
                Console.Write("Update Price: ");
                string priceInput = Console.ReadLine();
                currentProduct[4] = string.IsNullOrEmpty(priceInput) ? currentProduct[4] : priceInput;

                // Update Is Prescription
                Console.Write("Update Prescription (true/false): ");
                string isPrescription = Console.ReadLine();
                currentProduct[5] = string.IsNullOrEmpty(isPrescription) ? currentProduct[5] : isPrescription;
                if (currentProduct[5] == "false") { currentProduct[5] = " "; }
                // Update Info
                Console.Write("Update Info: ");
                string info = Console.ReadLine();
                currentProduct[6] = string.IsNullOrEmpty(info) ? currentProduct[6] : info;

                // Update the record in the file
                lines[productIndex] = string.Join(",", currentProduct);

                // Write the updated data back to the file
                File.WriteAllLines(productListPath, lines);

                // Output the updated product details
                Console.WriteLine("\nUpdated product details:");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(" {0,-15}  {1,-25} ", "Field", "New Value");
                Console.WriteLine("-------------------------------");
                Console.WriteLine(" {0,-15}  {1,-25} ", "Code", currentProduct[0], currentProduct[0]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "Name", currentProduct[1], currentProduct[1]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "Brand", currentProduct[2], currentProduct[2]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "Quantity", currentProduct[3], currentProduct[3]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "Price", currentProduct[4], currentProduct[4]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "IsPrescription", currentProduct[5], currentProduct[5]);
                Console.WriteLine(" {0,-15}  {1,-25} ", "Info", currentProduct[6], currentProduct[6]);
                Console.WriteLine("--------------------------------------");
            }
            catch (IOException ex)
            {
                // Handle file access issues (e.g., file being used by another process)
                Console.WriteLine($"An error occurred while accessing the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other general errors
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
