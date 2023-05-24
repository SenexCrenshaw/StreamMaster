using System.Text;

namespace StreamMasterDomain.Common;

public partial class GetStreamGroupEPGHandler
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}