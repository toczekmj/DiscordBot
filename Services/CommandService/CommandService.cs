using System.Reflection;
using System.Reflection.Emit;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Services.CommandService;

public static class CommandService
{
    public static void CreateHandler(IMethodDefinition method)
    {
        AssemblyName assemblyName = new AssemblyName("DynamicHandler");
        AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        TypeBuilder typeBuilder =
            moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public | TypeAttributes.Class);

        Type type = typeof(Task);
        Type[] parametersType = new[] { typeof(SocketSlashCommand) };

        MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Private,
            returnType: type, parametersType);

        ILGenerator il = methodBuilder.GetILGenerator();

        string[] statements = method.Body.Split(';');

        foreach (var statement in statements)
        {
            string trimmedStatement = statement.Trim();
            if(string.IsNullOrWhiteSpace(trimmedStatement))
                continue;
            il.EmitWriteLine(trimmedStatement);
        }
        il.Emit(OpCodes.Ret);

        var assembly = assemblyBuilder.GetModules();
        
        Console.WriteLine(assembly);
        

    }
}