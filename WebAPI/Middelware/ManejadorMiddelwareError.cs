using System;
using System.Net;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebAPI.Middelware
{
    public class ManejadorMiddelwareError
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorMiddelwareError> _logger;

        public ManejadorMiddelwareError(RequestDelegate next, ILogger<ManejadorMiddelwareError> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ManejadorExcepcionAsincrono(context, ex, _logger);

            }
        }

        private async Task ManejadorExcepcionAsincrono(HttpContext context, Exception ex,
                ILogger<ManejadorMiddelwareError> logger)
        {
            object errores = null;
            switch (ex)
            {
                case ManejadorExcepcion me:
                    logger.LogError(ex, "Manejador Error");
                    errores = me.Errores;
                    context.Response.StatusCode = (int)me.Codigo;
                    break;

                case Exception e:
                    logger.LogError(ex, "Error del Servidor");
                    errores = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            if (errores != null)
            {
                var resultados = JsonConvert.SerializeObject(new{errores});
                await context.Response.WriteAsync(resultados);
            }
        }
    }
}