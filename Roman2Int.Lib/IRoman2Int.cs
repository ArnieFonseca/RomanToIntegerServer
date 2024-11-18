namespace Roman2Int.Lib;
public interface IRoman2Int
{
    (bool Success, string? Token, uint Answer) Convert(string rn);    
}