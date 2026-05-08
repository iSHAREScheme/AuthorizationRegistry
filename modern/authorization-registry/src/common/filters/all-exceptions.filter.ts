import {
  ArgumentsHost,
  Catch,
  ExceptionFilter,
  HttpException,
  HttpStatus,
  Logger,
} from '@nestjs/common';
import { Request, Response } from 'express';

@Catch()
export class AllExceptionsFilter implements ExceptionFilter {
  private readonly logger = new Logger(AllExceptionsFilter.name);

  catch(exception: unknown, host: ArgumentsHost): void {
    const ctx = host.switchToHttp();
    const res = ctx.getResponse<Response>();
    const req = ctx.getRequest<Request>();

    if (exception instanceof HttpException) {
      const status = exception.getStatus();
      res.status(status).json(this.normalize(exception.getResponse()));
      return;
    }

    this.logger.error(`Unhandled error on ${req.method} ${req.url}`, exception as Error);
    res.status(HttpStatus.INTERNAL_SERVER_ERROR).json({ error: 'internal_server_error' });
  }

  private normalize(body: unknown): unknown {
    if (typeof body === 'string') return { error: body };
    return body;
  }
}
