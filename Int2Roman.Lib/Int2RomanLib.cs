using LanguageExt;

namespace Int2Roman;
/// <summary>
/// This class translate an Integer to a Roman Number
/// </summary>
public class Int2Roman : IInt2Roman
{
    /// <summary>
    /// Converts an Integer to a Roman Numbers
    /// </summary>
    /// <param name="num">Input number to be converted</param>
    /// <returns></returns>
    string IInt2Roman.ConvertToRoman(uint num)
    {
        IEnumerable<uint> rng = CreateRange();

        IEnumerable<Mapper> mapperComplex = CreateComplex(rng, mapperBasic);

        IEnumerable<Mapper> mapper = CreateMapper(mapperBasic, mapperComplex);

        // Auxiliary function to perform tail recursion to return the roman number
        string Helper(uint num, string result)
        {
            if (num == 0)
            {
                return result;
            }
            Mapper elem = mapper.Last(n => n.Number <= num);

            return Helper(num - elem.Number, result + elem.Roman);
        }

        return Helper(num, string.Empty);
    }
    /// <summary>
    /// Combines the basic one character romans number with the two characters roman numbers
    /// </summary>
    /// <param name="basic">Single character roman numbers</param>
    /// <param name="complex">Duals characters roman numbers</param>
    private static IEnumerable<Mapper> CreateMapper(IEnumerable<Mapper> basic, IEnumerable<Mapper> complex) =>
        basic.Concat(complex)
             .OrderBy(n => n.Number);
    /// <summary>
    /// Builds the combined romans numbers (e.g., IV, IX, XL, XC, CD, MC ...)
    /// </summary>
    /// <param name="rng">Sequence of powers of ten</param>
    /// <param name="mapper">Original one symbol roman numbers</param>
    private static IEnumerable<Mapper> CreateComplex(IEnumerable<uint> rng, IEnumerable<Mapper> mapper)
    {
        // Auxiliary function to perform tail recursion and return the complex roman numbers
        IEnumerable<Mapper> Helper(IEnumerable<uint> rng, IEnumerable<Mapper> extraRoman)
        {
            if (rng.Any())                                                              // When sequence has data
            {
                uint item = rng.First();                                                // Get the first element

                Mapper upperElem = mapper.First(x => x.Number == item);
                Mapper middleElem = mapper.Last(x => x.Number < item);
                Mapper lowerElem = mapper.Last(x => x.Number < middleElem.Number);

                Mapper extraRomanLower = new()
                {
                    Number = middleElem.Number - lowerElem.Number,
                    Roman = lowerElem.Roman + middleElem.Roman
                };

                Mapper extraRomanUpper = new()
                {
                    Number = upperElem.Number - lowerElem.Number,
                    Roman = lowerElem.Roman + upperElem.Roman
                };

                IEnumerable<Mapper> extraRomanNext =  extraRoman.Append(extraRomanLower)
                                                                .Append(extraRomanUpper);

                return Helper(rng.Skip(1), extraRomanNext);
            }

            return extraRoman;

        }

        List<Mapper> extraRoman = [];
        return Helper(rng, extraRoman);

    }
    /// <summary>
    /// Builds a sequences of powers of ten (e.g.; 10, 100, ... 1,000,000)
    /// </summary>
    private static IEnumerable<uint> CreateRange() =>
        Enumerable.Range(1, 6).Select(n => (uint)Math.Pow(10, n));

    /// <summary>
    /// Original one symbol roman numbers
    /// </summary>
    private readonly IEnumerable<Mapper> mapperBasic =
    [
        new() {Number=1, Roman="I"},
            new() {Number=5, Roman="V"},
            new() {Number=10, Roman="X"},
            new() {Number=50, Roman="L"},
            new() {Number=100, Roman="C"},
            new() {Number=500, Roman="D"},
            new() {Number=1000, Roman="M"},
            new() {Number=5000, Roman="V\u0305"},
            new() {Number=10000, Roman="X\u0305" },
            new() {Number=50000, Roman="L\u0305" },
            new() {Number=100000, Roman="C\u0305" },
            new() {Number=500000, Roman="D\u0305" },
            new() {Number=1000000, Roman="M\u0305" }
    ];
}
