using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using AdaptiveCards;
using System.IO;
using System.Linq;


namespace TeamsBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded.Any(m => m.Name != "Bot"))
            {
                await SendWelcomeCard(turnContext);
            }
        }

        public async Task SendWelcomeCard(ITurnContext turnContext)
        {
            var card = File.ReadAllText(@".\Cards\WelcomeCard.json");
            var parsedResult = AdaptiveCard.FromJson(card);
            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = parsedResult.Card
            };

            var activity = turnContext.Activity.CreateReply();
            activity.Attachments.Add(attachment);

            await turnContext.SendActivityAsync(activity);
        }
    }
}
