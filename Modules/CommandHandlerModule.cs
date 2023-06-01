using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;
using Microsoft.CSharp;
using ModuleBuilder = Discord.Interactions.Builders.ModuleBuilder;

namespace DiscordBot_tutorial.Modules;

public class CommandHandlerModule
{
    private readonly DiscordSocketClient _client;
    private readonly ISettingsService _settingsService;

    public CommandHandlerModule(DiscordSocketClient client, ISettingsService settingsService)
    {
        _client = client;
        _settingsService = settingsService;
    }

    public static bool CompileCSharpCode(string sourceFile, string exeFile)
    {
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        CompilerParameters compilerParameters = new CompilerParameters();
        compilerParameters.ReferencedAssemblies.Add("System.dll");
        compilerParameters.GenerateExecutable = false;
        compilerParameters.OutputAssembly = exeFile;
        compilerParameters.GenerateInMemory = false;
        string code = File.ReadAllText(sourceFile);
        CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, code);
        if (compilerResults.Errors.Count > 0)
        {
            Console.WriteLine($"Error building {sourceFile} into {compilerResults.PathToAssembly}");
            foreach (CompilerError error in compilerResults.Errors)
                Console.WriteLine($" {error.ToString()}\n");
            return false;
        }
        Console.WriteLine($"Source {sourceFile} built into {compilerResults.PathToAssembly} succesfully.");
        return true;
    }
    
    public static string GenerateCSharpCode(CodeCompileUnit compileUnit)
    {
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();

        string sourceFile;
        if (codeProvider.FileExtension[0] == '.')
        {
            sourceFile = "HelloWorld" + codeProvider.FileExtension;
        }
        else
        {
            sourceFile = "HelloWorld." + codeProvider.FileExtension;
        }

        using (StreamWriter sw = new StreamWriter(sourceFile, false))
        {
            IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
            codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
            tw.Close();
        }

        return sourceFile;
    }

    // private async Task PapiezCommandHandler1(SocketSlashCommand command)
    // {
    //     await _client.GetGuild(_settingsService.Settings.GuildId)
    //         .GetTextChannel(1105183278266331156)
    //         .SendMessageAsync("wiadomosc");
    //     var t = (string)command.Data.Options.First().Value;
    //     await command.RespondAsync(t);
    // }
    // private async Task TestCommandHandler(SocketSlashCommand command)
    // {
    //     await command.RespondAsync("xdxdxd");
    // }
}