﻿// Copyright (c) 2017 Colectica. All rights reserved
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cogs.Dto
{
    public class TopicIndex
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ItemTypes { get; set; } = new List<string>();
    }
}
