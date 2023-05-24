using PluralizeService.Core;

using System.Text;

namespace signlar_function_builder;

internal static class EndPointOutput
{
    public static string GetDelete(EndPoint end)
    {
        (_, _, string deleteHub) = Update(end);
        StringBuilder towrite = new();

        _ = towrite.AppendLine($"                    const deleteResult = (\r\n");
        _ = towrite.AppendLine($"                        // eslint-disable-next-line @typescript-eslint/no-explicit-any");
        _ = towrite.AppendLine($"                        {end.IndexBy}: any");
        _ = towrite.AppendLine($"                    ) => {{\r\n                        updateCachedData(\r\n                            (\r\n                                draft: {end.DTOArray}");
        _ = towrite.AppendLine("                            ) => {");

        _ = towrite.AppendLine($"                              return draft.filter((obj) => obj.{end.IndexBy} !== {end.IndexBy});");

        //towrite.AppendLine("                                    (x) => x.id == id");
        //towrite.AppendLine("                                );");
        //towrite.AppendLine("");
        //towrite.AppendLine("                                if (foundIndex !== -1) {");
        //towrite.AppendLine("                                    draft = draft.filter(");
        //towrite.AppendLine("                                        (obj) => obj.id !== id");
        //towrite.AppendLine("                                    );");
        //towrite.AppendLine("                                }");
        _ = towrite.AppendLine("                            }");
        _ = towrite.AppendLine("                        );");
        _ = towrite.AppendLine("                    };\r\n");
        _ = towrite.AppendLine("                    hubConnection.on(");
        _ = towrite.AppendLine($"                        '{deleteHub}',");
        _ = towrite.AppendLine($"                        (id: number) => {{");
        _ = towrite.AppendLine("                            deleteResult(id);");
        _ = towrite.AppendLine("                        }\r\n                    );");

        return towrite.ToString();
    }

    public static string GetUpdate(EndPoint end)
    {
        (string updateHub, _, _) = Update(end);
        StringBuilder towrite = new();
        {
            _ = towrite.AppendLine($"          const applyResult = (");
            _ = towrite.AppendLine($"            data: {end.DTONoArray}");
            _ = towrite.AppendLine($"          ) => {{\r\n            updateCachedData(");
            _ = towrite.AppendLine($"              (");

            if (!end.IsSingle)
            {
                _ = towrite.AppendLine($"                draft: {(end.IsSingle ? end.DTONoArray : end.DTOArray)}");
            }

            _ = towrite.AppendLine("              ) => {");

            if (!end.IsSingle)
            {
                _ = towrite.AppendLine("                const foundIndex = draft.findIndex(");
                _ = towrite.AppendLine($"                  (x) => x.{end.IndexBy} === data.{end.IndexBy}");
                _ = towrite.AppendLine("                );");
                _ = towrite.AppendLine("");
                _ = towrite.AppendLine("                if (foundIndex === -1) {");
                _ = towrite.AppendLine("                  draft.push(data);");
                _ = towrite.AppendLine("                } else {");
                _ = towrite.AppendLine("                  draft[foundIndex] = data;");
                _ = towrite.AppendLine("                }\r\n");
            }
            if (end.SortBy == "")
            {
                _ = towrite.AppendLine($"                return {(end.IsSingle ? "data;" : "draft;")}");
            }
            else
            {
                _ = towrite.AppendLine("                return draft.sort((a, b) =>");
                _ = towrite.AppendLine($"                  a.{end.SortBy} < b.{end.SortBy} ? -1 : 1");
                _ = towrite.AppendLine("          );");
            }
            _ = towrite.AppendLine("        }");
            _ = towrite.AppendLine("            );");

            _ = towrite.AppendLine("                    };\r\n");

            _ = towrite.AppendLine("                    hubConnection.on(");
            _ = towrite.AppendLine($"                        '{updateHub}',");
            _ = towrite.AppendLine($"                        (data: {end.DTONoArray}) => {{");
            _ = towrite.AppendLine("                            applyResult(data);");
            _ = towrite.AppendLine("                        }\r\n                    );");

            return towrite.ToString();
        }
    }

    public static string GetUpdates(EndPoint end)
    {
        (_, string updateHubs, _) = Update(end);

        StringBuilder towrite = new();

        _ = towrite.AppendLine($"          const applyResults = (\r\n            data: {end.DTOArray}");
        _ = towrite.AppendLine($"          ) => {{\r\n            updateCachedData(\r\n              ( draft: {end.DTOArray}) => {{");
        _ = towrite.AppendLine($"                data.forEach(function (cn) {{");
        _ = towrite.AppendLine($"                  const foundIndex = draft.findIndex(");
        _ = towrite.AppendLine($"                    (x) => x.id === cn.id");
        _ = towrite.AppendLine($"                  );");
        _ = towrite.AppendLine($"                  if (foundIndex !== -1) {{");
        _ = towrite.AppendLine($"                    draft[foundIndex] = cn;");
        _ = towrite.AppendLine($"                  }} else {{");
        _ = towrite.AppendLine($"                    draft.push(cn);");
        _ = towrite.AppendLine($"                  }}");
        _ = towrite.AppendLine($"                }});");
        if (end.SortBy == "")
        {
            _ = towrite.AppendLine($"                return draft;");
        }
        else
        {
            _ = towrite.AppendLine($"                return draft.sort((a, b) =>");
            _ = towrite.AppendLine($"                  a.{end.SortBy} < b.{end.SortBy} ? -1 : 1");
            _ = towrite.AppendLine($"                );");
        }
        _ = towrite.AppendLine($"              }}");
        _ = towrite.AppendLine($"            );");
        _ = towrite.AppendLine($"          }};\r\n");

        _ = towrite.AppendLine("          hubConnection.on(");
        _ = towrite.AppendLine($"            '{updateHubs}',");
        _ = towrite.AppendLine($"            (data: {end.DTOArray}) => {{");
        _ = towrite.AppendLine("              applyResults(data);");
        _ = towrite.AppendLine("            }\r\n          );\r\n");

        return towrite.ToString();
    }

    private static (string updateHub, string updateHubs, string deleteHub) Update(EndPoint end)
    {
        if (end.IndexBy != "id")
        {
            string test = end.DTONoArray[(end.DTONoArray.IndexOf(".") + 1)..];
            end.NS = test + "es";
        }

        string nos = end.NS;

        if (!nos.EndsWith("us"))
        {
            nos = PluralizationProvider.Pluralize(end.NS);
            if (!nos.ToLower().EndsWith('s'))
            {
                nos += 's';
            }
        }

        string nop = end.NS;
        if (!nop.EndsWith("us"))
        {
            nop = PluralizationProvider.Singularize(end.NS);
        }
        if (!nop.EndsWith("us") && nop.EndsWith('s'))
        {
            nop = nop[..^1];
        }

        string noh = PluralizationProvider.Singularize(end.HubBroadCast);
        if (!noh.EndsWith("us") && noh.EndsWith('s'))
        {
            _ = noh[..^1];
        }

        string deleteHub = nop + "Delete";
        string updateHub = nop + "Update";
        string updateHubs = nos + "Update";
        return (updateHub, updateHubs, deleteHub);
    }
}
