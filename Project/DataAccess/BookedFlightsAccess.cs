using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class BookedFlightsAccess
{
    static string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/bookedflights.json"));

    public static Dictionary<string, List<BookedFlightsModel>> LoadAll()
    {
        // Load JSON from the file
        if (!File.Exists(path))
        {
            // If the file doesn't exist, return an empty dictionary
            return new Dictionary<string, List<BookedFlightsModel>>();
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, List<BookedFlightsModel>>>(json);
    }

    public static void WriteAll(string email, List<BookedFlightsModel> newBookedFlights)
    {
        // Load the current booked flights data from the file
        Dictionary<string, List<BookedFlightsModel>> bookedFlights = LoadAll();

        // Check if the email (key) already exists in the dictionary
        if (bookedFlights.ContainsKey(email))
        {
            // If it exists, replace the existing list with the new one
            bookedFlights[email] = new List<BookedFlightsModel>(newBookedFlights);
        }
        else
        {
            // If the email doesn't exist, create a new entry
            bookedFlights[email] = new List<BookedFlightsModel>(newBookedFlights);
        }

        // Serialize and write the updated data back to the JSON file
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(bookedFlights, options);
        File.WriteAllText(path, json);
    }
    public static void Save(string email, List<BookedFlightsModel> newBookedFlights)
    {

        Dictionary<string, List<BookedFlightsModel>> bookedFlights = LoadAll();


        if (bookedFlights.ContainsKey(email))
        {

            foreach (var newFlight in newBookedFlights)
            {
                bool flightUpdated = false;


                for (int i = 0; i < bookedFlights[email].Count; i++)
                {
                    if (bookedFlights[email][i].FlightID == newFlight.FlightID)
                    {

                        bookedFlights[email][i] = newFlight;
                        flightUpdated = true;
                        break;
                    }
                }


                if (!flightUpdated)
                {
                    bookedFlights[email].Add(newFlight);
                }
            }
        }
        else
        {
            // If the email doesn't exist, create a new entry
            bookedFlights[email] = new List<BookedFlightsModel>(newBookedFlights);
        }

        // Serialize and write the updated data back to the JSON file
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(bookedFlights, options);
        File.WriteAllText(path, json);
    }

    public static List<BookedFlightsModel> LoadByEmail(string email)
    {
        // Load all booked flights from the JSON file
        var allBookedFlights = LoadAll();

        // Check if the email exists and return the list of booked flights for that email
        if (allBookedFlights.ContainsKey(email))
        {
            return allBookedFlights[email];
        }

        // Return an empty list if the email doesn't exist
        return new List<BookedFlightsModel>();
    }


}
