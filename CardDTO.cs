namespace StatusAPI.DTO
{
    public class CardDTO
    {
        public int Id { get; set; }
        public string? IPAddress { get; set; }
        public int Port { get; set; }
        public string? Status { get; set; }
    }
}
