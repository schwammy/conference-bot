using AdaptiveCards;
using ConferenceBot.Model;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ConferenceBot.Adapters
{
    public interface ICardAdapter
    {
        Attachment ToCard(Session session);
    }
    public class CardAdapter: ICardAdapter
    {
        public Attachment ToCard(Session session)
        {
            var card = new AdaptiveCard();
            card.Body.Add(new TextBlock() { Text = session.Title, Weight = TextWeight.Bolder, Size = TextSize.Medium, Wrap = true });
          
            card.Body.Add(new TextBlock() { Text = $"{session.Speaker} - {session.Day} at {session.Time}", Size = TextSize.Medium });
            //card.Body.Add(new TextBlock() { Text = session.Description, Wrap = true });


            ShowCardAction showCardAction = new ShowCardAction();
            showCardAction.Title = "Rate this session";
            card.Actions.Add(showCardAction);

            var reviewCard = new AdaptiveCard();
            var ratings = new ChoiceSet()
            {
                Style = ChoiceInputStyle.Compact,
                IsMultiSelect = false,

                Choices = new List<Choice>()
                {
                    new Choice() {Title = "1", Value = "1"},
                    new Choice() {Title = "2", Value = "2"},
                    new Choice() {Title = "3", Value = "3"},
                    new Choice() {Title = "4", Value = "4"},
                    new Choice() {Title = "5", Value = "5"},
                }
            };
            reviewCard.Body.Add(new TextBlock() { Text = "Let us know what you thought of this session" });
            reviewCard.Body.Add(ratings);
            var data = new { session.Title, Action = "ReviewSession" };
            reviewCard.Actions.Add(new SubmitAction() { Title = "Submit feedback", Data = data, DataJson = JsonConvert.SerializeObject(data) });

            showCardAction.Card = reviewCard;

            Attachment attachment = new Attachment()

            {

                ContentType = AdaptiveCard.ContentType,

                Content = card

            };

            return attachment;
        }

    }
}
