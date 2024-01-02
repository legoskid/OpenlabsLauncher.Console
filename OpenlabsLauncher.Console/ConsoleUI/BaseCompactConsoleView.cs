using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsLauncher.ConsoleUI
{
    internal abstract class BaseCompactConsoleView : IEquatable<BaseCompactConsoleView>
    {
        public bool Equals(BaseCompactConsoleView other)
            => other.Name != Name;

        public abstract string Name { get; }

        public abstract Task OnLoad();
        public abstract Task OnUpdate(float delta);
        public abstract Task OnClose();
    }
}
