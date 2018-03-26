using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class CalendarDialog : IDialog<object>
    {
        private int attempts = 3;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("When could you have a minute to talk with us?");
            context.Wait(MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            /* Else, try again by re-prompting the user. */
            else
            {
                //attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply. What is your name (e.g. 'Bill', 'Melinda')?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                        parent/calling dialog. */
                    context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
                }
            }
        }
    }
}