using System.Text;

public static class SignalRGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        AddImports(content, methods);
        content.Append(GenerateContextAndInterfaces());
        content.Append(GenerateProvider(methods));

        content.AppendLine("  return <SignalRContext.Provider value={signalRService}>{children}</SignalRContext.Provider>;");
        content.AppendLine("}");

        foreach (MethodDetails method in methods.Where(a => a.Name.StartsWith("Get")))
        {

            //content.Append(GenerateInterfaces(method));
            //content.Append(GenerateHookContent(method));

        }
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

        foreach (MethodDetails method in methods.Where(a => a.IsGetPaged))
        {
            content.AppendLine($"  const {method.Name.ToCamelCase()} = use{method.Name}();");
        }

        content.AppendLine();
        content.AppendLine(AddMessage());
        content.AppendLine(DataRefresh(methods));
        content.AppendLine(SetField(methods));
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
        content.AppendLine("    signalRService.removeListener('SendMessage', addMessage);");
        content.AppendLine("    signalRService.removeListener('DataRefresh', dataRefresh);");
        content.AppendLine("    signalRService.removeListener('SetField', setField);");
        content.AppendLine("  }, [addMessage, dataRefresh, setField, signalRService]);");
        content.AppendLine();
        content.AppendLine("  const CheckAndAddConnections = useCallback(() => {");
        content.AppendLine("    console.log('SignalR AddConnections');");
        content.AppendLine("    signalRService.addListener('SendMessage', addMessage);");
        content.AppendLine("    signalRService.addListener('DataRefresh', dataRefresh);");
        content.AppendLine("    signalRService.addListener('SetField', setField);");
        content.AppendLine("  }, [addMessage, dataRefresh, setField, signalRService]);");

        return content.ToString();
    }

    private static string SetField(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("  const setField = useCallback(");
        content.AppendLine("    (fieldDatas: FieldData[]): void => {");
        content.AppendLine("      fieldDatas.forEach((fieldData) => {");
        foreach (MethodDetails? method in methods.Where(a => a.IsGetPaged))
        {
            content.AppendLine($"        if (fieldData.entity === '{method.ReturnEntityType}') {{");
            content.AppendLine($"          {method.Name.ToCamelCase()}.set{method.Name}Field(fieldData);");
            content.AppendLine("          return;");
            content.AppendLine("        }");
            deps.Add(method.Name.ToCamelCase());
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
        foreach (MethodDetails? method in methods.Where(a => a.IsGetPaged))
        {
            content.AppendLine($"      if (entity === '{method.ReturnEntityType}') {{");
            content.AppendLine($"        {method.Name.ToCamelCase()}.refresh{method.Name}();");
            content.AppendLine("        return;");
            content.AppendLine("      }");
            deps.Add(method.Name.ToCamelCase());
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

    private static void AddImports(StringBuilder content, List<MethodDetails> methods)
    {
        content.AppendLine("import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';");
        content.AppendLine("import SignalRService from './SignalRService';");
        content.AppendLine("import { SMMessage } from './SMMessage';");
        content.AppendLine("import { FieldData } from '@lib/apiDefs';");
        content.AppendLine("import { useSMMessages } from '@lib/redux/slices/messagesSlice';");

        foreach (MethodDetails method in methods.Where(a => a.IsGetPaged))
        {
            content.AppendLine($"import use{method.Name} from '@lib/smAPI/{method.NamespaceName}/use{method.Name}';");
        }

        content.AppendLine();
    }

}
