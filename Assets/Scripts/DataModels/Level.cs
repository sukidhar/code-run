using System;
using System.Collections.Generic;
using Newtonsoft.Json;
public class Level
{
    [JsonProperty("questions")]
    public List<Question> questions { get; set; }
}
