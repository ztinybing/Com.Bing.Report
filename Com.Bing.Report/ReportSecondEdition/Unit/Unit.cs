using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
 
namespace Com.Bing.Report
{
    [TypeConverter(typeof(UnitConverter))]

    public struct Unit
    {
        public Unit(double value, UnitTypes type)
        {
            this._value = value;
            this._type = type;
        }
        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private UnitTypes _type;
        public UnitTypes Type
        {
            get { return _type; }
            set
            {
                this.Value = this.To(value).Value;
                _type = value;
            }
        }
        public double GetPixelPer(UnitTypes unitType)
        {
            switch (unitType)
            {
                case UnitTypes.Cm:
                    return this.GetPixelPer(UnitTypes.In) / 2.54F;
                case UnitTypes.In:
                    return 96;
                case UnitTypes.Mm:
                    return this.GetPixelPer(UnitTypes.Cm) / 10;
                default:
                    return 1;
            }
        }
        public Unit To(UnitTypes unitType)
        {
            return new Unit((this.Value * this.GetPixelPer(this.Type)) /
                            this.GetPixelPer(unitType), unitType);
        }
        public static Unit operator +(Unit a, Unit b)
        {
            return new Unit(a.To(UnitTypes.Px) + b.To(UnitTypes.Px), UnitTypes.Px);
        }
        public static Unit operator -(Unit a, Unit b)
        {
            return new Unit(a.To(UnitTypes.Px) - b.To(UnitTypes.Px), UnitTypes.Px);
        }
        public static Unit operator *(Unit a, Unit b)
        {
            return new Unit(a.To(UnitTypes.Px) * b.To(UnitTypes.Px), UnitTypes.Px);
        }
        public static Unit operator /(Unit a, Unit b)
        {
            return new Unit(a.To(UnitTypes.Px) / b.To(UnitTypes.Px), UnitTypes.Px);
        }
        public static implicit operator double(Unit u)
        {
            return u.To(UnitTypes.Px);
        }
        public override string ToString()
        {
            return string.Format("{0} {1}", this.Value.ToString(), this.Type.ToString());
        }
    }
}
