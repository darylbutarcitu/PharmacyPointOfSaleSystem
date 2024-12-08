using System;
using System.Threading;
using PharmacyPointOfSaleSystem;

internal class Program
{
    static int currentSelection = 0; // To track which option is selected
    static byte choice = 0;   // to track keypress
    static ConsoleKeyInfo keyPress; // to track keypress
    static bool isExit = false; // To track if to end program
    static bool isCustomer = true;  // change back to true
    static bool isManager = false;  // change back to false
    static bool isCashier = false;  // change back to false
    static string code = " "; // for string inputs;

    public static void Main(string[] args)
    {
        Customer customer = new Customer();
        StorageManager manager = new StorageManager();
        Cashier cashier = new Cashier();
        BootingScreen(); // Optional booting screen
        while (!isExit)
        {
            /* CUSTOMER */
            while (isCustomer)
            {
                PromptCustomer();
                switch (currentSelection)
                {
                    case 0: // Switch User
                        PromptUserChange();
                        switch (currentSelection)
                        {
                            case 0: // Customer
                                break;  // just go back
                            case 1: // Storage Manager
                                isCustomer = false;
                                isManager = true;
                                isCashier = false;
                                CheckPassword();
                                Console.WriteLine();
                                Console.Write("[Switching to Storage Manager]\nLoading...");
                                SimulateLoading();
                                break;
                            case 2: // Cashier
                                isCustomer = false;
                                isManager = false;
                                isCashier = true;
                                CheckPassword();
                                Console.WriteLine();
                                Console.Write("[Switching to Cashier]\nLoading...");
                                SimulateLoading();
                                break;
                        }
                        break;
                    case 1: // Show List of Products
                        Console.Clear();
                        customer.ShowProductList();
                        Console.WriteLine("Press 'Enter' to go back. 'S' to search.");

                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (keyPress.KeyChar == 's' || keyPress.KeyChar == 'S')
                            {
                                goto case 2;    // Search Product case
                            }
                        }
                        break;
                    case 2: // Search Product
                        bool isEndSearch = false;
                        while (!isEndSearch)
                        {
                            Console.Clear();
                            customer.ShowProductList();
                            Console.Write("\nEnter product name (Press enter to cancel search): ");
                            string key = Console.ReadLine();

                            
                            // Cancel product search by pressing enter or inputting whitespace
                            if (key != "" && key != " ")
                            {
                                string[] result = customer.SearchProduct(key);
                                if (result == null)
                                {
                                    Console.WriteLine("\nPress 'Enter' to search again.");
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                    Console.Clear();
                                    customer.ShowProductList(); // TO REMOVE
                                }
                                else
                                {
                                    Console.Clear();
                                    customer.DisplayProductInTable(result);
                                    Console.WriteLine("\n1 product found!");
                                    currentSelection = 0;
                                    PromptAddToCart();
                                    choice = 0;
                                    if (currentSelection == 0) // Add Product to Cart
                                    {
                                        Console.Write("Confirm ADD TO CART? [y/n]: ");
                                        while (choice != 121 && choice != 110) { choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar); }   // check for 'y' or 'n' keypress
                                        Console.WriteLine((char)choice);

                                        if ((char)choice == 'y')
                                        {
                                            Console.WriteLine("");
                                            customer.AddToCart(result[0]);
                                            goto case 3;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                isEndSearch = true; // exit current case
                            }
                        }
                        break;
                    case 3: // View Cart
                        Console.Clear();
                        customer.ViewCart();
                        Console.WriteLine("Press 'Enter' to go back. 'A' to add product. 'P' to place order. 'C' to clear cart.");
                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (keyPress.KeyChar == 'a' || keyPress.KeyChar == 'A')
                            {
                                goto case 2;    // Search Product case
                            }
                            else if (keyPress.KeyChar == 'p' || keyPress.KeyChar == 'P')
                            {
                                customer.PlaceOrder();
                                Console.WriteLine("Press 'Enter' to continue.");
                                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                break;
                            }
                            else if (keyPress.KeyChar == 'c' || keyPress.KeyChar == 'C')
                            {
                                customer.ClearCart();
                                Console.WriteLine("Press 'Enter' to continue.");
                                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                break;
                            }
                        }
                        break;
                    case 4: // Exit
                        isCustomer = false;
                        isExit = true;
                        break;
                }
            }

            /* STORAGE MANAGER */
            while (isManager)
            {
                PromptManager();
                Console.Clear();
                switch (currentSelection)
                {
                    case 0: // Switch User
                        PromptUserChange();
                        switch (currentSelection)
                        {
                            case 0: // Customer
                                isCustomer = true;
                                isManager = false;
                                isCashier = false;
                                Console.WriteLine();
                                Console.Write("[Switching to Customer]\nLoading...");
                                SimulateLoading();
                                break;
                            case 1: // Storage Manager
                                break;  // just go back
                            case 2: // Cashier
                                isCustomer = false;
                                isManager = false;
                                isCashier = true;
                                CheckPassword();
                                Console.WriteLine();
                                Console.Write("[Switching to Cashier]\nLoading...");
                                SimulateLoading();
                                break;
                        }
                        break;
                    case 1: // Show List of Products
                        Console.Clear();
                        manager.ShowProductList();
                        Console.WriteLine("Press 'Enter' to go back. 'S' to search. 'A' to add. 'R' to remove. 'U' to update product.");

                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (keyPress.KeyChar == 's' || keyPress.KeyChar == 'S')
                            {
                                goto case 2;    // Search Product case
                            }
                            else if (keyPress.KeyChar == 'a' || keyPress.KeyChar == 'A')
                            {
                                goto case 3;    // Add product case
                            }
                            else if (keyPress.KeyChar == 'r' || keyPress.KeyChar == 'R')
                            {
                                goto case 4;    // Remove product case
                            }
                            else if (keyPress.KeyChar == 'u' || keyPress.KeyChar == 'U')
                            {
                                goto case 5;    // Update product case
                            }
                        }
                        break;
                    case 2: // Search Product
                        bool isEndSearch = false;
                        Console.Clear();
                        manager.ShowProductList();
                        while (!isEndSearch)
                        {
                            Console.Write("\nEnter product code/name (Press enter to cancel search): ");
                            string key = Console.ReadLine();

                            Console.Clear();
                            // Cancel product search by pressing enter or inputting whitespace
                            if (key != "" && key != " ")
                            {
                                string[] result = manager.SearchProduct(key);
                                if (result == null)
                                {
                                    Console.WriteLine("\nPress 'Enter' to search again.");
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                    Console.Clear();
                                    manager.ShowProductList();
                                }
                                else
                                {
                                    manager.DisplayProductInTable(result);
                                    Console.WriteLine("\n1 product found!");
                                    PromptRemoveOrUpdate();

                                    choice = 0;
                                    switch (currentSelection)
                                    {
                                        case 0: // Remove Product
                                            Console.Write("Confirm REMOVE? [y/n]: ");
                                            while (choice != 121 && choice != 110) { choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar); }   // check for 'y' or 'n' keypress
                                            Console.WriteLine((char)choice);

                                            if ((char)choice == 'y')
                                            {
                                                Console.WriteLine("Removing product: " + result[0]);
                                                manager.RemoveProduct(result[0]);
                                            } else
                                            {
                                                Console.WriteLine("Product NOT removed.");
                                            }
                                            Console.WriteLine();
                                            break;
                                        case 1: // Update Product
                                            Console.Write("Confirm UPDATE? [y/n]: ");
                                            while (choice != 121 && choice != 110) { choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar); }   // check for 'y' or 'n' keypress
                                            Console.WriteLine((char)choice);

                                            if ((char)choice == 'y')
                                            {
                                                Console.WriteLine("Updating product: " + result[0]);
                                                manager.UpdateProduct(result[0]);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Product NOT updated.");
                                            }
                                            Console.WriteLine();
                                            break;
                                        case 3: // Go back
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                isEndSearch = true; // exit current case
                            }
                        }
                        break;
                    case 3:    // Add Product 
                        Console.Clear();
                        manager.ShowProductList();
                        Product product = new Product();
                        Console.WriteLine("\nEnter details of new Product: ");
                        Console.Write("Code: ");
                        while (true)
                        {
                            product.code = Console.ReadLine();
                            if (product.code == " " || product.code == "")
                            {
                                Console.Write("Code cannot be empty. Try again: ");
                            } else if (product.code == "exit")
                            {
                                goto default;
                            } 
                            else
                            {
                                break;
                            }
                        }
                        // Search for the code if it exists in the ProductList.csv
                        if (manager.SearchProduct(product.code) != null)
                        {
                            Console.Write("Existing code. Update Product instead? [y/n]: ");
                            choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar);
                            if (choice == 121 || choice == 110)  // 'y' or 'n'
                            {
                                Console.WriteLine((char)choice);
                                if (choice == 121)
                                {
                                    manager.UpdateProduct(product.code.ToUpper());
                                    Console.Write("\nPress 'Enter' to continue...");
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                    goto default;
                                }
                            }
                        }

                        Console.Write("Name: ");
                        while (true)
                        {
                            product.name = Console.ReadLine();
                            if (product.name == " " || product.name == "")
                            {
                                Console.Write("Name cannot be empty. Try again: ");
                            }
                            else if (product.name == "exit")
                            {
                                goto default;
                            } 
                            else
                            {
                                break;
                            }
                        }
                        Console.Write("Info: ");
                        while (true)
                        {
                            product.info = Console.ReadLine();
                            if(product.info == " " || product.info == "")
                            {
                                Console.Write("Info cannot be empty. Try again: ");
                            }
                            else if (product.info == "exit")
                            {
                                goto default;
                            } 
                            else
                            {
                                break;
                            }
                        }
                        
                        // Branded? [y/n] validation
                        Console.Write("Branded? [y/n]: ");
                        while (true)
                        {
                            choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar);
                            if (choice == 121 || choice == 110)  // 'y' or 'n'
                            {
                                break;
                            }
                            else if (choice == 27) // Check if Esc key is pressed
                            {
                                Console.WriteLine("\nOperation cancelled.");
                                goto default; // Exit the case and return to main menu or previous flow
                            }
                            else
                            {
                                Console.Write("\nInvalid input. Please press 'y' for Yes or 'n' for No: ");
                            }
                        }
                        Console.WriteLine((char)choice);
                        product.isBranded = (choice == 121) ? true : false;
                        if (choice == 121)
                        {
                            Console.Write("Brand: ");
                            product.Brand = Console.ReadLine();

                            if (product.Brand == "exit")
                            {
                                goto default;
                            }
                        }

                        // Prescription? [y/n] validation
                        Console.Write("Prescription? [y/n]: ");
                        while (true)
                        {
                            choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar);
                            if (choice == 121 || choice == 110)  // 'y' or 'n'
                            {
                                break;
                            }
                            else if (choice == 27) // Check if Esc key is pressed
                            {
                                Console.WriteLine("\nOperation cancelled.");
                                goto default; // Exit the case and return to main menu or previous flow
                            }
                            else
                            {
                                Console.Write("\nInvalid input. Please press 'y' for Yes or 'n' for No: ");
                            }
                        }
                        Console.WriteLine((char)choice);
                        product.isPrescription = (choice == 121) ? true : false;

