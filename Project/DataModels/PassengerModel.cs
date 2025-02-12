namespace DataModels
{
    public class PassengerModel : IDataModel
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string AgeGroup { get; set; } // e.g., "adult", "child", "infant"
        public DateTime? DateOfBirth { get; set; } // Only for infants
        public PassengerModel(int id)
        {
            Id = id;
        }
    }
}

