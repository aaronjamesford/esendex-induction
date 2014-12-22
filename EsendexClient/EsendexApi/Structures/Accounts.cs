using System.Collections.Generic;
using System.Xml.Serialization;

namespace EsendexApi.Structures
{
    [XmlRoot("accounts", Namespace = "http://api.esendex.com/ns/")]
    internal class Accounts
    {
        public List<Account> AccountDetails { get; set; }
    }
}