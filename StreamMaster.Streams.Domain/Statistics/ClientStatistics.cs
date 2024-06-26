using Reinforced.Typings.Attributes;

namespace StreamMaster.Streams.Domain.Statistics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class ClientStatistics : BaseStatistics
{
    public int Clients { get; set; }
    public void DecrementClient()
    {
        --Clients;
    }

    public void IncrementClient()
    {
        ++Clients;
    }
}
