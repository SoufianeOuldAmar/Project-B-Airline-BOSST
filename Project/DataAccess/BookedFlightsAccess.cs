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
            // If it exists, add the new booked flights to the existing list
            bookedFlights[email].AddRange(newBookedFlights);
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

}
