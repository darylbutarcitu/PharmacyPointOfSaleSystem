using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyPointOfSaleSystem
{
    public interface Interfaces
    {
        interface IViewInventory // interface for ALL users
        {
            void ShowProductList(); // record of products including other details
            string[] SearchProduct(string key); // search product using a keyword
        }

        interface ICreateOrder  // for customer
        {
            void AddToCart(string name); // add to cart
            void ViewCart(); // view cart
            void ClearCart(); // clear cart
            void PlaceOrder(); // place order
        }

        interface ICreateProduct    // for storage manager
        {
            void AddProduct(Product product);  // add new product to invetory
            int RemoveProduct(string code);  // remove product
        }
        interface IModifyProduct    // for storage manager and cashier
        {
            void UpdateProduct(string code);  // update product details
        }
        interface IManageOrder    // for cashier
        {
            void CreateReceipt();       // display transaction details
            void DailySalesReport();    // display daily sales report
            void WeeklySalesReport();    // display weekly sales report
            void MonthlySalesReport();   // display monthly sales report
        }
    }
}
