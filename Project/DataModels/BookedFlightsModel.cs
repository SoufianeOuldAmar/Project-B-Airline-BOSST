using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DataModels;

public class BookedFlightsModel
{
    public int FlightID { get; set; }
    public List<string> BookedSeats { get; set; }
    public List<PetLogic> Pets { get; set; }
    public List<BaggageLogic> BaggageInfo { get; set; }
    public bool IsCancelled { get; set; }

    public double TicketBill { get; set; }
    public int FlightPoints { get; set; }


    public BookedFlightsModel(int flightID, List<string> bookedSeats, List<BaggageLogic> baggageInfo, List<PetLogic> pets, bool isCancelled)
    {

        FlightID = flightID;
        BookedSeats = bookedSeats;
        Pets = pets;
        BaggageInfo = baggageInfo;
        IsCancelled = isCancelled;
        TicketBill = 0;
        FlightPoints = 0;
    }


    // Total fee 
    public double FeeTotal()
    {
        double fee = 0;
        foreach (var bag in BaggageInfo)
        {
            fee += bag.CalcFee();
        }
        return fee;
    }


}