using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NoteBoard
{
    /// <summary>
    /// Класс-хранилище всех данных
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(BuyNow))]
    [XmlInclude(typeof(DoNow))]
    [XmlInclude(typeof(BuyLater))]
    public class NBData
    {
        public BuyNowCollection BuyNowCol { get; set; }
        public DoNowCollection DoNowCol { get; set; }
        public BuyLaterCollection BuyLaterCol { get; set; }
    }
}
