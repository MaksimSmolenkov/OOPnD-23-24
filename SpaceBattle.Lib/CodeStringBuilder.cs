using Hwdtech;
using Scriban;

namespace SpaceBattle.Lib;
public class CodeStringAdapterBuilder : IBuilder
{

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
        var templateText = IoC.Resolve<string>("Template");
        var template = Template.Parse(templateText);
        var result = template.Render(new { name = _className, properties = _members.ToArray() });
        return result;
    }
}
