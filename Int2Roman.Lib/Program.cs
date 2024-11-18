using LanguageExt;
using static LanguageExt.Prelude;

namespace Int2Roman;

public class Program
{
    public static void Main()
    {

        uint num = 321;
        IInt2Roman int2Roman = new Int2Roman();

        string romanNumber = int2Roman.ConvertToRoman(num);

        Console.WriteLine($"Integer {num} is {romanNumber} in Roman");
    }

}