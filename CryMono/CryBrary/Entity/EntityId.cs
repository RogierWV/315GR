﻿namespace CryEngine
{
    /// <summary>
    /// Entity ID's store references to game entities as unsigned integers. This class wraps that functionality for CLS compliance.
    /// </summary>
    public struct EntityId
    {
        internal uint _value;

        [System.CLSCompliant(false)]
        public EntityId(uint id)
        {
            _value = id;
        }

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj is EntityId)
                return (EntityId)obj == this;
            if (obj is int)
                return (int)obj == _value;
            if (obj is uint)
                return (uint)obj == _value;

            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
        #endregion

        #region Operators
        public static bool operator ==(EntityId entId1, EntityId entId2)
        {
            return entId1._value == entId2._value;
        }

        public static bool operator !=(EntityId entId1, EntityId entId2)
        {
            return entId1._value != entId2._value;
        }

        [System.CLSCompliant(false)]
        public static implicit operator EntityId(uint value)
        {
            return new EntityId(value);
        }

        public static implicit operator EntityId(int value)
        {
            return new EntityId((uint)value);
        }

        public static implicit operator int(EntityId value)
        {
            return System.Convert.ToInt32(value._value);
        }

        [System.CLSCompliant(false)]
        public static implicit operator uint(EntityId value)
        {
            return System.Convert.ToUInt32(value._value);
        }
        #endregion
    }
}
