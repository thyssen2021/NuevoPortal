using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Portal_2_0.Helpers
{
    public static class OwnHelpers
    {
        public static MvcHtmlString DecimalBoxFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, decimal?>> expression, string format, object htmlAttributes = null)
        {
            var name = ExpressionHelper.GetExpressionText(expression);

            decimal? dec = expression.Compile().Invoke(html.ViewData.Model);

            // Here you can format value as you wish
            var value = dec.HasValue ? (!string.IsNullOrEmpty(format) ? dec.Value.ToString(format) : dec.Value.ToString())
                        : "";

            return html.TextBox(name, value, htmlAttributes);
        }
    }
}