using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class RecordEqualityGenerator : ISourceGenerator
{
    private const string AttributeText = @"
using System;
namespace RecordEquality
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""EqualityAttribute_DEBUG"")]
    sealed class EqualityAttribute : Attribute
    {
    }
}
";

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterForPostInitialization(i => i.AddSource("RecordEqualityAttribute", AttributeText));
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
            return;

        Debugger.Break();

        foreach (var record in receiver.Records)
        {
            var propertyEquals = string.Join(" && ", record.Properties
                .Select(p => $"{p} == other.{p}")
                .ToList());
            
            var hashCode = new StringBuilder("\n");
            for (var index = 0; index < record.Properties.Count; index++) {
                var property = record.Properties[index];
                hashCode.AppendLine(index == 0
                    ? $"        var hashCode = {property}.GetHashCode();"
                    : $"        hashCode = (hashCode * 397) ^ {property}.GetHashCode();");
            }
            hashCode.AppendLine("        return hashCode;");
            
            var source =
$@"// Auto-generated code for {record.Name}

public partial record {record.Name}
{{
    public virtual bool Equals({record.Name}? other)
    {{
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return {propertyEquals};
    }}

    public override int GetHashCode()
    {{{hashCode}
    }}
}}";
            context.AddSource($"{record.Name}__equality.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    class SyntaxReceiver : ISyntaxContextReceiver
    {
        public record RecordResult(string Name, List<string> Properties);
        
        public List<RecordResult> Records { get; } 
            = new ();
        
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is RecordDeclarationSyntax rds &&
                rds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                var record = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, rds);
                if (record is null) return;
                
                var result = new RecordResult(record.Name, new List<string>());
                var addRecord = false;

                foreach (var parameter in rds.ParameterList.Parameters)
                {
                    var ps = ModelExtensions.GetDeclaredSymbol(context.SemanticModel,
                        parameter) as IParameterSymbol;

                    if (ps.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "RecordEquality.EqualityAttribute"))
                    {
                        result.Properties.Add(ps.Name);
                        addRecord = true;
                    }
                }

                if (addRecord)
                {
                    Records.Add(result);
                }
            }
        }
    }
}