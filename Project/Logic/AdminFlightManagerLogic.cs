using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using DataModels;

namespace DataAccess
{

    public static class AdminFlightManagerLogic
    {

        public static List<FlightModel> allFlights = DataAccessClass.ReadList<FlightModel>("DataSources/flights.json");

        public static bool SaveChangesLogic(FlightModel flight)
        {
            var flights = DataAccessClass.ReadList<FlightModel>("DataSources/flights.json");
            var flightToUpdate = flights.FirstOrDefault(f => f.Id == flight.Id);
            if (flightToUpdate != null)
            {
                flightToUpdate.TicketPrice = flight.TicketPrice;
                flightToUpdate.Gate = flight.Gate;
                flightToUpdate.DepartureAirport = flight.DepartureAirport;
                flightToUpdate.ArrivalDestination = flight.ArrivalDestination;
                flightToUpdate.IsCancelled = flight.IsCancelled;
                flightToUpdate.DepartureDate = flight.DepartureDate;
                flightToUpdate.FlightTime = flight.FlightTime;

                DataAccessClass.WriteList<FlightModel>("DataSources/flights.json", flights);
                return true;
            }
            return false;
        }

        public static bool TicketPriceLogic(double newTicketPrice)
        {
            if (newTicketPrice >= 0)
            {
                return true;
            }
            return false;

        }

        public static bool GateLogic(string newGate)
        {
            if (newGate.Length >= 2 && newGate.Length <= 3 && "ABCDEF".Contains(char.ToUpper(newGate[0])) &&
            int.TryParse(newGate.Substring(1), out int number) &&
            number >= 1 && number <= 30)
            {
                return true;
            }
            return false;
        }


        public static bool Date(string input)
        {
            string[] dateParts = input.Split('-');
            if (dateParts.Length == 3)
            {
                string yearStr = dateParts[2];
                string monthStr = dateParts[1].PadLeft(2, '0');
                string dayStr = dateParts[0].PadLeft(2, '0');
                input = $"{dayStr}-{monthStr}-{yearStr}";

                if (yearStr.Length == 4 && monthStr.Length == 2 && dayStr.Length == 2)
                {
                    int year = int.Parse(yearStr);
                    int month = int.Parse(monthStr);
                    int day = int.Parse(dayStr);

                    if ((month == 4 || month == 6 || month == 9 || month == 11) && day >= 1 && day <= 30 && year >= 2024)
                    {
                        return true;
                    }
                    else if ((month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) && day >= 1 && day <= 31 && year >= 2024)
                    {
                        return true;
                    }
                    else if (month == 2 && day >= 1 && day <= 28 && year >= 2024)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool CheckForFlights()
        {
            return allFlights == null || allFlights.Count == 0;
        }

        public static int CalculatePages(int pageSize)
        {
            return (int)Math.Ceiling(allFlights.Count / (double)pageSize);
        }

        public static List<FlightModel> GetFlightsForPage(int currentPage, int pageSize)
        {
            return allFlights
            .Skip(currentPage * pageSize)
            .Take(pageSize)
            .ToList();
        }

        public static int ChangePage(int currentPage, int totalPages, string input)
        {
            if (input == "N" && currentPage < totalPages - 1)
            {
                currentPage++;
            }
            else if (input == "B" && currentPage > 0)
            {
                currentPage--;
            }

            return currentPage;
        }

        public static string CreateGate(string newGate)
        {
            string letterPart = newGate.Substring(0, 1).ToUpper();
            string numberPart = newGate.Substring(1);
            string newGateStr = $"{letterPart}{numberPart}";

            return newGateStr;
        }
    }
}