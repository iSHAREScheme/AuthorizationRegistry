namespace iSHARE.Abstractions.Json.Interfaces
{
    public interface IJsonSchemaValidator
    {
        JsonSchemaValidationResult Validate(string json);
    }
}
