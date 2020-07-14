namespace iSHARE.Abstractions.Json.Interfaces
{
    public interface IJsonSchemaStore
    {
        IJsonSchemaValidator GetSchema(JsonSchema jsonSchema);
    }
}
