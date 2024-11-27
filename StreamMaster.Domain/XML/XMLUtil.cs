using System.Globalization;

using StreamMaster.Domain.Comparer;

using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.Domain.XML
{
    public static class XMLUtil
    {
        public static XMLTV NewXMLTV => new()
        {
            Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            SourceInfoName = "Stream Master",
            GeneratorInfoName = "Stream Master",
            GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            Channels = [],
            Programs = []
        };

        public static void SortXmlTv(this XMLTV xmlTv)
        {
            xmlTv.Channels = [.. xmlTv.Channels.OrderBy(c => c.Id, new NumericStringComparer())];
            xmlTv.Programs = [.. xmlTv.Programs.OrderBy(p => p.Channel).ThenBy(p => p.StartDateTime)];
        }
    }
}
