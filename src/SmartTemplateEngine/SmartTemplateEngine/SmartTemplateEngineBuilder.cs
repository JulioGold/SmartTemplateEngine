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
                string tagFromTemplate = match.Groups[1].ToString();

                var resultValue = tagFromTemplate
                    .Split('.')
                    .Aggregate(
                        (object)contextObjJson,
                        (a, tagName) =>
                        {
                            if (((JObject.Parse(a.ToString())) ?? a as JToken) is var jObj && jObj != null)
                            {
                                if (jObj[tagName].HasValues && jObj[tagName].Type == JTokenType.Array)
                                {
                                    var localTemplate = tagsAndTemplates[$"{{{{{tagName}}}}}"];
                                    var sb = new StringBuilder();

                                    (contextObj.GetType().GetProperty(tagName).GetValue(contextObj, null) as IEnumerable<object>)
                                        .ToList()
                                        .ForEach(fff =>
                                        {
                                            sb.Append(SmartTemplateEngineBuilder.ProcessTemplate(localTemplate, fff, tagsAndTemplates));
                                        });

                                    var propertyListParsedTemplate = (new JValue(sb.ToString()) as JToken).ToString();

                                    return propertyListParsedTemplate;
                                }
                                else
                                {
                                    var propertyValue = jObj[tagName].ToString();

                                    return propertyValue;
                                }
                            }

                            return string.Empty;
                        }
                    );

                return resultValue.ToString() ?? string.Empty;
            });
        }
    }
}
