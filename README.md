# 《卡莱战纪》：假如你是拜占庭将军

 

>   ...
>
>   去，是刺进胸口的波斯弯刀；不去，是处决叛徒的罗马绞架。他该怎么办？
>
>   公元前53年早春，古老的两河平原漫天风雪，峰火遍燃。罗马将军卡尤斯的案上摆着来自第一军团先锋营，第三军团，以及亚美尼亚将军艾西农斯的信函，内容都请求速出兵会战防务空虚的巴比伦城。这千载难逢的好机会连同亚美尼亚人的谄媚文字都没能让卡尤斯得意忘形——戎马三十载的老将此时反而深陷入痛苦与矛盾之中。
>
>   亚美尼亚人自去年秋起就开始行径可疑，如果他们趁此机会倒戈波斯守军，罗马五个精锐营下场只有横尸荒野 —— 道理如此直白，可是卡尤斯却不能拒绝出兵。因为他没法在这糟糕的天气里把拒信送达所有其他营，他甚至无法确定他们已经到了哪里。假如亚美尼亚人最终没有叛变，那么战斗就一定会因他的缺席蒙受巨大损失，他就成了叛徒，被绑去见残忍而多疑的执政官克拉苏。
>
>   去，是扎进胸口的弯刀；不去，是处决叛徒的绞架…… 更可怕的是，他可能只剩几个小时做决策。否则即使决定出发也无法按时赶到巴比伦了。
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







上篇讲述的是改变人类历史20大战役之一：卡莱战役里的片段，也展现了一个典型的拜占庭将军问题。假如你是拜占庭将军，该如何在有限的通信条件和共识方法下辨别真伪敌友，用最佳的分配调度来获得胜利？这个难题在今天有区块链技术解决，但在两千年前历史背景下，将军们的一切决断都必须基于超人的嗅觉、判断力和勇气。你，能做到么？



此篇概括叙述《卡莱战纪》的玩法和技术特点并鸣谢对项目有帮助的人。其他内容请参见：

[核心玩法:在拜占庭网络上模拟拜赞庭将军问题](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E8%AE%BE%E8%AE%A1.md)

[软分服: 大规模链游解决方案](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%88%86%E6%9C%8D.md)

[用去中心化小说影响游戏，检验人性](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%B0%8F%E8%AF%B4.md)

[技术: Neunity框架，非对称熵，以及其他](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md)





![Logo](pics/Logo.jpg)

##  玩法概要

**《卡莱战纪》是一款模拟拜占庭将军问题的卡牌链游，在共识网络上第一次尝试重现那个难以共识的时代**。玩家将在宏大的古欧亚大陆历史背景中扮演独立决策的将军，分配手中的卡牌军队发动或参与攻城战。

拜占庭将军问题背后的故事是：几个将军想协同攻城，必须有足够人共同参与才能成功，可是相互通信又不便。那么协同攻城的邀约是否可信？是否足够多盟友出兵？畏缩退兵是否会导致盟友的损失？在《卡莱战纪》里，你只有兵临城下时才能确知答案。

**《卡莱战纪》 还是第一款靠去中心化连载小说影响链上逻辑的链游**。每一章的周期里，各分服都可由玩家投票选出最佳章节并上链，作者将获得奖励并成为该阶段游戏世界里的特殊角色“吟游诗人”而影响该分服逻辑。你将在[这篇文章](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%B0%8F%E8%AF%B4.md)中了解到具体的设计将如何使得《卡莱战纪》成为一场考验人性的实验。另外卡莱战纪有宏大的历史背景及新颖的写作题材空间，期望随着游戏进行会在某分服逐渐出现伟大的链上公投协作文学作品。



## 技术概要

### 1. 引擎层

借助在《卡莱战纪》的机会，我们开发并开源了项目[Neunity](https://github.com/norchain/Neunity) (MIT)，旨在提高NEO智能合约开发效率以实现大规模上链逻辑的工程可实践性。Neunity目前以实现的主要内容包括：

1. [完成]研发适配器以实现NEO智能合约本地离线断点调试及测试用例，以及C#客户端直接复用合约逻辑。[视频教程](https://youtu.be/vTkNzx403p8)
2. [完成]高灵活度类型描述规范及自定义序列化方案NuSD。
3. [完成]仿HTTP智能合约通信协议Neunity Transfer Protocol(NuTP)。
4. [完成]类URI的Storage管理方案NuIO。

Neunity自开源半月来获得社区广泛关注和帮助，为Neunity或《卡莱战纪》贡献代码或创新解决方案的朋友包括：

1. [generalkim00](https://github.com/generalkim00) 及[maxpown3r](https://github.com/maxpown3r)：为带有竞争性的Dapp提供公平性、防拥堵及帮助记账者避嫌的“非对称熵(Asymmentropy)” 。[[原版原理说明](https://github.com/generalkim00/neogame)，[中文简介](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) (2.2节)]
2. [FabioRick](https://github.com/FabioRick) 及 [Jason L Perry](https://medium.com/@ambethia)：区块内多次获取随机数。并发表文章"[Random Number at Runtime](https://medium.com/@fabiohcardoso/random-number-at-runtime-neo-smartcontract-60c4e6cb0bb1)"
3. [gongxiaojing0825](https://github.com/gongxiaojing0825)及[exclusivesunil](https://github.com/exclusivesunil) ： 提供Mac上Neunity的连续集成自动化编译脚本

在此对他们表示深深感谢！

同时，十分高兴看到Neunity能帮助本次参赛一些队伍提速智能合约研发流程，这些项目包括（排名不分先后）：\#140 [NEOPixelBattle](https://github.com/NEOGaming/NEOGames/tree/master/NEOPixelBattle) ,\#78 [BigRedLottery](https://github.com/generalkim00/neogame), \#95 [CarveLoveonBlockchain](https://github.com/exclusivesunil/howmuchyaknowabotme), \#111 [Pirateo](https://github.com/leonhano/SeaExplorer), \#100 [FomoNEO](https://github.com/qw1985/FomoNEO), \#55 [Devourer](https://github.com/norchain/NEOGameDevourer), \#46 [CarryBattle](https://github.com/norchain/NEOCarryBattle) 

Neunity同时在进一步集成NEO-Lux RPC，并在与neocompiler.io讨论建立API，以提供本地调试完毕后一键部署neocompiler.io进行测试的连续集成工具链。下一步也十分希望能加深与包括NEL和BlaCat的钱包在内的其他CoZ优质项目的合作，为促进NEO实践大规模系统化链上逻辑做贡献。



### 2. 应用层

* 为了实现“在共识网络上重现那个难以共识的时代”， 为了平衡链游PvP因信息公开性造成的后发优势，我们应用前文所述的非对称熵方法在非匿名公链上实现了**制造玩家真实行动和此行动传播到其他玩家的时间差**（[简介](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) 2.2节)。
* 卡莱战役是一款逻辑上链游戏，玩家无需依赖中心化服务器便能体验完整的游戏流程（甚至无需客户端）。  全链游保证玩家资产不仅永远不会丢失，而且永远能有用或交易给想继续用的人。
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
