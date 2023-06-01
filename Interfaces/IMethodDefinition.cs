namespace DiscordBot_tutorial.Interfaces;

public interface IMethodDefinition
{
    public string Name { get; set; }
    public List<string> Parameters { get; set; }
    public string Body { get; set; }
}