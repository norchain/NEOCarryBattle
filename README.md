# 《卡莱战纪》：假如你是拜占庭将军

 

>   ...
>
>   去，是刺进胸口的波斯弯刀；不去，是处决叛徒的罗马绞架。他该怎么办？
>
>
>
>   公元前53年早春，古老的两河平原漫天风雪，峰火遍燃。罗马将军卡尤斯的案上摆着来自第一军团先锋营，第三军团，以及亚美尼亚将军艾西农斯的信函，内容都请求速出兵会战防务空虚的巴比伦城。这千载难逢的好机会连同亚美尼亚人的谄媚文字都没能让卡尤斯得意忘形——这位戎马三十载的老将此时反而陷入了极度的痛苦和矛盾中。
>
>   自去年秋起亚美尼亚人行径就开始变得有些可疑，如果他们趁此机会倒戈帮助波斯守军，罗马五个精锐营下场只有横尸荒野。可即便道理如此简单，卡尤斯也无法拒绝这战约。因为他没法在这糟糕的天气里把拒信送达所有其他营，他甚至无法确定他们已经到了哪里。假如亚美尼亚人其实没有叛变，那么战斗就会因为他的缺席蒙受巨大损失，他就成了叛徒，被绑去面对暴躁而多疑的执政官克拉苏。
>
>   去，是扎进胸口的弯刀；不去，是处决叛徒的绞架…… 更可怕的是，他可能只剩几个小时做决策。因为如果决定要去，必须今晚出发才可能及时赶到巴比伦。
>
>   该怎么办？嘀嗒、嘀嗒，羊毛毯上晕开一层层水渍，那是死神的沙漏。空气中的碎冰渣拌着大帐里的潮腐气息一同撞进鼻子，隐隐刺痛。东方渐白，厚厚的阴云被大风狂撵着，在冰封的幼发拉底河上飞快的掠过倒影，这无尽旷野里唯一的静动。
>
>   ...
>
>   "父亲大人，孩儿或有一计....." 一个年轻的声音打破了沉默。
>
>
>
>   —— 帕提亚人史 奥特耶洛纪 卷三







上篇讲述的是改变人类历史20大战役之一：卡莱战役里的片段，也展现了一个典型的拜占庭将军问题。假如你是拜占庭将军，该如何在有限的通信条件和共识方法下辨别真伪敌友，用最佳的分配调度来获得胜利？大家都知道这个难题在今天有区块链技术解决，但在两千年前历史背景下，将军们的一切决断都必须基于超人的嗅觉、判断力和勇气。

![Logo](pics/Logo.jpg)

##  玩法概要

**《卡莱战纪》是一款模拟拜占庭将军问题的卡牌链游，在共识网络上第一次尝试重现那个难以共识的时代**。玩家将在宏大的古欧亚大陆历史背景中扮演独立决策的将军，分配手中的卡牌军队发动或参与攻城战。不同于普通组队PvP游戏，虽然宣战时各方都必须声称自己隶属攻方或守方，但只有在兵临城下时大家才会亮牌知道会战的各方谁是真的盟友，谁是内奸叛徒，谁仅仅是放空了消息。

对其他玩家的行为判断当然不是无据可循。作为一款全逻辑上链游戏，所有玩家的参战历史都公开可查，谁是见利忘义的墙头草，谁是最有信誉的坚定战士，谁和谁明显每次都在线下商量好了暗地协作，谁是最狡猾的局势洞察者，都可以通过参考链上数据进行判断和预测。玩家也因此逐渐形成自己的群落，更多的和自己喜欢的人会战，发展出不同的敌友分析及游戏策略来。

**《卡莱战纪》 还是第一款靠去中心化连载小说影响链上逻辑的链游**。每一章的周期里，各分服都可由玩家投票选出最佳章节并上链，作者将获得奖励并成为该阶段游戏世界里的特殊角色“吟游诗人(Bard)”而部分影响该分服逻辑。每次 “吟游诗人” 吟诵自作的这段历史后（小说投票结果确定后）都会让世界某局部的气候或地形发生永久变化。

卡莱战纪有宏大的历史背景及新颖的写作题材空间。随着游戏进行会在某分服逐渐出现伟大的链上协作文学作品。

