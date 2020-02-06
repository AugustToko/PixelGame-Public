namespace PixelGameAssets.Scripts.EventManager
{
    /// <summary>
    /// 事件回调
    /// </summary>
    public delegate void CallBack();
    public delegate void CallBack<in T>(T arg);
    public delegate void CallBack<in T, in TX>(T arg1, TX arg2);
    public delegate void CallBack<in T, in TX, in TY>(T arg1, TX arg2, TY arg3);
    public delegate void CallBack<in T, in TX, in TY, in TZ>(T arg1, TX arg2, TY arg3, TZ arg4);
    public delegate void CallBack<in T, in TX, in TY, in TZ, in TW>(T arg1, TX arg2, TY arg3, TZ arg4, TW arg5);
}