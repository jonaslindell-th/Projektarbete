# Introduction
For our first course final assignment we were to create a GUI application to simulate a store with a product and coupon range editor (optional for a higher grade). 
It's entirely written in C# using a WPF without XAML template in Visual Studio.

# Documentation
Before starting the project the layout of the application was first visualized on paper, which made it easier to focus on the appearance in order to avoid getting stuck on syntax.
For our layout we focused on using grids mainly, in order to keep the contents positioning regardless of the window size.
To make the content adjust to the window size we use undefined width and height in certain rows and columns to fill unwanted voids. 
To gain access to other colours than the predefined brush-enums we use a brushconverter to turn hex color codes into a solid color brush.

The content of the grids are constantly changing while implementing new controls, in order to facilitate the placement of our content we are using the boolean property ShowGridlines through out the development phase.
The greatest change in our grids occurs upon discovering the drop-down menu on msdn.
At first the outer grid consists of three columns: shopping cart, product range and display selected product.
With the help of the expander control we can easily set the nestled shopping cart grid as the expanders content, now we can display the content of the shopping cart on demand thereby reducing content compression by a third.

We chose to work with lists over dictionaries to store the shopping cart and product range which we declare as class fields, in order to gain access to the variable from methods and eventhandlers.
Using listboxes to display it's content in a appropriate way which lets us use the selectedindex property to display the content of our lists or manipulate it.
We chose to work with lists since its slighlty faster than dictionaries when looping over its content frequently occurs through out the program excecution.
To prevent the same product from appearing several times in the shopping cart we added a product property for amount in the product class, instead of using the alternative Dictionary<Product, amount>.

In order for changes to appear in the shopping cart listbox a method is declared, which clears the listbox items and adds the items in the shopping cart list anew, then calcutaltes the new sum.
To display additional information about a selected product apart from name and price we use the event handler SelectionChanged where we utilize the SelectedIndex property to reference from marked product in the listbox to the correct product in the product list in order to display its image and description in the right column.

At first we were using csv to store and read products from file, but was later changed to json which main advantage is being able to store and read objects directly using the key/value pairs syntax. A method is declared which runs upon start, reading the product range from file using the streamreader and json Deserialise classmethod.

With a larger product range a search function would facilitate finding the products the user is looking for by name or category. A product property for category is added to the product class, a combobox is used to display the available categories and a textbox is used to be able to search for a product by name.
To be able to discard the products which doesn't match the given search term, we use the event handler TextChanged which clears the product range listbox. To be able to only add serach related products to the listbox we use a lambda expression which loops through the product range list and only adds the items to the listbox which contains the search term, to make searches caseinsensitive we use tolower on the search term and the product names.

```csharp
            var searchTermProducts = productList.Where(product => product.Title.ToLower().Contains(searchTerm.ToLower()));
```

By simply clearing the product listbox and only displaying the products matching the search term, we are no longer able to refer to the correct product in the product range list by index.
We considered reading the products from file every time a search was made, but with a larger product range this solution would affect performance.
To solve this problem we use two separate lists, one which reads the entire product range from file upon application start up, and one search term list which is displayed in the listbox. To display and refer to the correct SelectedIndex in the listbox, the search term list is cleared every time the textbox content changes and the eventhandler adds the matching products. To display the entire product range upon start up using this method we simply run it with a empty string as its search term parameter. 

To display a receipt upon checkout we want to avoid using Messagebox.Show since its incalculable to use for anything more then a few lines. Since we are not allowed to open new windows for our assignment and we still want to be able to use the application once a order has been placed, we decided to reuse the product description column for our receipt.
To display a large amount of data in a perspicuous way we use a readonly DataGrid.

# Screenshots
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/Startup.png?raw=true)
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/SelectedProduct.png?raw=true)
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/Cart.png?raw=true)
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/Checkout.png?raw=true)
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/EditProduct.png?raw=true)
![](https://github.com/jonaslindell-th/Projektarbete/blob/master/Screenshots/EditCoupon.png?raw=true)
