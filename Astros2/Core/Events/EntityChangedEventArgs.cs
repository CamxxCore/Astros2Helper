using System;
using Astros2.Core.Interfaces;

namespace Astros2.Core.Events
{
    public delegate void EntityChangedEventHandler(IManagedEntity sender, EntityChangedEventArgs args);

    public sealed class EntityChangedEventArgs : EventArgs
    {
        public EntityChangedEventArgs(IManagedEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// The entity that fired the event
        /// </summary>
        public IManagedEntity Entity { get; private set; }
    }
}
