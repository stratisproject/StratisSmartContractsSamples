namespace SaleDeedRegistry.Lib.Command
{
    public class CommandResponse
    {
        public int fee { get; set; }
        public string hex { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public string transactionId { get; set; }
    }
}
