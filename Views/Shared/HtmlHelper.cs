using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inno.Views.Shared
{
    public static class HtmlHelper
    {
        public static IHtmlContent ErrorContainer(this IHtmlHelper html)
        {
            return new HtmlString(@"
            <div id='errordiv'
                 class='alert alert-danger py-1 fw-semibold fade show'
                 style='display:none'
                 role='alert'></div>
        ");
        }
    }
}