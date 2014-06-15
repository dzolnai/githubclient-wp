﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Entity
{
    [JsonConverter(typeof(FileContentsConverter))]
    public class FileContents
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public string Encoding { get; set; }
        public string Url { get; set; }
    }
}