using ConferenceBot.Model;
using ConferenceBot.Model.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoicePrompt = Microsoft.Bot.Builder.Dialogs.ChoicePrompt;
using TextPrompt = Microsoft.Bot.Builder.Dialogs.TextPrompt;

namespace ConferenceBot.Dialog
{

    public interface ISessionEvaluationDialog : IDialogContinue
    {

    }
    public class SessionEvaluationDialog : DialogContainer, ISessionEvaluationDialog
    {
        public const string Id = "sessionEvaluationDialog";

        private const string SessionTitleKey = "sessionTitle";
        private const string SpeakerKey = "speaker";

        private readonly ISessionService _sessionService;

        public SessionEvaluationDialog(ISessionService sessionService) : base(Id)
        {
            _sessionService = sessionService;

            Dialogs.Add(Id, new WaterfallStep[]
            {
                // collection of delegates
                SpeakerNameStep,
                SpeakerConfirmStep,
                SessionStep,
                PresentationSkillsStep,
                ContentStep,
                FeedbackStep,
                DoneStep
            });

            //Define the prompts used in this conversation flow.
            Dialogs.Add("text", new TextPrompt());

            var pickList = new ChoicePrompt(Culture.English);
            pickList.Style = ListStyle.SuggestedAction;
            Dialogs.Add("pickList", pickList);
            Dialogs.Add("confirm", new Microsoft.Bot.Builder.Dialogs.ConfirmPrompt(Culture.English));

        }

        private async Task SpeakerNameStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
           
            var techBashLuisRecognizer = args["RecognizerResult"] as TechBashLuisRecognizerResult;
            if (techBashLuisRecognizer.Entities.SpeakerFullName?.Any() == true)
            {
                // load the prompted values and skip this step
                dc.ActiveDialog.State["speakerInput"] = techBashLuisRecognizer.Entities.SpeakerFullName.FirstOrDefault();
                await next(args);
                return;
            }

            if (techBashLuisRecognizer.Entities.speaker?.Any() == true)
            {
                // load the prompted values and skip this step
                dc.ActiveDialog.State["speakerInput"] = techBashLuisRecognizer.Entities.speaker.FirstOrDefault();
                await next(args);
                return;
            }

            dc.ActiveDialog.State = new Dictionary<string, object>();
            await dc.Prompt("text", "Which speaker would you like to evaluate?",
                new PromptOptions { RetryPromptString = "Please provide a speaker name (first, last, or both)." });
        }

        private async Task SpeakerConfirmStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            if (args.Keys.Contains("Value"))
            {
                dc.ActiveDialog.State["speakerInput"] = args["Value"];
            }
 
            var speakers = _sessionService.GetSpeakerNames(dc.ActiveDialog.State["speakerInput"].ToString());

            var choices = new List<Choice>();
            foreach(var speaker in speakers.OrderBy(s => s))
            {
                choices.Add(new Choice { Value = speaker });

            }

            await dc.Prompt("pickList", "Please confirm the speaker choice:", new ChoicePromptOptions() { Choices = choices });
        }

        private async Task SessionStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {

            // get speaker name and look up sessions
            var choiceResult = args as ChoiceResult;
            dc.ActiveDialog.State["speakerName"] = choiceResult.Value.Value;

            var sessions = _sessionService.GetSessions(choiceResult.Value.Value);

            var choices = new List<Choice>();
            foreach (var session in sessions.OrderBy(s => s.Title))
            {
                choices.Add(new Choice { Value = session.Title });
            }

            await dc.Prompt("pickList", "Which session would you like to evaluate? ", new ChoicePromptOptions() { Choices = choices });
        }

        private async Task PresentationSkillsStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var choiceResult = args as ChoiceResult;
            dc.ActiveDialog.State["sessionName"] = choiceResult.Value.Value;


            var choices = new List<Choice>();
            choices.Add(new Choice { Value = "5" });
            choices.Add(new Choice { Value = "4" });
            choices.Add(new Choice { Value = "3" });
            choices.Add(new Choice { Value = "2" });
            choices.Add(new Choice { Value = "1" });

            await dc.Prompt("pickList", "How do you rate the speakers presentation skills (5 is best)? ", new ChoicePromptOptions() { Choices = choices });
        }

        private async Task ContentStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var choices = new List<Choice>();
            choices.Add(new Choice { Value = "5" });
            choices.Add(new Choice { Value = "4" });
            choices.Add(new Choice { Value = "3" });
            choices.Add(new Choice { Value = "2" });
            choices.Add(new Choice { Value = "1" });

            await dc.Prompt("pickList", "How do you rate the content presented (5 is best)? ", new ChoicePromptOptions() { Choices = choices });
        }
        private async Task FeedbackStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Prompt("text", "Please provide a comment to the speaker.",
                new PromptOptions { RetryPromptString = "Please type your comment." });
        }
        private async Task DoneStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            string message;
            string speaker = dc.ActiveDialog.State["speakerName"].ToString();
            string session = dc.ActiveDialog.State["sessionName"].ToString();

            message = $"Thanks for providing feedback for {session} presented by {speaker}.";

            await dc.Context.SendActivity(message);
            await dc.End();

        }
    }
}
