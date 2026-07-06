using UnityEngine;

/// <summary>
/// 天气管理系统 - 控制三种天气状态切换
/// 切换方式：键盘数字1、2、3对应晴天/雨天/雪天
/// </summary>
public class WeatherManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例，全局唯一访问点
    /// </summary>
    public static WeatherManager Instance;

    /// <summary>
    /// 天气类型枚举
    /// </summary>
    public enum WeatherType
    {
        Sunny,   // 晴天
        Rainy,   // 雨天
        Snowy,   // 雪天
        Sunset   // 晚霞
    }

    [Header("粒子系统")]
    [Tooltip("下雨粒子效果（ParticleSystem组件所在的GameObject）")]
    public GameObject rainParticle;
    [Tooltip("下雪粒子效果（ParticleSystem组件所在的GameObject）")]
    public GameObject snowParticle;
    [Tooltip("喷泉粒子（永久显示，不受天气影响）")]
    public GameObject fountainParticle;

    [Header("相机引用")]
    [Tooltip("主相机（用于强制刷新天空盒渲染）")]
    public Camera mainCamera;

    [Header("天空盒材质")]
    [Tooltip("晴天天空盒材质（Skybox Shader）")]
    public Material sunnySkybox;
    [Tooltip("雨雪天天空盒材质（Skybox Shader）")]
    public Material rainSnowSkybox;
    [Tooltip("晚霞天空盒材质（Skybox Shader）")]
    public Material sunsetSkybox;

    [Header("环境光照设置")]
    [Tooltip("晴天环境光强度（0-1）")]
    public float sunnyAmbientIntensity = 1.0f;
    [Tooltip("雨雪天环境光强度（0-1）")]
    public float rainSnowAmbientIntensity = 0.5f;
    [Tooltip("晚霞环境光强度（0-1）")]
    public float sunsetAmbientIntensity = 0.7f;

    [Header("默认天气")]
    [Tooltip("游戏启动时的初始天气状态")]
    public WeatherType defaultWeather = WeatherType.Sunny;

    /// <summary>
    /// 当前天气状态
    /// </summary>
    private WeatherType currentWeather;

    /// <summary>
    /// 单例模式初始化，确保切换场景不丢失
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化天气系统
    /// </summary>
    private void Start()
    {
        InitializeWeather();
    }

    /// <summary>
    /// 初始化天气状态
    /// - 自动查找主相机
    /// - 喷泉粒子永久开启
    /// - 设置默认天气
    /// </summary>
    private void InitializeWeather()
    {
        // 自动查找主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }

        // 喷泉粒子永久显示，不受天气影响
        if (fountainParticle != null)
        {
            fountainParticle.SetActive(true);
        }

        // 设置初始天气
        SetWeather(defaultWeather);
    }

    /// <summary>
    /// 更新循环，处理键盘输入
    /// </summary>
    private void Update()
    {
        HandleWeatherInput();
    }

    /// <summary>
    /// 处理天气切换键盘输入
    /// - 数字键1：晴天
    /// - 数字键2：雨天
    /// - 数字键3：雪天
    /// - 数字键4：晚霞
    /// </summary>
    private void HandleWeatherInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeather(WeatherType.Sunny);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeather(WeatherType.Rainy);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeather(WeatherType.Snowy);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWeather(WeatherType.Sunset);
        }
    }

    /// <summary>
    /// 设置天气状态（公开方法，可外部调用）
    /// </summary>
    /// <param name="weather">目标天气类型</param>
    public void SetWeather(WeatherType weather)
    {
        currentWeather = weather;

        switch (weather)
        {
            case WeatherType.Sunny:
                SetSunnyWeather();
                break;
            case WeatherType.Rainy:
                SetRainyWeather();
                break;
            case WeatherType.Snowy:
                SetSnowyWeather();
                break;
            case WeatherType.Sunset:
                SetSunsetWeather();
                break;
        }

        Debug.Log("天气已切换为: " + weather.ToString());
    }

    /// <summary>
    /// 设置晴天状态
    /// - 关闭雨、雪粒子
    /// - 使用晴天天空盒
    /// - 恢复明亮环境光
    /// </summary>
    private void SetSunnyWeather()
    {
        // 关闭雨粒子
        if (rainParticle != null)
            rainParticle.SetActive(false);

        // 关闭雪粒子
        if (snowParticle != null)
            snowParticle.SetActive(false);

        // 切换晴天天空盒
        if (sunnySkybox != null)
            RenderSettings.skybox = sunnySkybox;

        // 设置晴天环境光强度
        RenderSettings.ambientIntensity = sunnyAmbientIntensity;

        // 强制刷新天空盒渲染
        RefreshSkyboxRender();
    }

    /// <summary>
    /// 设置雨天状态
    /// - 开启雨粒子、关闭雪粒子
    /// - 使用雨雪天空盒
    /// - 降低环境光强度
    /// </summary>
    private void SetRainyWeather()
    {
        // 开启雨粒子
        if (rainParticle != null)
            rainParticle.SetActive(true);

        // 关闭雪粒子
        if (snowParticle != null)
            snowParticle.SetActive(false);

        // 切换雨雪天空盒
        if (rainSnowSkybox != null)
            RenderSettings.skybox = rainSnowSkybox;

        // 设置雨雪天环境光强度
        RenderSettings.ambientIntensity = rainSnowAmbientIntensity;

        // 强制刷新天空盒渲染
        RefreshSkyboxRender();
    }

    /// <summary>
    /// 设置雪天状态
    /// - 开启雪粒子、关闭雨粒子
    /// - 使用雨雪天空盒
    /// - 降低环境光强度
    /// </summary>
    private void SetSnowyWeather()
    {
        // 关闭雨粒子
        if (rainParticle != null)
            rainParticle.SetActive(false);

        // 开启雪粒子
        if (snowParticle != null)
            snowParticle.SetActive(true);

        // 切换雨雪天空盒
        if (rainSnowSkybox != null)
            RenderSettings.skybox = rainSnowSkybox;

        // 设置雨雪天环境光强度
        RenderSettings.ambientIntensity = rainSnowAmbientIntensity;

        // 强制刷新天空盒渲染
        RefreshSkyboxRender();
    }

    /// <summary>
    /// 设置晚霞状态
    /// - 关闭雨、雪粒子
    /// - 使用晚霞天空盒
    /// - 设置晚霞环境光强度
    /// </summary>
    private void SetSunsetWeather()
    {
        // 关闭雨粒子
        if (rainParticle != null)
            rainParticle.SetActive(false);

        // 关闭雪粒子
        if (snowParticle != null)
            snowParticle.SetActive(false);

        // 切换晚霞天空盒
        if (sunsetSkybox != null)
            RenderSettings.skybox = sunsetSkybox;

        // 设置晚霞环境光强度
        RenderSettings.ambientIntensity = sunsetAmbientIntensity;

        // 强制刷新天空盒渲染
        RefreshSkyboxRender();
    }

    /// <summary>
    /// 获取当前天气状态（公开方法，可外部查询）
    /// </summary>
    /// <returns>当前天气类型</returns>
    public WeatherType GetCurrentWeather()
    {
        return currentWeather;
    }

    /// <summary>
    /// 强制刷新天空盒渲染（解决运行时看不到天空盒变化的问题）
    /// </summary>
    private void RefreshSkyboxRender()
    {
        RenderSettings.skybox = RenderSettings.skybox;
        
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            mainCamera.Render();
        }
        
        DynamicGI.UpdateEnvironment();
    }
}

