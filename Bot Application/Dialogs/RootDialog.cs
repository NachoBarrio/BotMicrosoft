using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
        // First dialog, root
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text.ToLower().Contains("hello") || activity.Text.ToLower().Contains("hi"))
            {
                // User said 'order', so invoke the New Order Dialog and wait for it to finish.
                context.Call(new  WelcomeDialog(),this.ResumeAfterWelcomeDialog);
            }
            
            if (activity.Text.ToLower().Equals("phonecall") ||  activity.Text.ToLower().Equals("Appointment"))
            {
                //create a new Dialog to select a day to do the activity
                context.Call(new CalendarDialog(), this.ResumeAfterWelcomeDialog);
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterWelcomeDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {             // Store the value the name returned. 
                          // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
                var activity = await result as Activity;

                await context.PostAsync($"Hello {activity.Text.ToString()} I´m glad to guide you during this experience");
                await context.PostAsync($"You are just about to enter in the investors world");

                var reply = activity.CreateReply("Which action do you want to perform?");
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                    {
                        new CardAction(){ Title = "Receive email with information", Type=ActionTypes.ImBack, Value="Email" },
                        new CardAction(){ Title = "Receive a call from one of our experts", Type=ActionTypes.ImBack, Value="phonecall" },
                        new CardAction(){ Title = "Let´s try and hang around with a real investor manager", Type=ActionTypes.ImBack, Value="Appointment" }
                    }
                };

                context.Wait(MessageReceivedAsync);
            }
            catch(Exception e)
            {
                await context.PostAsync($"Oooops, something happened.... {e.Message.ToString()}");
            }
        }
    }
}