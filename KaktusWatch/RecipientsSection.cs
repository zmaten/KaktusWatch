using System.Configuration;

namespace KaktusWatch
{
    public class RecipientsSection : ConfigurationSection
    {
        [ConfigurationProperty("Addresses")]
        public AddressesElementCollection Addresseses 
            => (AddressesElementCollection)this["Addresses"];
    }

    public class RecipientElement : ConfigurationElement
    {
        [ConfigurationProperty("address", IsKey = true, IsRequired = true)]
        public string Address 
            => (string)this["address"];
    }

    public class AddressesElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RecipientElement();
        }
        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecipientElement)element).Address;
        }
    }

}
