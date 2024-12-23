using System.Text.Json;
using DataModels;

namespace DataAccess
{
    public static class FlightsAccess
    {
        private static string filePath = "DataSources/flights.json";

        public static List<FlightModel> ReadAll()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var flights = JsonSerializer.Deserialize<List<FlightModel>>(jsonString, options);

                    // foreach (var flight in flights ?? new List<FlightModel>())
                    // {
                    //     if (flight.Layout == null)
                    //     {
                    //         flight.Layout = AssignDefaultLayout(flight.Airline);
                    //     }
                    // }

                    return flights ?? new List<FlightModel>();
                }
                return new List<FlightModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading flights data: {ex.Message}");
                return new List<FlightModel>();
            }
        }

        public static void WriteAll(List<FlightModel> flights)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(flights, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing flights data: {ex.Message}");
            }
        }

        // private static LayoutModel AssignDefaultLayout(string airline)
        // {
        //     string airlineLower = airline.ToLower();

        //     if (airlineLower.Contains("british airways"))
        //     {
        //         return LayoutModel.CreateBoeing737Layout();
        //     }
        //     else if (airlineLower.Contains("airbus"))
        //     {
        //         return LayoutModel.CreateAirbusA330200Layout();
        //     }
        //     else if (airlineLower.Contains("emirates"))
        //     {
        //         return LayoutModel.CreateAirbusA330200Layout();
        //     }
        //     else if (airlineLower.Contains("klm"))
        //     {
        //         return LayoutModel.CreateBoeing737Layout();
        //     }
        //     else if (airlineLower.Contains("lufthansa"))
        //     {
        //         return LayoutModel.CreateBoeing787Layout(); // Changed from Boeing 757
        //     }
        //     else if (airlineLower.Contains("turkish airlines"))
        //     {
        //         return LayoutModel.CreateAirbusA330200Layout();
        //     }
        //     else
        //     {
        //         return LayoutModel.CreateBoeing737Layout();
        //     }
        // }

        public static void AdminAddNewFlight(FlightModel newFlight)
        {
            var flights = ReadAll();
            flights.Add(newFlight);
            WriteAll(flights);
        }

        // Search flight method
        public static List<FlightModel> SearchFlights(
            string departureAirport = null,
            string arrivalDestination = null,
            DateTime? departureDate = null,
            string flightTime = null)
        {
            var flights = ReadAll();

            return flights.Where(flight =>
                (departureAirport == null || flight.DepartureAirport == departureAirport) &&
                (arrivalDestination == null || flight.ArrivalDestination == arrivalDestination) &&
                (!departureDate.HasValue ||
                    (DateTime.TryParse(flight.DepartureDate, out DateTime flightDepartureDate) &&
                     flightDepartureDate.Date == departureDate.Value.Date)) &&
                (flightTime == null || flight.FlightTime == flightTime)
            ).ToList();
        }
    }
}