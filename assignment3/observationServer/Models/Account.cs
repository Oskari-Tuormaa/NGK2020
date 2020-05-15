namespace observationServer.Models
{
  public class Account
  {
    public int    id            { get; set; }
    public string email         { get; set; }
    public string passwordHash  { get; set; }
    public string Token         { get; set; }
  }
}
