﻿using Marte.Exploracao.Dominio.Contratos;
using Marte.Exploracao.Dominio.ObjetoDeValor;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Marte.Api.Controllers
{
    public class BaseController : ApiController
    {
        public IEspecificacaoDeNegocio EspecificacaoDeNegocio;
        public HttpResponseMessage ResponseMessage;

        public BaseController()
        {
            this.ResponseMessage = new HttpResponseMessage();
            EspecificacaoDeNegocio = new EspecificacaoDeNegocio();
        }

        public Task<HttpResponseMessage> CreateResponse(HttpStatusCode code, object result)
        {
            var retorno = result;

            if (EspecificacaoDeNegocio.HouveViolacao())
            {
                EspecificacaoDeNegocio.RegrasDeNegocio.ToList().ForEach(item => retorno += item.Informacao);
                ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest, new { errors = retorno });
            }
            else
            {
                ResponseMessage = Request.CreateResponse(code, retorno);
            }

            return Task.FromResult<HttpResponseMessage>(ResponseMessage);
        }
    }
}
