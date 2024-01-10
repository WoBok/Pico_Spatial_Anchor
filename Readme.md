### 空间锚点相关接口
|接口|说明|
|---|---|
|CreateAnchorEntity|在应用内存中创建锚点|
|DestroyAnchorEntity|销毁应用内存中的指定锚点|
|PersistAnchorEntity|将锚点存储至设备的本地内存。
|UnPersistAnchorEntity|删除设备本地内存中的指定锚点|
|ClearPersistedAnchorEntity|清空设备本地内存中的所有锚点|
|GetAnchorPose|获取锚点姿态|
|GetAnchorEntityUuid|获取锚点的 UUID|
|LoadAnchorEntityByUuidFilter|根据 UUID 加载锚点|
|LoadAnchorEntityBySceneFilter|根据空间场景数据类型加载锚点|
### 空间标定相关接口
|接口|说明|
|---|---|
|StartSpatialSceneCapture|启用|
|Room Capture|应用进行空间标定|
|GetAnchorComponentFlags|获取锚点支持的组件|
|GetAnchorSceneLabel|获取锚点的场景标签|
|GetAnchorVolumeInfo|获取锚点的立方体信息|
|GetAnchorPlaneBoundaryInfo|获取锚点的矩形平面信息|
|GetAnchorPlanePolygonInfo|获取锚点的非矩形平面信息|
|GetAnchorEntityLoadResults|获取锚点的加载结果|
### 锚点相关事件
```
// 锚点创建事件
public static event Action<PxrEventAnchorEntityCreated> AnchorEntityCreated;
// 空间追踪更新事件，用于判断当前空间定位是否准确
public static event Action<PxrEventSpatialTrackingStateUpdate> SpatialTrackingStateUpdate;
// 锚点持久化事件
public static event Action<PxrEventAnchorEntityPersisted> AnchorEntityPersisted;
// 锚点反持久化事件
public static event Action<PxrEventAnchorEntityUnPersisted> AnchorEntityUnPersisted;
// 锚点清空事件
public static event Action<PxrEventAnchorEntityCleared> AnchorEntityCleared;
// 锚点加载事件
public static event Action<PxrEventAnchorEntityLoaded> AnchorEntityLoaded;
// 空间标定事件
public static event Action<PxrEventSpatialSceneCaptured> SpatialSceneCaptured;
```
### 锚点相关标签
|标签|说明|
|---|---|
|UnKnown|其他|
|Floor|地板|
|Ceiling|天花板|
|Wall|墙面|
|Door|门|
|Window|暂未使用|
|Opening|暂未使用|
|Table|桌子|
|Sofa|沙发|
|Object|所有三维物体都为 Object|

**Unity Version:** 2021.3.16f1c1
**Render Pipeline:** URP
**Pico SDK Version:** 2.4.3
