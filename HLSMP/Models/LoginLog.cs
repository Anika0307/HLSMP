namespace HLSMP.Models
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string District { get; set; }
        public DateTime LoginTime { get; set; }
        public string IPAddress { get; set; }
    }
}
