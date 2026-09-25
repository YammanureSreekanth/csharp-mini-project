using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ecom.MvcWebApp.TagHelpers;

[HtmlTargetElement("email")]
public class EmailTagHelper: TagHelper
{
    public string Address {get; set;} = "";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.Attributes.SetAttribute("href", $"mailto:{Address}");
        output.Content.SetContent(Address);
    }
}