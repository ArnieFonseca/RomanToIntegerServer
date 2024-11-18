namespace AngularRoman2Int.Server.Model
{
    public class RomanDTO
    {
        public bool Success { get; set; } = false;
        public string? Token { get; set; }
        public uint Answer { get; set; }
    }
    //(bool Sucess, string? Token, uint Answer)
}
