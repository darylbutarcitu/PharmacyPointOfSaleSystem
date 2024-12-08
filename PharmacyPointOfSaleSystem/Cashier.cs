using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PharmacyPointOfSaleSystem.Interfaces;

namespace PharmacyPointOfSaleSystem
{
    public class Cashier : User, IManageOrder
    {
        private double totalSales;
        private int quantitySold;
        string filePath = "C:\\Users\\rowoon\\source\\repos\\PharmacyPOS\\PharmacyPOS\\db\\ProductList.csv";
        string receiptsDirectoryPath = $"C:\\Users\\rowoon\\source\\repos\\PharmacyPointOfSaleSystem\\PharmacyPointOfSaleSystem\\files\\Receipts";
        
        public Cashier()
        {
            // totalSales = read from sales report file
            // quantitysold = read from sales report file
            totalSales = 0;
            quantitySold = 0;
            // if no file, set to zero and create sales report file
        }
        public void CreateReceipt()
        {
            try
            {
                var orderEntries = File.ReadAllLines(ordersLogPath).ToList();
                if (orderEntries.Count == 0)
                {
                    Console.WriteLine("No pending orders to process.");
                    return;
                }

                bool orderFound = false;

                // Continue processing each entry until a valid order with items is found
                while (orderEntries.Count > 0 && !orderFound)
                {
                    string firstOrderEntry = orderEntries[0];
                    string orderFilePath = Path.Combine(directoryPath, $"{firstOrderEntry}.csv");

                    // Remove the entry from the list and update Orders.txt file
                    orderEntries.RemoveAt(0);
                    File.WriteAllLines(ordersLogPath, orderEntries);

                    // Check if the file exists
                    if (!File.Exists(orderFilePath))
                    {
                        Console.WriteLine($"Order file {firstOrderEntry} not found. Moving to the next entry.");
                        continue;
                    }

                    // Read the order file and skip if it only contains the header
                    var orderLines = File.ReadAllLines(orderFilePath);
                    if (orderLines.Length <= 1)
                    {
                        // If file contains only header, delete the file and proceed to the next
                        File.Delete(orderFilePath);
                        Console.WriteLine($"Order file {firstOrderEntry} contains no items and has been deleted.");
                        continue;
                    }

                    // If a valid order file is found, display it and proceed with processing
                    DisplayCurrentOrder(orderFilePath);
                    orderFound = true;  // Mark that a valid order has been found and break the loop

                    double orderTotal = 0;
                    var header = orderLines[0].Split(',');
                    var salesReportData = new List<(string Code, string Name, int Quantity, double Amount)>();
                    var receiptData = new List<(string Code, string Name, int Quantity, double Price, double Total)>(); // For the receipt

                    // Calculate total sales and update quantities
                    for (int i = 1; i < orderLines.Length; i++)
                    {
                        var values = orderLines[i].Split(',');

                        string code = values[0];
                        string name = values[1];
                        int quantity = int.Parse(values[2]);
                        double price = double.Parse(values[3]);
                        double itemTotal = quantity * price;

                        orderTotal += itemTotal;
                        quantitySold += quantity;
                        totalSales += itemTotal;

                        salesReportData.Add((Code: code, Name: name, Quantity: quantity, Amount: itemTotal));

                        // Prepare the receipt data
                        receiptData.Add((Code: code, Name: name, Quantity: quantity, Price: price, Total: itemTotal));

                        // Decrement quantity per transaction
                        UpdateProduct(code, quantity);
                    }

                    Console.WriteLine($"Total for order {firstOrderEntry}: {orderTotal:F2} php");

                    // Generate the receipt file path
                    string receiptFileName = $"TRN{firstOrderEntry}_{DateTime.Now:yyyy_MM_dd}.csv";
                    string receiptFilePath = Path.Combine(receiptsDirectoryPath, receiptFileName);

                    // Write the receipt data to the CSV file
                    using (var receiptWriter = new StreamWriter(receiptFilePath))
                    {
                        receiptWriter.WriteLine("Code,Name,Quantity,Price,Total");

                        foreach (var entry in receiptData)
                        {
                            receiptWriter.WriteLine($"{entry.Code},{entry.Name},{entry.Quantity},{entry.Price},{entry.Total}");
                        }
                        receiptWriter.WriteLine($"Overall Total, {orderTotal:F2}");
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Receipt for order {firstOrderEntry} saved as {receiptFileName}");

                    // Delete the order file after processing
                    File.Delete(orderFilePath);
                    Console.WriteLine();
                }

                if (!orderFound)
                {
                    Console.WriteLine("No valid orders with items found to process.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the order: {ex.Message}");
            }
        }



        // Method to display the current order details
        private void DisplayCurrentOrder(string orderFilePath)
        {
            const int LineWidth = 55; // Adjust as needed for consistent width in separators

            try
            {
                if (!File.Exists(orderFilePath))
                {
                    Console.WriteLine("No order details to show.");
                    return;
                }

                // Read all lines from the order CSV file
                var lines = File.ReadAllLines(orderFilePath);

                if (lines.Length <= 1)
                {
                    Console.WriteLine("No items in this order.");
                    return;
                }

                // Split header line to get column names
                var headers = lines[0].Split(',');

                // Print header with proper formatting
                Console.WriteLine();
                Console.WriteLine("Order Details: " + DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                Console.WriteLine(new string('-', LineWidth));
                Console.WriteLine($"{headers[0],-10} {headers[1],-20} {headers[2],-10} {headers[3],-15}");
                Console.WriteLine(new string('-', LineWidth));

                // Print each order item with alignment
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(',');

                    // Check that the line has the right number of columns
                    if (values.Length == headers.Length)
                    {
                        Console.WriteLine($"{values[0],-10} {values[1],-20} {values[2],-10} {values[3],-15}");
                    }
                }

                Console.WriteLine(new string('-', LineWidth)); // Bottom border line
                //Console.WriteLine("End of order details.\n"); // Indicate end of order list
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

        public void UpdateProduct(string code, int quantitySold)
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


                // Update Quantity
                int currentQuantity = int.Parse(currentProduct[3]) - quantitySold;
                string quantityInput = currentQuantity.ToString();
                currentProduct[3] = string.IsNullOrEmpty(quantityInput) ? currentProduct[3] : quantityInput;


                // Update the record in the file
                lines[productIndex] = string.Join(",", currentProduct);

                // Write the updated data back to the file
                File.WriteAllLines(productListPath, lines);
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

        public void DailySalesReport()
        {
            // Define paths
            string receiptsDirectoryPath = @"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\Receipts";
            string dailySalesReportPath = @$"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\SalesReports\Daily\DailySalesReport_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv";

            try
            {
                // Get today's date in the required format
                string todayDate = DateTime.Now.ToString("yyyy_MM_dd");

                // Get all receipt files matching today's date
                var receiptFiles = Directory.GetFiles(receiptsDirectoryPath, $"*_{todayDate}.csv");

                if (receiptFiles.Length == 0)
                {
                    Console.WriteLine("No receipts found for today's date.");
                    return;
                }

                // Dictionary to store aggregated product data
                var productSummary = new Dictionary<string, (string Name, int Quantity, double Price, double Subtotal)>();
                double totalSales = 0;
                int totalItemsSold = 0;

                foreach (var file in receiptFiles)
                {
                    var lines = File.ReadAllLines(file);

                    // Skip the header row and process the rest
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(',');

                        if (columns.Length >= 4) // Ensure enough columns exist
                        {
                            string code = columns[0].Trim();
                            string name = columns[1].Trim();
                            int quantity = int.TryParse(columns[2].Trim(), out var qty) ? qty : 0;
                            double price = double.TryParse(columns[3].Trim(), out var prc) ? prc : 0.0;
                            double subtotal = quantity * price;

                            // Combine duplicate products
                            string productKey = $"{code}-{name}";
                            if (productSummary.ContainsKey(productKey))
                            {
                                var existing = productSummary[productKey];
                                productSummary[productKey] = (
                                    existing.Name,
                                    existing.Quantity + quantity,
                                    price, // Keep the same price
                                    existing.Subtotal + subtotal
                                );
                            }
                            else
                            {
                                productSummary[productKey] = (name, quantity, price, subtotal);
                            }

                            // Accumulate overall totals
                            totalSales += subtotal;
                            totalItemsSold += quantity;
                        }
                    }
                }

                // Write the daily sales report to CSV
                using (var writer = new StreamWriter(dailySalesReportPath))
                {
                    // Write the header
                    writer.WriteLine("Code,Name,Quantity,Price,Subtotal");

                    // Write all aggregated sales data
                    foreach (var product in productSummary)
                    {
                        writer.WriteLine($"{product.Key.Split('-')[0]},{product.Value.Name},{product.Value.Quantity},{product.Value.Price:F2},{product.Value.Subtotal:F2}");
                    }

                    // Write summary data
                    writer.WriteLine();
                    writer.WriteLine($"Total Items Sold: {totalItemsSold}");
                    writer.WriteLine($"Total Sales: {totalSales:F2}");
                }

                // Display the daily sales report in the console
                Console.WriteLine($"\nDaily Sales Report for {todayDate}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine("{0,-10} {1,-20} {2,-10} {3,-10} {4,-10}", "Code", "Name", "Quantity", "Price", "Subtotal");
                Console.WriteLine(new string('-', 60));

                foreach (var product in productSummary)
                {
                    Console.WriteLine(
                        "{0,-10} {1,-20} {2,-10} {3,-10:F2} {4,-10:F2}",
                        product.Key.Split('-')[0],
                        product.Value.Name,
                        product.Value.Quantity,
                        product.Value.Price,
                        product.Value.Subtotal
                    );
                }

                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Total Items Sold: {totalItemsSold}");
                Console.WriteLine($"Total Sales: PHP {totalSales:F2}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine($"\nDaily sales report saved to: {dailySalesReportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating the daily sales report: {ex.Message}");
            }
        }

        public void WeeklySalesReport()
        {
            string receiptsDirectoryPath = @"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\Receipts";
            string weeklySalesReportPath = @$"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\SalesReports\Weekly\WeeklySales_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv";

            try
            {
                // Get today's date and calculate the date 7 days ago
                DateTime today = DateTime.Today;
                DateTime sevenDaysAgo = today.AddDays(-7);

                // Get all receipt files matching today's date and the last 7 days
                var receiptFiles = Directory.GetFiles(receiptsDirectoryPath, "*.csv")
                                            .Where(file =>
                                            {
                                                string fileName = Path.GetFileName(file);
                                                for (DateTime date = sevenDaysAgo; date <= today; date = date.AddDays(1))
                                                {
                                                    if (fileName.Contains(date.ToString("yyyy_MM_dd")))
                                                        return true;
                                                }
                                                return false;
                                            })
                                            .ToArray();

                if (receiptFiles.Length == 0)
                {
                    Console.WriteLine("No receipts found for the last 7 days.");
                    return;
                }

                // Extract dates from file names
                var receiptDates = receiptFiles
                    .Select(file => Path.GetFileName(file))
                    .Select(fileName =>
                    {
                        var parts = fileName.Split('_');
                        if (parts.Length >= 4 && DateTime.TryParseExact($"{parts[1]}_{parts[2]}_{parts[3].Split('.')[0]}", "yyyy_MM_dd", null, System.Globalization.DateTimeStyles.None, out var date))
                        {
                            return date;
                        }
                        return (DateTime?)null;
                    })
                    .Where(date => date.HasValue)
                    .Select(date => date.Value)
                    .OrderBy(date => date)
                    .ToList();

                // Determine the start date and end date from receipt dates
                DateTime startDate = receiptDates.First();
                DateTime endDate = receiptDates.Last();

                // Dictionary to store aggregated product data
                var productSummary = new Dictionary<string, (string Name, int Quantity, double Price, double Subtotal)>();
                double totalSales = 0;
                int totalItemsSold = 0;

                foreach (var file in receiptFiles)
                {
                    var lines = File.ReadAllLines(file);

                    // Skip the header row and process the rest
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(',');

                        if (columns.Length >= 4) // Ensure enough columns exist
                        {
                            string code = columns[0].Trim();
                            string name = columns[1].Trim();
                            int quantity = int.TryParse(columns[2].Trim(), out var qty) ? qty : 0;
                            double price = double.TryParse(columns[3].Trim(), out var prc) ? prc : 0.0;
                            double subtotal = quantity * price;

                            // Combine duplicate products
                            string productKey = $"{code}-{name}";
                            if (productSummary.ContainsKey(productKey))
                            {
                                var existing = productSummary[productKey];
                                productSummary[productKey] = (
                                    existing.Name,
                                    existing.Quantity + quantity,
                                    price, // Keep the same price
                                    existing.Subtotal + subtotal
                                );
                            }
                            else
                            {
                                productSummary[productKey] = (name, quantity, price, subtotal);
                            }

                            // Accumulate overall totals
                            totalSales += subtotal;
                            totalItemsSold += quantity;
                        }
                    }
                }

                // Write the weekly sales report to CSV
                using (var writer = new StreamWriter(weeklySalesReportPath))
                {
                    // Write the report period
                    writer.WriteLine($"Weekly Sales Report");
                    writer.WriteLine($"Report Period: {startDate:yyyy_MM_dd} to {endDate:yyyy_MM_dd}");
                    writer.WriteLine();

                    // Write the header
                    writer.WriteLine("Code,Name,Quantity,Price,Subtotal");

                    // Write all aggregated sales data
                    foreach (var product in productSummary)
                    {
                        writer.WriteLine($"{product.Key.Split('-')[0]},{product.Value.Name},{product.Value.Quantity},{product.Value.Price:F2},{product.Value.Subtotal:F2}");
                    }

                    // Write summary data
                    writer.WriteLine();
                    writer.WriteLine($"Total Items Sold: {totalItemsSold}");
                    writer.WriteLine($"Total Sales: {totalSales:F2}");
                }

                // Display the weekly sales report in the console
                Console.WriteLine($"\nWeekly Sales Report from {startDate:yyyy_MM_dd} to {endDate:yyyy_MM_dd}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine("{0,-10} {1,-20} {2,-10} {3,-10} {4,-10}", "Code", "Name", "Quantity", "Price", "Subtotal");
                Console.WriteLine(new string('-', 60));

                foreach (var product in productSummary)
                {
                    Console.WriteLine(
                        "{0,-10} {1,-20} {2,-10} {3,-10:F2} {4,-10:F2}",
                        product.Key.Split('-')[0],
                        product.Value.Name,
                        product.Value.Quantity,
                        product.Value.Price,
                        product.Value.Subtotal
                    );
                }

                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Total Items Sold: {totalItemsSold}");
                Console.WriteLine($"Total Sales: PHP {totalSales:F2}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine($"\nWeekly sales report saved to: {weeklySalesReportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating the weekly sales report: {ex.Message}");
            }
        }

        public void MonthlySalesReport()
        {
            string receiptsDirectoryPath = @"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\Receipts";
            string monthlySalesReportPath = @$"C:\Users\rowoon\source\repos\PharmacyPointOfSaleSystem\PharmacyPointOfSaleSystem\files\SalesReports\Monthly\MonthlySales_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv";

            try
            {
                // Get today's date and calculate the date 30 days ago
                DateTime today = DateTime.Today;
                DateTime thirtyDaysAgo = today.AddDays(-30);

                // Get all receipt files matching today's date and the last 30 days
                var receiptFiles = Directory.GetFiles(receiptsDirectoryPath, "*.csv")
                                            .Where(file =>
                                            {
                                                string fileName = Path.GetFileName(file);
                                                for (DateTime date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
                                                {
                                                    if (fileName.Contains(date.ToString("yyyy_MM_dd")))
                                                        return true;
                                                }
                                                return false;
                                            })
                                            .ToArray();

                if (receiptFiles.Length == 0)
                {
                    Console.WriteLine("No receipts found for the last 30 days.");
                    return;
                }

                // Extract dates from file names
                var receiptDates = receiptFiles
                    .Select(file => Path.GetFileName(file))
                    .Select(fileName =>
                    {
                        var parts = fileName.Split('_');
                        if (parts.Length >= 4 && DateTime.TryParseExact($"{parts[1]}_{parts[2]}_{parts[3].Split('.')[0]}", "yyyy_MM_dd", null, System.Globalization.DateTimeStyles.None, out var date))
                        {
                            return date;
                        }
                        return (DateTime?)null;
                    })
                    .Where(date => date.HasValue)
                    .Select(date => date.Value)
                    .OrderBy(date => date)
                    .ToList();

                // Determine the start date and end date from receipt dates
                DateTime startDate = receiptDates.First();
                DateTime endDate = receiptDates.Last();

                // Dictionary to store aggregated product data
                var productSummary = new Dictionary<string, (string Name, int Quantity, double Price, double Subtotal)>();
                double totalSales = 0;
                int totalItemsSold = 0;

                foreach (var file in receiptFiles)
                {
                    var lines = File.ReadAllLines(file);

                    // Skip the header row and process the rest
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(',');

                        if (columns.Length >= 4) // Ensure enough columns exist
                        {
                            string code = columns[0].Trim();
                            string name = columns[1].Trim();
                            int quantity = int.TryParse(columns[2].Trim(), out var qty) ? qty : 0;
                            double price = double.TryParse(columns[3].Trim(), out var prc) ? prc : 0.0;
                            double subtotal = quantity * price;

                            // Combine duplicate products
                            string productKey = $"{code}-{name}";
                            if (productSummary.ContainsKey(productKey))
                            {
                                var existing = productSummary[productKey];
                                productSummary[productKey] = (
                                    existing.Name,
                                    existing.Quantity + quantity,
                                    price, // Keep the same price
                                    existing.Subtotal + subtotal
                                );
                            }
                            else
                            {
                                productSummary[productKey] = (name, quantity, price, subtotal);
                            }

                            // Accumulate overall totals
                            totalSales += subtotal;
                            totalItemsSold += quantity;
                        }
                    }
                }

                // Write the monthly sales report to CSV
                using (var writer = new StreamWriter(monthlySalesReportPath))
                {
                    // Write the report period
                    writer.WriteLine($"Monthly Sales Report");
                    writer.WriteLine($"Report Period: {startDate:yyyy_MM_dd} to {endDate:yyyy_MM_dd}");
                    writer.WriteLine();

                    // Write the header
                    writer.WriteLine("Code,Name,Quantity,Price,Subtotal");

                    // Write all aggregated sales data
                    foreach (var product in productSummary)
                    {
                        writer.WriteLine($"{product.Key.Split('-')[0]},{product.Value.Name},{product.Value.Quantity},{product.Value.Price:F2},{product.Value.Subtotal:F2}");
                    }

                    // Write summary data
                    writer.WriteLine();
                    writer.WriteLine($"Total Items Sold: {totalItemsSold}");
                    writer.WriteLine($"Total Sales: {totalSales:F2}");
                }

                // Display the monthly sales report in the console
                Console.WriteLine($"\nMonthly Sales Report from {startDate:yyyy_MM_dd} to {endDate:yyyy_MM_dd}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine("{0,-10} {1,-20} {2,-10} {3,-10} {4,-10}", "Code", "Name", "Quantity", "Price", "Subtotal");
                Console.WriteLine(new string('-', 60));

                foreach (var product in productSummary)
                {
                    Console.WriteLine(
                        "{0,-10} {1,-20} {2,-10} {3,-10:F2} {4,-10:F2}",
                        product.Key.Split('-')[0],
                        product.Value.Name,
                        product.Value.Quantity,
                        product.Value.Price,
                        product.Value.Subtotal
                    );
                }

                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Total Items Sold: {totalItemsSold}");
                Console.WriteLine($"Total Sales: PHP {totalSales:F2}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine($"\nMonthly sales report saved to: {monthlySalesReportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating the monthly sales report: {ex.Message}");
            }
        }

    }
}
