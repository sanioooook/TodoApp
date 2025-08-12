namespace Todo.Infrastructure.Tests.Helpers;

using System.Reflection;
using Dapper;

public static class CommandDefinitionExtensions
{
    /// <summary>
    /// Gets the value of a parameter from CommandDefinition.Parameters by name.
    /// </summary>
    public static T GetParameterValue<T>(this CommandDefinition cmd, string name)
    {
        if (cmd.Parameters == null)
            return default;

        var prop = cmd.Parameters.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (prop == null)
            return default;

        return (T)prop.GetValue(cmd.Parameters);
    }
}