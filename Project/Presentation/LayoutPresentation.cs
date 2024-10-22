// LayoutPresentation.cs
using System;
using System.Collections.Generic;

public static class LayoutPresentation
{
    public static void ShowLayout(LayoutLogic layoutLogic)
    {
        layoutLogic.PrintLayout();
    }

    public static void ChooseSeat(LayoutLogic layoutLogic)
    {
        Console.Write("Enter the seat you wish to book: ");
        string seat = Console.ReadLine();
        layoutLogic.BookFlight(seat);
        
        Console.Write("Do you want to confirm your booking? (yes/no): ");
        string confirmation = Console.ReadLine();
        
        if (confirmation.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            layoutLogic.ConfirmBooking();
        }
        else
        {
            Console.WriteLine("Booking cancelled.");
        }
    }
}
