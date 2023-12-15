using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models
{
    public class MxfProvider
    {
        private string _id;

        [XmlIgnore] public int Index { get; set; }

        /// <summary>
        /// An ID that is unique to the document and defines this element.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get => _id ?? $"provider{Index}";
            set => _id = value;
        }

        /// <summary>
        /// The name of the supplier of the listings.
        /// The maximum length is 255 characters.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// More information about the supplier of the listings.
        /// The maximum length is 255 characters.
        /// </summary>
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// The copyright notice from the supplier of the listings.
        /// The maximum length is 1024 characters.
        /// </summary>
        [XmlAttribute("copyright")]
        public string Copyright { get; set; }

        /// <summary>
        /// The status of the Stream Master MXF file creation.
        /// </summary>
        [XmlAttribute("status")]
        [DefaultValue(0)]
        public int Status { get; set; }
    }
}