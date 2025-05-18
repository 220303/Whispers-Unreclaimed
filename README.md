# 烟尘记


## 基本架构：
* #### MainWindows
  > Start
  >
  > Rebillion qoutes
  >
  > New start   
  >
  > Game  
  >
  > Pause
  >
  > Trace  
  >
  > Option  
* #### 后台类库  
  > 烟尘记游戏  
  >
  > Data 

## 开发设计：

#### `MainWindows`：

作为游戏的主窗口，也是唯一的窗口，所有的游戏界面都作为页面在其上切换，采用`Frame`框架来承载页面，并且隐藏导航栏。它的构造函数包括初始化`Frame`和加载`Start`页面，它的析构函数种包含写入存档和设置文件。

##### 页面切换动画：

`MainWindow.xaml`：添加蒙版层，用于展示页面切换动画以及屏蔽动画发生时的输入。

`MainWindow.xaml.cs`：监听并处理`Page_frame.Navigating`，`Page_frame.Navigated` 两个导航事件，分别代表页面切出和切入。

* `MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)`：页面切出动画，蒙版由透明转不透明 (全黑)
* `MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)`：页面切入动画，蒙版由不透明 (全黑)转透明

#### `Start`页面：

用户一进入游戏，就会处在`Start`页面上，`Start`页面会显示封面，可以接受输入并跳转到`Rebillion qoutes`页面，`New start`页面，`Trace`页面，`Option`页面。

##### 交互逻辑：

> 鼠标：点击屏幕任意区域进入游戏。
>
> 键盘：
>
> * 输入 `shift`+`;`进入`Option`页面。
>
> * 输入 `shift`+`a`进入`Trace`页面。
>
> * 按其余任意键进入游戏。
>
> ***进入游戏***：如果什么都存档都没有，则进入`New_start`页面创建新的存档；如果有存档，则进入`Rebillion qoutes`页面加载存档

* `Start()`：初始化背景、水波纹、聚焦键盘输入
* `Start_game()`：执行 *进入游戏* 逻辑，会有两种情况，一是什么都存档都没有，则进入`New_start`界面创建新的存档；二是有存档，那么`Option`文件中必然有`save_choose`值，则进入`Ribillion_qoutes`页面。
* `Page_KeyDown(object sender, KeyEventArgs e)`：根据用户输入进入`Trace`页面，`Option`页面，或调用`Start_game()`
* `Page_MouseDown(object sender, MouseButtonEventArgs e)`：无条件调用`Start_game()`

##### 背景：

背景图是一张左右可以无缝衔接的`48:9`的图片 (豆包生成) ，通过两个并列的`Image`控件实现 (用户视角的) 横向循环播放。全部在`Start.xaml.cs`中实现。

* `scaled_width`：图片的宽度(根据窗口高度动态改变)
* `speed`：移动速度，单位: 像素/秒
* `offset`：图片偏移量(表示移动到什么位置了)，始终是负值(因为是左移)
* `last_update`：上次移动图片的时间，下次移动时要用
* `InitializeAnimation()`：初始化动画
* `Image_update_dimensions()`：处理改变窗口大小后图片的大小及位置变化
* `UpdatePosition(object sender, EventArgs e)`：更新图片位置

##### 水面波纹效果：

在用户的鼠标周围呈现出水波纹，通过将`HLSL`编译为`GPU`可执行的`Shader`，并集成到`WPF`渲染管线来实现。

* `water_ripple.hlsl`：定义了波纹的效果，采用指数衰减并设置相应参数。
  + 在`烟尘记.csproj`中定义与生成事件，使用`Windows SDK`中的`fxc.exe`将其编译成`water_ripple.ps` 。
  + 与`Filesystem`中的其他文件不同，`water_ripple.plsl`不包含在生成内容中，`water_ripple.ps`被设为内嵌设为资源。

* `water_ripple_effect.cs`：与`water_ripple.ps`对接，提供`C#`代码访问`water_ripple.ps`的接口
* `Start.xaml.cs`：实现有关波纹生成和跟踪鼠标移动的逻辑。
  + `rippleTimer`：定时器
  + `startTime`：波纹开始时间
  + `Water_ripple_start()`：初始化水波动画
  + `Watter_ripple_update_dimensions()`：窗口大小改变时，改变水波shader的宽高比

