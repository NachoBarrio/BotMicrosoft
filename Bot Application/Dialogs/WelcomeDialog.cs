using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<Object>
    {
        private int attempts = 3;

        public async Task StartAsync(IDialogContext context)
        {
             context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync("Hello there, who is there? What is your name?");
            context.Wait(this.NameReceivedAsync);
        }


        public virtual async Task NameReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;

            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.UserData.SetValue("userName", message.Text);
                context.Done(message);
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
