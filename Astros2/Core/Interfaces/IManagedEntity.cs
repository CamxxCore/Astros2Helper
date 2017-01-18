using Astros2.Core.Events;

namespace Astros2.Core.Interfaces
{
    public interface IManagedEntity
    {
        void OnUpdate();
        void Remove();
        event EntityChangedEventHandler Alive;
        event EntityChangedEventHandler Dead;
        event EntityChangedEventHandler EnterWater;
        int SpawnTime { get; }
        int TotalTicks { get; }
        int InWaterTicks { get; }
        int DeadTicks { get; }
    }
}
