# Sprites
###### Maintainer: [Gikode]()

**所有游戏内精灵素材**

<hr/>

### 目录说明
> 1. **UI: 用于用户界面**
>> `UI/weapon icons`: 用于武器缩略图  
>> `UI/ammo numbers`: 用于显示武器弹药的字体图片  
> 2. **Scrolls: 用于菜单**
> 3. **Player: 玩家素材, 内有各种状态**
> 4. **NPC: NPC的各种素材**

### 统一规范 (Standardized)
> 0. 实体请加上一个像素的描边(RGB: 24,20,37), 人物(RGB: 0,0,0)
> 1. **所有图片必须为 `.png` 格式**
> 2. **整体大图置于 `Sprites/` 下, 分类小图置于相关类型(图像所表现类型而非图片格式类型)的文件夹下 `Sprites/TypeOfImage`**
> 3. **大图命名首字母大写; 分类小图命名全部小写, 下划线分割; 文件夹首字母大写驼峰命名**
> 4. **分类小图命名 `type_description_frame.png`, eg: `coins_pickup_0.png`**
> 5. **TileMap 基准大小为 **16px * 16px****
> 6. **置于地面上的不超过一个基准大小的物体, 也必须将其设置为 **16px * 16px****
> 7. **对于超过一个基准大小的物体, 可以使用更大且合适的像素, 避免进行拼接**
> 8. **对于放置在地面上的微小装饰物, 可以将其与地面同化, 在 `Adobe Photo Shop` 中处理成一张图片 (**16px * 16px**)**
> 9. **对于有影子(Shadow)的实体, 请提前在图中画出**

### Tile层级说明
> 1. **BG: 地面**
> 2. **SolidsBack: 墙面**
> 3. **SolidsFront: 墙顶**
> 4. **Prefabs: 实体**
> 5. **Pit: 陷阱**  
**ps: 请勿私自添加 Tile 层级, 如果需要使用叠层来实现得静态效果, 请使用 `Adobe Photo Shop`, 动态请使用实体而非 Tile Map**

# 通用规定
> 1. 所有可交互的物体不能使用 `TileMap`
> 2. 所有有动画, 可操作的陷阱独立
> 3. ...

### 实体
> 0. **所有实体置于 `Prefabs` 层下, [Tile 层级说明](#tile层级说明)**
> 1. **物体大小不受限制**0
> 2. **无动画物体命名 `itemType_status_frame.png`, eg: `desk_common_0.png`**
> 3. **带物体动画参考 [物体动画](#物体动画)**
> 4. **可破坏物体使用 `Health.cs` 实现, 根据代码添加血量机制而非动画**
> 5. **交互式动画参考 `WeaponChest.cs`**
> 6. **非特殊情况, 实体必须带有影子(Shadow)**

### Weapon 武器
**ps1: ./Weapon 下, 使用武器名称(大写, 驼峰命名)建立文件夹进行分类**  
**ps2: weaponType: 1. gun; 2. Melee**
> 1. 在具体的武器文件夹下, 必须存在的文件: icon, inHands
>> icon(武器图标): `weaponName_weaponType_icon_frame.png`  
>> inHands(武器拿在手上的样子): `.weaponName_weaponType_inHands_framepng`  
> 2. 上述文件夹下, 如果武器是枪械, 存在三个文件: bullet, muzzle, projectile  
>> bullet(子弹): `weaponName_bullet_projectile_frame.png`  
>> muzzle(枪口闪光): `weaponName_muzzle_muzzleFlash_frame.png`  
>> projectile(抛射物): `weaponName_projectile_projectile_frame.png`  
> 3. 上述文件夹下, 如果武器非枪械  
>> icon(武器图标): `weaponName_weaponType_icon_frame.png`  
>> inHands(武器拿在手上的样子): `.weaponName_weaponType_inHands_framepng`  
> 4. 子弹:
>> 击中时: `on hit_0.png` ~ `on hit_2.png`, 共 3 帧  
>> 子弹三种状态:
>>> bullet_projectile_0.png  
>>> muzzle_muzzle flash_0.png  
>>> projectile_projectile_0.png

### NPC像素人物规定
> 1. 大小 **16px * 32px**, 需要正, 左右两张图 (特殊情况分左右共三张图片)
>> 命名规范:
>>> 正面: `npcName_front_0.png`  
>>> 侧面(左): `npcName_side_l_0.png`  
>>> 侧面(右): `npc_Name_side_r_0.png`
> 2. 人物形象参考**中世纪欧洲像素游戏**
> 3. NPC 人物必须带有 IDLE 待机动画, 规范参考[NPC像素人物动画](#npc像素人物动画)

### Player像素人物拆分规定
> 1. 像素人无上肢图片  
> 2. 像素人正面图片  
> 3. 像素人侧身图片  

### Player 素材规定
> 1. 像素人无上肢图片  
> 2. 像素人全身图片  
> 3. 像素人侧身图片  
**--动画**
> 4. 待机动画(无上肢 和 有上肢)  
> 5. 移动动画(无上肢)  
> 6. 滚动动画  
> 7. 被击中动画 (无上肢)
> 8. 死亡动画
> 9. 掉落到坑动画

# **Animation**

### 物体动画
> 1. 命名: `itemType_whichAnimation_frame.png` 帧数不限

### 武器动画
> 1. 待定

### Player像素人物动画
**ps: 对于 Player 像素人物必须将其拆分, 参考[Player像素人物拆分规定](#player像素人物拆分规定)**  
> 1. 人物死亡动画: `death_player death 1_00.png` ~ `death_player death 1_10.png`, 共 11 帧
> 2. 人物掉落动画: `pitfall_fall anim_00.png` ~ `pitfall_fall anim_13.png`, 共 14 帧
> 3. 人物待机动画: 
>> 无武器状态待机: `idle without gun_idle_0.png` ~ `idle without gun_idle_4.png`, 共 5 帧
> 5. 人物滚动: roll anim_roll_0.png ~ roll anim_roll_5.png, 共 6 帧 (无武器)
> 6. 跑动: 
>> 无武器跑动: run without gun_run_0.png ~ run without gun_run_7.png, 共 8 帧
> 7. 人物被击中: take hit_0.png ~ take hit_1.png, 共 2 帧

### NPC像素人物动画
**站立不动对话式npc**
> 0. 阴影单独动画
>> npcName_shadow_idle_0.png ~ npcName_shadow_idle_4.png, 共 5 帧
> 1. 人物待机动画: 
>> 正面待机: npcName_idle_front_0.png ~ npcName_idle_front_4.png, 共 5 帧  
>> 侧(左)面待机: npcName_idle_side_l_0.png ~ npcName_idle_side_l_4.png, 共 5 帧  
>> 侧(右)面待机: npcName_idle_side_r_0.png ~ npcName_idle_side_r_4.png, 共 5 帧