using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace _490Bot.Handlers.ProfileHandler {
    internal class Profile {
        IGuildUser Member;
        String statusField;
        String aboutField;
        Badge[] badges;
        int level;
        int experienceCurrent;
        int experienceNeeded;

        int levelUp() {
            int newLevel = 0;
            return newLevel;
        }
    }
}
