using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyPointOfSaleSystem
{
    public class Product
    {
        public string code;
        public string name; // generic
        private string brand; // brand name
        public int quantity;
        public double price;
        public bool isBranded;      // false = generic
        public bool isPrescription; // false = available to public
        public string info;

        public string Brand
        {
            get { return brand; }
            set
            {
                this.brand = (this.brand == "") ? " " : value;
            }
        }

        public bool IsBranded
        {
            get { return isBranded; }
            set
            {
                this.isBranded = (this.brand == "" || this.brand == " ") ? false: value;
            }
        }

        public Product()
        {
            code = " ";
            name = " ";
            brand = " ";
            quantity = 0;
            price = 0.00;
            isBranded = false;
            isPrescription = false;
            info = " ";
        }
    }
}
