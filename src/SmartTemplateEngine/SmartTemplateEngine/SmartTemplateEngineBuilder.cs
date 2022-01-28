using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartTemplateEngine
{
    public sealed class SmartTemplateEngineBuilder
    {
        public static string ProcessTemplate(string template, object contextObj, Dictionary<string, string> tagsAndTemplates)
        {
            if (string.IsNullOrEmpty(template) || contextObj == null)
            {
                return string.Empty;
            }

            JObject contextObjJson = JObject.FromObject(contextObj ?? new object());

            return Regex.Replace(template, @"\{\{([\w\.]+)\}\}", delegate (Match match)
            {
                string group = match.Groups[1].ToString();

                var value = group
                    .Split('.')
                    .Aggregate((object)contextObjJson, (a, b) =>
                    {
                        var jObj = (JObject.Parse(a.ToString())) ?? a as JToken;

                        if (jObj != null)
                        {
                            if (jObj[b].HasValues && jObj[b].Type == JTokenType.Array)
                            {
                                var localTemplate = tagsAndTemplates[$"{{{{{b}}}}}"];
                                var targetProperty = contextObj.GetType().GetProperty(b);
                                var sourcePropertyValue = targetProperty.GetValue(contextObj, null) as IEnumerable<object>;
                                var sb = new StringBuilder();

                                sourcePropertyValue.ToList()
                                .ForEach(fff =>
                                {
                                    sb.Append(SmartTemplateEngineBuilder.ProcessTemplate(localTemplate, fff, tagsAndTemplates));
                                });

                                var jValor = new JValue(sb.ToString()) as JToken;

                                return jValor.ToString();

                                //var valor = string.Join(", ", jObj[b].ToList().Select(s => s.ToString()));
                                //var jValor = new JValue(valor) as JToken;

                                //return jValor.ToString();
                            }
                            else
                            {
                                var result = jObj[b];

                                return result.ToString();
                            }
                        }

                        return null;
                    });

                return value != null ? (value.ToString()) : string.Empty;
            });
        }
    }
}