* `Start.xaml`：给两个`Image`附上`Effect`来展现水波纹以及参数设定。
  + `Amplitude`：振幅
  + `Frequency`：频率
  + `Speed`：速度

#### `New start`页面:

创建新的存档。用户可以输入新存档名字，然后选择创建新存档。也可以选择返回`Start`页面。

新建存档，设置存档名字然后进入存档。

#### `Rebillion qoutes`页面：

输出李任语录。

后台加载`Nodes`，如果没有可加载的就等`4`秒。

#### `Game`页面:

游戏界面，所有游戏操作都在此页面进行。总的来说，程序会轮流输出剧情(逐字输出)和选项，用户做出选择后又会输出新的剧情，用户的历史选择会影响当下可以做出的选择。

鼠标：

> 点击选项按钮做出选择。
>
> 点击点击屏幕任意区域输出剧情(仅在第一次进入`Game`页面时需要)。

键盘：

> 输入 `shift`+`;`进入`Option`页面。
>
> 输入`Esc`进入`Pause`页面。
>
> 输入数字键做出选择(前提是有数字对应的选项)。
>
> 输入其余任意键输出剧情(仅在第一次进入`Game`页面时需要)。

#### `Pause`页面：

玩家可以保存进度，退出到`Start`页面，直接退出游戏(退出到桌面)。

键盘：

> 输入`Esc`返回`Game`页面。

注意保存是立即写入文件的持久化保存，且不可撤销。

注意未保存直接退出会丢失所有本次游玩的进度。

#### `Trace`页面：

按名称排序显示存档列表，用户可以选择一个存档作为活动存档(改变了`Options`中的`Save_choose`并立即写入`Options`文件)，下次进入游戏时会默认进入活动存档。

游戏结局时会展示`Trace`页面。

键盘：

> 输入`Esc`返回`Start`页面 (对于游戏结局时显示的`Trace`页面而言，是重新回到`Start`页面)。

#### `Option`页面：

玩家可以更改`Options`中的`Plot_font_size`，`Plot_print_speed`，`Music_volume`，并保存(立即写入`Options`文件)。

`Plot_font_size`：字体大小

`Plot_print_speed`：逐字输出速度

`Music_volume`：介于`0-10000`间的整数。

键盘：

> 输入`Shift`+`:`返回上一个页面 (`Game`或`Start`)。

注意：未保存的`Options`更改将在离开`Option`页面时丢失。

注意：不合法的`Options`更改在保存时不会写入`Options`文件，若此时离开`Option`页面，原数据不会丢失。

#### `烟尘记游戏`类：

实例类，其实例代表一局戏，在游戏开始时创建，游戏结束时销毁。

`Choice`：`Choice`结构，含有`Content`，`Jump`，`condition`，分别代表选项文字，选项要跳转到的`Node`编号，选项可用的条件。`Choice(string content, int jump, List<string> condition)`是构造函数，`Check(List<string> check_record)`是自带的方法，用于根据用户游玩历史判断选项是否可用。

`Node`结构：含有`Plot`，`Choices`，分别代表节点的剧情内容和选项列表，`Node(string plot, List<Choice> choices)`是构造函数。

`Nodes`：全局唯一的剧情节点列表，包含所有剧情及选项。

`Jump`：读取自某个存档`Save`，代表接下来要输出的`Node`编号。

`Record`：读取自某个存档`Save`，代表该存档过去的游戏历史。

`Plot_directory_path`：存放`Plots`的位置。

`Choices_directory_path`：存放`Choices`的位置。

`Nodes_read_in()`：读取所有剧情及选项，在游戏启动时使用。

`Choose(int input)`：根据玩家的选择切换到下一个`Node`。

`烟尘记游戏(Data.Save save)`构造函数：给定一个存档`Save`，可以构造一个`烟尘记游戏`的实例。

`Save_game()`函数：用于保存进度到程序内的`Saves`。

#### Library.cs 类库：

