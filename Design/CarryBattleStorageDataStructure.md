## CarryBattle Storage Data Structure

### 1. Card

First off, we have the data to track all cards' intrinsic properties:

```
"C<cardID>"	=<card intrinsic properties as byte[]>
```

The card's properties includes:

| Properties | Type       | Description                               |
| ---------- | ---------- | ----------------------------------------- |
| type       | byte       | Type of Army: Infantry, Archer or Cavalry |
| lvls       | byte[4x4]  | The level of each grid                    |
| prtID1     | BigInteger | ParentID1                                 |
| prtID2     | BigInteger | ParentID2                                 |
| creator    | BigInteger | Creator                                   |
| owner      | BigInteger | Owner                                     |
| score      | BigInteger | The score the card got                    |
| wins       | BigInteger | The wars the card won                     |
| wars       | BigInteger | The wars the card participated            |
|            |            |                                           |

Above information will be serialized to byte[] and put into below location.

| KeyPath | Value                               | Parameter          |
| ------- | ----------------------------------- | ------------------ |
| C{ID}   | The Card info serialized in table 1 | ID: The ID of card |



We have another set of keys to track card's availability in order to reduce to cost of search. Concretly, there are two set of cards: live and discard. For each of them (take live ones for example), we have the storage structure as:

```
"pC<liveID>"	=>"c"	=<cardID>	//(related cardID. If empty, it's vacant)
				=>"m"   =<cardID>   //(merged card)
				=>"p"   =<warID>
```

| KeyPath    | Value                                   | Parameter                              |
| ---------- | --------------------------------------- | -------------------------------------- |
| pC{setID}c | CardID                                  | setID: The ID in live/discard card set |
| pC{setID}p | The war position the card is placed now |                                        |

Rules:

1. When function `CardMerge` is called, The new card will be generated and put into the live set, while the parents cards will be moved from live set to discard set.
2. Only the cards in live set can join the war or transferred.
3. 

### 2. War

For wars:

```
"W<warID>" =><war intrinsic properties as byte[]>	//include endHeight, creatorPlayer
```
| Properties | Type       | Description               |
| ---------- | ---------- | ------------------------- |
| creatorID  | byte[]     | Creator                   |
| endHeight  | BigInteger | The height of the war end |
| prtID1     | BigInteger | ParentID1                 |
| prtID2     | BigInteger | ParentID2                 |
| creatorID  | byte[]     | Creator                   |


| Parameter    | Type                | Parameter                       |
| ------------ | ------------------- | ------------------------------- |
| W{ID}        | WarID of {ID}       | ID: The ID in live war category |
| pC{ID}p{Pos} | The CardID at {PoS} | Pos: The position of the war    |

We also have live and dead wars. Take live ones for example:

```
"pW<liveID>" 	=>"w"						=<warID> 		//If empty, it's vacant
				=>"p"<byte[] pos>	=>"p"	=<playerID>		//if empty upmove the rest
				   					   =>"c"<byte[]order>	=<cardID>	//cardOrder
```

| Path         | Value                                   | Parameter                       |
| ------------ | --------------------------------------- | ------------------------------- |
| pW{ID}w      | WarID of {ID}                           | ID: The ID in live war category |
| pC{ID}p{Pos} | The CardID at {PoS}                     | Pos: The position of the war    |
| pC{ID}s      | The score the card got                  |                                 |
| pC{ID}w      | The wars the card won                   |                                 |
| pC{ID}l      | The wars the card loss                  |                                 |
| pC{ID}p      | The war position the card is placed now |                                 |
| pC{ID}o      | The ownerID of the card                 |                                 |

### 3. Player

```
"U<pID>"	=>"a"	=<address>
			
```



### 4. Global

Global variables

```
"G"		=>"C"	//num all cards
		=>"c"	//index highest live cards
		=>"W"	//num all wars
		=>"w"	//index highest live wars
		=>"p"	//num registered players
```

