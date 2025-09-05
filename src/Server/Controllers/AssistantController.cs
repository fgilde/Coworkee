using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using GptInvoke.Contracts;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using lib.Coworkee.Application.Common.Models;
using System.Collections.Generic;
using OllamaSharp;


namespace Coworkee.Server.Controllers
{


    public class AssistantController : BaseApiController<AssistantController>
    {
        [HttpPost(nameof(AskOllama))]
        public async Task<ActionResult> AskOllama(string question)
        {
            var res = new StringBuilder();
            var ollama = Get<OllamaApiClient>();
            await foreach (var stream in ollama.GenerateAsync(question))
                res.Append(stream.Response);
            return Ok(res.ToString());
        }

        [Authorize]
        [HttpPost(nameof(Ask))]
        public async Task<IActionResult> Ask(string prompt, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsk(prompt, null, cancellationToken);
        }

        [Authorize]
        [HttpPost(nameof(AskWithHistory))]
        public async Task<IActionResult> AskWithHistory(IList<AssistantCommandDto> commands, CancellationToken cancellationToken = default)
        {
            if (commands == null || !commands.Any())
                return BadRequest("Command list is empty or null.");


            var lastMessage = commands.Last().Message;
            commands.RemoveAt(commands.Count - 1);
            return await ExecuteAsk(lastMessage, commands, cancellationToken);
        }

        private async Task<IActionResult> ExecuteAsk(string prompt, IList<AssistantCommandDto> history, CancellationToken cancellationToken)
        {
            var responseBodyFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>();
            var writer = responseBodyFeature.Writer;
            var invoker = Get<IAIActionInvoker>();

            if (history?.Any() == true)
                invoker.SetHistory(history.Select(dto => new AIMessage { Role = dto.Role, Content = dto.Message }));

            var result = await invoker.PromptAsync(prompt, async s =>
            {
                if (s != null)
                {
                    try
                    {
                        await writer.WriteAsync(Encoding.UTF8.GetBytes(s), cancellationToken);
                        await writer.FlushAsync(cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Error writing to response stream");
                    }
                }
            }, cancellationToken);

            await responseBodyFeature.Writer.CompleteAsync();
            return Ok(result);
        }
    }
}