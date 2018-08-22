using ConferenceBot.Adapters;
using ConferenceBot.Model;
using ConferenceBot.Model.Luis;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace ConferenceBot.Dialog
{
    public interface IScheduleDialog : IDialog, IDialogContinue
    {
    
    }
    public class ScheduleDialog : DialogContainer, IScheduleDialog
    {
        public const string Id = "scheduleDialog";

        private readonly ICardAdapter _cardAdapter;
        private readonly ISessionService _sessionService;

        public ScheduleDialog(ICardAdapter cardAdapter, ISessionService sessionService) : base(Id)
        {
            _cardAdapter = cardAdapter;
            _sessionService = sessionService;

            Dialogs.Add(Id, new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    var dialogState = dc.ActiveDialog.State as IDictionary<string, object>;

                    var techBashLuisRecognizer = args["RecognizerResult"] as TechBashLuisRecognizerResult;

                    // figure out speaker full name (seems like a bug)
                    var speakerNames = techBashLuisRecognizer.Entities.speaker;
                    if(techBashLuisRecognizer.Entities.SpeakerFullName?[0].speaker.Length > 0)
                    {
                        speakerNames = techBashLuisRecognizer.Entities.SpeakerFullName[0].speaker;
                    }

                    // query the session list for topic, speaker, date, etc
                    var selectedSessions = _sessionService.GetSessions(techBashLuisRecognizer.Entities.topic,
                        speakerNames, 
                        techBashLuisRecognizer.Entities.datetime);
                    
                    List<Attachment> cards = new List<Attachment>();
                    foreach (var session in selectedSessions)
                    {
                        var attachment = _cardAdapter.ToCard(session);
                        cards.Add(attachment);
                    }

                    var activity = MessageFactory.Attachment(cards);
                    await dc.Context.SendActivity(activity);

                    dc.ActiveDialog.State = new Dictionary<string, object>(); // clear the dialog state 
                    await dc.End();
                }
            });

            // Define the prompts used in this conversation flow.
            Dialogs.Add("topicPrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }

        


    }
}
