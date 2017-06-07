using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Accounting.Architecture.AccountHelpers
{
    public static class AccountHelpers
    {
      
        public static MvcHtmlString AccountTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,object htmlAttributes)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("class","acctxtbox");
          //  var name = ExpressionHelper.GetExpressionText(expression);
          //  var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
          // // TagBuilder tb = new TagBuilder("input");
          ////  tb.Attributes.Add("type", "text");
          // // tb.Attributes.Add("class", "text");
          // // tb.Attributes.Add("name", name);
          // // tb.Attributes.Add("value", metadata.Model as string);
          // // tb.Attributes.Add("style", "color:red");
          //  //return new MvcHtmlString(tb.ToString());
            return InputExtensions.TextBoxFor(htmlHelper, expression, attributes);
        }
    }
}