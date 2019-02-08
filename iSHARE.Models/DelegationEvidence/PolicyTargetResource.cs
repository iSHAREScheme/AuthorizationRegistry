using System.Collections.Generic;

namespace iSHARE.Models.DelegationEvidence
{
    public class PolicyTargetResource
    {
        public string Type { get; set; }
        public List<string> Identifiers { get; set; }
        public List<string> Attributes { get; set; }

        public bool HasIdentifier(string identifier) =>
            Identifiers.Has(identifier);

        public bool HasAnyIdentifiers(IEnumerable<string> identifiers) =>
            Identifiers.HasAny(identifiers);

        public bool HasAttribute(string attribute)
        {
            if (!AttributesSet)
            {
                return true; // permit
            }
            return Attributes.Has(attribute);
        }

        public bool HasAnyAttributes(IEnumerable<string> attributes)
        {
            if (!AttributesSet)
            {
                return true; // permit
            }
            return Attributes.HasAny(attributes);
        }

        public bool HasSameType(PolicyTargetResource target) => Type == target.Type;

        // the policy did not specify Actions so when the mask requires access it should return true
        private bool AttributesSet => Attributes.HasElements();
    }
}
