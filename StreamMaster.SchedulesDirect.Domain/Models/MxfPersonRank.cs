using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models
{
    public class MxfPersonRank
    {
        private string _person;

        public string Name => _mxfPerson.Name;

        [XmlIgnore] private readonly MxfPerson _mxfPerson;

        public MxfPersonRank(MxfPerson person)
        {
            _mxfPerson = person;
        }
        public MxfPersonRank() { }

        /// <summary>
        /// A reference to the id value of the Person element.
        /// </summary>
        [XmlAttribute("person")]
        public string Person
        {
            get => _person ?? _mxfPerson.Id;
            set { _person = value; }
        }

        /// <summary>
        /// Used to sort the names that are displayed.
        /// Lower numbers are displayed first.
        /// </summary>
        [XmlAttribute("rank")]
        [DefaultValue(0)]
        public int Rank { get; set; }

        /// <summary>
        /// The role an actor plays
        /// </summary>
        [XmlAttribute("character")]
        public string Character { get; set; }
    }
}