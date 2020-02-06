# Scripts
Game Scripts (c#)

<hr>

**每个单独的子模块(WeaponSystem/*)均需要添加 README.md**
**Debug 目录下脚本请在项目设置中调整为最后加载**

<hr>

**模块:**
> 1. **Camera**
>> 摄像机抖动  
>> 摄像机视角跟随  
> 2. **Core**
>> Actor 规范  
>> 音乐、音效管理(工具类)  
>> Calc 计算辅助类  
>> 角色朝向  
>> 游戏管理类(全局控制一些实体或实现功能)  
>> Player 玩家定义  
>> 游戏分数管理类  
> 3. **Damage**
>> 伤害逻辑处理  
>> Actor 生命值管理  
> 4. **DebugUtil**
>> 屏幕调试信息显示  
>> 自定义log工具  
> 5. **Enemies**
>> 敌人抽象及其细节实现  
> 6. **EventManager**
>> 全局事件监听  
> 7. **GKUtils**
>> 杂项工具类  
> 8. **Input**
>> 操作输入管理类  
> 9. **Misc**
>> 动画事件监听类  
>> 精灵闪光(枪口)  
>> 玩家准星(Desktop 平台)  
> 10. **Entity**
>> 游戏内障碍物  
>> 关卡内 Door 的实现  
>> 关卡内陷阱  
> 11. **Pickups**
>> 游戏内可拾取物体  
> 12. **SceneLoader**
>> 场景加载器  
> 13. **Skill**
>> 技能框架  
> 14. **UI**
>> 游戏内 UI 逻辑, 事件等  
>> 自定义控件  
> 15. **WeaponSystem**
>> 武器实现  
>> 子弹实现  
>> 武器技能  

**NOTE:**
> 1. 待处理代码请使用 TODO
> 2. 待修复请使用 FIXME

**API 规范**  
*Log*
> 使用 GKLog(TAG, MESSAGE)
>> TAG 为当前方法名称