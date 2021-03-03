using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTodoist.TodoistModel
{
    public class BaseEntity 
    {
        internal BaseEntity(ComplexId id)
        {
            Id = id;
        }

        internal BaseEntity()
        {
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public ComplexId Id { get; set; }

        internal bool ShouldSerializeId()
        {
            return !Id.IsEmpty;
        }
    }
}
