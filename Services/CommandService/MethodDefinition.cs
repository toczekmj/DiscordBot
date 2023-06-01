using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Services.CommandService;

public class MethodDefinition : IMethodDefinition
{
    public string Name { get; set; }
    public List<string> Parameters { get; set; }
    public string Body { get; set; }
}