                        // Quantity validation
                        Console.Write("Quantity: ");
                        int quantity;
                        while (!int.TryParse(Console.ReadLine(), out quantity))
                        {
                            Console.Write("Invalid quantity. Please enter a valid number. ");
                            Console.Write("Quantity: ");
                        }
                        product.quantity = quantity;
                        if (product.quantity < 0)
                        {
                            goto default;
                        }

                        // Price validation
                        Console.Write("Price: ");
                        double price;
                        while (!double.TryParse(Console.ReadLine(), out price))
                        {
                            Console.Write("Invalid price. Please enter a valid number. ");
                            Console.Write("Price: ");
                        }
                        product.price = price;
                        if (product.price < 0)
                        {
                            goto default;
                        }

                        Console.Write("Confirm product details? [y/n]: ");
                        while (true)
                        {
                            choice = (byte)char.ToLower(Console.ReadKey(true).KeyChar);
                            if (choice == 121 || choice == 110)  // 'y' or 'n'
                            {
                                break;
                            }
                            else if (choice == 27) // Check if Esc key is pressed
                            {
                                Console.WriteLine("\nOperation cancelled.");
                                goto default; // Exit the case and return to main menu or previous flow
                            }
                            else
                            {
                                Console.Write("\nInvalid input. Please press 'y' for Yes or 'n' for No: ");
                            }
                        }
                        Console.WriteLine((char)choice);

