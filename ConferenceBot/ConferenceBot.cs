using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceBot.Dialog;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ConferenceBot.Model.Luis;

namespace ConferenceBot
{
    public class ConferenceBot : IBot
    {
        private const string MainMenuId = "mainMenu";
        private DialogSet _dialogs { get; } 

        private readonly IScheduleDialog _scheduleDialog;
        private readonly ISessionEvaluationDialog _sessionEvaluationDialog;
        private readonly ITicketInfoDialog _ticketInfoDialog;
        private readonly IConfiguration _configuration;
        public ConferenceBot(IScheduleDialog scheduleDialog, ITicketInfoDialog ticketInfoDialog, ISessionEvaluationDialog sessionEvaluationDialog, IConfiguration configuration)
        {
            _scheduleDialog = scheduleDialog;
            _ticketInfoDialog = ticketInfoDialog;
            _sessionEvaluationDialog = sessionEvaluationDialog;

            _configuration = configuration;


            _dialogs = new DialogSet();

            _dialogs.Add("schedule", _scheduleDialog);
            _dialogs.Add("ticketInfo", _ticketInfoDialog);
            _dialogs.Add("sessionEvaluation", _sessionEvaluationDialog);
        }



        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type is ActivityTypes.Message)
            {
                var conversationState = context.GetConversationState<Dictionary<string, object>>();

                // Establish dialog state from the conversation state.
                var dc = _dialogs.CreateContext(context, conversationState);

                // Continue any current dialog.
                await dc.Continue();

                // Every turn sends a response, so if no response was sent,
                // then there no dialog is currently active.
                if (!context.Responded)
                {
                    // if this isn't a response, call LUIS to get intent.
                    var luisResult = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                    TechBashLuisRecognizerResult recognizerResult = new TechBashLuisRecognizerResult();
                    recognizerResult.Convert(luisResult);
                    
                    var dialogArgs = new Dictionary<string, object>();
                    

                    switch (recognizerResult.TopIntent().intent)
                    {
                        case TechBashLuisRecognizerResult.Intent.None:
                            QnAMakerEndpoint endpoint = new QnAMakerEndpoint();
                            endpoint.KnowledgeBaseId = "UPDATE_THIS";
                            endpoint.EndpointKey = "UPDATE_THIS";
                            endpoint.Host = "https://UPDATE_THIS.azurewebsites.net/qnamaker";

                            QnAMaker qnaMaker = new QnAMaker(endpoint);
                            var answers = await qnaMaker.GetAnswers(context.Activity.Text.Trim()).ConfigureAwait(false);
                            if (answers.Any())
                            {
                                await context.SendActivity(answers.First().Answer);
                            }
                            else
                            {
                                await context.SendActivity($"Couldn't find an answer.");
                            }

                            break;
                        case TechBashLuisRecognizerResult.Intent.Evaluation:
                            dialogArgs.Add("RecognizerResult", recognizerResult);
                            await dc.Begin("sessionEvaluation", dialogArgs);
                            break;
                        case TechBashLuisRecognizerResult.Intent.Schedule:
                            dialogArgs.Add("RecognizerResult", recognizerResult);
                            await dc.Begin("schedule", dialogArgs);
                            break;
                        case TechBashLuisRecognizerResult.Intent.Registration:
                            dialogArgs.Add("RecognizerResult", recognizerResult);
                            await dc.Begin("ticketInfo", dialogArgs);
                            break;
                        //case "Help":
                        //    await context.SendActivity("<here's some help>");
                        //    break;
                        //case "Cancel":
                        //    // Cancel the process.
                        //    await context.SendActivity("<cancelling the process>");
                        //    break;
                        default:
                            // Received an intent we didn't expect, so send its name and score.
                            await context.SendActivity($"Intent: {recognizerResult.TopIntent().intent} ({recognizerResult.TopIntent().score}).");
                            break;
                    }
                }
            }
        }
    }    
}
