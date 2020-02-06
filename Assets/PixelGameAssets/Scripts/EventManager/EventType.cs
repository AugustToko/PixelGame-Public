using System;

namespace PixelGameAssets.Scripts.EventManager
{
    /// <summary>
    /// 事件类型
    /// </summary>
//    [Obsolete("使用具体化事件, 而非统一称为'EventType'.")]
    public enum EventType
    {
        /// <summary>
        ///     打开关卡内的门
        /// </summary>
        OpenDoor,



        BeginLoadScene,

        #region Ui-Canvas
        
        /// <summary>
        /// 升级
        /// </summary>
        UpdateLevel,
        
        [Obsolete]
        UpdateCoin,
        
        /// <summary>
        ///     更新音乐信息
        /// </summary>
        SetUpMusicInfo,

        /// <summary>
        /// 更新武器弹药
        /// </summary>
        UpdateAmmo,

        /// <summary>
        /// 更新连击数
        /// </summary>
        UpdateHits,
        
        /// <summary>
        /// 更新系统事件
        /// </summary>
        UpdateSystemEvent,

        #endregion

        ShowNotification,
        
        NotifyHide
    }
}