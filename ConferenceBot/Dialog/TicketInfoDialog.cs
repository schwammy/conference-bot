using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChoicePrompt = Microsoft.Bot.Builder.Dialogs.ChoicePrompt;
using TextPrompt = Microsoft.Bot.Builder.Dialogs.TextPrompt;

namespace ConferenceBot.Dialog
{
    public interface ITicketInfoDialog : IDialog, IDialogContinue
    {

    }

    public class TicketInfoDialog : DialogContainer, ITicketInfoDialog
    {
        public const string Id = "ticketInfoDialog";

        public TicketInfoDialog() : base(Id)
        {

            Dialogs.Add(Id, new WaterfallStep[]
            {
                QuantityPrompt,
                TicketTypePrompt,
                WorkshopPrompt,
                ConfirmationPrompt,
                DonePrompt

            });

            //Define the prompts used in this conversation flow.
            Dialogs.Add("number", new TextPrompt());

            var pickList = new ChoicePrompt(Culture.English);
            pickList.Style = ListStyle.SuggestedAction;
            
            Dialogs.Add("pickList", pickList);

            Dialogs.Add("confirm", new Microsoft.Bot.Builder.Dialogs.ConfirmPrompt(Culture.English));
        }

        private async Task QuantityPrompt(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            dc.ActiveDialog.State = new Dictionary<string, object>();
            await dc.Prompt("number", "How many tickets would you like?",
                new PromptOptions {RetryPromptString = "Please provide a number"});
        }

        private async Task TicketTypePrompt(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            dc.ActiveDialog.State["quantity"] = args["Value"];


            var choices = new List<Choice>();
            choices.Add(new Choice { Value = "3 Day Conference Ticket", Synonyms = new List<string> { "3 Day", "conference" } });
            choices.Add(new Choice { Value = "3 Day Conference plus Workshop", Synonyms = new List<string> { "4 Day", "workshop", "full" } });
            choices.Add(new Choice { Value = "1 Day", Synonyms = new List<string> { "1", "single" } });

            await dc.Prompt("pickList", "What kind of ticket: ", new ChoicePromptOptions() { Choices = choices});
        }

        private async Task WorkshopPrompt(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {

            var choiceResult = args as ChoiceResult;
            if (choiceResult != null)
            {
                dc.ActiveDialog.State["ticketType"] = choiceResult.Value.Value;
                await dc.Context.SendActivity("you picked: " + choiceResult.Value.Value);
            }

            var choices = new List<Choice>();
            choices.Add(new Choice { Value = "Get Started Building a Web Application with ASP.NET Core", Synonyms = new List<string> { "asp.net", "asp.net core", "core", "web" } });
            choices.Add(new Choice { Value = "DevOps with Visual Studio Team Services (VSTS)", Synonyms = new List<string>{"VSTS", "devops", "team services" } });
            choices.Add(new Choice { Value = "Azure for Developers", Synonyms = new List<string> { "azure", "azure for dev" } });
            choices.Add(new Choice { Value = "Deploying Multi-OS applications with Docker EE", Synonyms = new List<string> { "docker", "docker ee" } });

            await dc.Prompt("pickList", "Which workshop would you like to attend?", new ChoicePromptOptions() { Choices = choices });

        }

        private async Task ConfirmationPrompt(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {

            var choiceResult = args as ChoiceResult;
            if (choiceResult != null)
            {
                dc.ActiveDialog.State["workshop"] = choiceResult.Value.Value;
                await dc.Context.SendActivity("you picked: " + choiceResult.Value.Value);
            }

            await dc.Prompt("confirm", "Are you sure?");
        }

        private async Task DonePrompt(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            string message;
            if ((bool)args["Confirmation"] == true)
            {
                message = "Order Placed";
            }
            else
            {
                message = "Order Cancelled";
            }
            await dc.Context.SendActivity(message);
            await dc.End();

        }
    }
}