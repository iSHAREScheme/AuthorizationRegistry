using System;

namespace iSHARE.Abstractions.Json
{
    internal static class JsonSchemaEnumToFileNameMapper
    {
        public static string Map(JsonSchema jsonSchema)
        {
            switch (jsonSchema)
            {
                case JsonSchema.DelegationEvidence:
                    return "delegationEvidenceSchema.json";
                case JsonSchema.DelegationMask:
                    return "delegationMaskSchema.json";
                case JsonSchema.Policy:
                    return "policySchema.json";
                default:
                    throw new ArgumentOutOfRangeException(nameof(jsonSchema), jsonSchema, "Unrecognized param.");
            }
        }
    }
}
