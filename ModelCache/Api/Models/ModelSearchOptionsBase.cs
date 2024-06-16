using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCache.Api.Models
{
    public class ModelSearchOptionsBase
    {
        public int Skip { get; set; } = 0;

        public int? Take { get; set; } = null;

        public string Query { get; set; } = string.Empty;

        public ModelSearchOptionsBase() { }

        public ModelSearchOptionsBase(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public ModelSearchOptionsBase(int skip, int take, string query)
        {
            Skip = skip;
            Take = take;
            Query = query;
        }
    }
}
