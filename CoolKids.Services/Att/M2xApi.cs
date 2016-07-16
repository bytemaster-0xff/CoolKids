using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATTM2X;
using ATTM2X.Classes;

namespace CoolKids.Services.Att
{
	public class M2xApi
	{
		const string ApiKey = "315eaca8d4dbf87742cd1432605bd3be"; // direct to SafeCar-2016-000001
		const string DeviceId = "377896128a555058bfeccc0eef1fba96"; // for SafeCar-2016-000001

		public static class CarDoorType
		{
			public const string DriverFront = "driver_door_front_state";
			public const string DriverRear = "driver_door_rear_state";
			public const string PassengerFront = "passenger_door_front_state";
			public const string PassengerRear = "passenger_door_rear_state";
		}

		public static class CarDoorValue
		{
			public const string Ajar = "ajar";
			public const string Closed = "closed";
			public const string Open = "open";
		}

		public static class CarWindowType
		{
			public const string DriverFront = "driver_window_front_openness";
			public const string DriverRear = "driver_window_rear_openness";
			public const string PassengerFront = "passenger_window_front_openness";
			public const string PassengerRear = "passenger_window_rear_openness";
		}

		public static class TemperatureType
		{
			public const string Exterior = "temperature_exterior";
			public const string Interior = "temperature_interior";
		}

		/// <summary>
		/// Posts value expressing the state of the specified door.
		/// </summary>
		/// <param name="carDoorType">A selection from CarDoorType.</param>
		/// <param name="carDoorValue">A selection from CarDoorValue.</param>
		/// <returns></returns>
		public async Task<string> PostValue(string carDoorType, string carDoorValue)
		{
			using (var client = new M2XClient(ApiKey))
			{
				var device = client.Device(DeviceId);
				var stream = device.Stream(carDoorType);
				var valueParams = $"{{ \"timestamp\": \"{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")}\", \"value\": \"{carDoorValue}\" }}";
				var postResult = await stream.UpdateValue(valueParams);
				return postResult.Raw;
			}
		}

		/// <summary>
		/// Posts value expressing the openness of the specified window.
		/// </summary>
		/// <param name="carIntegerType">A selection from a type that uses integer values, such as TemperatureType.</param>
		/// <param name="carIntegerValue">An integer percentage value equal to or between from 0 and +N.</param>
		/// <returns></returns>
		public async Task<string> PostValue(string carIntegerType, int carIntegerValue)
		{
			using (var client = new M2XClient(ApiKey))
			{
				var device = client.Device(DeviceId);
				var stream = device.Stream(carIntegerType);
				var valueParams = $"{{ \"timestamp\": \"{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")}\", \"value\": {carIntegerValue} }}";
				var postResult = await stream.UpdateValue(valueParams);
				return postResult.Raw;
			}
		}

		public async Task<string> GetCarLocation()
		{
			using (var client = new M2XClient(ApiKey))
			{
				var device = client.Device(DeviceId);
				var locationResult = await device.Location();
				return locationResult.Raw;
			}
		}
	}
}