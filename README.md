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
