using OpenAI_API;
using OpenAI_API.Chat;

namespace GPTTextCompletions.ChatGPT
{
    public class GptCaller : IGpt
    {
        /// <summary>
        /// The api object for setup.
        /// </summary>
        private OpenAIAPI api;

        /// <summary>
        /// The current conversation.
        /// </summary>
        private Conversation conversation;

        /// <summary>
        /// Instructions to give the chatbot context.
        /// </summary>
        private string instructions = 
            "You are a work/study coach.  Your goal is to break down tasks that " +
            "are given into no more than five simpler " +
            "subtasks separated by newlines.";

        /// <summary>
        /// Example input for the conversation.
        /// </summary>
        private string[] exampleInput =
        {
            "Laundry",
            "Create a web crawler"
        };

        /// <summary>
        /// Example output for the conversation.
        /// </summary>
        private string[] exampleOutput =
        {
            "Gather dirty laundry from around the house.\n" +
                "Sort laundry into appropriate categories (whites, colors, delicates, etc.).\n" +
                "Load the washing machine with a sorted batch of laundry, add detergent, and start the appropriate cycle.\n" +
                "Once the cycle is complete, transfer the washed laundry to the dryer or hang it to dry.\n" +
                "Fold or hang the dried laundry and put it away in the proper location.",
            "Choose a programming language and relevant libraries or frameworks for web crawling (e.g., Python with Beautiful Soup and requests, or Scrapy).\n" +
                "Identify the target website(s) and their structure to determine the URLs and HTML elements that need to be parsed.\n" +
                "Write code to send HTTP requests to the target URLs and retrieve the HTML content.\n" +
                "Parse the retrieved HTML content, extracting the desired information using the chosen library (e.g., Beautiful Soup).\n" +
                "Save the extracted data into a desired format (e.g., CSV, JSON, or a database) and implement error handling and rate-limiting to respect website policies."
        };

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        public string Instructions 
        {
            get => instructions; 
            set
            {
                instructions = value;
            }
        }

        /// <summary>
        /// Gets or sets the tuples of instructions.
        /// </summary>
        public Tuple<string, string>[] Examples { get; set; }

        /// <summary>
        /// Initializes an instance of GptCaller
        /// </summary>
        public GptCaller()
        {
            Examples = new Tuple<string, string>[2];
            Examples[0] = new Tuple<string,string>(exampleInput[0], exampleOutput[0]);
            Examples[1] = new Tuple<string,string>(exampleInput[1], exampleOutput[1]);

            api = new OpenAIAPI("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");
            conversation = InitializeConversation();
        }

        /// <summary>
        /// Prompts GPT for input.
        /// </summary>
        /// <param name="userPrompt">The prompt for the chatbot.</param>
        /// <returns>
        /// The response from ChatGPT as a string.
        /// </returns>
        public async Task<string> PromptAsync(string userPrompt)
        {
            conversation.AppendUserInput(userPrompt);
            return await conversation.GetResponseFromChatbotAsync();
        }

        /// <summary>
        /// Starts a new conversation context with ChatGPT.
        /// </summary>
        /// <returns>
        /// The conversation context.
        /// </returns>
        private Conversation InitializeConversation()
        {
            var chat = api.Chat.CreateConversation();

            // Give the chat context.
            chat.AppendSystemMessage(Instructions);

            // Provide example input and output.
            chat.AppendUserInput(Examples[0].Item1);
            chat.AppendExampleChatbotOutput(Examples[0].Item2);
            chat.AppendUserInput(Examples[1].Item1);
            chat.AppendExampleChatbotOutput(Examples[1].Item2);

            return chat;
        }
    }
}