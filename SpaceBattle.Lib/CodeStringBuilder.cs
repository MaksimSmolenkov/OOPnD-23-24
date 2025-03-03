using Scriban;

namespace SpaceBattle.Lib;

public class CodeStringAdapterBuilder : IBuilder
{
    private const string TemplateText = @"using System;
public class {{name }}
{
    private object obj;
    {{ for property in properties }}
    private {{ property.type }} {{ property.name }}{
    {{if property.set}}
    set
    {
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>(""{{property.name}}.Set"", obj, value).Execute();
    }
    {{end}}  
    get
    {{if property.get}}
    {
        return Hwdtech.IoC.Resolve<{{property.type}}>(""{{property.name}}.Get"", obj);
    } 
    {{ end }}
    }
    {{ end }}
    public {{ name }}(object obj)
    {
        this.obj = obj;
    }
}";

    private readonly string _className;
    private readonly List<object> _members;

    public CodeStringAdapterBuilder(string className)
    {
        _className = className;
        _members = new List<object>();
    }

    public CodeStringAdapterBuilder AddMember(object property)
    {
        _members.Add(property);
        return this;
    }

    public string Build()
    {
        var template = Template.Parse(TemplateText);
        var result = template.Render(new { name = _className, properties = _members.ToArray() });
        return result;
    }
}
