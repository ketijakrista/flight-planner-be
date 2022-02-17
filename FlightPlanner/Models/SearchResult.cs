using System.Collections.Generic;

namespace FlightPlanner.Models
{
    public class SearchResult
    {
        public int Page { get; set; }

        public int TotalItems { get; set; }

        public List<Flight> Items { get; set; }
    }
}
