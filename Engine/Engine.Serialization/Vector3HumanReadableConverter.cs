using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Vector3))]
    public class Vector3HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			var vector = (Vector3)value;
			return HumanReadableConverter.ValuesListToString<float>(',', vector.X, vector.Y, vector.Z);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
            return array.Length == 3 ? (object)new Vector3(array[0], array[1], array[2]) : throw new Exception();
        }
    }
}