/* ==========================================
 * 使用步骤说明（How to Use）
 * ==========================================
 * 
 * 一、创建粒子系统（创建步骤）
 * 1. 在Hierarchy窗口右键 -> Effects -> Particle System，创建三个粒子对象
 *    - 命名为：RainParticle（下雨粒子）
 *    - 命名为：SnowParticle（下雪粒子）
 *    - 命名为：FountainParticle（喷泉粒子）
 * 
 *    晚霞天气不需要额外粒子，仅切换天空盒
 * 
 * 2. 调整粒子参数（Inspector面板）：
 *    - RainParticle：
 *      - Start Rate：500-1000（下雨密度）
 *      - Start Speed：10-20
 *      - Start Size：0.1-0.3
 *      - Start Color：蓝色/灰色调
 *      - Gravity Modifier：2-5（加速下落）
 *      - Shape：Box，方向向下
 *      - Emission：持续发射
 *   
 *    - SnowParticle：
 *      - Start Rate：200-500（下雪密度）
 *      - Start Speed：1-3
 *      - Start Size：0.2-0.5
 *      - Start Color：白色/浅灰色
 *      - Gravity Modifier：0.5-1（缓慢下落）
 *      - Shape：Sphere，全方位发射
 *      - Emission：持续发射
 *      - 添加Rotation模块，设置随机旋转
 *   
 *    - FountainParticle：
 *      - 根据需求调整喷泉效果
 *      - 保持默认激活状态
 * 
 * 二、创建天空盒材质（创建步骤）
 * 1. 在Project窗口右键 -> Create -> Material
 *    - 命名为：SunnySkybox（晴天天空盒）
 *    - 命名为：RainSnowSkybox（雨雪天空盒）
 *    - 命名为：SunsetSkybox（晚霞天空盒）
 * 
 * 2. 设置材质Shader：
 *    - Inspector面板 -> Shader -> Skybox -> Procedural（程序化天空盒）
 *    - 或使用自定义天空盒贴图（6张面贴图）
 * 
 * 3. 调整天空盒参数：
 *    - SunnySkybox：
 *      - Sky Tint：浅蓝色（接近天空颜色）
 *      - Ground：绿色/棕色
 *      - Sun Intensity：高（1.0-2.0）
 *      - Ambient：明亮
 *   
 *    - RainSnowSkybox：
 *      - Sky Tint：深灰色/深蓝色
 *      - Ground：暗灰色
 *      - Sun Intensity：低（0.2-0.5）
 *      - 增加Cloudiness（云层）
 *   
 *    - SunsetSkybox（晚霞）：
 *      - Sky Tint：橙红色/紫红色（晚霞色调）
 *      - Ground：橙黄色/深棕色
 *      - Sun Intensity：中等（0.5-0.8）
 *      - 增加Horizon Height（地平线高度）
 *      - 设置Sun Tint为橙红色
 * 
 * 三、挂载脚本（挂载步骤）
 * 1. 在Hierarchy窗口右键 -> Create Empty，创建空物体
 *    - 命名为：WeatherManager
 * 
 * 2. 选中WeatherManager对象，添加WeatherManager脚本组件
 *    - Inspector面板 -> Add Component -> Search "WeatherManager"
 * 
 * 四、赋值变量（拖拽步骤）
 * 1. 在WeatherManager组件的Inspector面板中：
 *    - 粒子系统区域：
 *      - Rain Particle：拖拽RainParticle对象到此字段
 *      - Snow Particle：拖拽SnowParticle对象到此字段
 *      - Fountain Particle：拖拽FountainParticle对象到此字段
 *   
 *    - 天空盒材质区域：
 *      - Sunny Skybox：拖拽SunnySkybox材质到此字段
 *      - Rain Snow Skybox：拖拽RainSnowSkybox材质到此字段
 *      - Sunset Skybox：拖拽SunsetSkybox材质到此字段
 *   
 *    - 默认天气：
 *      - 在下拉菜单选择默认天气（默认Sunny）
 * 
 * 五、测试运行
 * 1. 点击Play按钮运行游戏
 * 2. 按键盘数字键切换天气：
 *    - 按1键：晴天（关闭雨雪粒子，明亮天空）
 *    - 按2键：雨天（开启雨粒子，昏暗天空）
 *    - 按3键：雪天（开启雪粒子，昏暗天空）
 *    - 按4键：晚霞（关闭雨雪粒子，橙红天空）
 * 
 * ==========================================
 * 注意事项（Notes）
 * ==========================================
 * 1. 确保粒子对象在场景中处于合适位置（雨/雪粒子通常放在相机上方）
 * 2. 天空盒材质必须使用Skybox类型的Shader
 * 3. 脚本使用DontDestroyOnLoad，切换场景时配置不会丢失
 * 4. 如果需要添加更多天气类型，可扩展WeatherType枚举
 * ==========================================
 */