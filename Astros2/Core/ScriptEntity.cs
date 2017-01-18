using System;
using Astros2.Core.Interfaces;
using Astros2.Core.Events;
using Astros2.Core.Types;
using GTA;

namespace Astros2.Core
{
    public abstract class ScriptEntity<T> : IManagedEntity where T : Entity
    {
        private TimeSpan totalTime;
        private int deadTicks, aliveTicks, waterTicks, totalTicks;
        private int spawnTime;

        /// <summary>
        /// Base entity instance.
        /// </summary>
        public T BaseRef { get; }

        /// <summary>
        /// Scipt extension pool.
        /// </summary>
        public ScriptEntityExtensionPool<T> Extensions { get; }

        /// <summary>
        /// Fired when the entity has been revived.
        /// </summary>
        public event EntityChangedEventHandler Alive;

        /// <summary>
        /// Fired when the entity has died.
        /// </summary>
        public event EntityChangedEventHandler Dead;

        /// <summary>
        /// Fired when the entity has entered water.
        /// </summary>
        public event EntityChangedEventHandler EnterWater;

        /// <summary>
        /// Total entity ticks.
        /// </summary>
        public int TotalTicks
        {
            get { return totalTicks; }
        }

        /// <summary>
        /// Total time entity has been available to the script.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return totalTime; }
        }

        /// <summary>
        /// Total ticks for which the entity has been alive.
        /// </summary>
        public int AliveTicks
        {
            get { return aliveTicks; }
        }

        /// <summary>
        /// Total ticks for which the entity has been dead.
        /// </summary>
        public int DeadTicks
        {
            get { return deadTicks; }
        }

        /// <summary>
        /// Total ticks for which the entity has been in water.
        /// </summary>
        public int InWaterTicks
        {
            get { return waterTicks; }
        }

        /// <summary>
        /// Total at which the entity was created.
        /// </summary>
        public int SpawnTime
        {
            get { return spawnTime; }
        }

        /// <summary>
        /// Add a script extension to this entity.
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(IScriptEntityExtension<T> extension)
        {
            if (!Extensions.ContainsKey(extension.ID))
            {
                extension.Entity = this;
                Extensions.Add(extension.ID, extension);
            }
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        public ScriptEntity(T baseRef)
        {
            BaseRef = baseRef;
            Extensions = new ScriptEntityExtensionPool<T>();
        }

        /// <summary>
        /// Fired when the entity has died.
        /// </summary>
        protected virtual void OnDead(EntityChangedEventArgs e)
        {
            Dead?.Invoke(this, e);
        }

        protected virtual void OnAlive(EntityChangedEventArgs e)
        {
            Alive?.Invoke(this, e);
        }

        protected virtual void OnWaterEnter(EntityChangedEventArgs e)
        {
            EnterWater?.Invoke(this, e);
        }

        /// <summary>
        /// Call this method each tick to update entity related information.
        /// </summary>
        public virtual void OnUpdate()
        {
            foreach (var extension in Extensions.Values)
            {
                extension.Update();
            }

            if (BaseRef.IsDead)
            {
                if (deadTicks == 0)
                    OnDead(new EntityChangedEventArgs(this));

                aliveTicks = 0;
                deadTicks++;
            }

            else
            {
                if (BaseRef.IsInWater)
                {
                    if (waterTicks == 0)
                        OnWaterEnter(new EntityChangedEventArgs(this));

                    waterTicks++;
                }
                else
                    waterTicks = 0;

                if (aliveTicks == 0)
                    OnAlive(new EntityChangedEventArgs(this));

                deadTicks = 0;
                aliveTicks++;
            }

            totalTicks++;
            totalTicks = totalTicks % int.MaxValue;

            totalTime = TimeSpan.FromMilliseconds(Game.GameTime - spawnTime);
        }

        /// <summary>
        /// Dispose of all active extensions for this entity.
        /// </summary>
        public void ClearExtensions()
        {
            foreach (var extension in Extensions.Values)
            {
                extension.Dispose();
            }

            Extensions.Clear();
        }

        /// <summary>
        /// Remove from the world.
        /// </summary>
        public virtual void Remove()
        {
            ClearExtensions();

            BaseRef.Delete();
        }
    }
}
