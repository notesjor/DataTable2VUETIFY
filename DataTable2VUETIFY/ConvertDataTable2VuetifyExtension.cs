using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Collections;

namespace DataTable2Vuetify
{
  public static class ConvertDataTable2VuetifyExtension
  {
    public static string ToVuetifyFormBaseSchema(this DataTable dataTable, HashSet<string> requiered = null, Dictionary<string, string> hints = null, Dictionary<string, string> overrideMapping = null, CheckboxStyle checkboxStyle = CheckboxStyle.Checkbox)
    {
      var schema = new JObject();

      foreach (DataColumn c in dataTable.Columns)
      {
        var cN = c.ColumnName;
        var columnSchema = new JObject
        {
            { "label", cN },
            { "type",
            overrideMapping != null && overrideMapping.ContainsKey(cN)
              ? overrideMapping[cN]
              : GetVuetifyFormBaseType(cN, c.DataType, checkboxStyle) },
            { "required", requiered == null ? false : requiered.Contains(cN) },
        };

        if (hints != null && hints.ContainsKey(cN))
          columnSchema.Add("hint", hints[cN]);

        schema.Add(cN, columnSchema);
      }

      return JsonConvert.SerializeObject(schema, Formatting.Indented);
    }

    private static string GetVuetifyFormBaseType(string name, Type dataType, CheckboxStyle checkboxStyle = CheckboxStyle.Checkbox)
    {
      switch (dataType)
      {
        case Type t when t == typeof(bool):
          return checkboxStyle == CheckboxStyle.Checkbox ? "checkbox" : "switch";
        case Type t when t == typeof(DateTime):
          return "date";
        case Type t when t == typeof(string):
          var low = name.ToLower();
          if (low.Contains("mail"))
            return "email";
          if (low.Contains("url"))
            return "file";
          if (low.Contains("password") || low.Contains("psw"))
            return "password";
          if (low.Contains("rtf"))
            return "richtext";
          if (low.Contains("url"))
            return "file";
          return "textarea";
        case Type t when t == typeof(IEnumerable):
          return "list";
        default:
          return "text";
      }
    }

    public enum CheckboxStyle
    {
      Checkbox,
      Switch
    }
  }
}