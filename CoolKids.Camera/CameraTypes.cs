using CoolKids.Camera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera
{
    public class CameraTypes
    {
        public static CameraAPIConfig GetCGI()
        {
            return new CameraAPIConfig()
            {
                APIIndex = 200,
                ProfileName = "Modern CGI Standard",
                Description = "This profile should work with modern IP Cameras that implment the IP Camera CGI API.",
                SendSettingURI = "http://{0}:{1}/camera_control.cgi?param={2}&value={3}",
                SendCameraAction = "http://{0}:{1}/decoder_control.cgi?command={2}",
                GetCameraSettings1 = "http://{0}:{1}/get_camera_params.cgi",
                GetCameraSettings2 = "http://{0}:{1}/get_params.cgi",
                GetCameraSettings3 = "http://{0}:{1}/get_misc.cgi",
                VideoURI = "http://{0}:{1}/videostream.cgi?resolution=32&rate=0",
                SnapshotURI = "http://{0}:{1}/snapshot.cgi",
                SupportsSnapShot = true,
                AreParamsXML = false,
                IsDigestAuth = false
            };
        }

        public static CameraAPIConfig GetFoscamHD()
        {
            return new CameraAPIConfig()
            {
                APIIndex = 201,
                ProfileName = "FOSCAM HD",
                Description = "This profile should work with modern IP Cameras that implment the IP Camera CGI API.",
                SendSettingURI = "http://{0}:{1}/camera_control.cgi?param={2}&value={3}",
                SendCameraAction = "http://{0}:{1}/cgi-bin/CGIProxy.fcgi?usr=[USERNAME]&pwd=[PASSWORD]&cmd={2}",
                GetCameraSettings1 = "http://{0}:{1}/cgi-bin/CGIProxy.fcgi?usr=[USERNAME]&pwd=[PASSWORD]&cmd={2}",
                GetCameraSettings2 = String.Empty,
                GetCameraSettings3 = String.Empty,
                VideoURI = "http://{0}:{1}/cgi-bin/CGIStream.cgi?cmd=GetMJStream&usr=[USERNAME]&pwd=[PASSWORD]",
                SnapshotURI = "http://{0}:{1}/cgi-bin/CGIProxy.fcgi?cmd=snapPicture2&usr=[USERNAME]&pwd=[PASSWORD]",
                SupportsSnapShot = true,
                AreParamsXML = true,
                IsDigestAuth = false
            };
        }

        public static CameraAPIConfig GetDigestCGI()
        {
            return new CameraAPIConfig()
            {
                APIIndex = 202,
                ProfileName = "Modern CGI Standard",
                Description = "This profile should work with modern IP Cameras that implment the IP Camera CGI API.",
                SendSettingURI = "http://{0}:{1}/camera_control.cgi?param={2}&value={3}",
                SendCameraAction = "http://{0}:{1}/decoder_control.cgi?command={2}",
                GetCameraSettings1 = "http://{0}:{1}/get_camera_params.cgi",
                GetCameraSettings2 = "http://{0}:{1}/get_params.cgi",
                GetCameraSettings3 = "http://{0}:{1}/get_status.cgi",
                VideoURI = "http://{0}:{1}/videostream.cgi?resolution=32&rate=0",
                SnapshotURI = "http://{0}:{1}/snapshot.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                SupportsSnapShot = true,
                AreParamsXML = false,
                IsDigestAuth = true

            };
        }

        public static CameraAPIConfig GetCGI2()
        {
            return new CameraAPIConfig()
            {
                APIIndex = 203,
                ProfileName = "Modern CGI2 Standard",
                Description = "This profile should work with modern IP Cameras that implment the IP Camera CGI API.",
                SendSettingURI     = "http://{0}:{1}/get_status.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                SendCameraAction   = "http://{0}:{1}/decoder_control.cgi?user=[USERNAME]&pwd=[PASSWORD]&param={2}&value={3}",
                GetCameraSettings1 = "http://{0}:{1}/get_camera_params.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                GetCameraSettings2 = "http://{0}:{1}/get_params.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                GetCameraSettings3 = "http://{0}:{1}/get_misc.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                VideoURI = "http://{0}:{1}/videostream.cgi?resolution=32&rate=0",
                SnapshotURI = "http://{0}:{1}/snapshot.cgi?user=[USERNAME]&pwd=[PASSWORD]",
                SupportsSnapShot = true,
                AreParamsXML = false,
                IsDigestAuth = false
            };
        }

        public static CameraAPIConfig GetAMCrest()
        {
            return new CameraAPIConfig()
            {
                APIIndex = 204,
                ProfileName = "AMCrest",
                Description = "This profile should work with modern IP Cameras that implment the IP Camera CGI API.",
                SendSettingURI = "http://{0}:{1}/get_status.cgi",
                SendCameraAction = "http://{0}:{1}/decoder_control.cgi?param={2}&value={3}",
                GetCameraSettings1 = "http://{0}:{1}/cgi-bin/configManager.cgi?action=getConfig&name=General",
                GetCameraSettings2 = "http://{0}:{1}/get_params.cgi",
                GetCameraSettings3 = "http://{0}:{1}/get_misc.cgi",
                VideoURI = "http://{0}:{1}/cgi-bin/mjpg/video.cgi",
                SnapshotURI = "http://{0}:{1}/cgi-bin/snapshot.cgi",
                SupportsSnapShot = true,
                AreParamsXML = false,
                IsDigestAuth = true
            };
        }



    }
}
