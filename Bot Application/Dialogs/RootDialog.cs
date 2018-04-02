using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bot_Application.Bot_To_CRM;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        // First dialog, root
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            
            if (activity.Text.ToLower().Contains("hello") || activity.Text.ToLower().Contains("hi"))
            {
                // User said 'order', so invoke the New Order Dialog and wait for it to finish.
                //await this.SendWelcomePackAsyn(context);
                await context.Forward(new WelcomeDialog(), this.ResumeAfterWelcomeDialog, activity, CancellationToken.None);
            }          
        }

        private async Task ResumeAfterWelcomeDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {             // Store the value the name returned. 
                          // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
                var activity = await result as Activity;
                // invoke next dialog to ask for an action
                await context.Forward(new ActivityDialog(), this.ResumeAfterActivityDialog, activity, CancellationToken.None);

            }
            catch (Exception e)
            {
                await context.PostAsync($"Oooops, something happened.... {e.Message.ToString()}");
            }
        }
        private async Task ResumeAfterActivityDialog(IDialogContext context, IAwaitable<Object> result)
        {
            try
            {             // option set is returned
                          // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
                var activity = await result as Activity;
                
                // invoke next dialog to ask for an action 
                //TODO insert case for each response
                if (activity.Text.Equals("1"))
                {
                    await context.Forward(new EmailDialog(), this.ResumeAfterEmailDialog, activity, CancellationToken.None);
                }
                

            }
            catch (Exception e)
            {
                await context.PostAsync($"Oooops, something happened.... {e.Message.ToString()}");
            }
        }

        private async Task ResumeAfterEmailDialog(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result;
            //In CRM a task will be created periodically to send emails as well as lead
            if (activity.Equals("subscribe")) {
                var name = context.UserData.ContainsKey("userName") ? context.UserData.GetValue<string>("userName").ToString() : "empty";
                var email = context.UserData.ContainsKey("userEmail") ? context.UserData.GetValue<string>("userEmail").ToString() : "empty";
                EmailDialogToCRM.createLead(name, email);
            }
            

            await context.PostAsync("Thank you for subscribe to our monthly white paper");
            context.Wait(this.MessageReceivedAsync);
        }
    }
}