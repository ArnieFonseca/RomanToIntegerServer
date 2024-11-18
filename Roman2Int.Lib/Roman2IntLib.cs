using LanguageExt;

using System;
using System.Text.RegularExpressions;

namespace Roman2Int.Lib;
 
public class RomanConverter : IRoman2Int
{
    /// <summary>
    /// Zero Numeric
    /// </summary>
    public const int ZERO = 0;
    /// <summary>
    /// One Numeric
    /// </summary>
    public const int ONE = 1;
    /// <summary>
    /// Two Numeric
    /// </summary>
    public const int TWO = 2;

    /// <summary>
    /// THREE Numeric
    /// </summary>
    public const int THREE = 3;


    /// <summary>
    /// Validates the Roman Input Number is valid ∀x, { A(x) ∧ B(x) ∧ C(x) ∧ D(x)}
    /// and convert it to integer if no errors
    /// </summary>
    /// <param name="rn">Input Roman Number</param>
    /// <returns>True when is valid</returns>
    public (bool Success, string? Token, uint Answer) Convert(string? rn)
    {
 
        // Wrap the roman number in the Either Monad 
        Either<RomanNumberError, string> rnMonad = rn ?? string.Empty;

        // Validate using Railroad pattern
        var result = rnMonad.Bind(IsNotEmpty)
                            .Bind(IsValidSymbol)
                            .Bind(IsValidRepeat)
                            .Bind(IsValidComplex)
                         ;

        // Send back the Tuple 
        return result.IsRight
                ? (result.IsRight, rn, ConvertToInteger(rn))
                : (result.IsRight,
                   result.Right(x => x)
                         .Left(x => x.ToString()),
                   ZERO)
                ;         
    
    }

    /// <summary>
    /// Convert a Roman Number into its corresponding Integer
    /// </summary>
    /// <param name="rn">Roman Number Representation</param>
    /// <returns>Output Integer converted number</returns>
    static private uint ConvertToInteger(string rn)    
    {
        List<Mapper> lst = rn.Select(x => mapper.First(y => y.Roman == x.ToString()))
                             .ToList();
        return ConvertToIntegerHelper(lst, ZERO);
    }

    /// <summary>
    /// Recursive Auxiliary function to convert a Roman Number into its corresponding Integer
    /// </summary>
    /// <param name="rnList">List of Mapper</param>
    /// <param name="accumulator">Accumulator</param>
    /// <returns>Output Integer converted number</returns>
    /// <exception cref="Exception"></exception>

    static private uint ConvertToIntegerHelper(List<Mapper> rnList,  uint accumulator)    
    {
        // Lazy  Evaluation
        Mapper Current() => rnList.First();
        Mapper Next() => rnList[ONE];
        bool IsCurrentComplex() => Current().Roman == Next().Prefix;

        // Pattern Matching
        return rnList switch
        {
            // The List is Empty
            var _ when rnList.Count == ZERO => accumulator,
            var _ when rnList.Count == ONE  => accumulator + Current().Number,
            var _ when rnList.Count > ONE  && !IsCurrentComplex() => ConvertToIntegerHelper(rnList.Skip(ONE).ToList(),accumulator + Current().Number),
            var _ when rnList.Count > ONE  && IsCurrentComplex() => ConvertToIntegerHelper(rnList.Skip(TWO).ToList(),accumulator + Next().Number - Current().Number),
            var _ => throw new  Exception("Non Catch condition"),
        };
        
    }
    /// <summary>
    /// Ensure that the input is not an Empty String
    /// </summary>
    /// <param name="rn">Input Roman Number</param>
    /// <returns>Either of RomanNumberError or string</returns>
    static private Either<RomanNumberError, string> IsNotEmpty(string rn)
    {
        return !string.IsNullOrEmpty(rn.Trim())
            ? rn
            : RomanNumberError.InvalidEmpty
            ;
    }
    /// <summary>
    /// Ensure only valid Roman Symbol exists in the input ∀x, { x ∈ Roman }
    /// </summary>
    /// <param name="rn">Input Roman Number</param>
    /// <returns>Either of RomanNumberError or string</returns>
    static private Either<RomanNumberError, string> IsValidSymbol(string rn)
    {
        // Get the Symbols
        var symbols = mapper.Select(x => x.Roman);

        // Ensure all input elements exists in the symbols sequence
        var answer = rn.ToList().All(x => symbols.Contains(x.ToString()));

        return (answer) 
                ? rn
                : RomanNumberError.InvalidSymbol;
         
    }

