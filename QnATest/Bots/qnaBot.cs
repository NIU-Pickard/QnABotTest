// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.0

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Bot.Builder.AI.QnA;
using System.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace QnATest.Bots
{
    public class qnaBot : ActivityHandler
    {
        public QnAMaker QnABot { get; private set; }

        public qnaBot(QnAMakerEndpoint endpoint)
        {
            // connects to QnA Maker endpoint for each turn
            QnABot = new QnAMaker(endpoint);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Echo: {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            await AccessQnAMaker(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hi, I am Tabot—a chatbot for ACCY 288. Currently, I can answer basic questions about the course syllabus and schedule. My creators just launched me, so I have a lot to learn about how to answer your questions in ACCY 288. But, by asking me questions, you will help me know what I need to learn.";
            
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private async Task AccessQnAMaker(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var metadata = new Microsoft.Bot.Builder.AI.QnA.Metadata();
            var qnaOptions = new QnAMakerOptions();

            metadata.Name = "section";
            metadata.Value = "0001";
            qnaOptions.StrictFilters = new Microsoft.Bot.Builder.AI.QnA.Metadata[] { metadata };

            var results = await QnABot.GetAnswersAsync(turnContext, qnaOptions);
            if (results.Any())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("QnA Maker Returned: " + results.First().Answer), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the knowledge base."), cancellationToken);
            }
        }
    }
}
