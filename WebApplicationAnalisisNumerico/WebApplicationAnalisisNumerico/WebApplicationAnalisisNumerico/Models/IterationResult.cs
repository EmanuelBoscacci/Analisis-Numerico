namespace WebApplicationAnalisisNumerico.Models
{
    public class IterationResult
    {
        public int Iteration { get; set; }
        public double Xi { get; set; }
        public double Xd { get; set; }
        public double Xr { get; set; }
        public double Fx { get; set; }
        public double Error { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}