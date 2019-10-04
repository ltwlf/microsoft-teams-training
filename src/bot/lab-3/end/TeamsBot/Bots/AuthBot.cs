using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TeamsBot.Bots
{
    public class AuthBot : ActivityHandler
    {
        // Connection name of out Azure Active Directory Provider in the Bot Channel Registration
        private const string ConnectioName = "AzureAD";
        private BotState conversationState;
        private UserState userState;
        private DialogSet dialogSet;

        // The services will automatically injected via dependency injection
        public AuthBot(ConversationState conversationState, UserState userState)
        {
            this.conversationState = conversationState;
            this.userState = userState;

            // Create a DialogSet and add our AuthDialog
            // We also need to create a DialogState to persits the state of the dialogs between diffretne turns
            this.dialogSet = new DialogSet(conversationState.CreateProperty<DialogState>(nameof(DialogState)));
            this.dialogSet.Add(new AuthDialog(nameof(AuthDialog), ConnectioName));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a DialogContext
            var dc = await this.dialogSet.CreateContextAsync(turnContext);

            // Check if the Bot Framework has already a cached token for the current user and if there is already an AuthDialog active
            var tokenResponse = await ((BotFrameworkAdapter)turnContext.Adapter).GetUserTokenAsync(turnContext, ConnectioName, null, cancellationToken);
            if (tokenResponse == null)
            {
                // If there is a token and the AuthDialog is still active continue the dialog
                if (dc.ActiveDialog?.Id == nameof(AuthDialog))
                {
                    await dc.ContinueDialogAsync();
                }
                else
                {
                    // If not start the AuthDialog (token null and and running AuthDialog means unsually Magic Code authentication)
                    await dc.BeginDialogAsync(nameof(AuthDialog));
                }
            }
            else
            {
                // If there is a token and the AuthDialog is still active continue the dialog
                if (dc.ActiveDialog?.Id == nameof(AuthDialog))
                {
                    await dc.ContinueDialogAsync();
                }
                else
                {
                    // When autentication is done call the base class which triggers the activity handlers like "OnMessageActivityAsync"
                    await base.OnTurnAsync(turnContext, cancellationToken);
                }
            }

            // Persist state changes at the end of every turn
            await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
