using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;

using Android.Widget;
using Android.OS;
using Android.Views;
using Java.Util;
using Stream = System.IO.Stream;

namespace CarApp
{
    public enum Direction : int
    {
        FORWARD = 10,
        RIGHT = 11,
        STOP = 12,
        LEFT = 13,
        BACKWARD = 14
    }


    [Activity(Label = "CarApp", MainLauncher = true, Icon = "@drawable/icon", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : Activity
    {
        private Stream outStream = null;

        private Button _forwardButton = null;
        private Button _backwardButton = null;
        private Button _rightButton = null;
        private Button _leftButton = null;
        private Button _stopButton = null;
        private Button _turnOffButton = null;
        private Button _searchButton = null;
        private Button _sendTextButton = null;
        private EditText _textData = null;
        private SeekBar _speedSlider = null;
    

        private BluetoothAdapter myBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        private ListView devicesList;

        private List<BluetoothDevice> devices = new List<BluetoothDevice>();
        private BluetoothDevice selectedDevice = null;
        private UUID myUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            initalizeMainButtons();

        }

        private void initalizeMainButtons()
        {
            SetContentView(Resource.Layout.Main);
            _searchButton = FindViewById<Button>(Resource.Id.buttonSearch);
            devicesList = FindViewById<ListView>(Resource.Id.deviceList);
            

              DeviceListAdapter listAdapter = new DeviceListAdapter(this, devices);
              devicesList.Adapter = listAdapter;

            _searchButton.Click += SearchButton_Click;
            devicesList.ItemClick += DevicesList_ItemClick;
        }

        private void initializeControlButtons()
        {
            _forwardButton = FindViewById<Button>(Resource.Id.buttonForward);
            _stopButton = FindViewById<Button>(Resource.Id.buttonStop);
            _backwardButton = FindViewById<Button>(Resource.Id.buttonBackward);
            _rightButton = FindViewById<Button>(Resource.Id.buttonRight);
            _leftButton = FindViewById<Button>(Resource.Id.buttonLeft);
            _turnOffButton = FindViewById<Button>(Resource.Id.buttonTurnOff);
            _sendTextButton = FindViewById<Button>(Resource.Id.buttonSendText);
            _textData = FindViewById<EditText>(Resource.Id.dataText);
            _speedSlider = FindViewById<SeekBar>(Resource.Id.speedSlider);

            _forwardButton.Touch += _forwardButton_Touch;
            _backwardButton.Touch += _backwardButton_Touch;
            _leftButton.Touch += _leftButton_Touch;
            _rightButton.Touch += _rightButton_Touch;
            _stopButton.Touch += _stopButton_Touch;
            _turnOffButton.Click += _turnOffButton_Click;
            _sendTextButton.Click += _sendTextButton_Click;
            _speedSlider.ProgressChanged += _speedSlider_ProgressChanged;
        }

        private void _speedSlider_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            int speed = _speedSlider.Progress;
            SendData(9);
            SendData(speed);
        }

        private void _sendTextButton_Click(object sender, EventArgs e)
        {
            SendData(_textData.Text);
        }

        private void _forwardButton_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if(e.Event.Action == MotionEventActions.Down)
                 SendData((int)Direction.FORWARD);
            else if (e.Event.Action == MotionEventActions.Up)
            {
                SendData((int) Direction.STOP);
            }

        }

        private void _turnOffButton_Click(object sender, EventArgs e)
        {
            btSocket.Close();
            initalizeMainButtons();
        }

        private void _stopButton_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            SendData((int)Direction.STOP);
        }

        private void _rightButton_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
                SendData((int)Direction.RIGHT);
            else if (e.Event.Action == MotionEventActions.Up)
            {
                SendData((int)Direction.STOP);
            }
        }

        private void _leftButton_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
                SendData((int)Direction.LEFT);
            else if (e.Event.Action == MotionEventActions.Up)
            {
                SendData((int)Direction.STOP);
            }
        }

        private void _backwardButton_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
                SendData((int)Direction.BACKWARD);
            else if (e.Event.Action == MotionEventActions.Up)
            {
                SendData((int)Direction.STOP);
            }
        }

        public void SendData(string data)
        {
            if (outStream == null)
            {
                try
                {
                    outStream = btSocket.OutputStream;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            

            byte[] bytes = Encoding.ASCII.GetBytes(data);

            try
            {
                outStream?.Write(bytes,0,data.Length);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
            }
        }

        public void SendData(int data)
        {
            if (outStream == null)
            {
                try
                {
                    outStream = btSocket.OutputStream;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            try
            {
                outStream?.WriteByte(Convert.ToByte(data));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        }

        private void DevicesList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            BluetoothDevice device = devices[e.Position];

            myBluetoothAdapter.CancelDiscovery();
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(myUUID);
                btSocket.Connect();

                Toast.MakeText(this, "Polaczono", ToastLength.Short).Show();

                SetContentView(Resource.Layout.controll);
                initializeControlButtons();
            }
            catch ( Exception exception)
            {

                btSocket.Close();
                Toast.MakeText(this, "Nie polaczono...", ToastLength.Short).Show();

                Console.WriteLine(exception.Message);
            }
        }

        private void SearchButton_Click(object sender, System.EventArgs e)
        {
            LookForDevices();
        }

        public void LookForDevices()
        {
            myBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (myBluetoothAdapter == null)
            {
                Toast.MakeText(this, "Nie wykryto adaptera bluetooth", ToastLength.Short).Show();
            }
            else if (!myBluetoothAdapter.IsEnabled)
            {
                Toast.MakeText(this, "Buletooth jest wylaczone", ToastLength.Short).Show();
                var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, 2);
            }
            else
            {
                var _devices = myBluetoothAdapter.BondedDevices;
                devices = _devices.ToList();
                DeviceListAdapter listAdapter = new DeviceListAdapter(this, devices);
                devicesList.Adapter = listAdapter;

            }

        }
    }
}

