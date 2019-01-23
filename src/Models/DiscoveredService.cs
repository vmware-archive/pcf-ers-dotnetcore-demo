using System.Collections.Generic;

namespace Articulate.Models
{
    public class DiscoveredService
    {
        public string Name { get; set; }
        public List<string> Urls { get; set; }
    }

    public class DiscoveryModel
    {
        public bool IsEurekaBound { get; set; }
        public List<DiscoveredService> Services { get; set; }
    }
}