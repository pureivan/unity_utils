using UnityEngine;
using System;

#if UNITY_ANDROID
namespace dpull
{
    public class AndroidAssetStream : System.IO.Stream
    {
        AndroidJavaObject AndroidInputStream;
        long AndroidInputStreamLength;
         
        public AndroidAssetStream(string fileName)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var assetManager = activity.Call<AndroidJavaObject>("getAssets")) //android.content.res.AssetManager
                    {
                        using (var assetFileDescriptor = assetManager.Call<AndroidJavaObject>("openFd", fileName)) //assets/ //android.content.res.AssetFileDescriptor
                        {
                            AndroidInputStreamLength = assetFileDescriptor.Call<long>("getLength");
                        }
                        AndroidInputStream = assetManager.Call<AndroidJavaObject>("open", fileName);
                    }
                }
            }
            
            if (AndroidInputStream == null)
                throw new System.IO.FileNotFoundException("getAssets failed", fileName);
        }
        
        public override void Flush ()
        {
            throw new NotImplementedException();
        }

    	public override int Read(byte[] buffer, int offset, int count)
    	{
    		return Read(AndroidInputStream, buffer, offset, count);
    	}

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
        
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        
        public override bool CanRead { get { return AndroidInputStream != null; } }
        public override bool CanSeek { get { return false;} }
        public override bool CanWrite { get { return false;} }
        public override long Length  { get { return AndroidInputStreamLength; } }
        
        public override long Position
        {
            get 
            {
                throw new NotImplementedException ();
            }
            set 
            {
                throw new NotImplementedException ();
            }
        }   


    	int Read(AndroidJavaObject javaObject, byte[] buffer, int offset, int count)
    	{
    		var args = new object[]{buffer, offset, count};
    		IntPtr methodID = AndroidJNIHelper.GetMethodID<int>(javaObject.GetRawClass(), "read", args, false);
    		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
    		try
    		{
    			var readLen = AndroidJNI.CallIntMethod(javaObject.GetRawObject(), methodID, array);
    			for (var i = 0; i < readLen; ++i)
    			{
    				buffer[i] = AndroidJNI.GetByteArrayElement(array[0].l, i);
    			}
    			return readLen;
    		}
    		finally
    		{
    			AndroidJNIHelper.DeleteJNIArgArray(args, array);
    		}
    	}
    }
}
#endif