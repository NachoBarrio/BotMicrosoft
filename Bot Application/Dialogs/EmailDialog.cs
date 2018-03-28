using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Bot_Application.Dialogs
{
    [Serializable]
    public class EmailDialog : IDialog<Object>
    {
        private int attempts = 3;
        public enum BooleanOptionSubscription
        {
            subscribe,
            noSubscribe,
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Which is your email address?");
            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;
            try
            {

                if ((message.Text != null) && (message.Text.Trim().Length > 0) && ComprobarFormatoEmail(message.Text))
                {
                    /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                        dialog. */
                    context.UserData.SetValue("userEmail", message.Text);
                    context.Wait(this.EmailReceivedAsync);
                }
                /* Else, try again by re-prompting the user. */
                else
                {
                    //attempts;
                    if (attempts > 0)
                    {
                        await context.PostAsync("I'm sorry, your email is not correcty built (ex. aa@gmail.com)");

                        context.Wait(this.MessageReceivedAsync);
                    }
                    else
                    {
                        /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                            parent/calling dialog. */
                        context.Fail(new TooManyAttemptsException("Email is not correcty built."));
                    }
                }
            }
            catch (Exception e)
            {
                context.Fail(new Exception("Error while getting email: " + e.Message.ToString()));
            }
        }

        public virtual async Task EmailReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            try
            {
                //present subscription to email marketing list
                PromptDialog.Choice(

                   context: context,

                   resume: SubscriptionReceivedAsync,

                   options: (IEnumerable<BooleanOptionSubscription>)Enum.GetValues(typeof(BooleanOptionSubscription)),

                   prompt: "Do you want to subscribe to our marketing list ?",

                   retry: " Please try again.",

                   promptStyle: PromptStyle.Auto

               );
            }
            catch (Exception e)
            {
                context.Fail(new Exception("Error while selecting subscription: " + e.Message.ToString()));
            }
        }

        public virtual async Task SubscriptionReceivedAsync(IDialogContext context, IAwaitable<BooleanOptionSubscription> result)
        {
            await context.PostAsync("Thanks for submitting your information .");

            context.Done(this);
        }
        public static bool ComprobarFormatoEmail(string sEmailAComprobar)
        {
            String sFormato;
            sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(sEmailAComprobar, sFormato))
            {
                if (Regex.Replace(sEmailAComprobar, sFormato, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static Attachment GetInvestorsCard()
        {
            var investorsSubscriptionCard = new HeroCard
            {
                Title = "Monthly reports about invest ",

                Subtitle = "Follow our tips",

                Text = "With this information updated each month you are up to earn money without much effort",

                Images = new List<CardImage> { new CardImage("https://www.google.es/imgres?imgurl=https%3A%2F%2Fi.ytimg.com%2Fvi%2F5wZ5ZHYWoTE%2Fmaxresdefault.jpg&imgrefurl=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3D5wZ5ZHYWoTE&docid=DAar-q_6P5Q2kM&tbnid=Ju7xgMtjliuLJM%3A&vet=10ahUKEwic7ofY5I7aAhWMbxQKHa0JAN0QMwg7KAYwBg..i&w=1280&h=720&bih=734&biw=1536&q=imagenes%20bestinver&ved=0ahUKEwic7ofY5I7aAhWMbxQKHa0JAN0QMwg7KAYwBg&iact=mrc&uact=8") },

                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Investors School", value: "https://www.bestinver.es/escuela-de-inversion/") }
            };
            return investorsSubscriptionCard.ToAttachment();
        }
    }
}