详细玩法说明请进一步阅读[玩法设计篇](https://github.com/norchain/NEOGameComp/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E7%8E%A9%E6%B3%95.md)。

讨论玩家在游戏里可获得的娱乐体验请阅读体验篇

## 技术概要

### 1. 引擎层

在《卡莱战纪》研发过程中，我们顺便研发了项目[Neunity](https://github.com/norchain/Neunity) ，旨在提高NEO智能合约开发效率以实现大规模上链逻辑的工程可实践性。Neunity目前以实现的主要内容包括：

1. NEO智能合约本地离线断点调试及测试用例，以及C#客户端直接复用合约逻辑。[视频教程](https://youtu.be/vTkNzx403p8)
2. 高灵活度类型描述规范及自定义序列化方案NuSD。
3. 仿HTTP智能合约通信协议Neunity Transfer Protocol(NuTP)。
4. 类URI的Storage管理方案NuIO。

Neunity自开源半月来获得社区广泛关注和帮助，为Neunity或《卡莱战纪》贡献代码或创新解决方案的朋友包括：

1. [generalkim00](https://github.com/generalkim00) 及[maxpown3r](https://github.com/maxpown3r)：为依赖blockhash随机的Dapp提供公平性、防拥堵及帮助记账者避嫌的“非对称熵(Asymmentropy)” 。[[原版原理说明](https://github.com/generalkim00/neogame)，[中文简介](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) (见第二章)]
2. [FabioRick](https://github.com/FabioRick) 及 [Jason L Perry](https://medium.com/@ambethia)：区块内多次获取随机数。并发表文章"[Random Number at Runtime](https://medium.com/@fabiohcardoso/random-number-at-runtime-neo-smartcontract-60c4e6cb0bb1)"
3. [gongxiaojing0825](https://github.com/gongxiaojing0825)及[exclusivesunil](https://github.com/exclusivesunil) ： 提供Mac上Neunity的连续集成自动化编译脚本

在此对他们表示深深感谢！

同时，十分高兴看到Neunity能帮助本次参赛一些队伍提速智能合约研发流程，这些项目包括（排名不分先后）：\#140 [NEOPixelBattle](https://github.com/NEOGaming/NEOGames/tree/master/NEOPixelBattle) ,\#78 [BigRedLottery](https://github.com/generalkim00/neogame), \#95 [CarveLoveonBlockchain](https://github.com/exclusivesunil/howmuchyaknowabotme), \#111 [Pirateo](https://github.com/leonhano/SeaExplorer), \#100 [FomoNEO](https://github.com/qw1985/FomoNEO), \#55 [Devourer](https://github.com/norchain/NEOGameDevourer), \#46 [CarryBattle](https://github.com/norchain/NEOCarryBattle) 

Neunity同时在进一步集成NEO-Lux RPC，并在与NeoCompiler讨论建立API以提供本地调试完毕后一键部署neocompiler.io进行测试。下一步也希望能加深包括NEL和BlaCat的钱包以及其他CoZ优质项目的合作，为促进NEO实践大规模链上逻辑做贡献。



### 2. 应用层

* 卡莱战役是一款逻辑上链游戏，玩家无需依赖中心化服务器便能体验完整的游戏流程（甚至无需客户端）。  全链游保证玩家资产不仅永远不会丢，而且永远能有用或交易给想继续用的人。
* 卡莱战役基于Unity游戏引擎开发，支持全平台移植。
* 为了应对区块链一些两面性的特质，以及扩展逻辑和数据后可能发生的很多问题，我们设计了诸如软分服、信息补贴等。

详细技术特点说明请见：[技术介绍](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md)

## 相关资源

首支[宣传视频](https://www.bilibili.com/video/av29822452/):
[![Preview In Bilibili](http://www.imageurl.ir/images/94223527848154454953.png)](https://www.bilibili.com/video/av29822452/)

试玩地址: [海外](http://www.norchain.io/neounity)  [中国大陆](http://119.23.254.156/neounity/)

即时通信社区: [Discord](<https://discord.gg/pKQyyrP> )

Twitter: https://twitter.com/carry_battle

Email: info.carrybattle@gmail.com

开发者主页：[norchain.io](norchain.io)

开发者邮箱：rct1985@qq.com



## 感谢

十分感谢[NEL](https://github.com/NewEconoLab)及BlaCat对活动的资助及技术支持，感谢[neo.game](neo.game)组织者Gene及其他朋友的辛勤劳动。

感谢NeoResearch的[igormcoelho](https://github.com/igormcoelho)，[FabioRick](https://github.com/FabioRick) 及[vncoelho](https://github.com/vncoelho) 提供[neocompiler.io](https://neocompiler.io/) 这样好用的在线私链工具。

感谢[Neo-lux](https://github.com/CityOfZion/neo-lux)的[Relfos](https://github.com/Relfos) 提供Unity中RPC接口。

感谢参赛游戏[NEOPixelBattle](https://github.com/NEOGaming/NEOGames/tree/master/NEOPixelBattle)作者[FabioRick](https://github.com/FabioRick) (也是neocompiler开发者)彻夜讨论各种细节实现，以及区块内多随机数算法方案。他的新开源项目[SCTracker](https://github.com/FabioRick/SCTracker) 对我们帮助也不小。

感谢参赛游戏[BigRedLottery](https://github.com/generalkim00/neogame)的作者[generalkim00](https://github.com/generalkim00) 及[maxpown3r](https://github.com/maxpown3r) 分享无敌经典的绝无争议跨区块随机机制Asymmentropy（我帮他翻译为：**非对称熵**。该算法被同时应用到BigRedLottery和CarryBattle里，在BigRedLottery里能完美解决跨区块随机数产生时，由于NEO记账节点少带来可能的争议）。

感谢参赛游戏[CarveLoveonBlockchain](https://github.com/exclusivesunil/howmuchyaknowabotme) 的作者[gongxiaojing0825](https://github.com/gongxiaojing0825)及[exclusivesunil](https://github.com/exclusivesunil) 为Neunity提供Mac上的编译脚本。
