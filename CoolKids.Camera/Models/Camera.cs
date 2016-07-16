using CoolKids.Common.Commanding;
using CoolKids.Camera.Services;
using CoolKids.Common.Resources;
using CoolKids.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoolKids.Common;
using CoolKids.Common.IOC;
using CoolKids.Common.PlatformSupport;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using System.Runtime.CompilerServices;

namespace CoolKids.Camera.Models
{
	[DataContract]
	public class Camera : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler StartCameraPanTilt;
		public event EventHandler EndCameraPanTitle;

		public event EventHandler TransmitBegin;
		public event EventHandler TransmitEnd;

		public event EventHandler StreamConnecting;
		public event EventHandler StreamConnected;
		public event EventHandler StreamDisconnected;

		public event EventHandler Buffering;

		public event EventHandler<FrameReadyEventArgs> FrameReady;
		public event EventHandler<ErrorEventArgs> Error;

		public event EventHandler<StateChangedEventArgs> StateChanged;

		protected void OnStateChanged<T>(T state, bool useTransitions = true) where T : struct
		{
			if (StateChanged != null)
			{
				StateChanged(this, new StateChangedEventArgs
				{
					StateName = state.ToString(),
					UseTransitions = useTransitions
				});
			}
		}

		public enum UserModes
		{
			Undefined = -1,
			Operator = 100,
			Guest = 101,
			Admin = 102
		}

		List<KeyValuePair<UserModes, String>> _userModeList = null;
		[IgnoreDataMember]
		public List<KeyValuePair<UserModes, String>> UserModeList
		{
			get
			{
				if (_userModeList == null)
				{

					_userModeList = new List<KeyValuePair<Camera.UserModes, string>>();
					_userModeList.Add(new KeyValuePair<Camera.UserModes, string>(Camera.UserModes.Admin, IPCameraResources.UserModeAdmin));
					_userModeList.Add(new KeyValuePair<Camera.UserModes, string>(Camera.UserModes.Operator, IPCameraResources.UserModeOperator));
					_userModeList.Add(new KeyValuePair<Camera.UserModes, string>(Camera.UserModes.Guest, IPCameraResources.UserModeGuest));
				}

				return _userModeList;
			}
		}

		[IgnoreDataMember]
		public KeyValuePair<UserModes, String> SelectedUserMode
		{
			get
			{
				var result = UserModeList.Where(um => um.Key == UserModes.Operator).FirstOrDefault();
				return result;
			}
			set
			{
				UserMode = value.Key;
			}
		}

		private IWatchdog _feedWatchDog;
		[IgnoreDataMember]
		public IWatchdog FeedWatchDog
		{
			get { return _feedWatchDog; }
			set
			{
				if (_feedWatchDog != null)
					_feedWatchDog.Elapsed -= _feedWatchDog_Elapsed;

				_feedWatchDog = value;
				_feedWatchDog.Period = TimeSpan.FromSeconds(2);
				_feedWatchDog.Elapsed += _feedWatchDog_Elapsed;
			}
		}

		void _feedWatchDog_Elapsed(object sender, EventArgs e)
		{
			IsBuffering = true;

			if (Buffering != null)
			{
				var dispatcher = SLWIOC.Get<IDispatcherServices>();
				dispatcher.Invoke(() =>
				{
					Buffering(this, null);
				});
			}
		}

		public enum ActionTypes
		{
			None,
			Goto,
			Set
		}

		private bool CheckExpressionForMemberAccess(System.Linq.Expressions.Expression propertyExpression)
		{
			return propertyExpression.NodeType == ExpressionType.MemberAccess;
		}

		public string GetPropertyNameFromExpression<TResult>(Expression<Func<TResult>> propertyExpression)
		{
			System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)propertyExpression.Body;

