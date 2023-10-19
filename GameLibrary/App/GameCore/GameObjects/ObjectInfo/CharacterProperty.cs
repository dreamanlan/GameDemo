using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public sealed class CharacterProperty
    {
        public EntityInfo Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }
        internal float GetFloat(CharacterPropertyEnum id)
        {
            return Get(id) / 1000.0f;
        }
        internal void SetFloat(CharacterPropertyEnum id, float val)
        {
            Set(id, (long)(val * 1000.0f));
        }
        internal void IncreaseFloat(CharacterPropertyEnum id, float delta)
        {
            Increase(id, (long)(delta * 1000.0f));
        }
        internal int GetInt(CharacterPropertyEnum id)
        {
            return (int)Get(id);
        }
        internal void SetInt(CharacterPropertyEnum id, int val)
        {
            Set(id, val);
        }
        internal void IncreaseInt(CharacterPropertyEnum id, int delta)
        {
            Increase(id, delta);
        }
        internal long GetLong(CharacterPropertyEnum id)
        {
            return Get(id);
        }
        internal void SetLong(CharacterPropertyEnum id, long val)
        {
            Set(id, val);
        }
        internal void IncreaseLong(CharacterPropertyEnum id, long delta)
        {
            Increase(id, delta);
        }

        internal void CopyFrom(CharacterProperty other)
        {
            m_Values.Clear();
            foreach (var pair in other.m_Values) {
                m_Values.Add(pair.Key, pair.Value);
            }
        }
        
        private void Increase(CharacterPropertyEnum id, long delta)
        {
            long val = Get(id);
            val += delta;
            Set(id, val);
        }
        private long Get(CharacterPropertyEnum id)
        {
            long val = 0;
            int idVal = (int)id;
            m_Values.TryGetValue(idVal, out val);
            return val;
        }
        private void Set(CharacterPropertyEnum id, long val)
        {
            int idVal = (int)id;
            m_Values[idVal] = val;
        }
        
        private Dictionary<int, long> m_Values = new Dictionary<int, long>();
        private EntityInfo m_Owner = null;
    }
}