                        // If confirmed, add the product
                        if ((char)choice == 'y')
                        {
                            manager.AddProduct(product);
                            Console.WriteLine("Product added successfully.");
                        }

                        product = null; // reset product

                        Console.ReadKey(); // TO DELETE
                        break;

                    case 4: // Remove Product
                        bool isEndRemove = false;
                        while(!isEndRemove)
                        {
                            Console.Clear();
                            manager.ShowProductList();
                            Console.Write("Enter code/name of product to remove (type 'exit' to go back): ");
                             code = Console.ReadLine();
                            if(code == "exit")
                            {
                                isEndRemove = true;
                            } else
                            {
                                manager.RemoveProduct(code.ToUpper()); // pass the uppercase to enforce non-case sensitivity
                                Console.Write("\nPress 'Enter' to continue...");
                                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                            }
                        }
                        break;
                    case 5: // Update Product
                        bool isEndUpdate = false;
                        while (!isEndUpdate)
                        {
                            Console.Clear();
                            manager.ShowProductList();
                            Console.Write("Enter code/name of product to update (type 'exit' to go back): ");
                            code = Console.ReadLine();
                            if (code == "exit")
                            {
                                isEndUpdate = true;
                            }
                            else
                            {
                                manager.UpdateProduct(code.ToUpper()); // pass the uppercase to enforce non-case sensitivity
                                Console.Write("\nPress 'Enter' to continue...");
                                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                            }
                        }
                        break;
                    case 6: // Show out of stock
                        Console.Clear();
                        manager.ShowZeroProducts();
                        Console.WriteLine("Press 'Enter' to go back. 'S' to search. 'A' to add. 'R' to remove. 'U' to update product.");
                        
                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }else if (keyPress.KeyChar == 's' || keyPress.KeyChar == 'S')
                            {
                                goto case 2;    // Search Product case
                            }
                            else if (keyPress.KeyChar == 'a' || keyPress.KeyChar == 'A')
                            {
                                goto case 3;    // Add product case
                            }
                            else if (keyPress.KeyChar == 'r' || keyPress.KeyChar == 'R')
                            {
                                goto case 4;    // Remove product case
                            }
                            else if (keyPress.KeyChar == 'u' || keyPress.KeyChar == 'U')
                            {
                                goto case 5;    // Update product case
                            }
                        }
                        break;
                    case 7: // Exit
                        isManager = false;
                        isExit = true;
                        break;
                    default:
                        break;
                }
            }
            /* CASHIER */
            while (isCashier)
            {
                PromptCashier();
                Console.Clear();
                switch (currentSelection)
                {
                    case 0: // Switch User
                        PromptUserChange();
                        //Console.ReadKey();
                        switch (currentSelection)
                        {
                            case 0: // Customer
                                isCustomer = true;
                                isManager = false;
                                isCashier = false;
                                Console.WriteLine();
                                Console.Write("[Switching to Customer]\nLoading...");
                                SimulateLoading();
                                break;
                            case 1: // Storage Manager
                                isCustomer = false;
                                isManager = true;
                                isCashier = false;
                                CheckPassword();
                                Console.WriteLine();
                                Console.Write("[Switching to Cashier]\nLoading...");
                                SimulateLoading();
                                break;
                            case 2: // Cashier
                                break;  // just go back
                        }
                        break;
                    case 1: // Show List of Products
                        Console.Clear();
                        cashier.ShowProductList();
                        Console.WriteLine("Press 'Enter' to go back. 'S' to search.");

                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (keyPress.KeyChar == 's' || keyPress.KeyChar == 'S')
                            {
                                goto case 2;    // Search Product case
                            }
                        }
                        break;
                    case 2: // Search Product
                        bool isEndSearch = false;
                        while (!isEndSearch)
                        {
                            Console.Clear();
                            cashier.ShowProductList();
                            Console.Write("\nEnter product code/name (Press enter to cancel search): ");
                            string key = Console.ReadLine();

                            Console.Clear();
                            // Cancel product search by pressing enter or inputting whitespace
                            if (key != "" && key != " ")
                            {
                                string[] result = cashier.SearchProduct(key);
                                if (result == null)
                                {
                                    Console.WriteLine("\nPress 'Enter' to search again.");
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                    Console.Clear();
                                }
                                else
                                {
                                    Console.Clear();
                                    cashier.DisplayProductInTable(result);
                                    Console.WriteLine("\n1 product found!");

                                    Console.WriteLine("\nPress 'Enter' to search again.");
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                                }
                            }
                            else
                            {
                                isEndSearch = true; // exit current case
                            }
                        }
                        break;
                    case 3: // Process Order
                        cashier.CreateReceipt();
                        Console.WriteLine();
                        Console.WriteLine("Press 'Enter' to go back.");
                        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                        break;
                    case 4: // Generate Sales Report
                        Console.Clear();
                        Console.WriteLine("GENERATE SALES REPORT.");
                        Console.WriteLine("Press 'Enter' to go back. 'D' for Daily. 'W' for Weekly. 'M' for Monthly");

                        while (true)
                        {
                            keyPress = Console.ReadKey(true);
                            if (keyPress.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (keyPress.KeyChar == 'd' || keyPress.KeyChar == 'D')
                            {
                                cashier.DailySalesReport(); 
                                break;
                            }
                            else if (keyPress.KeyChar == 'w' || keyPress.KeyChar == 'W')
                            {
                                cashier.WeeklySalesReport();
                                break;
                            }
                            else if (keyPress.KeyChar == 'm' || keyPress.KeyChar == 'M')
                            {
                                cashier.MonthlySalesReport();
                                break;
                            }
                        }

                        Console.WriteLine("Press 'Enter' to go back.");
                        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { } // check for 'Enter' keypress
                        break;
                    case 5: // Exit
                        isCashier = false;
                        isExit = true;
                        break;
                    default:
                        break;
                }
            }

        }
        Console.Write("POS System is Shutting down in ");
        SimulateLoading();
        Console.WriteLine();
    }

    // This method handles the customer menu with arrow selection
    public static void PromptCustomer()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayCustomerMenu(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 4)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the customer menu with dynamic arrow highlighting
    public static void DisplayCustomerMenu()
    {
        Console.Clear(); // Clear the console to redraw the menu

        Console.WriteLine("Pharmacy POS");
        Console.WriteLine();
        Console.WriteLine("Hello CUSTOMER! How may I help you?");

        // Display options with the arrow highlighting the selected option
        string[] customerMenuOptions = {
            "Switch User",
            "Show List of Products",
            "Search Product",
            "View Cart",
            "Exit"
        };

        // Loop through each option and display it
        for (int i = 0; i < customerMenuOptions.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {customerMenuOptions[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {customerMenuOptions[i]}"); // Regular options
            }
        }
    }

    // This method handles the storage manager menu with arrow selection
    public static void PromptManager()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayManagerMenu(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 7)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the storage manager menu with dynamic arrow highlighting
    public static void DisplayManagerMenu()
    {
        Console.Clear(); // Clear the console to redraw the menu

        Console.WriteLine("Pharmacy POS");
        Console.WriteLine();
        Console.WriteLine("Hello STORAGE MANAGER! How may I help you?");

        // Display options with the arrow highlighting the selected option
        string[] managerMenuOptions = {
            "Switch User",
            "Show List of Products",
            "Search Product",
            "Add Product",
            "Remove Product",
            "Update Product",
            "Show out of stock",
            "Exit"
        };

        // Loop through each option and display it
        for (int i = 0; i < managerMenuOptions.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {managerMenuOptions[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {managerMenuOptions[i]}"); // Regular options
            }
        }
    }

    // This method handles the storage manager menu with arrow selection
    public static void PromptCashier()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayCashierMenu(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 5)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the storage manager menu with dynamic arrow highlighting
    public static void DisplayCashierMenu()
    {
        Console.Clear(); // Clear the console to redraw the menu

        Console.WriteLine("Pharmacy POS");
        Console.WriteLine();
        Console.WriteLine("Hello CASHIER! How may I help you?");

        // Display options with the arrow highlighting the selected option
        string[] cashierMenuOptions = {
            "Switch User",
            "Show List of Products",
            "Search Product",
            "Process Order",
            "Generate Sales Report",
            "Exit"
        };

        // Loop through each option and display it
        for (int i = 0; i < cashierMenuOptions.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {cashierMenuOptions[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {cashierMenuOptions[i]}"); // Regular options
            }
        }
    }

    // This method handles the user-change menu with arrow selection
    public static void PromptUserChange()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayUserList(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 2)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the user-change menu with dynamic arrow highlighting
    public static void DisplayUserList()
    {
        Console.Clear(); // Clear the console to redraw the menu

        Console.WriteLine("Pharmacy POS");
        Console.WriteLine();
        Console.WriteLine("Choose a user: ");

        // Display options with the arrow highlighting the selected option
        string[] userListOptions = {
            "Customer",
            "Storage Manager",
            "Cashier"
        };

        // Loop through each option and display it
        for (int i = 0; i < userListOptions.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {userListOptions[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {userListOptions[i]}"); // Regular options
            }
        }
    }

    // This method displays the remove/update menu with arrow selection
    public static void PromptRemoveOrUpdate()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayRemoveOrUpdate(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 2)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the user-change menu with dynamic arrow highlighting
    public static void DisplayRemoveOrUpdate()
    {
        Console.SetCursorPosition(0, 8);

        Console.WriteLine("Choose an action: ");

        // Display options with the arrow highlighting the selected option
        string[] removeUpdateOptions = {
            "Remove ",
            "Update ",
            "Search again "
        };

        // Loop through each option and display it
        for (int i = 0; i < removeUpdateOptions.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {removeUpdateOptions[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {removeUpdateOptions[i]}"); // Regular options
            }
        }
    }

    // This method displays the remove/update menu with arrow selection
    public static void PromptAddToCart()
    {
        ConsoleKeyInfo key;
        do
        {
            DisplayAddToCart(); // Redraw the menu with the updated selection
            key = Console.ReadKey(intercept: true); // Intercepts key input without displaying it

            if (key.Key == ConsoleKey.UpArrow && currentSelection > 0)
            {
                currentSelection--; // Move the arrow up
            }
            else if (key.Key == ConsoleKey.DownArrow && currentSelection < 1)
            {
                currentSelection++; // Move the arrow down
            }

        } while (key.Key != ConsoleKey.Enter); // Break the loop when Enter is pressed
    }

    // This method displays the user-change menu with dynamic arrow highlighting
    public static void DisplayAddToCart()
    {
        Console.SetCursorPosition(0, 8);

        Console.WriteLine("Choose an action: ");

        // Display options with the arrow highlighting the selected option
        string[] addToCartOption = {
            "Add to cart ",
            "Search again "
        };

        // Loop through each option and display it
        for (int i = 0; i < addToCartOption.Length; i++)
        {
            if (i == currentSelection)
            {
                // Highlight selected option with an arrow (→)
                Console.ForegroundColor = ConsoleColor.Cyan; // Optional: Change color for selection
                Console.WriteLine($" ->  {addToCartOption[i]}");
                Console.ResetColor(); // Reset color after the highlighted line
            }
            else
            {
                Console.WriteLine($"    {addToCartOption[i]}"); // Regular options
            }
        }
    }

    public static void BootingScreen()
    {
        string[] spinner = new string[] { "|", "/", "-", "\\" };
        int spinnerIndex = 0;

        // Combined ASCII Art (header + body)
        string[] asciiArt = new string[] {
            "            ░█▀█░█░█░█▀█░█▀▄░█▄█░█▀█░█▀▀░█░█░░░█▀█░█▀█░█▀▀",
            "            ░█▀▀░█▀█░█▀█░█▀▄░█░█░█▀█░█░░░░█░░░░█▀▀░█░█░▀▀█",
            "            ░▀░░░▀░▀░▀░▀░▀░▀░▀░▀░▀░▀░▀▀▀░░▀░░░░▀░░░▀▀▀░▀▀▀",
            " ",
            "             .-=*##%%%%%%%%%=    =#%=    -%%%%%%%%%##*=-.",
            "     *%%%%%%%%%%%%*-:-+###%%#   -%%%%=   #%%###*=:-*%%%%%%%%%%%%*",
            "       :---::-=*#%%%*-=*#+%%%*   +%%+   +%%%+#*=-+%%%#*=-::---:  ",
            "        +%%%%%%#===#%%==%+#==#%*. ## .*%#==#+%==%%#+-=#%%%%%%+   ",
            "          :. :+%%%%*:*%#-%+ -#%+: ## .+%#= =%=*%*:*%%%%*- .:     ",
            "             -**+.-%%#:*%= %%%%%%=##-%%%%%%.-%#:#%%-.=**-        ",
            "                 -#+ -%%. +%#     **     #%* .%%= +#-            ",
            "                           %%+    **    +%%.                      ",
            "                            #%%%*:-+.*%%%#                        ",
            "                              :+%%%%%*+:                          ",
            "                             +%%=:---=%%*                         ",
            "                             +%%+:-=:+%%+                         ",
            "                               +*%%%%+:                          ",
            "                              :%%-=-:%%:                          ",
            "                              .%%%#=:#%.                          ",
            "                               .#+:=%%:                           ",
            "                                +%%*+-                            ",
            "                                :#-*%-                            ",
            "                                 *++#                             ",
            "                                  -=                              ",
            "                                  .:                              ",
            "                                  ..                              "
        };

        // Starting positions for ASCII Art and progress bar
        int asciiArtStartRow = 1;
        int progressBarRow = asciiArtStartRow + asciiArt.Length + 1;

        for (int i = 0; i <= 100; i++)
        {
            // Print ASCII art progressively, line by line
            int asciiLine = i * asciiArt.Length / 100;
            if (asciiLine < asciiArt.Length)
            {
                Console.SetCursorPosition(0, asciiArtStartRow + asciiLine);
                Console.WriteLine(asciiArt[asciiLine]);
            }

            // Generate the progress bar
            int progressWidth = (Console.WindowWidth - 4) / 2;
            string progressBar = new string('=', i * progressWidth / 100) + new string(' ', progressWidth - i * progressWidth / 100);
            string status = $"[{progressBar}] {i}% {spinner[spinnerIndex]}";

            // Overwrite previous progress line
            Console.SetCursorPosition(0, progressBarRow);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear the previous line
            Console.SetCursorPosition(0, progressBarRow); // Reset cursor position
            Console.Write(status); // Write the updated progress bar

            // Cycle through spinner symbols
            spinnerIndex = (spinnerIndex + 1) % spinner.Length;

            // Simulate work with a short pause
            Thread.Sleep(2);  // Adjust this delay for the desired speed
        }

        Thread.Sleep(1000);
        // Print message below the progress bar
        Console.Write("\n-> Welcome! Press any key to continue...");

        // Wait for user input before clearing the console
        Console.ReadKey();
        Console.SetCursorPosition(0, 0); // Reset cursor position
        Console.Clear();
    }

    public static void SimulateLoading()
    {
        Console.Write("2");
        Thread.Sleep(100);
        Console.Write(".");
        Thread.Sleep(300);
        Console.Write(".");
        Thread.Sleep(300);
        Console.Write(".");
        Thread.Sleep(300);
        Console.Write("1");
        Thread.Sleep(100);
        Console.Write(".");
        Thread.Sleep(300);
        Console.Write(".");
        Thread.Sleep(300);
        Console.Write(".");
    }

    public static void CheckPassword()
    {
        while(true)
        {
            Console.Clear();
            Console.Write("Enter password: ");
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true); // Read key without displaying it

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Handle backspace
                    password = password[..^1];
                    Console.Write("\b \b"); // Erase the last asterisk
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    // Append character to the password
                    password += key.KeyChar;
                    Console.Write("*"); // Display an asterisk
                }

            } while (key.Key != ConsoleKey.Enter); // Stop on Enter

            if (password == "123")
            {
                break;
            }
            else
            {
                Console.WriteLine("Incorrect password. Try again...");
                Console.ReadKey();
            }
        }
    }
}
