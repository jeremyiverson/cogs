﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Cogs.Publishers
{
    internal class JsonSchemaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<JsonSchema>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is List<JsonSchema> schema)
            {
                var obj2 = new JObject();
                foreach (var prop in schema)
                {
                    var obj = new JObject();
                    if (prop.Title == "~~reference~~")
                    {
                        obj2.Add(new JProperty("TopLevelReference",
                            new JObject(new JProperty("type", "array"),
                            new JProperty("items", new JObject(new JProperty("$ref", "#/definitions/Reference"), new JProperty("minItems", 0))))));

                    }
                    else
                    {
                        foreach (var inner_prop in prop.Properties)
                        {
                            if (inner_prop.Reference != null)
                            {
                                if (inner_prop.MultiplicityElement.MaxCardinality == "1")
                                {
                                    obj.Add(new JProperty(inner_prop.Name,
                                    new JObject(new JProperty("$ref", inner_prop.Reference),
                                            new JProperty("MultiplicityElement", (new JObject(new JProperty("lower", Convert.ToInt32(inner_prop.MultiplicityElement.MinCardinality)), new JProperty("upper", Convert.ToInt32(inner_prop.MultiplicityElement.MaxCardinality))))),
                                                    new JProperty("Description", inner_prop.Description))));
                                }
                                else
                                {
                                    obj.Add(new JProperty(inner_prop.Name,
                                    new JObject(new JProperty("type", "array"), new JProperty("items", new JObject(new JProperty("$ref", inner_prop.Reference))),
                                            new JProperty("minItems", Convert.ToInt32(inner_prop.MultiplicityElement.MinCardinality)),
                                                    new JProperty("Description", inner_prop.Description))));
                                }
                            }
                            else
                            {
                                if (inner_prop.MultiplicityElement.MaxCardinality == "1")
                                {
                                    obj.Add(new JProperty(inner_prop.Name,
                                    new JObject(new JProperty("type", inner_prop.Type),
                                            new JProperty("MultiplicityElement", (new JObject(new JProperty("lower", Convert.ToInt32(inner_prop.MultiplicityElement.MinCardinality)), new JProperty("upper", Convert.ToInt32(inner_prop.MultiplicityElement.MaxCardinality))))),
                                                    new JProperty("Description", inner_prop.Description))));
                                }
                                else
                                {
                                    obj.Add(new JProperty(inner_prop.Name,
                                    new JObject(new JProperty("type", "array"), new JProperty("items", new JObject(new JProperty("type", inner_prop.Type))),
                                            new JProperty("minItems", Convert.ToInt32(inner_prop.MultiplicityElement.MinCardinality)),
                                                    new JProperty("Description", inner_prop.Description))));
                                }
                            }
                        }
                        obj2.Add(new JProperty(prop.Title, new JObject(new JProperty("type", "object"), new JProperty("patternProperties", new JObject(new JProperty(@"^(?!\s*$).+", new JObject(new JProperty("type", prop.Type), new JProperty("id", prop.Id), new JProperty("properties", obj), new JProperty("required", prop.Required))))))));
                    }
                }
                obj2.WriteTo(writer);
            }
        }
    }
}