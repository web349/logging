using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Web349.Logging.Slack
{
    public class SlackMessage
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        public SlackMessage()
        {
        }

        public SlackMessage(string text)
        {
            this.Text = text;
        }
    }
}