准确的来说`Library.cs`只是个普通`C#`代码文件，因为我尝试过使用分离项目的类库，若类库引用主程序会造成循环依赖，但其中的`Data`类里面的`Main_window`需要引用主程序中的`MainWindow`来将其全局化，所以不能这样构建项目。

包含所有全局数据，包括`Data`，`Rebillion Qoute`，`Save`，`Option`这几个静态类。

##### `Data`类：

存放全局静态变量 (实际上也可以说是杂项全局静态变量)。

`Filesystem_directory_path`：游戏所有文件的根目录

`Image_directory_path`：存放背景图片的位置

`Main_window`：为`MainWindow`生成的一个全局对象，使得在其他`Page`和类中也能访问到`MainWindow`中的内容：

* 通过`Data.MainWindow.Page_frame.NavigationService.GoBack()`来返回上一个页面。
* 通过`Data.Main_window.Page_frame.NavigationService.Navigate(Page newpage)`来导航到新页面。
* 通过`Data.Main_window.global_media_element`访问`MainWindow`中的音乐播放器。
* 通过`Data.Main_window.Page_frame.Navigated += Change_music`监听`Frame`导航来更改音乐。
* 通过`Data.Main_window.Close();`调用关闭代码。

##### `Music_player`类

`Music_directory_path`：存放背景音乐的位置。

`music_player`：全局唯一静态音乐播放器，保存`MainWindow中的Global_media_element`。

`Current_music_page_group`：当前页面所属的`音乐播放页面分组`的记录。

`Start(string Current_music_page_group)`静态函数：初始化`Global_music`，在程序启动时使用，`Change_music(object sender, NavigationEventArgs e)`：页面切换事件的处理程序，负责根据不同的`音乐播放页面分组`更换背景音乐。

##### `Rebillion_qoute`类

类名没有`s`是因为要避让`Rebillion_qoutes`这个`Page`类

`Rebillion_qoutes_file_path`：存放`Rebillion Qoutes.txt`的位置

`rebillion_qoutes`：全局唯一的李任语录(`string`类型数组)

`Read_in()`静态函数：读取李任语录文件，在游戏启动时使用。

##### `Save`类

`Saves_directory_path`：存放存档的位置

`save`结构：含有`Name`，`Jump`，`Record`，分别代表存档的名字，玩到的剧情`Node`编号，历史选择。其中`Name`是属性，因为要被依赖到WPF界面上显示。

`saves`：全局唯一的存档列表，包含玩家创建的多个存档。

`Read_in()`静态函数：读取存档文件，在游戏启动时使用。

`Write_out()`静态函数：写入存档文件，在保存存档时使用。

##### `Option`类

`Options_file_path`：存放`Option.txt`的位置

`Save_choose`：默认选择的存档。

`Plot_font_size`：字体大小。

`Plot_print_speed`：文字输出速度。

`Music_volume`：背景音乐音量大小。

`Read_in()`静态函数：用来读取置文件，在游戏启动时使用。

`Wrtie_out()`全局静态函数：写入设置文件，在保存设置时使用。


## 未来计划：
* #### 第二阶段开发
  
  * [ ] 编写自定义用户控件，替换掉原有的默认控件
  * [x] 增加音频播放功能
  * [ ] 制作内嵌视频与动画
  * [ ] 催Rebillion同志交稿
  
* #### 跨平台移植
  > 采用基于.NET 的跨平台框架
  > 很有可能是 WPF 的上游 avalonia 框架

  

## 历史回顾：

*** 2025年5月18日21：01***

到现在我又完成了一个目标，成功写出了全局页面切换动画，也就是说所有页面之间的切换已经搞定了！我和AI讨论了好久，也采用过他们的好几版不同的方案实验修改了很久，但都因为反复尝试还是无法修改的致命缺陷被我一票否决。好在我在这一过程中了解了相关内容，最终直接自己原创了一版出来，完美符合要求！

文档见`MainWindows`的`页面切换动画`部分。

*** 2025年5月18日14：54***

今天是个值得纪念的日子！

截止到现在，我彻底完成了`Start`页面的美工部分，包括背景和水波纹。

