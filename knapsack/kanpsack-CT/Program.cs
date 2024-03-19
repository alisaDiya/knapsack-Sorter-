using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kanpsack_CT
{
    internal class Program
    { 
        static void Main(string[] args)
        {
            try
            {
                // Read the contents of the text file named 'TextFile1' into a variable called 'readText'
                string readText = File.ReadAllText("TextFile1.txt");

                // Split the text file into an array of strings
                string[] fileSplit = readText.Split('\n');

                // Create a list of list of tuples to store the backpack items
                List<List<Tuple<int, double, double>>> backpack = new List<List<Tuple<int, double, double>>>();

                //all infomration is about the United kindgdm , in this case the currency 
                CultureInfo culture = new CultureInfo("en-GB");


                //The array is used to iterate over elements in the array in this case filesplit 
                foreach (string line in fileSplit)
                {
                    //the line will be split from the colon 
                    string[] aspects = line.Trim().Split(':');

                    /* an if statement is being used 
                    the mian object is to check if the 'aspects' array has an length of 2 
                    */
                    if (aspects.Length != 2)
                    {
                        continue; // will Skip any invalid lines
                    }

                    /*the objective of this line is to ensure it will parse the 1st element of the array 'aspects'
                     * as an int ( integer) and once it does that it will get assigned to the weight varaible 
                     * The 'trim' method is to ensure that any form of traillinh whitespace has been removed , and it wont be parsed
                    */
                    int weight = int.Parse(aspects[0].Trim());

                    // Create a list , called items , which is used to store 3 specfic values which are an 'int' and a 'double x2'
                    List<Tuple<int, double, double>> items = new List<Tuple<int, double, double>>();

                    // the 2nd element will be parsed to the itemsection varaible
                    string itemsection = aspects[1].Trim();

                    //responsible for removing trailing in this case we are reffering to the brackets
                    itemsection = itemsection.Substring(1, itemsection.Length - 2);

                    /*the itemSection string will become split into an array
                     * the 'split method' is being used as well as the stringSplit.RemoveEmptyEntries option
                     */
                    string[] itemStrings = itemsection.Split(new string[] { ") (" }, StringSplitOptions.RemoveEmptyEntries);


                    foreach (string itemString in itemStrings)
                    {
                        // Split each item based on commas
                        string[] itemParts = itemString.Trim().Split(',');

                        // Extraction of the the item's ID, weight, and price takes place between line 75 and 79
                        int itemId = int.Parse(itemParts[0].Trim());
                        // A  specified culture is being used - united kingdom  
                        double itemWeight = double.Parse(itemParts[1].Trim(), culture);
                        double itemPrice = double.Parse(itemParts[2].Trim().Substring(1), culture);

                        // Add the item information to the list
                        items.Add(new Tuple<int, double, double>(itemId, itemWeight, itemPrice));
                    }

                    // Add the list of items to the main list called items 
                    backpack.Add(items);
                }

                /* Solve the knapsack problem for each case and generate the output string
                 *It initializes a new instance of the StringBuilder class
                 * and then it gets assigned to the varaible which is named output
                 */
                StringBuilder output = new StringBuilder();


                //A forloop is being used , its iterates all the elements of the backpack collection 
                for (int i = 0; i < backpack.Count; i++)
                {

                    /*The AppendLine method adds the string as new line to the stringbuilder 'output'
                     * the form of concatination being used is known as interpolated strings 
                     */
                    output.AppendLine($"Case {i + 1} -");

                    //the element is retrieved at the index 'i' from the backpack list and then is sent to the 'items varaible'
                    List<Tuple<int, double, double>> items = backpack[i];

                    // the knapacksolver method is been called as the items list will be passed as an arugument 
                    List<int> selectedItems = KnapsackSolver(items);

                    // the elements here are seperated by commas 
                    output.Append(string.Join(",", selectedItems));

                    output.AppendLine();

                    //foreach loop is being used again , to iterate each element in the selectedItems list  
                    foreach (int itemId in selectedItems)
                    {
                        /* A find method is being used to serach for an item in the items list , if it meets the correction conditions 
                         * it will be saved to the selectedItem varaible 
                         * 'item => item.Item1 == itemId' this is known as an lamda it checks if the values are equal basically 
                         */
                        Tuple<int, double, double> selectedItem = items.Find(item => item.Item1 == itemId);


                        //displays the information using interpolation 
                        output.AppendLine($"ID: {selectedItem.Item1}, Weight: {selectedItem.Item2}, Price: £{selectedItem.Item3}");
                    }

                    // leaves a line in the output message 
                    output.AppendLine();
                }

                // colour changing method which changes the font colour to yellow 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Take note of the following output, I hope you are happy with the application I have provided you with :)\n");

                //colour changing method that changes the colour to Cyan 
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("These are the best possible combinations of products to choose from, making the most of the £100 budget limit as well \n as ensuring you are below the weight limit of products \n");

                //Colour changing method that changes the colour to blue 
                Console.ForegroundColor = ConsoleColor.Blue;

                //prints out the contents , that has being stored in the output string builder object
                Console.WriteLine(output.ToString());

                //makes a beeping sound when the project has run 
                Console.Beep();

                // ensure the cdm remains open so you can read the output , if not the console will close
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Handle any exceptions and throw APIException
                throw new APIException($"An error occurred. Please contact the developer of this application: {ex.Message}");
            }
        }

        // seen as a static method , which is called 'KnapsackSolver' , it takes the list as tuples and stores it as input 
        static List<int> KnapsackSolver(List<Tuple<int, double, double>> items)
        {
            //line 166 - 168 , are the constraints given in the question
            int maxWeight = 100;
            int maxItems = 15;

            // calculates the smaller value between the actual number of items and maxItems
            int n = Math.Min(items.Count, maxItems);
            //line 172 -174 intializes 2D arrays which are called 'dr3' and 'lh44'
            double[,] dr3 = new double[n + 1, maxWeight + 1];
            bool[,] lh44 = new bool[n + 1, maxWeight + 1];

            // An forloop is being used
            for (int i = 1; i <= n; i++)
            {

                // obtains the weight as well as price of the current item which is being run through the loop
                double weight = items[i - 1].Item2;
                double price = items[i - 1].Item3;

                // this forloop is in the main forloop and it iterates the different possible weights that can be used 
                for (int a = 1; a <= maxWeight; a++)
                {

                    /* if else statement , This line determines whether the current item's weight is less than or equal
                     to the weight currently being taken into consideration  'a'.
                    */
                    if (weight <= a && price + dr3[i - 1, a - (int)weight] > dr3[i - 1, a])
                    {
                        /*the dr3 and lh44 array will be updated , showing that the current item
                        *is a part of the ideal solution for that weight
                        *for both the weight and the current item.
                        */
                        dr3[i, a] = price + dr3[i - 1, a - (int)weight];
                        lh44[i, a] = true;
                    }

                    //if the above statement is not met , then the else fragement of the code will be executed 
                    else
                    {
                        dr3[i, a] = dr3[i - 1, a];
                        lh44[i, a] = false;
                    }
                }
            }

            // a list called outputKS 
            List<int> outputKS = new List<int>();
            //intialisation of a varaible called MV33 to the max weight
            int MV33 = maxWeight;
            //an forloop which showcases an interation in the reverse order 
            for (int i = n; i > 0 && MV33 > 0; i--)
            {
                //checks the item at index i and the remaining weight at mv33
                if (lh44[i, MV33])
                {
                    //adds the item id to the outputKS list 
                    outputKS.Add(items[i - 1].Item1);
                    /*the weight of the current item is deducted from the remaining weight in this line.
                    After adding the current item to the knapsack
                    ensuring the remaining weight is updated.
                    */
                    MV33 -= (int)items[i - 1].Item2;
                }
            }

            //retruns the outputKS list
            return outputKS;
        }
    }



}
    

