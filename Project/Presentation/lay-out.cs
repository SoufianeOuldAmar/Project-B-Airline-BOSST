public class Seat
    {
        public string Class { get; set; }
        public decimal Price { get; set; }
        public bool IsReserved { get; set; }

        static Seat[,] seats = new Seat[100, 10];

        public static void ReserveSeat(int row, int seat, string seatClass, decimal Price)
        {

            Console.WriteLine($"You have chosen this seat: {row + 1}{(char)(seat + 'A')}. Class: {seatClass}, Price: {Price}");

            int currentOption = 0;
            string[] yesNoOptions = new string[] { "yes", "no" };

            Console.WriteLine("Do you want to select this seat?");
            Console.WriteLine();
            Console.WriteLine();
            ConsoleKeyInfo key;

            do
            {
                Console.SetCursorPosition(0, Console.CursorTop - yesNoOptions.Length);
                for (int i = 0; i < yesNoOptions.Length; i++)
                {
                    if (i == currentOption)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(yesNoOptions[i]);

                    Console.ResetColor();
                }

                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        currentOption = Math.Max(0, currentOption - 1);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        currentOption = Math.Min(yesNoOptions.Length - 1, currentOption + 1);
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
            
        }
        public static string createSeat(int seat, int row)
        {
            string seatplace = "";
            int newseat = seat + 1;
            if (newseat == 1)
            {
                seatplace = (row + 1).ToString() + " - " + "A";
            }
            else if (newseat == 2)
            {
                seatplace = (row + 1).ToString() + " - " + "B";
            }
            else if (newseat == 3)
            {
                seatplace = (row + 1).ToString() + " - " + "C";
            }
            else if (newseat == 4)
            {
                seatplace = (row + 1).ToString() + " - " + "D";
            }
            else if (newseat == 5)
            {
                seatplace = (row + 1).ToString() + " - " + "E";
            }
            else if (newseat == 6)
            {
                seatplace = (row + 1).ToString() + " - " + "F";
            }
            return seatplace;
        }
        public static void ShowSeats()
        {
            Console.WriteLine("Seats: ");
            Console.WriteLine();
            Console.WriteLine("  A B C D E F G H I J");
            for (int i = 0; i < 100; i++)
            {
                Console.Write(i + 1);
                for (int j = 0; j < 10; j++)
                {
                    if (seats[i, j] == null)
                    {
                        Console.Write(" O");
                    }
                    else if (seats[i, j].IsReserved)
                    {
                        Console.Write(" X");
                    }
                    else
                    {
                        Console.Write(" O");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}

