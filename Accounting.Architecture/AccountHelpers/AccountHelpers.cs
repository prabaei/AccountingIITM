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

        public static MvcHtmlString AccountTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
          string classatt= (string) attributes.Where(m => m.Key.Equals("class")).Select(m => m.Value).FirstOrDefault();
            attributes.Remove("class");
            attributes.Add("class", "form-control"+" "+ classatt);
            
            return InputExtensions.TextBoxFor(htmlHelper, expression, attributes);
        }
        public static MvcHtmlString AccountTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes,bool disable=false)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string classatt = (string)attributes.Where(m => m.Key.Equals("class")).Select(m => m.Value).FirstOrDefault();
            attributes.Remove("class");
            attributes.Add("class", "form-control" + " " + classatt);
            if(disable)
            attributes.Add("disabled", new { });
            return InputExtensions.TextBoxFor(htmlHelper, expression, attributes);
        }
        public static MvcHtmlString CTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, bool disable = false)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            
            if (disable)
                attributes.Add("disabled", new { });
            return InputExtensions.TextBoxFor(htmlHelper, expression, attributes);
        }
        public static MvcHtmlString AccountTextBoxDisabledFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string classatt = (string)attributes.Where(m => m.Key.Equals("class")).Select(m => m.Value).FirstOrDefault();
            attributes.Remove("class");
            attributes.Add("class", "form-control" + " " + classatt);
            attributes.Add("disabled", new { });
            return InputExtensions.TextBoxFor(htmlHelper, expression, attributes);
        }
        public static MvcHtmlString AccountLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return LabelExtensions.LabelFor(htmlHelper, expression, attributes);
        }
        public static MvcHtmlString AccountDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return DisplayExtensions.DisplayFor(htmlHelper, expression, attributes);
        }
    }
}