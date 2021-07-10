using System;
using System.Collections.Generic;
using Newtonsoft.Json;
public class CompilationResult
{
    [JsonProperty("output")]
    public List<String> output { get; set; }
}
