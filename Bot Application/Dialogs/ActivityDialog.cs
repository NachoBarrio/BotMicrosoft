using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class ActivityDialog : IDialog<Object>
    {
        private int attempts = 3;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;
            try
            {
                var name = context.UserData.ContainsKey("userName") ? context.UserData.GetValue<string>("userName").ToString() : "empty";
                await context.PostAsync($"Hello {activity.Text.ToString()}/ {name} I´m glad to guide you during this experience, you are just about to enter in the investors world");
                var reply = activity.CreateReply("Which action do you want to perform?");
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                        {
                            new CardAction(){ Title = "Receive email with information", Type=ActionTypes.ImBack, Value=1 },
                            new CardAction(){ Title = "Receive a call from one of our experts", Type=ActionTypes.ImBack, Value=2 },
                            new CardAction(){ Title = "Let´s try and hang around with a real investor manager", Type=ActionTypes.ImBack, Value=3 }
                        }
                };
                await context.PostAsync(reply);
                context.Wait(this.ActivityReceivedAsync);
            }
            catch(Exception e)
            {
                context.Fail(new Exception("Error while getting activity: " + e.Message.ToString()));
            }
        }


        public virtual async Task ActivityReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var activity = await result as Activity;
                if(activity.Text.Length == 1)
                {
                    context.Done(activity.Text);
                }
                else
                {
                    //attempts;
                    if (attempts > 0)
                    {
                        await context.PostAsync("I'm sorry, you must select one of the options shown");

                        context.Wait(this.MessageReceivedAsync);
                    }
                    else
                    {
                        /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                            parent/calling dialog. */
                        context.Fail(new TooManyAttemptsException("Incorrect selection"));
                    }
                }

            }
            catch (Exception e)
            {
                await context.PostAsync($"Oooops, something happened.... {e.Message.ToString()}");
            }

        }
    }
}
