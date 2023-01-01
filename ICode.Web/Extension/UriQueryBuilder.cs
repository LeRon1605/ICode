using System.Web;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace ICode.Web.Extension
{
    public static class UriQueryBuilder
    {
        public static void AddQuery(this QueryBuilder builder, string name, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                builder.Add(name, value.ToString());
            }
        }
    }
}
