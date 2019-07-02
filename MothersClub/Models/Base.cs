using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace UserReferenceModule.Models
{
    public class Base
    {
        protected Base()
        {
            createdDate = DateTime.Now;
            modifiedDate = DateTime.Now;
        }

        [Key, JsonIgnore]
        public int id { get; protected set; }
        [JsonIgnore]
        public DateTime createdDate { get; set; }

        [JsonIgnore]
        public DateTime modifiedDate { get; set; }

        public virtual void ValidateModel()
        {
            var fieldValues = GetType()
                             .GetFields(BindingFlags.Public)
                             .Select(field => field.GetValue(this))
                             .ToList();
        }
    }
}