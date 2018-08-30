# Carrhae Battle


 

[![Preview In Bilibili](http://www.imageurl.ir/images/94223527848154454953.png)](https://www.bilibili.com/video/av29822452/)


Chinese Version: [HERE](https://github.com/norchain/NEOCarryBattle/blob/master/README_CN.md)

Gameplay Demo: [HERE](http://www.norchain.io/neounity) 



> 53BC, Eurasia, the war between Roman Republic and Parthian Empire come to the decisive period.  The powerful Roman encountered the toughest defence and could not make a move near the town of Carrhae. However they had another plot. 
>
> The poetry-loving Persians in Carrhae had an obscure custom. The bards would play their new chapter of epos in turns during the ceremonies. The townees and visitors voted the most beautiful one as the tribute to the Gods, which would induce luck, wealth or disaster to different people, cities and countries. Althrough the impacts were predictable, townees insisted to vote the best by their hearts even if the disaster would arrive on themselves.
>
> But this time, Romans attempted to manipulate the ceremony, for the result of the Battle of Carrhae...



This article discribes the the gameplay and technology features of *CarrhaeBattle*, and acknowledge those who helped this project. For other content please refer to (In Chinese Language and will translate soon):

 [Core Gameplay: Simulate Byzantine General Problem on a BFT network](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%A0%B8%E5%BF%83%E7%8E%A9%E6%B3%95.md)

[Soft Server: The solution of mutiple-server blockchain games ](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%88%86%E6%9C%8D.md)

[Vote for the next chapter:  In the name of art, power or profit?](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%B0%8F%E8%AF%B4.md)

[Technology: Neunity，Asymmentropy and others](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md)



![Logo](pics/Logo.jpg)

##  Gameplay Brief

Carrhae Battle is a blockchain card game to simulate the famous trust dilemma Byzantine General Problem. The players act the generals in the ancient Eurasia, deploy the cards to the trigger or participate in the sieges for the trophies.

The story behind Byzantine General Problem is: Several allied generals could win the siege only if enough forces eventually joined. But since the communication was not reliable, each one of them had to think of the authenticity and the latency of the messages they received. *Carrhae Battle* uses Asymmentropy to temporary "cloak" the users' real actions on blockchain to simulate this process, so people could only analyse the coming force is on which side or just a smokescreen when it's matching. (Note: simulating BGP is optional when players create a new server. It will come to be conventional card siege game if this option is off. More details please see [Soft Server](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%88%86%E6%9C%8D.md)).

Carrhae Battle is also the first blockchain game **to use decentralized voted epos to impact the game world**. In every chapter's cycle, the players in each server vote the best next chapter. The hash result of the chapter decides the positive or negative impacts on the cities and players respectively. The bard as the author of the voted chapter would be awarded by the voting tokens and be granted some superpower during this cycle as well. By reading the epos, we'll know the majority players of such server are art sponsors, the self-sacrificers, or the power chasers. (Note: No topic restriction for the "epos") 

 



## Technology Brief

### 1. Engine Layer

With the opportunity of developing the NEO version of *Carrhae Battle*, we developed and open sourced the project [Neunity](https://github.com/norchain/Neunity) (MIT), in order to help increase the development efficiency of developing large scale NEO smart contracts. The features Neunity realized include：

1. [Finished] The Adapter layer to realize NEO smart contract local offline breakpoint debug and test cases, while share the SC logic (class definitions and algorithms) with C# client. [Video Tutorial](https://youtu.be/vTkNzx403p8)
2. [Finished] Flexible class serialization method [NuSD](https://github.com/norchain/Neunity/blob/master/NeunityBytesSeralization.md). 
3. [Finished] HTTP-like SC invocation protocol: Neunity Transfer Protocol([NuTP](https://github.com/norchain/Neunity/blob/master/Neunity/Neunity/Tools/NUTransferProtocol.cs)).
4. [Finished] URI-like Storage management method [NuIO](https://github.com/norchain/Neunity/blob/master/NeunityStorageManagement.md).

Thanks the following people for helping Carrhae Battle and Neunity with code and innovative solutions:

1. [generalkim00](https://github.com/generalkim00) and [maxpown3r](https://github.com/maxpown3r): The idea of "Asymmentropy" to keep competitive Dapps fair and jam-proof. Asymmentropy can also help bookkeepers to avoid transaction-reversing suspicion in competitive Dapps.  [[Introduction](https://github.com/generalkim00/neogame)，[Chinese Version](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) (Chapter 2.2)]
2. [FabioRick](https://github.com/FabioRick) and  [Jason L Perry](https://medium.com/@ambethia)：How to generate multiple random numbers in one block. Also published the article "[Random Number at Runtime](https://medium.com/@fabiohcardoso/random-number-at-runtime-neo-smartcontract-60c4e6cb0bb1)"
3. [gongxiaojing0825](https://github.com/gongxiaojing0825) and [exclusivesunil](https://github.com/exclusivesunil) ： Neunity Continuous Integration bash script on Mac.

Thank you guys very much.

At the same time, very glad to see Neunity can help other teams to increase their development efficiency during this competition. Including: #140 [NEOPixelBattle](https://github.com/NEOGaming/NEOGames/tree/master/NEOPixelBattle) ,\#78 [BigRedLottery](https://github.com/generalkim00/neogame), \#95 [CarveLoveonBlockchain](https://github.com/exclusivesunil/howmuchyaknowabotme), \#111 [Pirateo](https://github.com/leonhano/SeaExplorer), \#100 [FomoNEO](https://github.com/qw1985/FomoNEO), \#55 [Devourer](https://github.com/norchain/NEOGameDevourer), \#46 [CarryBattle](https://github.com/norchain/NEOCarryBattle) 

Neunity is going to integrate NEO-Lux RPC to further simplify the development process. 



### 2. Application Layer

* In order to realize the "information delay" to simulate Byzantine General Problem, we'll employ the  Asymmentropy algorthim.（[Intro](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) Chapter 2.2)
* Carrhae Battle is a blockchain game with full logic on chain. The players can experience the full game without dependency of centralized server (Not even require the client app). The full onchain logic can ensure the digital asset that players own not only be kept forever, but also be usable forever.
* Carrhae Battle client is developed with Unity, support deployment to iOS, Android, H5 and PC/Mac clients.
* Since information on blockchain are public, the late-mover advantage could be very strong in many cases. We designed the Information Subsidy ([Intro](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md) Chapter 3.1) to balance it. 
* Soft Server ([Intro](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E5%88%86%E6%9C%8D.md)) is designed specifically for blockchain games to reduce the invoking cost when number of players and assets increases.



For more information about the technology part, please read from [HERE](https://github.com/norchain/NEOCarryBattle/blob/master/%E5%8D%A1%E8%8E%B1%E6%88%98%E7%BA%AA-%E6%8A%80%E6%9C%AF.md).

## Resources



Instant Message Channel: [Discord](<https://discord.gg/pKQyyrP> )

Twitter: https://twitter.com/carry_battle

Email: info.carrybattle@gmail.com

Developer Page：[norchain.io](norchain.io)

Developer Email：rct1985@qq.com



## Credit

Thanks very much for [NEL](https://github.com/NewEconoLab) and BlaCat for funding NEO game competition，Thanks [neo.game](neo.game) organizer Gene and other guys for your hardworking。

Thanks [igormcoelho](https://github.com/igormcoelho), [vncoelho](https://github.com/vncoelho) and [FabioRick](https://github.com/FabioRick) from NeoResearch to provide the public testchain tool [neocompiler.io](https://neocompiler.io/). It's very convenient for testing SC.

Thanks [Relfos](https://github.com/Relfos) from [Neo-lux](https://github.com/CityOfZion/neo-lux) for the RPC interface in Unity.

Thanks [FabioRick](https://github.com/FabioRick) the author of another game [NEOPixelBattle](https://github.com/NEOGaming/NEOGames/tree/master/NEOPixelBattle) for many useful technology discussions. His newly open sourced project [SCTracker](https://github.com/FabioRick/SCTracker) also helped a lot.

Thanks [generalkim00](https://github.com/generalkim00)  and [maxpown3r](https://github.com/maxpown3r)  the authors of another game [BigRedLottery](https://github.com/generalkim00/neogame) for the method of Asymmentropy. This is used in both BigRedLottery and Carrhae Battle to generate undisputed inter-block randomness.

Thanks [gongxiaojing0825](https://github.com/gongxiaojing0825) and [exclusivesunil](https://github.com/exclusivesunil) the authors of another game [CarveLoveonBlockchain](https://github.com/exclusivesunil/howmuchyaknowabotme) for providing Neunity CI script.



