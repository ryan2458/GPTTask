using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPTTextCompletions.ChatGPT
{
    public interface IGpt
    {
        string Instructions { get; set; }

        Tuple<string, string>[] Examples { get; set; }

        Task<string> PromptAsync(string userPrompt);
    }
}
