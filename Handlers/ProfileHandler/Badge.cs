using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _490Bot.Handlers.ProfileHandler {
    public class Badge {
        public String BadgeName { get; set; }
        public String BadgeDesc { get; set; }
        public String BadgeIcon { get; set; }
        public int BadgeLevel { get; set; }

        public Badge() {
            BadgeName = "Name";
            BadgeDesc = "Test description.";
            BadgeIcon = "../ExamplePath/Example.png";
            BadgeLevel = 1;
        }

        
    }
}
