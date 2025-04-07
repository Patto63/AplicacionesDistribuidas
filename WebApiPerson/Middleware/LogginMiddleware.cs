﻿using Microsoft.AspNetCore.Http;
namespace WebApiPerson.Middleware
{
    public class LogginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogginMiddleware> _logger;

        public LogginMiddleware(RequestDelegate next, ILogger<LogginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke (HttpContext context)
        {
            _logger.LogInformation
                ("-> Request: {method} {url}", context.Request?.Method, context.Request?.Path.Value);

            await _next(context);

            _logger.LogInformation("<- Response: {statusCode}", context.Response?.StatusCode);
        }
    }
}
