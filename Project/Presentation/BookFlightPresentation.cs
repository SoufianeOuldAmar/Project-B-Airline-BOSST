using System;
using System.Collections.Generic;
using System.Linq;
using DataModels;
using DataAccess;
using System.Threading;

public static class BookFlightPresentation
{
    public static List<FlightModel> allFlights = FlightsAccess.ReadAll();
    public static Dictionary<string, List<BookedFlightsModel>> allBookedFlights = BookedFlightsAccess.LoadAll();

    public static AccountModel currentAccount = AccountsLogic.CurrentAccount;

    private static string GenerateInitials(PassengerModel passenger)
    {
        if (string.IsNullOrWhiteSpace(passenger.FirstName) || string.IsNullOrWhiteSpace(passenger.LastName))
        {
            throw new ArgumentException("First name and last name must not be empty");
        }

        return $"{passenger.FirstName[0]}{passenger.LastName[0]}".ToUpper();
    }

    private static void ProcessPassengerDetails(PassengerModel passenger, string seat, ref double totalPrice, double baseTicketPrice, string initials, List<PassengerModel> passengers, List<string> chosenSeats, List<BaggageLogic> baggageInfo, List<PetLogic> petInfo, FlightModel flight)
    {
        // Add passenger and calculate price
        passengers.Add(passenger);
        chosenSeats.Add(seat);

        double seatPrice = baseTicketPrice;
        if (passenger.AgeGroup == "child")
        {
            seatPrice *= 0.75; // 25% discount for children
        }
        else if (passenger.AgeGroup == "infant")
        {
            seatPrice *= 0.1; // 90% discount for infants
        }
        totalPrice += seatPrice;

        // Baggage handling
        Console.WriteLine("Do you want to add baggage? (yes/no):");
        string userBaggage = Console.ReadLine().ToLower();

        while (userBaggage != "yes" && userBaggage != "no")
        {
            Console.WriteLine("Invalid input. Please enter 'yes' or 'no':");
            userBaggage = Console.ReadLine().ToLower();
        }

        if (userBaggage == "no")
        {
            return;
        }

        if (userBaggage == "yes")
        {
            double totalWeightSeat = 0;
            double feeBaggage = 0;
            double totalCarryOnWeight = 0;


            Console.WriteLine("You get 25kg of baggage per seat (this includes both checked and carry-on (enter yes to continue).");
            string addCheckedBaggage = Console.ReadLine().ToLower();


            while (addCheckedBaggage != "yes" && addCheckedBaggage != "no")
            {
                Console.WriteLine("Invalid input. Please enter 'yes' or 'no':");
                addCheckedBaggage = Console.ReadLine().ToLower();
            }

            if (addCheckedBaggage == "yes")
            {
                while (true)
                {
                    Console.WriteLine("Enter weight for checked baggage (choose 20kg or 25kg): ");
                    double checkedWeight;

                    // Ensure valid input for checked baggage weight
                    if (double.TryParse(Console.ReadLine(), out checkedWeight))
                    {
                        if (checkedWeight == 20)
                        {
                            feeBaggage += 45;
                            totalWeightSeat += 20;
                            Console.WriteLine("Checked baggage of 20kg added for 45 EUR.");
                            break;
                        }
                        else if (checkedWeight == 25)
                        {
                            feeBaggage += 50;
                            totalWeightSeat += 25;

                            Console.WriteLine("Checked baggage of 25kg added for 50 EUR.");
                            Console.WriteLine("You have reached the 25kg per seat limit. No carry-on allowed.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid weight. Please enter either 20kg or 25kg.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric value.");
                    }
                }
            }

            // Add Carry-On Baggage if Space Left
            if (totalWeightSeat < 25)
            {
                double remainingCarryOnWeight = 25 - totalWeightSeat;
                Console.WriteLine($"You have {remainingCarryOnWeight}kg left for carry-on baggage. Do you want to add carry-on baggage? (yes/no):");
                string addCarryOn = Console.ReadLine().ToLower();

                while (addCarryOn == "yes" && totalWeightSeat < 25)
                {

                    Console.WriteLine($"Enter weight for carry-on bag (max {Math.Min(remainingCarryOnWeight, 5)} per bag per bag, left weight: {25 - totalWeightSeat}kg): ");
                    double carryOnWeight;

                    // Ensure valid input for carry-on weight
                    if (double.TryParse(Console.ReadLine(), out carryOnWeight))
                    {
                        if (carryOnWeight > 5)
                        {
                            Console.WriteLine("Carry-on weight cannot exceed 5kg per bag. Please enter a valid weight.");
                            continue;
                        }

                        if (totalWeightSeat + carryOnWeight > 25)
                        {
                            Console.WriteLine("Adding this carry-on bag will exceed the 25kg limit. Please adjust the weight.");
                            continue;
                        }

                        totalWeightSeat += carryOnWeight;
                        totalCarryOnWeight += carryOnWeight;
                        remainingCarryOnWeight -= carryOnWeight;
                        Console.WriteLine($"Added a carry-on bag of {carryOnWeight}kg for €15.");

                        if (totalCarryOnWeight < 5)
                        {
                            Console.WriteLine("Do you want to add another carry-on bag? (yes/no):");
                            addCarryOn = Console.ReadLine().ToLower();
                        }
                        else
                        {
                            break; // No more carry-on weight available
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric value.");
                    }
                }
            }

            if (totalCarryOnWeight > 0)
            {
                feeBaggage += 15;
                Console.WriteLine($"Total carry-on baggage fee: €15.");
            }

            // Confirmation message
            Console.WriteLine($"Baggage added successfully. Total weight: {totalWeightSeat}kg, Total fee: €{feeBaggage}.");
            baggageInfo.Add(new BaggageLogic(initials, "checked + carry-on", totalWeightSeat) { Fee = feeBaggage });
        }

        // Pet handling
        Console.WriteLine("Do you want to add a pet? (yes/no): ");
        string userPet = Console.ReadLine()?.ToLower();

        if (userPet == "yes")
        {
            if (flight.TotalPets >= 7)
            {
                Console.WriteLine("Sorry, no more pet space available on this flight.");
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("What type of pet do you have? (dog, cat, bunny, bird): ");
                    string petType = Console.ReadLine()?.ToLower();

                    if (petType == "dog" || petType == "cat" || petType == "bunny" || petType == "bird")
                    {
                        var newPet = new PetLogic(petType) { Fee = 50.0 };
                        petInfo.Add(newPet);
                        flight.TotalPets++;
                        Console.WriteLine($"Pet {petType} added. Fee: 50 EUR.");

                        if (flight.TotalPets >= 7)
                        {
                            Console.WriteLine("Maximum pet capacity reached for this flight.");
                            break;
                        }

                        Console.WriteLine("Do you want to add another pet? (yes/no):");
                        string addAnother = Console.ReadLine()?.ToLower();
                        if (addAnother != "yes")
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid pet type. Please choose from (dog, cat, bunny, bird).");
                    }
                }
            }
        }
    }

    public static void BookFlightMenu(bool searchFlightFunction = false, FlightModel flightModel = null)
    {
        var currentAccount = AccountsLogic.CurrentAccount;
        // int totalFlightpoints = 0;

        List<BaggageLogic> baggageInfo = new List<BaggageLogic>();
        List<PetLogic> petInfo = new List<PetLogic>();
        List<PassengerModel> passengers = new List<PassengerModel>();
        double totalPrice = 0;

        FlightModel selectedFlight = flightModel;

        if (!searchFlightFunction)
        {
            while (true)
            {
                Console.WriteLine("=== Book Ticket ===\n");
                if (allFlights.Count == 0)
                {
                    Console.WriteLine("No flights available.");
                    return;
                }

                Console.WriteLine("{0,-5} {1,-25} {2,-46} {3,-58} {4,-15} {5,-14} {6,-14} {7,-15} {8,-10}",
                    "ID", "Airline", "Departure Airport", "Arrival Destination",
                    "Flight Date", "Flight Time", "Ticket Price", "Return Flight", "Cancelled");
                Console.WriteLine(new string('-', 213));

                foreach (var flight in allFlights)
                {
                    Console.WriteLine("{0,-5} {1,-25} {2,-46} {3,-58} {4,-15} {5,-14} {6,-14} {7,-15} {8,-10}",
                        flight.Id,
                        flight.Airline,
                        flight.DepartureAirport,
                        flight.ArrivalDestination,
                        flight.DepartureDate,
                        flight.FlightTime,
                        flight.TicketPrice,
                        flight.ReturnFlight == null ? "No" : "Yes",
                        flight.IsCancelled ? "Yes" : "No");
                }

                Console.WriteLine("\n" + new string('-', 213));
                Console.Write("\nEnter the ID of the flight you wish to book (or 'Q' to quit): ");

                var input = Console.ReadLine();
                if (input?.ToUpper() == "Q")
                {
                    Console.WriteLine("Booking canceled. Returning to main menu.");
                    MenuLogic.PopMenu();
                    break;
                }

                if (int.TryParse(input, out int selectedId))
                {
                    selectedFlight = BookFlightLogic.SearchFlightByID(selectedId);
                    if (selectedFlight != null)
                    {
                        break;  // Exit the while loop if we found a valid flight
                    }
                    else
                    {
                        Console.WriteLine("Invalid flight ID. Please try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a numeric ID.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        if (selectedFlight != null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nYou have selected the following flight:\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0,-20} {1,-35}", "Airline:", selectedFlight.Airline);
            Console.WriteLine("{0,-20} {1,-35}", "Departure Airport:", selectedFlight.DepartureAirport);
            Console.WriteLine("{0,-20} {1,-35}", "Arrival Destination:", selectedFlight.ArrivalDestination);
            Console.WriteLine("{0,-20} {1,-35}", "Flight Date:", selectedFlight.DepartureDate);
            Console.WriteLine("{0,-20} {1,-35}", "Flight Time:", selectedFlight.FlightTime);
            Console.WriteLine("{0,-20} {1,-35}", "Ticket Price:", selectedFlight.TicketPrice);
            Console.WriteLine("{0,-20} {1,-35}", "Available Seats:", selectedFlight.AvailableSeats);
            Console.WriteLine("{0,-20} {1,-35}", "Is Cancelled:", (selectedFlight.IsCancelled ? "Yes" : "No"));
            Console.ResetColor();

            Console.Write("\nAre you sure you want to book this flight? (yes/no) ");
            string confirmation = Console.ReadLine();
            Console.Clear();

            if (confirmation.ToLower() == "yes")
            {
                List<string> chosenSeats = new List<string>();
                selectedFlight.Layout.PrintLayout();

                while (true)
                {
                    Console.Write("\nWhich seat do you want? (press Q to quit or Enter to confirm booking and keep choosing by seat number if you want more seats): ");
                    string seat = Console.ReadLine()?.ToUpper();

                    if (seat == "Q")
                    {
                        break;
                    }
                    else if (string.IsNullOrWhiteSpace(seat))
                    {
                        if (chosenSeats.Count == 0)
                        {
                            Console.WriteLine("Please select at least one seat.");
                            continue;
                        }
                        Console.WriteLine("Confirming your selected seats...");
                        break;
                    }

                    // Check if seat is already booked
                    if (!selectedFlight.Layout.TryBookSeat(seat))
                    {
                        Console.WriteLine("This seat is already booked or invalid. Please choose another seat.");
                        continue;
                    }

                    // If we get here, the seat is available, so we can collect passenger information
                    PassengerModel passenger = new PassengerModel();
                    Console.WriteLine($"\nEnter Passenger Information for seat {seat}:");


                    while (true)
                    {
                        Console.Write("First Name: ");
                        string firstName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(firstName) && firstName.All(char.IsLetter))
                        {
                            passenger.FirstName = firstName;
                            break;
                        }
                        Console.WriteLine("Invalid First Name. Please enter alphabets only.");
                    }

                    while (true)
                    {
                        Console.Write("Last Name: ");
                        string lastName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(lastName) && lastName.All(char.IsLetter))
                        {
                            passenger.LastName = lastName;
                            break;
                        }
                        Console.WriteLine("Invalid Last Name. Please enter alphabets only.");
                    }

                    while (true)
                    {
                        Console.Write("Title (Mr/Ms/Dr): ");
                        string title = Console.ReadLine();
                        if (new[] { "Mr", "Ms", "Dr" }.Contains(title))
                        {
                            passenger.Title = title;
                            break;
                        }
                        Console.WriteLine("Invalid Title. Please choose from (Mr, Ms, Dr).");
                    }

                    while (true)
                    {
                        Console.Write("Age Group (adult/child/infant): ");
                        string ageGroup = Console.ReadLine().ToLower();
                        if (new[] { "adult", "child", "infant" }.Contains(ageGroup))
                        {
                            passenger.AgeGroup = ageGroup;

                            if (ageGroup == "infant")
                            {
                                while (true)
                                {
                                    Console.Write("Date of Birth (dd-MM-yyyy): ");
                                    string dobInput = Console.ReadLine();
                                    if (DateTime.TryParseExact(dobInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dob)
                                        && dob <= DateTime.Now)
                                    {
                                        passenger.DateOfBirth = dob;
                                        break;
                                    }
                                    Console.WriteLine("Invalid Date of Birth. Please enter a valid past date in format dd-MM-yyyy.");
                                }
                            }
                            break;
                        }
                        Console.WriteLine("Invalid Age Group. Please enter 'adult', 'child', or 'infant'.");
                    }

                    try
                    {
                        string initials = GenerateInitials(passenger);
                        selectedFlight.Layout.BookFlight(seat, initials);
                        ProcessPassengerDetails(passenger, seat, ref totalPrice, selectedFlight.TicketPrice, initials, passengers, chosenSeats, baggageInfo, petInfo, selectedFlight);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        continue;
                    }

                    Console.Clear();
                    selectedFlight.Layout.PrintLayout();
                    // totalFlightpoints += selectedFlight.FlightPoints;
                }

                // Show booking summary
                Console.Clear();
                Console.WriteLine("\n=== Booking Summary ===");
                Console.WriteLine($"Flight: {selectedFlight.Airline}");
                Console.WriteLine($"Route: {selectedFlight.DepartureAirport} to {selectedFlight.ArrivalDestination}");
                Console.WriteLine($"Date: {selectedFlight.DepartureDate}, Time: {selectedFlight.FlightTime}");

                Console.WriteLine("\nPassenger Details:");
                for (int i = 0; i < passengers.Count; i++)
                {
                    var p = passengers[i];
                    Console.WriteLine($"{i + 1}. {p.Title} {p.FirstName} {p.LastName}");
                    Console.WriteLine($"   Seat: {chosenSeats[i]}");
                    Console.WriteLine($"   Age Group: {p.AgeGroup}");
                    if (p.DateOfBirth.HasValue)
                    {
                        Console.WriteLine($"   Date of Birth: {p.DateOfBirth.Value:dd-MM-yyyy}");
                    }
                }

                // Calculate final price including fees
                double baggageTotalFee = baggageInfo.Sum(b => b.Fee);
                double petTotalFee = petInfo.Sum(p => p.Fee);
                totalPrice += baggageTotalFee + petTotalFee;

                Console.WriteLine($"\nPrice Breakdown:");
                Console.WriteLine($"Ticket(s): {totalPrice - baggageTotalFee - petTotalFee:C}");
                if (baggageTotalFee > 0) Console.WriteLine($"Baggage Fees: {baggageTotalFee:C}");
                if (petTotalFee > 0) Console.WriteLine($"Pet Fees: {petTotalFee:C}");
                Console.WriteLine($"Total Price: {totalPrice:C}");

                int allFlightPoints = currentAccount.TotalFlightPoints;
                Console.Write($"\nBefore confirming your booking do you want to use your flight points for discount? You have {(allFlightPoints)} points. (yes/no): ");

                string discountYesOrNo = Console.ReadLine();


                if (discountYesOrNo == "yes" && allFlightPoints > 0)
                {
                    Console.Write("How many points would you like to use? (1 point equals 1 euro, and you can use your points for up to a 20% discount on the price.): ");
                    string amountFlightPointsStr = Console.ReadLine();

                    if (int.TryParse(amountFlightPointsStr, out int amountFlightPoints))
                    {
                        // Calculate max discount (20% of totalPrice)
                        double maxDiscount = totalPrice * 0.2;

                        // Ensure points do not exceed max discount or available points
                        double discountToApply = Math.Min(amountFlightPoints, Math.Min(maxDiscount, allFlightPoints));

                        totalPrice -= discountToApply;
                        currentAccount.TotalFlightPoints -= (int)discountToApply;

                        Console.WriteLine($"You used {discountToApply:C} worth of flight points.");
                        Console.WriteLine($"Updated Total Price: {totalPrice:C}");
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input. Enter a valid integer.");
                    }

                }
                else if (discountYesOrNo == "yes" && allFlightPoints == 0)
                {
                    Console.WriteLine("You don't have enough flight points for a discount.");
                    // break;
                }

                else if (discountYesOrNo == "no")
                {
                    // break;
                    Console.WriteLine();
                }
                else
                {

                }

                Console.Write("\nConfirm booking? (yes/no): ");
                string finalConfirmation = Console.ReadLine().ToLower();
                if (finalConfirmation == "yes")
                {
                    selectedFlight.Layout.ConfirmBooking();
                    // var FlightPointData = new FlightPoint(DateTime.Now, totalFlightpoints, selectedFlight.Id);
                    // currentAccount.FlightPointsDataList.Add(FlightPointData);

                    // Save passenger information
                    var existingPassengers = PassengerAccess.LoadPassengers();
                    existingPassengers.AddRange(passengers);
                    PassengerAccess.SavePassengers(existingPassengers);

                    var bookedFlight1 = new BookedFlightsModel(selectedFlight.Id, selectedFlight.Layout.BookedSeats, baggageInfo, petInfo, false);
                    bookedFlight1.TicketBill += totalPrice;

                    List<BookedFlightsModel> bookedFlightModel = new List<BookedFlightsModel>
                    {
                        bookedFlight1
                    };

                    AccountsAccess.WriteAll(AccountsLogic._accounts);
                    AccountsAccess.UpdateCurrentAccount(currentAccount);
                    FlightsAccess.WriteAll(allFlights);
                    foreach (var bookedFlight in bookedFlightModel)
                    {
                        BookFlightLogic.RemoveDuplicateSeats(bookedFlight);
                    }
                    BookedFlightsAccess.WriteAll(currentAccount.EmailAddress, bookedFlightModel);
                    Console.WriteLine("\nBooking confirmed successfully!");
                    Console.WriteLine("All passenger information has been saved.");
                }
                else
                {
                    Console.WriteLine("\nBooking cancelled.");
                }
            }
            else if (confirmation.ToLower() == "no")
            {
                Console.WriteLine("Booking flight is cancelled, choose a new flight.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Invalid input, try again.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        else
        {
            Console.WriteLine("Invalid flight selection. Please try again.");
        }
    }
}