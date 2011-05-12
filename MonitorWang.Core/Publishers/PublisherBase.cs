using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Publishers
{
    public abstract class PublisherBase
    {
        public Status Status { get; set;}

        public virtual void Initialise()
        {
            // do nothing
        }

        public bool Enabled { get; set; }

        public string FriendlyId { get; set; }
    }
}