using System;
using System.Globalization;
using UnityEngine;

namespace GameKit
{
    // we have to use UDateTime instead of DateTime on our classes
    // we still typically need to either cast this to a DateTime or read the DateTime field directly
    [Serializable]
    public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
    {
        // if you don't want to use the PropertyDrawer then remove HideInInspector here
        [HideInInspector] [SerializeField] private string _dateTime;
        [HideInInspector] public DateTime dateTime;

        public static implicit operator DateTime(UDateTime udt)
        {
            return udt?.dateTime ?? DateTime.MinValue;
        }

        public static implicit operator UDateTime(DateTime dt)
        {
            return new UDateTime {dateTime = dt};
        }

        public static TimeSpan operator -(UDateTime dt, UDateTime dt2)
        {
            return dt.dateTime - dt2.dateTime;
        }

        public static bool operator ==(UDateTime dt, UDateTime dt2)
        {
            if (Equals(dt, null) && Equals(dt2, null)) return true;
            if (Equals(dt, null) || Equals(dt2, null)) return false;
            return dt.dateTime == dt2.dateTime;
        }

        public static bool operator !=(UDateTime dt, UDateTime dt2)
        {
            if (Equals(dt, null) && Equals(dt2, null)) return false;
            if (Equals(dt, null) || Equals(dt2, null)) return true;
            return dt.dateTime != dt2.dateTime;
        }

        public void OnAfterDeserialize()
        {
            DateTime.TryParse(_dateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        public void OnBeforeSerialize()
        {
            _dateTime = dateTime.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(UDateTime other)
        {
            if (other == null) return 1;
            return dateTime.CompareTo(other.dateTime);
        }

        public int CompareTo(DateTime other)
        {
            return dateTime.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is UDateTime uDateTime)
            {
                return dateTime.Equals(uDateTime.dateTime);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return dateTime.GetHashCode();
        }

        public override string ToString()
        {
            return dateTime.ToString("g");
        }

        public string ToString(string format)
        {
            return dateTime.ToString(format);
        }
    }
}