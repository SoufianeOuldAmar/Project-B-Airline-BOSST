public class PetLogic 
{
    public string AnimalType {get; set;}
    public double Fee{get; set;}

    public PetLogic(string animalType)
    {
        AnimalType= animalType;
        Fee= CalcFee();
    }

    public double CalcFee()
    {
        return 50;
    }


}