			if (memberExpression != null)
			{
				return memberExpression.Member.Name;
			}
			else
				throw new ArgumentException("propertyExpression");
		}


		protected void OnPropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
		{
			if (!this.CheckExpressionForMemberAccess(propertyExpression.Body))
				throw new ArgumentException("propertyExpression",
						string.Format("The expected expression is no 'MemberAccess'; its a '{0}'", propertyExpression.Body.NodeType));


			if (propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyNameFromExpression(propertyExpression)));
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			var eventHandler = this.PropertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected bool Set<T>(ref T storage, T value, string columnName = null, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value)) return false;

			storage = value;
			this.RaisePropertyChanged(propertyName);
			return true;
		}


		[DataMember]
		public String TileImageId { get; set; }

		[DataMember]
		public String LockScreenImageId { get; set; }

		[DataMember]
		public Boolean IsNew { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public int Index { get; set; }

		[DataMember]
		public String Manufacture { get; set; }
		[DataMember]
		public String Model { get; set; }

		private UserModes? _userMode;

		[DataMember]
		public UserModes UserMode
		{
			get
			{
				if (_userMode.HasValue)
					return _userMode.Value;

				return UserModes.Undefined;
			}
			set
			{
				if (_userMode != null)
				{
					_userMode = value;
					OnPropertyChanged(() => UserMode);
				}
			}
		}

		[DataMember]
		public List<PresetLocation> Presets { get; set; }

		private String _cameraName;
		[DataMember]
		public String CameraName
		{
			get { return _cameraName; }
			set
			{
				if (_cameraName != value)
				{
					_cameraName = value;
					OnPropertyChanged(() => CameraName);
				}
			}
		}

		private bool _isLockScreenImage;
		[DataMember]
		public bool IsLockScreenImage
		{
			get { return _isLockScreenImage; }
			set
			{
				if (_isLockScreenImage != value)
				{
					_isLockScreenImage = value;
					OnPropertyChanged(() => IsLockScreenImage);
				}
			}
		}


		public enum CameraStates
		{
			Idle,
			Creating,
			Connecting,
			Connected,
			Disconnecting,
			Disconnected,
			StreamTimeout,
			Error
		}

		CameraStates _cameraState = CameraStates.Idle;

		[IgnoreDataMember]
		public CameraStates CameraState
		{
			get
			{
				return _cameraState;
			}
			set
			{
				if (_cameraState != value)
				{
					_cameraState = value;
					OnPropertyChanged(() => CameraState);
				}
			}
		}

		private async void SendUpdate(String setting, String newValue)
		{
			if (TransmitBegin != null)
				TransmitBegin(this, null);

			var url = new Uri(String.Format(APIConfig.SendSettingURI, Url, Port, setting, newValue));
			Debug.WriteLine(url);

			using (var client = GetHttpClient(AuthType.Basic))
			using (var response = await client.GetAsync(url))
			{
				Debug.WriteLine(await response.Content.ReadAsStringAsync());
			}

			if (TransmitEnd != null)
				TransmitEnd(this, null);
		}

		bool _patrollingHorizontally = false;
		[IgnoreDataMember]
		public Boolean IsPatrollingHorizontally
		{
			get { return _patrollingHorizontally; }
			set
			{
				if (_patrollingHorizontally != value)
				{
					_patrollingHorizontally = value;
					OnPropertyChanged(() => IsPatrollingHorizontally);
				}
			}
		}

		bool _patrollingVertically = false;
		[IgnoreDataMember]
		public Boolean IsPatrollingVertically
		{
			get { return _patrollingVertically; }
			set
			{
				if (_patrollingVertically != value)
				{
					_patrollingVertically = value;
					OnPropertyChanged(() => IsPatrollingVertically);
				}
			}
		}

		private bool _invertTilt;
		[DataMember]
		public bool InvertTiltControls
		{
			get { return _invertTilt; }
			set
			{
				if (_invertTilt != value)
				{
					_invertTilt = value;
					OnPropertyChanged(() => InvertTiltControls);
				}
			}
		}

		private String _url;
		[DataMember]
		public String Url
		{
			get { return _url; }
			set
			{
				if (_url != value)
				{
					_url = value;
					OnPropertyChanged(() => Url);
				}
			}
		}

		private string CompositeStaticImageUrl => string.Concat("http://", Url, ":", Port, "/image/jpeg.cgi?ticks={0}");

		private UInt16 _port;
		[DataMember]
		public UInt16 Port
		{
			get { return _port; }
			set
			{
				if (_port != value)
				{
					_port = value;
					OnPropertyChanged(() => Port);
				}
			}
		}

		private String _userName;
		[DataMember]
		public String UserName
		{
			get { return _userName; }
			set
			{
				if (_userName != value)
				{
					_userName = value;
					OnPropertyChanged(() => UserName);
				}
			}
		}

		private String _password;
		[DataMember]
		public String Password
		{
			get { return _password; }
			set
			{
				if (_password != value)
				{
					_password = value;
					OnPropertyChanged(() => Password);
				}
			}
		}

		private bool _supportPanTilt;
		[DataMember]
		public bool SupportPanTilt
		{
			get { return _supportPanTilt; }
			set
			{
				if (_supportPanTilt != value)
				{
					_supportPanTilt = value;
					OnPropertyChanged(() => SupportPanTilt);
				}
			}
		}

		Uri _snapShotImageUri;
		[DataMember]
		public Uri SnapShotImageURI
		{
			get { return _snapShotImageUri; }
			set
			{
				if (_snapShotImageUri != value)
				{
					_snapShotImageUri = value;
					OnPropertyChanged(() => SnapShotImageURI);
				}
			}
		}

		Uri _thumbNameImageUri;
		[DataMember]
		public Uri ThumbNailImageURI
		{
			get { return _thumbNameImageUri; }
			set
			{
				if (_thumbNameImageUri != value)
				{
					_thumbNameImageUri = value;
					OnPropertyChanged(() => ThumbNailImageURI);
				}
			}
		}

		[IgnoreDataMember]
		public Uri MSAppThumbNailImageURI
		{
			get
			{
				if (ThumbNailImageURI != null)
				{
					var fileName = ThumbNailImageURI.Segments[ThumbNailImageURI.Segments.Length - 1];
					var parentFolder = ThumbNailImageURI.Segments[ThumbNailImageURI.Segments.Length - 2];

					var thumbNailAppURI = new Uri(String.Format("ms-appdata:///local/{0}{1}", parentFolder, fileName));
					return thumbNailAppURI;
				}
				return null;
			}
		}

		[IgnoreDataMember]
		int _brightness;
		public int Brightness
		{
			get { return _brightness; }
			set
			{
				if (_brightness != value)
				{
					_brightness = value;
					SendUpdate("1", _brightness.ToString());
				}
			}
		}
		int _contrast;

		[IgnoreDataMember]
		public int Contrast
		{
			get { return _contrast; }
			set
			{
				if (_contrast != value)
				{
					_contrast = value;
					SendUpdate("2", _contrast.ToString());
				}
			}
		}

		[IgnoreDataMember]
		private bool _isTransmitting;
		public bool IsTransmitting
		{
			get { return _isTransmitting; }
			set
			{
				if (_isTransmitting != value)
				{
					_isTransmitting = value;
					var dispatcher = SLWIOC.Get<IDispatcherServices>();
					dispatcher.Invoke(() =>
					{
						OnPropertyChanged(() => IsTransmitting);
					});
				}
			}
		}

		public async void PinCamera()
		{
			var tileServices = SLWIOC.Get<ITileServices>();
			await tileServices.Pin(Id.ToString(), CameraName, String.Format("cameraid={0}", Id), MSAppThumbNailImageURI);
		}

		private async void MoveCamera(Activity activity, String direction)
		{

			var dispatcher = SLWIOC.Get<IDispatcherServices>();
			dispatcher.Invoke(() =>
			{
				if (activity == Activity.Start)
					IsTransmitting = true;

				if (activity == Activity.Start && StartCameraPanTilt != null)
					StartCameraPanTilt(this, null);
			});

			var commandIdx = -1;


			switch (direction.ToLower())
			{
				case "u":
					if (InvertTiltControls)
						commandIdx = (Activity.Start == activity ? 2 : 3);
					else
						commandIdx = (Activity.Start == activity ? 0 : 1);
					break;
				case "d":
					if (InvertTiltControls)
						commandIdx = (Activity.Start == activity ? 0 : 1);
					else
						commandIdx = (Activity.Start == activity ? 2 : 3);
					break;
				case "l": commandIdx = (Activity.Start == activity ? 4 : 5); break;
				case "r": commandIdx = (Activity.Start == activity ? 6 : 7); break;
			}

			Uri uri = null;

			if (APIConfig.APIIndex == 201 || APIConfig.APIIndex == 203)
			{
				var strUrl = String.Empty;
				switch (direction.ToLower())
				{
					case "u": strUrl = String.Format(APIConfig.SendCameraAction, Url, Port, CameraService.STR_ACTION_UP); break;
					case "d": strUrl = String.Format(APIConfig.SendCameraAction, Url, Port, CameraService.STR_ACTION_DOWN); break;
					case "l": strUrl = String.Format(APIConfig.SendCameraAction, Url, Port, CameraService.STR_ACTION_LEFT); break;
					case "r": strUrl = String.Format(APIConfig.SendCameraAction, Url, Port, CameraService.STR_ACTION_RIGHT); break;

				}

				if (activity == Activity.Stop)
					strUrl = String.Format(APIConfig.SendCameraAction, Url, Port, CameraService.STR_ACTION_MOVEEND);

				uri = new Uri(strUrl.Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password));
			}
			else
			{
				uri = GetCameraCommandUri(commandIdx);
			}

			try
			{
				using (var client = GetHttpClient(AuthType.Basic))
				{
					Debug.WriteLine(uri.ToString());
					var response = await client.GetStringAsync(uri);
					Debug.WriteLine(response);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			dispatcher.Invoke(() =>
			{
				if (activity == Activity.Stop)
					IsTransmitting = false;

				if (activity == Activity.Stop && EndCameraPanTitle != null)
					EndCameraPanTitle(this, null);
			});
		}

		public async Task<bool> RefreshCamera()
		{
			if (!Common.PlatformSupport.Services.Network.IsInternetConnected)
				return false;

			try
			{
				using (var stream = await GetSnapshotAsync())
				{
					if (stream != null)
					{
						var fileName = String.Format("{0}pixa.png", Id.ToString().Replace("-", ""));
						if (SnapShotImageURI != null && SnapShotImageURI.ToString().EndsWith("pixa.png"))
							fileName = String.Format("{0}pixb.png", Id.ToString().Replace("-", ""));

						SnapShotImageURI = await Common.PlatformSupport.Services.Storage.StoreAsync(stream, Locations.Local, fileName, "SnapShots");
						LastSnapshotUpdated = DateTime.Now;

						using (var thumbnailStream = await Common.PlatformSupport.Services.Imaging.ResizeImage(stream, 150, 150))
						{
							ThumbNailImageURI = await Common.PlatformSupport.Services.Storage.StoreAsync(thumbnailStream, Locations.Local, fileName, "Thumbnails");
						}

						stream.Seek(0, SeekOrigin.Begin);
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Common.PlatformSupport.Services.Logger.LogException("Camera.Refresh", ex);
			}

			return false;
		}

		public RelayCommand PinCommand { get { return new RelayCommand((parm) => PinCamera()); } }
		public RelayCommand StartMoveCamera { get { return new RelayCommand((parm) => MoveCamera(Activity.Start, parm.ToString())); } }
		public RelayCommand EndMoveCamera { get { return new RelayCommand((parm) => MoveCamera(Activity.Stop, parm.ToString())); } }
		public RelayCommand StartHorizontalPatrol { get { return new RelayCommand((parm) => SendDecoderControlCommand(28)); } }
		public RelayCommand StartVerticalPatrol { get { return new RelayCommand((parm) => SendDecoderControlCommand(26)); } }
		public RelayCommand StopHorizontalPatrol { get { return new RelayCommand((parm) => SendDecoderControlCommand(29)); } }
		public RelayCommand StopVerticalPatrol { get { return new RelayCommand((parm) => SendDecoderControlCommand(27)); } }
		public RelayCommand SetCentralPosition { get { return new RelayCommand((parm) => SendDecoderControlCommand(29)); } }
		public RelayCommand GotoCentralPosition { get { return new RelayCommand((parm) => SendDecoderControlCommand(27)); } }
		public RelayCommand FlipHorizontal
		{
			get
			{
				return new RelayCommand((parm) =>
				{
					if (Flip == FlipValues.None)
						Flip = FlipValues.HorizontalFlip;
					else if (Flip == FlipValues.VerticalFlip)
						Flip = FlipValues.VerticalHorizontalFlip;
					else if (Flip == FlipValues.HorizontalFlip)
						Flip = FlipValues.None;
					else
						Flip = FlipValues.VerticalFlip;

					SendUpdate("5", Convert.ToInt16(Flip).ToString());

				});
			}
		}

		public RelayCommand FlipVertical
		{
			get
			{
				return new RelayCommand((parm) =>
				{
					if (Flip == FlipValues.None)
						Flip = FlipValues.VerticalFlip;
					else if (Flip == FlipValues.HorizontalFlip)
						Flip = FlipValues.VerticalHorizontalFlip;
					else if (Flip == FlipValues.VerticalFlip)
						Flip = FlipValues.None;
					else
						Flip = FlipValues.HorizontalFlip;

					SendUpdate("5", Convert.ToInt16(Flip).ToString());
				});
			}
		}

		public bool SupportSnapShot
		{
			get { return APIConfig.SupportsSnapShot; }
		}

		public AuthenticationHeaderValue CreateBasicHeader()
		{
			byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
			return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}

		public AuthenticationHeaderValue CreateDigestHeader()
		{
			byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
			return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}


		public enum Activity
		{
			Start,
			Stop
		}

		public enum ResolutionValues
		{
			QVGA = 8,
			VGA = 32
		}

		public enum ModeValues
		{
			FiftyHz = 0,
			SixtyHz = 1,
			Outdoor = 2
		}

		public enum FlipValues
		{
			None = 0,
			VerticalFlip = 1,
			HorizontalFlip = 2,
			VerticalHorizontalFlip = 3
		}

		private bool _updateInBackground;
		[DataMember]
		public bool UpdateInBackground
		{
			get { return _updateInBackground; }
			set
			{
				if (_updateInBackground != value)
				{
					_updateInBackground = value;
					OnPropertyChanged(() => UpdateInBackground);
				}
			}
		}

		private ModeValues _mode;
		[IgnoreDataMember]
		public ModeValues Mode
		{
			get { return _mode; }
			set
			{
				if (_mode != value)
				{
					_mode = value;
					SendUpdate("3", Convert.ToInt16(_mode).ToString());
				}
			}
		}

		public FlipValues _flip;
		[IgnoreDataMember]

		public FlipValues Flip
		{
			get { return _flip; }
			set
			{
				if (_flip != value)
				{
					_flip = value;
					SendUpdate("5", Convert.ToInt16(_flip).ToString());
				}
			}
		}

		private ResolutionValues _resolution;
		public ResolutionValues Resolution
		{
			get { return _resolution; }
			set
			{
				if (_resolution != value)
				{
					_resolution = value;
					SendUpdate("0", Convert.ToInt16(_resolution).ToString());
				}
			}
		}

		private void ParseCameraParams(string variables)
		{
			var detailedContents = new StringReader(variables);

			while (true)
			{
				var line = detailedContents.ReadLine();
				if (line == null)
					break;

				var parts = line.Substring(4).Split('=');
				var key = parts[0].ToLower();
				if (parts.Count() > 1)
				{
					var value = parts[1].Replace("'", "").Replace(";", "").Replace("\"", "");
					Debug.WriteLine("{0} ---> {1}", key, value);

					if (parts.Length == 2)
					{
						switch (key)
						{
							case "result":
								if (value == "Auth Failed")
									throw new UnauthorizedAccessException("Unauthorized.");
								break;
							case "brightness": Brightness = Convert.ToInt16(value); break;
							case "contrast": Contrast = Convert.ToInt16(value); break;
							case "alias": CameraName = value; break;
							case "mode":
								switch (value)
								{
									case "0": _mode = ModeValues.FiftyHz; break;
									case "1": _mode = ModeValues.SixtyHz; break;
									case "2": _mode = ModeValues.Outdoor; break;
								}
								break;
							case "resolution":
								switch (value)
								{
									case "8": _resolution = ResolutionValues.QVGA; break;
									case "32": _resolution = ResolutionValues.VGA; break;
								}
								break;
							case "flip":
								switch (value)
								{
									case "0": Flip = FlipValues.None; break;
									case "1": Flip = FlipValues.VerticalFlip; break;
									case "2": Flip = FlipValues.HorizontalFlip; break;
									case "3": Flip = FlipValues.VerticalHorizontalFlip; break;
								}
								break;
						}
					}
				}
			}
		}

		private void ParseXMLParams(String xml)
		{
			using (var strReader = new StringReader(xml))
			{
				var doc = XDocument.Load(strReader);
				CameraName = (from ele
							 in doc.Descendants()
							  where ele.Name == "devName"
							  select ele.Value).FirstOrDefault();

			}

		}


		public enum AuthType
		{
			Basic,
			Digest
		}

		private HttpClient GetHttpClient(AuthType authType)
		{
			var handler = new System.Net.Http.HttpClientHandler();

			var client = new HttpClient();

			if (UserName.Length > 0 && Password.Length > 0)
			{

				switch (authType)
				{
					case AuthType.Basic: client.DefaultRequestHeaders.Authorization = CreateBasicHeader(); break;
					case AuthType.Digest: client.DefaultRequestHeaders.Authorization = CreateDigestHeader(); break;
				}
			}


			return client;
		}

		private async Task<Boolean> GetCGIAPIParams()
		{
			try
			{
				if (!String.IsNullOrEmpty(APIConfig.GetCameraSettings1))
				{
					var url = new Uri(String.Format(APIConfig.GetCameraSettings1, Url, Port, CameraService.STR_GET_DEV_INFO).Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password));

					using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
					using (var response = await client.GetAsync(url))
					{
						if (!response.IsSuccessStatusCode)
							return false;

						if (APIConfig.AreParamsXML)
							ParseXMLParams(await response.Content.ReadAsStringAsync());
						else
							ParseCameraParams(await response.Content.ReadAsStringAsync());
					}
				}

				if (!String.IsNullOrEmpty(APIConfig.GetCameraSettings2))
				{
					var url = new Uri(String.Format(APIConfig.GetCameraSettings2.Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password), Url, Port));
					using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
					using (var response = await client.GetAsync(url))
					{
						if (response.IsSuccessStatusCode)
							ParseCameraParams(await response.Content.ReadAsStringAsync());
					}
				}

				if (!String.IsNullOrEmpty(APIConfig.GetCameraSettings3))
				{
					var url = new Uri(String.Format(APIConfig.GetCameraSettings3.Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password), Url, Port));
					using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
					using (var response = await client.GetAsync(url))
					{
						if (response.IsSuccessStatusCode)
							ParseCameraParams(await response.Content.ReadAsStringAsync());
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		public async Task<Boolean> GetCameraParams()
		{
			if (await GetCGIAPIParams())
				return true;

			return false;
		}

		private Uri GetCameraCommandUri(int idx)
		{
			return new Uri(String.Format(APIConfig.SendCameraAction.Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password), Url, Port, idx));
		}

		private async Task SendDecoderControlCommandAsync(int commandIdx)
		{
			if (TransmitBegin != null)
				TransmitBegin(this, null);

			var url = GetCameraCommandUri(commandIdx);

			using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
			{
				await client.GetAsync(url);
			}

			if (TransmitEnd != null)
				TransmitEnd(this, null);
		}

		private async void SendDecoderControlCommand(int commandIdx)
		{
			switch (commandIdx)
			{
				case 28: IsPatrollingHorizontally = true; break;
				case 29: IsPatrollingHorizontally = false; break;
				case 26: IsPatrollingVertically = true; break;
				case 27: IsPatrollingVertically = false; break;
			}

			IsTransmitting = true;

			if (TransmitBegin != null)
				TransmitBegin(this, null);

			var url = GetCameraCommandUri(commandIdx);

			using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
			{
				await client.GetAsync(url);
			}

			if (TransmitEnd != null)
				TransmitEnd(this, null);

			IsTransmitting = false;
		}

		public async Task CenterCamera()
		{
			await SendDecoderControlCommandAsync(25);
		}

		public async Task PatrolHorizontal(Activity activity)
		{
			await SendDecoderControlCommandAsync(activity == Activity.Start ? 28 : 29);
		}

		public async Task PatrolVertical(Activity activity)
		{
			await SendDecoderControlCommandAsync(activity == Activity.Start ? 26 : 27);
		}


		public async Task<Stream> GetSnapshotAsync()
		{
			try
			{
				using (var client = GetHttpClient(APIConfig.IsDigestAuth ? AuthType.Digest : AuthType.Basic))
				{
					var url = String.Format(APIConfig.SnapshotURI, this.Url, this.Port).Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password);

					using (var response = await client.GetAsync(url))
					using (var stream = await response.Content.ReadAsStreamAsync())
					{
						var buffer = new byte[stream.Length];

						await stream.ReadAsync(buffer, 0, buffer.Length);
						return new MemoryStream(buffer);
					}
				}
			}
			catch (Exception ex)
			{

				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace);

				return null;
			}
		}


		public static Camera Create()
		{
			var camera = new Camera();
			camera.Id = Guid.NewGuid();
			camera.IsNew = true;
			camera._invertTilt = true;
			camera._updateInBackground = true;
			camera._supportPanTilt = true;
			camera._isLockScreenImage = false;
			camera._isConnecting = false;
			camera.Presets = new List<PresetLocation>()
			{
				new     PresetLocation() {PresetIndex=30, PresetName = "1"},
				new     PresetLocation() {PresetIndex=32, PresetName = "2"},
				new     PresetLocation() {PresetIndex=34, PresetName = "3"},
				new     PresetLocation() {PresetIndex=36, PresetName = "4"},
				new     PresetLocation() {PresetIndex=38, PresetName = "5"},
				new     PresetLocation() {PresetIndex=40, PresetName = "6"},
			};

			return camera;
		}

		public List<String> ValidationErrors
		{
			get
			{
				return new List<String>();
			}
		}

		public async Task HandlePreset(ActionTypes actionType, PresetLocation preset)
		{
			var cmdIndex = preset.PresetIndex;

			if (actionType == ActionTypes.Goto)
				cmdIndex++;

			await SendDecoderControlCommandAsync(cmdIndex);
		}

		private void ProcessFrame(byte[] frame)
		{
			var dispatcher = SLWIOC.Get<IDispatcherServices>();
			dispatcher.Invoke(() =>
			{
				if (FrameReady != null)
					FrameReady(this, new FrameReadyEventArgs() { FrameBuffer = frame });

			});

			IsBuffering = false;

			if (_feedWatchDog != null)
				_feedWatchDog.Feed();
		}

		private bool _isConnecting = false;
		public bool IsConnecting
		{
			get { return _isConnecting; }
			set
			{
				if (_isConnecting != value)
				{
					_isConnecting = value;
					OnPropertyChanged(() => IsConnecting);
				}
			}
		}

		// pull down 1024 bytes at a time
		private const int ChunkSize = 1024;

		// used to cancel reading the stream
		private bool _streamActive;

		private void OnGetResponse(IAsyncResult asyncResult)
		{
			Debug.WriteLine("Begin Getting Response");
			byte[] JpegHeader = new byte[] { 0xff, 0xd8 };

			var imageBuffer = new byte[1024 * 1024];
			var req = (HttpWebRequest)asyncResult.AsyncState;

			if (_feedWatchDog != null)
				_feedWatchDog.Enable();

			try
			{
				var resp = (HttpWebResponse)req.EndGetResponse(asyncResult);

				// find our magic boundary value
				string contentType = resp.Headers["Content-Type"];
				if (contentType == "text/plain")
				{
					using (var stream = resp.GetResponseStream())
					using (var rdr = new StreamReader(stream))
					{
						var response = rdr.ReadToEnd();
						Debug.WriteLine(response);
						this.Error(this, new ErrorEventArgs() { Message = response });
						resp.Dispose();
						resp = null;
						return;
					}
				}

				Debug.WriteLine("Returning content type of: " + contentType);

				if (!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
					throw new Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.");

				var boundary = resp.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
				var boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

				var s = resp.GetResponseStream();
				var br = new BinaryReader(s);

				_streamActive = true;

				var buff = br.ReadBytes(ChunkSize);

				var dispatcher = SLWIOC.Get<IDispatcherServices>();
				dispatcher.Invoke(() =>
				{
					CameraState = CameraStates.Connected;
					IsConnecting = false;

					if (StreamConnected != null)
						StreamConnected(this, null);
				});

				while (_streamActive)
				{
					var imageStart = buff.Find(JpegHeader);

					if (imageStart != -1)
					{
						var size = buff.Length - imageStart;
						Array.Copy(buff, imageStart, imageBuffer, 0, size);

						while (true)
						{
							buff = br.ReadBytes(ChunkSize);

							var imageEnd = buff.Find(boundaryBytes);
							if (imageEnd != -1)
							{
								Array.Copy(buff, 0, imageBuffer, size, imageEnd);
								size += imageEnd;

								var frame = new byte[size];
								Array.Copy(imageBuffer, 0, frame, 0, size);

								ProcessFrame(frame);

								Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

								var temp = br.ReadBytes(imageEnd);

								Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
								break;
							}

							Array.Copy(buff, 0, imageBuffer, size, buff.Length);
							size += buff.Length;
						}
					}
					else
					{
						var msg = System.Text.UTF8Encoding.UTF8.GetString(buff, 0, buff.Length);
						Debug.WriteLine(msg);
					}
				}

				if (_feedWatchDog != null)
					_feedWatchDog.Disable();

				req.Abort();
				resp.Dispose();
				resp = null;

				dispatcher.Invoke(() =>
				{
					if (StreamDisconnected != null)
						StreamDisconnected(this, null);
				});
			}
			catch (Exception ex)
			{
				if (_feedWatchDog != null)
					_feedWatchDog.Disable();

				var dispatcher = SLWIOC.Get<IDispatcherServices>();
				dispatcher.Invoke(() =>
				{
					CameraState = CameraStates.Error;

					if (Error != null)
						Error(this, new ErrorEventArgs() { Message = ex.Message });
				});
			}
		}


		[DataMember]
		public DateTime LastSnapshotUpdated
		{
			get;
			set;
		}

		private int _camerAPI_Id;
		[DataMember]
		public int CameraAPI_Id
		{
			get { return _camerAPI_Id; }
			set
			{

				if (_camerAPI_Id != value)
				{
					_camerAPI_Id = value;
					OnPropertyChanged(() => CameraAPI_Id);
				}
			}
		}

		CameraAPIConfig _cameraAPIConfig = null;

		HttpWebRequest _activeReqeust;

		public async Task DownloadImage()
		{
			var request = System.Net.HttpWebRequest.CreateHttp(string.Format(CompositeStaticImageUrl, DateTime.Now.ToFileTime()));

			if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
				request.Credentials = new NetworkCredential(UserName, Password);

			using (var response = await request.GetResponseAsync())
			using (var stream = response.GetResponseStream())
			using (var ms = new MemoryStream())
			using (var randomAccessStream = new InMemoryRandomAccessStream())
			{
				stream.CopyTo(ms);
				var buffer = ms.ToArray();

				var writeStream = randomAccessStream.AsStreamForWrite();
				await writeStream.WriteAsync(buffer, 0, buffer.Count());
				await writeStream.FlushAsync();

				randomAccessStream.Seek(0);
				var image = new BitmapImage();
				await image.SetSourceAsync(randomAccessStream);

				CurrentPicture = image;
			}
		}

		public async void StartCameraStream()
		{
			await GetCGIAPIParams();

			var setStreamType = String.Format("http://{0}:{1}/cgi-bin/CGIProxy.fcgi?cmd=setSubStreamFormat&format=1&usr={2}&pwd={3}", Url, Port, UserName, Password);

			var client = new HttpClient();
			var response = client.GetStringAsync(setStreamType);
			Debug.WriteLine(response);

			var dispatcher = SLWIOC.Get<IDispatcherServices>();
			dispatcher.Invoke(() =>
			{
				IsConnecting = true;
				CameraState = CameraStates.Error;

				if (StreamConnecting != null)
					StreamConnecting(this, null);
			});

			var url = String.Format(APIConfig.VideoURI, Url, Port).Replace("[USERNAME]", UserName).Replace("[PASSWORD]", Password);
			Debug.WriteLine(url);

			_activeReqeust = (HttpWebRequest)WebRequest.Create(url);

			if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
				_activeReqeust.Credentials = new NetworkCredential(UserName, Password);

			_activeReqeust.AllowReadStreamBuffering = false;

			_activeReqeust.BeginGetResponse(OnGetResponse, _activeReqeust);
		}

		public void CloseCameraStream()
		{
			_streamActive = false;
		}

		private bool _buffering = false;
		[IgnoreDataMember]
		public Boolean IsBuffering
		{
			get { return _buffering; }
			set
			{
				if (_buffering != value)
				{
					_buffering = value;
					Common.PlatformSupport.Services.DispatcherServices.Invoke(() =>
					{
						OnPropertyChanged(() => IsBuffering);
					});
				}
			}
		}

		ImageSource _currentPicture;

		public ImageSource CurrentPicture
		{
			set { Set(ref _currentPicture, value); }
			get { return _currentPicture; }
		}

		private TimeSpan _updateInterval;
		public TimeSpan UpdateInterval
		{
			get { return _updateInterval; }
		}

		[IgnoreDataMember]
		public CameraAPIConfig APIConfig
		{
			get
			{
				if (_cameraAPIConfig == null)
					_cameraAPIConfig = CameraDataServices.Instance.CameraAPIConfigs.Configs.Where(cfg => cfg.APIIndex == CameraAPI_Id).First();

				return _cameraAPIConfig;
			}
			set
			{
				_camerAPI_Id = value.APIIndex;
				_cameraAPIConfig = value;
			}
		}
	}

	static class Extensions
	{
		public static int Find(this byte[] buff, byte[] search)
		{
			// enumerate the buffer but don't overstep the bounds
			for (int start = 0; start < buff.Length - search.Length; start++)
			{
				// we found the first character
				if (buff[start] == search[0])
				{
					int next;

					// traverse the rest of the bytes
					for (next = 1; next < search.Length; next++)
					{
						// if we don't match, bail
						if (buff[start + next] != search[next])
							break;
					}

					if (next == search.Length)
						return start;
				}
			}
			// not found
			return -1;
		}
	}

	public class StateChangedEventArgs : EventArgs
	{
		public string StateName { get; set; }
		public bool UseTransitions { get; set; }
	}

	public class FrameReadyEventArgs : EventArgs
	{
		public byte[] FrameBuffer;
	}

	public sealed class ErrorEventArgs : EventArgs
	{
		public string Message { get; set; }
		public int ErrorCode { get; set; }
	}
}