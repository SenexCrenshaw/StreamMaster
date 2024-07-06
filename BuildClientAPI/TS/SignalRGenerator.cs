using System.Text;

public static class SignalRGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        methods = methods.Where(a => a.IsGet).ToList();//.ReturnEntityType.EndsWith("Dto")).ToList();

        content.Append(AddImports(methods));
        content.Append(GenerateContextAndInterfaces());
        content.Append(GenerateProvider(methods));

        content.AppendLine("  return <SignalRContext.Provider value={signalRService}>{children}</SignalRContext.Provider>;");
        content.AppendLine("}");

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string GenerateProvider(List<MethodDetails> methods)
    {

        StringBuilder content = new();
        content.AppendLine("export const SignalRProvider: React.FC<SignalRProviderProps> = ({ children }) => {");
        content.AppendLine("  const smMessages = useSMMessages();");
        content.AppendLine("  const signalRService = SignalRService.getInstance();");

        foreach (MethodDetails method in methods.Where(a => a.IsGet))
        {
            content.AppendLine($"  const {method.Name.ToCamelCase()} = use{method.Name}();");
        }

        content.AppendLine();
        content.AppendLine(AddMessage());
        content.AppendLine(DataRefresh(methods));
        content.AppendLine(SetField(methods));
        content.AppendLine(ClearByTag(methods));
        content.AppendLine(Connections());
        content.AppendLine(UseEffect());

        return content.ToString();
    }
    private static string UseEffect()
    {
        StringBuilder content = new();

        // Append lines for the useEffect hook
        content.AppendLine("useEffect(() => {");
        content.AppendLine("    const handleConnect = () => {");
        content.AppendLine("      // setIsConnected(true);");
        content.AppendLine("      CheckAndAddConnections();");
        content.AppendLine("    };");
        content.AppendLine("    const handleDisconnect = () => {");
        content.AppendLine("      // setIsConnected(false);");
        content.AppendLine("      RemoveConnections();");
        content.AppendLine("    };");
        content.AppendLine("");
        content.AppendLine("    // Add event listeners");
        content.AppendLine("    signalRService.addEventListener('signalr_connected', handleConnect);");
        content.AppendLine("    signalRService.addEventListener('signalr_disconnected', handleDisconnect);");
        content.AppendLine("");
        content.AppendLine("    // Remove event listeners on cleanup");
        content.AppendLine("    return () => {");
        content.AppendLine("      signalRService.removeEventListener('signalr_connected', handleConnect);");
        content.AppendLine("      signalRService.removeEventListener('signalr_disconnected', handleDisconnect);");
        content.AppendLine("    };");
        content.AppendLine("  }, [CheckAndAddConnections, RemoveConnections, signalRService]);");

        return content.ToString();
    }

    private static string Connections()
    {
        StringBuilder content = new();
        content.AppendLine("  const RemoveConnections = useCallback(() => {");
        content.AppendLine("    console.log('SignalR RemoveConnections');");
        content.AppendLine("    signalRService.removeListener('ClearByTag', clearByTag);");
        content.AppendLine("    signalRService.removeListener('SendMessage', addMessage);");
        content.AppendLine("    signalRService.removeListener('DataRefresh', dataRefresh);");
        content.AppendLine("    signalRService.removeListener('SetField', setField);");
        content.AppendLine("  }, [addMessage, clearByTag, dataRefresh, setField, signalRService]);");
        content.AppendLine();
        content.AppendLine("  const CheckAndAddConnections = useCallback(() => {");
        content.AppendLine("    console.log('SignalR AddConnections');");
        content.AppendLine("    signalRService.addListener('ClearByTag', clearByTag);");
        content.AppendLine("    signalRService.addListener('SendMessage', addMessage);");
        content.AppendLine("    signalRService.addListener('DataRefresh', dataRefresh);");
        content.AppendLine("    signalRService.addListener('SetField', setField);");
        content.AppendLine("  }, [addMessage, clearByTag, dataRefresh, setField, signalRService]);");

        return content.ToString();
    }

    private static string ClearByTag(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("  const clearByTag = useCallback((data: ClearByTag): void => {");
        content.AppendLine("    const { Entity, Tag } = data;");
        foreach (MethodDetails? method in methods.Where(a => a.IsGet))
        {
            content.AppendLine($"    if (Entity === '{method.Name}') {{");
            content.AppendLine($"      {method.Name.ToCamelCase()}.ClearByTag(Tag)");
            content.AppendLine("      return;");
            content.AppendLine("    }");
            deps.Add(method.Name.ToCamelCase());
        }
        //content.AppendLine("    console.log('ClearByTag', Entity, Tag);");
        content.AppendLine("  }");
        content.AppendLine(",");
        string depString = string.Join(",", deps);
        content.AppendLine($"    [{depString}]");
        content.AppendLine("  );");



        return content.ToString();
    }

    private static string SetField(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("  const setField = useCallback(");
        content.AppendLine("    (fieldDatas: FieldData[]): void => {");
        content.AppendLine("      fieldDatas.forEach((fieldData) => {");
        foreach (MethodDetails? method in methods.Where(a => a.IsGet))
        {
            content.AppendLine($"        if (fieldData.Entity === '{method.Name}') {{");
            content.AppendLine($"          {method.Name.ToCamelCase()}.SetField(fieldData)");
            content.AppendLine("          return;");
            content.AppendLine("        }");
            deps.Add(method.Name.ToCamelCase());
        }
        Dictionary<string, List<MethodDetails>> keyValuePairs = methods.Where(a => a.IsGet).GroupBy(a => a.NamespaceName).ToDictionary(a => a.Key, a => a.ToList());


        foreach (KeyValuePair<string, List<MethodDetails>> namespaceName in keyValuePairs)
        {
            content.AppendLine($"      if ( fieldData.Entity === '{namespaceName.Key}') {{");
            foreach (MethodDetails method in namespaceName.Value)
            {

                content.AppendLine($"        {method.Name.ToCamelCase()}.SetField(fieldData);");


            }
            content.AppendLine("        return;");
            content.AppendLine("      }");
        }

        content.AppendLine("      });");
        content.AppendLine("    },");



        string depString = string.Join(",", deps);
        content.AppendLine($"    [{depString}]");
        content.AppendLine("  );");
        return content.ToString();
    }


    private static string DataRefresh(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("  const dataRefresh = useCallback(");
        content.AppendLine("    (entity: string): void => {");
        foreach (MethodDetails? method in methods.Where(a => a.IsGet))
        {
            content.AppendLine($"      if (entity === '{method.Name}') {{");
            content.AppendLine($"        {method.Name.ToCamelCase()}.SetIsForced(true);");
            content.AppendLine("        return;");
            content.AppendLine("      }");
            deps.Add(method.Name.ToCamelCase());
        }

        Dictionary<string, List<MethodDetails>> keyValuePairs = methods.Where(a => a.IsGet && (a.IsGetPaged || a.ParameterNames?.Length == 0)).GroupBy(a => a.NamespaceName).ToDictionary(a => a.Key, a => a.ToList());


        foreach (KeyValuePair<string, List<MethodDetails>> namespaceName in keyValuePairs)
        {
            content.AppendLine($"      if (entity === '{namespaceName.Key}') {{");
            foreach (MethodDetails method in namespaceName.Value)
            {
                content.AppendLine($"        {method.Name.ToCamelCase()}.SetIsForced(true);");
            }
            content.AppendLine("        return;");
            content.AppendLine("      }");
        }

        string depString = string.Join(",", deps);
        content.AppendLine("    },");
        content.AppendLine($"    [{depString}]");
        content.AppendLine("  );");
        return content.ToString();
    }

    private static string AddMessage()
    {
        StringBuilder content = new();
        content.AppendLine("  const addMessage = useCallback(");
        content.AppendLine("    (entity: SMMessage): void => {");
        content.AppendLine("      smMessages.AddMessage(entity);");
        content.AppendLine("    },");
        content.AppendLine("    [smMessages]");
        content.AppendLine("  );");
        return content.ToString();
    }


    private static string GenerateContextAndInterfaces()
    {
        StringBuilder content = new();
        content.AppendLine("const SignalRContext = createContext<SignalRService | undefined>(undefined);");
        content.AppendLine("");
        content.AppendLine("export const useSignalRService = () => {");
        content.AppendLine("  const context = useContext(SignalRContext);");
        content.AppendLine("  if (context === undefined) {");
        content.AppendLine("    throw new Error('useSignalRService must be used within a SignalRProvider');");
        content.AppendLine("  }");
        content.AppendLine("  return context;");
        content.AppendLine("};");
        content.AppendLine("");
        content.AppendLine("interface SignalRProviderProps {");
        content.AppendLine("  children: ReactNode;");
        content.AppendLine("}");
        return content.ToString();
    }

    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        content.AppendLine("import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';");
        content.AppendLine("import SignalRService from './SignalRService';");

        foreach (MethodDetails method in methods.Where(a => a.IsGet))
        {
            content.AppendLine($"import use{method.Name} from '@lib/smAPI/{method.NamespaceName}/use{method.Name}';");
        }

        content.AppendLine("import { useSMMessages } from '@lib/redux/hooks/useSMMessages';");
        content.AppendLine("import { ClearByTag, FieldData, SMMessage } from '@lib/smAPI/smapiTypes';");

        content.AppendLine();
        return content.ToString();
    }

}