背景：我大概用了10小时左右用豆包制作背景图，背景图要求长宽比为`48:9`并且左右能无缝衔接。由于图片长宽比过大 ，豆包无法编辑，好在能生成，所以我只能反复开启新对话生成新图片，其中最难的就是左右无缝衔接，在百张以上的图片只有三张还算勉强过关，我选择了左右衔接最好的那一张 (诚然它的画面和颜色不是最好的) 。

水波纹：我一开始采用了`canvas`控件+`WPF`动画的方式动态生成会扩散和消失的圆圈来模仿水波纹，最终确实实现了该效果，但是跟我想要的水面波纹效果一点都不像，所以我果断选择重新设计。我选择通过`Pixel Shader`像素着色器 (2.0版本)来动态实现水面波纹效果，我编写`water_ripple.hlsl`并使用`Windows SDK`中的`fxc.exe`将其编译成`water_ripple.ps`，之后使用`water_ripple_effect.cs`与之对接，然后在`Start.xaml`和`Start.xaml.cs`中使用。期间经历了多次发现`Bug`和修复`Bug`，最终我胜利完成目标！

文档见`Start页面`的`背景`部分和`水面波纹效果`部分。

*****

***2025年5月8日13：24***

今天完成了一项大工程！

把原来`Data`类中的`剧情`相关的内容移动到烟尘记游戏类中，因为`剧情`相关的数据结构和方法与`音乐`、`选项`、`存档`相关的数据结构和方法本质上是完全不同的，`剧情`相关的数据结构和方法附属于游戏实例，它们的生命周期也和游戏实例是一样的，这样改动之后封装模块更为合理，减少了`烟尘记游戏`和`Data`类之间不必要的耦合。

把`李任语录`相关内容改成`Rebillion_qoute`类 (没有s是因为要避让`Rebillion_qoutes`这个`Page`类)，移出`Data`类；把`选项`相关内容改成`Option`静态类，移出`Data`类；把`存档`相关内容改成`Save`静态类，移出`Data`类；`音乐`相关内容改成`Music_player`静态类，移出`Data`类。这样就把同一模块中的数据结构和方法很好地封装了起来，并且静态类也更好地体现了它们全局唯一的特性。我把它们放在了一个新的`Library.cs`文件中，原有的`Data`类现在仅用于储存一些零散的全局静态变量，比如`Main_window`等。

只是挨个更改调用好难啊，哪怕有`VS 2022`的辅助也还是累的要死！

我还优化了`Game`中生成按钮的逻辑，反正`check`都破坏`Nodes`了，也不差再破坏一点，重开游戏的时候重新加载就是了。

我还更改了`Pause`页面，`Option`页面，`New_start`页面的布局方式，从`Margin`硬布局改为用`Grid`内的`Column`和`Row`定义动态布局，目前所有页面的布局都已经改用了这种方式定义，效果很好。

从今天开始，把每次的更改内容写在历史里，而不是`Git commit`里面。

**********************

***2025年5月5日 12：14***

完成背景音乐播放功能，大幅度编写文档，增加`开发设计`部分。

********************************************

***2022年10月3日 13：55***  

Rebillion同志注册了Github账号，加入了这个仓库，从形式上正式成为了`烟尘记`项目的联合开发者。  

********************************************

***2024年9月21日 16：28***  

这个项目的第一阶段已经完成，我将它推送到了Github上我的第一个仓库里（私人），并写下了这篇ReadMe。  
在这两年中，我遇到了数不清的问题，大大小小，我也从一个照着书打代码200行能出20个报错的初级程序员变成了一个至少写过近两万行代码，读过多本技术书籍的入门级程序员.  
确实是入门级，我要学的东西还有太多了，自己都感觉学不完。  

********************************************


***2022年9月12日 21：49***  

我发出了我人生中第八条B站动态，宣告我和Rebillion共同成立一个名为《烟尘记》的长期合作项目。  
这是一个文字游戏，我主要负责其中的程序开发部分，那个时候我才刚刚读完人生中第一本计算机书籍《C#入门经典》。  
然后，我就下定决心投入到项目的开发中去。  

********************************************
