import { ArgumentMetadata, BadRequestException, Injectable, PipeTransform } from '@nestjs/common';
import Ajv, { ErrorObject, ValidateFunction } from 'ajv';
import addFormats from 'ajv-formats';

const ajv = new Ajv({ allErrors: true, strict: false });
addFormats(ajv);

const cache = new WeakMap<object, ValidateFunction>();

/**
 * Validates the incoming body against an iSHARE JSON Schema definition.
 * Mirrors the legacy iSHARE.Api.Filters.JsonSchemaValidateAttribute behaviour
 * (validate before model binding, return 400 with the AJV errors).
 */
@Injectable()
export class JsonSchemaPipe implements PipeTransform<unknown, unknown> {
  constructor(private readonly schema: object) {}

  transform(value: unknown, _metadata: ArgumentMetadata): unknown {
    let validate = cache.get(this.schema);
    if (!validate) {
      validate = ajv.compile(this.schema);
      cache.set(this.schema, validate);
    }
    if (!validate(value)) {
      throw new BadRequestException({
        error: 'invalid_request',
        details: (validate.errors ?? []).map(formatError),
      });
    }
    return value;
  }
}

function formatError(err: ErrorObject): { path: string; message: string } {
  return { path: err.instancePath || err.schemaPath, message: err.message ?? 'invalid' };
}