    /// <summary>
    /// Ensure the Roman Number has only valid symbol repetitions e.g., I,X,C no more that three and V,L,D only once
    /// </summary>
    /// <param name="rn">Input Roman Number</param>
    /// <returns>Either of RomanNumberError or string</returns>
    static private Either<RomanNumberError, string> IsValidRepeat(string rn)
    {
        // Retrieve symbol repetition
        Func<int,string> GetSymbols = (limit) =>
            mapper.Where(s => s.Limit == limit)
                              .Select(s => s.Roman)
                              .Aggregate(string.Empty, (p, q) => p + q)
                              ;
        
        // Get single repetition symbols  
        var vld = GetSymbols(ONE);

        // Get triple repetition symbols 
        var ixcm = GetSymbols(THREE);

        // Ensure the symbols are repeated up to one
        bool isVLDCountValid = 
            (rn.Where(s => vld.Contains(s))
                              .GroupBy(inpStr => inpStr)
                              .Any(s => s.Count() > 1) == false);

        // Regular Expression pattern to check the symbols that can repeat up to three
        var patternIXCM = @$"([{ixcm}])\1{{{THREE}}}";

        // Ensure the symbols are repeated up to Three
        bool isIXCMCountValid = Regex.IsMatch(rn, patternIXCM) == false;

        // True implies valid repetition
        bool answer = isIXCMCountValid && isVLDCountValid;
      
        return (answer)
                ? rn
                : RomanNumberError.InvalidRepeat; 
    }

    /// <summary>
    /// IsValidComplex :: string -> bool 
    /// Ensure a valid combination of Complex Roman Number
    /// </summary>
    /// <param name="rn">Input Roman Number</param>
    /// <returns>Either of RomanNumberError or string</returns>
    static private Either<RomanNumberError, string> IsValidComplex(string rn)
    {
        // Map the input string into a Mapper List
        List<Mapper> lst = rn.Select(x => mapper.First(y => y.Roman == x.ToString())).ToList();

        var answer =  IsValidComplexHelper(lst, false, mapper.Last(), mapper.Last());

        return (answer)
                ? rn
                : RomanNumberError.InvalidOrderOrComplex;

    }

    /// <summary>
    /// Auxiliary recursive function to ensure that ∀x,∀y {A(x,y) ∨ B(x)}
    /// </summary>
    /// <param name="rnList">List of Mapper</param>
    /// <param name="isPrevComplex">Boolean indicating is the previous parsed number was complex</param>
    /// <param name="penultimateValue">Penultimate Mapper</param>
    /// <param name="prevValue">Previous Mapper</param>
    /// <returns>True when is valid</returns>
    static bool IsValidComplexHelper(List<Mapper> rnList, bool isPrevComplex, Mapper penultimateValue, Mapper prevValue)
    {

        // Lazy  Evaluation
        Mapper Current() => rnList.First();
        Mapper Next() => rnList[ONE];
        bool IsCurrentComplex() => Current().Roman == Next().Prefix;

        // Pattern Matching
        return rnList switch
        {

            // The List is Empty
            var _ when rnList.Count == ZERO => true,

            // The last parsed number was complex
            var _ when rnList.Count == ONE && isPrevComplex && penultimateValue.Number > Current().Number => true,

            // The last parsed number was not complex
            var _ when rnList.Count == ONE && !isPrevComplex && prevValue.Number >= Current().Number => true,

            // Input is greater that one and it prev was not complex neither the current
            var _ when rnList.Count > ONE && !isPrevComplex && !IsCurrentComplex() && prevValue.Number >= Current().Number
                    => IsValidComplexHelper(rnList.Skip(ONE).ToList(), IsCurrentComplex(), Current(), Current()),

            // Input is greater that one and it prev was complex but the current is not                    
            var _ when rnList.Count > ONE && isPrevComplex && !IsCurrentComplex() && penultimateValue.Number > Current().Number
                    => IsValidComplexHelper(rnList.Skip(ONE).ToList(), IsCurrentComplex(), Current(), Current()),

            // Input is greater that one and it prev was not complex, but the current is complex
            var _ when rnList.Count > ONE && !isPrevComplex && IsCurrentComplex() && prevValue.Number >= Next().Number
                    => IsValidComplexHelper(rnList.Skip(TWO).ToList(), IsCurrentComplex(), Current(), Next()),

            // Input is greater that one and it prev was  complex and the current is also complex
            var _ when rnList.Count > ONE && isPrevComplex && IsCurrentComplex() && penultimateValue.Number >= Next().Number
                    => IsValidComplexHelper(rnList.Skip(TWO).ToList(), IsCurrentComplex(), Current(), Next()),

            // Otherwise the parsing just found an error
            var _ => false,
        };
    }

    /// <summary>
    /// Original one symbol roman numbers
    /// </summary>
    private static readonly IEnumerable<Mapper> mapper =
    [
        new() {Number=1, Roman="I", Limit=3},
        new() {Number=5, Roman="V", Limit=1, Prefix="I"},
        new() {Number=10, Roman="X", Limit=3,Prefix="I"},
        new() {Number=50, Roman="L", Limit=1, Prefix="X"},
        new() {Number=100, Roman="C", Limit=3, Prefix="X"},
        new() {Number=500, Roman="D", Limit=1, Prefix="C"},
        new() {Number=1000, Roman="M", Limit=3, Prefix="C"},

    ];
    public record Mapper
    {
        /// <summary>
        /// Non zero integer representation
        /// </summary>
        public required uint Number { get; init; }
        /// <summary>
        /// Roman number representation
        /// </summary>
        public required string Roman { get; init; }
        /// <summary>
        /// The max number of repetition the symbol may have
        /// </summary>
        public required int Limit { get; init; }
        /// <summary>
        /// Complex Prefix
        /// </summary>
        public string? Prefix { get; init; }
    }

}