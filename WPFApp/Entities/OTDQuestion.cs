using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPFApp.Entities
{
    public class OTDQuestion
    {
        //public OTDQuestionType Type { get; set; } = OTDQuestionType.UNKNOWN;

        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        public string? Difficulty { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string? Category { get; set; }

        [JsonProperty(PropertyName = "question")]
        public string? Question { get; set; }
        
        [JsonProperty(PropertyName = "correct_answer")]
        public string? CorrectAnswer { get; set; }

        [JsonProperty(PropertyName = "incorrect_answers")]
        public string[]? IncorrectAnswers { get; set; }

    }
}
