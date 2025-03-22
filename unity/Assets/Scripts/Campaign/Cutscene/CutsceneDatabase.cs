using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CutsceneDatabase", menuName = "ScriptableObjects/CutsceneDatabase", order = 2)]
public class CutsceneDatabase : ScriptableObject
{
    public List<Cutscene> cutsceneList = new List<Cutscene>()
    {
        new Cutscene(){
            cutsceneId = 40,
            cutsceneType = CutsceneType.Elementalist,
            firstEncounter = false,
            campaignLevel = 0,
            dialogue = new Dialogue(){
                npcId = 1,
                dialogueText = new string[]{
                    "Welcome, wizard! Come to browse my wares?",
                    "Oh, don’t mind my appearance, good help is hard to raise these days.",
                    "Now, what magical goodies can I interest you in? Freshly unearthed, I promise!"
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){
                        choiceIdx = 0,
                        choices = new Choice[]{
                            new Choice(){text="Wait, are you… undead?"},
                            new Choice(){text="Got anything good today, bonehead?"},
                            new Choice(){text="Do you always grin like that?"},
                        }
                    },
                    new ChoiceSet(){
                        choiceIdx = 2,
                        choices = new Choice[]{
                            new Choice(){text="Let’s hope your wares aren’t as dusty as you are."},
                            new Choice(){text="Alright, let’s see if your stuff’s worth the gold."},
                            new Choice(){text="Enough chit-chat, let's get down to business."},
                        }
                    }
                }
            }
        },
        // LEVEL 1 - INTRO
        new Cutscene(){
            cutsceneId = 0,
            cutsceneType = CutsceneType.Start,
            firstEncounter = true,
            campaignLevel = 1,
            dialogue = new Dialogue(){
                npcId = 0,
                dialogueText = new string[]{
                    "Hey, hey you! You're finally awake!",
                    "No time for naps! The kingdom is in mortal peril!",
                    "Every other wizard skipped town the second trouble arrived.",
                    "But not you, oh mighty wizard! Your courage is unmatched!",
                    "…Actually, come to think of it, you probably just overslept.",
                    "Anyways… I need you! My friends were <color=purple>mush-napped</color>!",
                    "Taken by the dreaded wizard <b><color=red>Sporelock the Rotten</color></b> and his goons!",
                    "He’s turned the whole kingdom into his evil lair!",
                    "Hey, I’m just a mushroom, take your complaints somewhere else!",
                    "But this guy <b><color=red>Sporelock</color></b> is real rotten, he's casting nasty spells, hoarding treasure, and worst of all—",
                    "He BANNED sleeping in!!!",
                    "Right?! That’s why you have to help!",
                    "Rescue my friends, defeat <b><color=red>Sporelock</color></b>, and save the kingdom!",
                    "You are the last wizard left… by default.",
                    "Yes! No! Maybe! Just go already!",
                    "The path is straight ahead, it'll be a long journey... so hurry!",
                    "Before <color=red>Sporelock</color> turns my friends into… mushroom soup!",
                    "<color=green>Are you ready?</color>"
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx = 0, choices = new Choice[]{
                        new Choice(){ text="Ugh… five more minutes…" },
                        new Choice(){ text="What? Who? Where? Am I being arrested?" },
                        new Choice(){ text="Wow, a talking mushroom! I must still be dreaming." },
                    }},
                    new ChoiceSet(){ choiceIdx = 4, choices = new Choice[]{
                        new Choice(){ text="That sounds like me." },
                        new Choice(){ text="A great wizard needs their rest, thank you very much." },
                        new Choice(){ text="Wait, the kingdom’s in peril? Can I go back to sleep?" },
                    }},
                    new ChoiceSet(){ choiceIdx = 7, choices = new Choice[]{
                        new Choice(){ text="Sporelock? What kind of name is that?" },
                        new Choice(){ text="Mush-napped? Got anymore corny puns?" },
                        new Choice(){ text="Wait, an evil wizard? Should I be offended?" },
                    }},
                    new ChoiceSet(){ choiceIdx = 10, choices = new Choice[]{
                        new Choice(){ text="That MONSTER!" },
                        new Choice(){ text="Eh, I'm more of an early morning guy myself." },
                        new Choice(){ text="This just got personal." },
                    }},
                    new ChoiceSet(){ choiceIdx = 13, choices = new Choice[]{
                        new Choice(){ text="That’s me, just the backup huh?" },
                        new Choice(){ text="Ugh, fine, but I expect a good reward." },
                        new Choice(){ text="If I save your friends, will you stop yelling?" },
                    }},
                    new ChoiceSet(){ choiceIdx = 17, choices = new Choice[]{
                        new Choice(){ text="Time to make these scoundrels pay!" },
                        new Choice(){ text="Heh, should be a cake walk, lets go." },
                    }},
                }
            }
        },

        new Cutscene(){
            cutsceneId = 1,
            cutsceneType = CutsceneType.Start,
            firstEncounter = false,
            campaignLevel = 1,
            dialogue = new Dialogue(){
                npcId = 0,
                dialogueText = new string[]{
                    "Hey, hey you! Huh? You're already back?",
                    "You DIED? Then how are you here?",
                    "Actually, don't answer that, you're a wizard, it was probably magic or something, right?",
                    "Hmm... maybe I should've given you some guidance before I sent you off...",
                    "So heres a tip: If you run out of <color=#40E0D0>mana</color>, use your <color=purple>knowledge spell</color>!",
                    "It looks like a <color=#40E0D0>blue</color> question mark...",
                    "It’ll quiz you on your knowledge of the world—so pay attention!",
                    "Answer correctly, and you’ll recharge your mana in no time!",
                    "Simple, right?",
                    "Every great wizard starts out clueless, uh, I mean curious. You’ll master this in no time!",
                    "Oh, and one more thing: Enemies sometimes drop <color=yellow>shiny gold coins</color> when defeated...",
                    "Hang onto them! You never know when you’ll <color=purple>find something worth spending them on</color>!",
                    "Well... thats all from me. <color=green>Are you ready?</color>"
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx = 2, choices = new Choice[]{
                        new Choice(){ text="Yep, definitely magic." },
                        new Choice(){ text="I have no idea to be honest." },
                    }},
                    new ChoiceSet(){ choiceIdx = 8, choices = new Choice[]{
                        new Choice(){ text="Huh? A question? You mean I have to learn!?" },
                        new Choice(){ text="It's over for me..." },
                        new Choice(){ text="Seems easy enough!" },
                    }},
                    new ChoiceSet(){ choiceIdx = 12, choices = new Choice[]{
                        new Choice(){ text="This time, things WILL be different!" },
                        new Choice(){ text="Heh, I won't hold back this time..." },
                    }},
                }
            }
        },
        new Cutscene(){cutsceneId=2, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=1, dialogue=null},
         new Cutscene(){
            cutsceneId = 3,
            cutsceneType = CutsceneType.Saved,
            firstEncounter = false,
            campaignLevel = 1,
            dialogue = new Dialogue(){
                npcId = 2,
                dialogueText = new string[]{
                    "Thanks for saving me!",
                    "I know where more of my buddies are! Follow me!"
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){
                        choiceIdx = 1,
                        choices = new Choice[]{
                            new Choice(){text="Okay..?"},
                            new Choice(){text="Sure!"}
                        }
                    }
                }
            }
        },
         new Cutscene(){
            cutsceneId = 4,
            cutsceneType = CutsceneType.Start,
            firstEncounter = false,
            campaignLevel = 2,
            dialogue = new Dialogue(){
                npcId = 2,
                dialogueText = new string[]{
                    "Time for sand between your toes—and enemies in your face! Ready?",
                    "Beach vacation, monster-slaying… same thing, right?"
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){
                        choiceIdx = 1,
                        choices = new Choice[]{
                            new Choice(){text="Let's do it!"},
                            new Choice(){text="As long as there's treasure."},
                        }
                    }
                }
            }
        },
        new Cutscene(){cutsceneId=5, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=2, dialogue=null},
        new Cutscene(){
            cutsceneId = 6,
            cutsceneType = CutsceneType.Saved,
            firstEncounter = false,
            campaignLevel = 2,
            dialogue = new Dialogue(){
                npcId = 3,
                dialogueText = new string[]{
                    "You rescued me! I almost became monster crab salad!",
                    "Sounds delicious. Glad I ruined lunch."
                },
                choiceSets = new ChoiceSet[]{
                    new ChoiceSet(){
                        choiceIdx = 1,
                        choices = new Choice[]{
                            new Choice(){text="You're welcome."},
                            new Choice(){text="Stay safe now."}
                        }
                    }
                }
            }
        },
        new Cutscene(){
            cutsceneId=7, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=3,
            dialogue=new Dialogue(){npcId=3, dialogueText=new string[]{"Careful out here—the sands hide more than just treasure."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Got it. Stay sharp."},
                        new Choice(){text="Sand can't scare me."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=8, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=3, dialogue=null},
        new Cutscene(){
            cutsceneId=9, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=3,
            dialogue=new Dialogue(){npcId=4, dialogueText=new string[]{"Phew, almost dried out there! Thanks!","No worries, let's find you some shade."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stay hydrated!"},
                        new Choice(){text="You're safe now."}
                    }}
                }
            }
        },

        // LEVEL 4 - CAVE
        new Cutscene(){
            cutsceneId=10, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=4,
            dialogue=new Dialogue(){npcId=4, dialogueText=new string[]{"Watch your step—it's dark and creepy in there!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="I love spooky caves."},
                        new Choice(){text="I'll tread carefully."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=11, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=4, dialogue=null},
        new Cutscene(){
            cutsceneId=12, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=4,
            dialogue=new Dialogue(){npcId=5, dialogueText=new string[]{"Thanks! I was almost cave troll food!","Glad you're okay—let's get out."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stick with me."},
                        new Choice(){text="No more caves for you."}
                    }}
                }
            }
        },

        // LEVEL 5 - FROZEN
        new Cutscene(){
            cutsceneId=13, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=5,
            dialogue=new Dialogue(){npcId=5, dialogueText=new string[]{"Brrr... keep moving or you'll freeze solid!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Let's warm things up."},
                        new Choice(){text="Cold never bothered me anyway."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=14, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=5, dialogue=null},
        new Cutscene(){
            cutsceneId=15, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=5,
            dialogue=new Dialogue(){npcId=6, dialogueText=new string[]{"You thawed me out! You're my hero!","Just glad you're warm again."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stay warm!"},
                        new Choice(){text="Time to move on."}
                    }}
                }
            }
        },

        // LEVEL 6 - GRAVE
        new Cutscene(){
            cutsceneId=16, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=6,
            dialogue=new Dialogue(){npcId=6, dialogueText=new string[]{"This graveyard's spooky—ready your spells!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Bring on the ghosts."},
                        new Choice(){text="Let's proceed carefully."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=17, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=6, dialogue=null},
        new Cutscene(){
            cutsceneId=18, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=6,
            dialogue=new Dialogue(){npcId=7, dialogueText=new string[]{"Almost became ghost dinner—thanks!","You're safe now."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stay safe."},
                        new Choice(){text="Let's get going."}
                    }}
                }
            }
        },
        new Cutscene(){
            cutsceneId=19, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=7,
            dialogue=new Dialogue(){npcId=7, dialogueText=new string[]{"Watch your step! Lava is hot... obviously."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="I’ll be careful."},
                        new Choice(){text="Nothing like a little heat."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=20, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=7, dialogue=null},
        new Cutscene(){
            cutsceneId=21, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=7,
            dialogue=new Dialogue(){npcId=8, dialogueText=new string[]{"Close one! Nearly roasted!","Thanks for pulling me out!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stay cool!"},
                        new Choice(){text="Let’s move on."}
                    }}
                }
            }
        },

        // LEVEL 8 - SKY
        new Cutscene(){
            cutsceneId=22, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=8,
            dialogue=new Dialogue(){npcId=8, dialogueText=new string[]{"Up we go! Don’t look down!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Flying sounds fun!"},
                        new Choice(){text="I’ll just close my eyes..."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=23, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=8, dialogue=null},
        new Cutscene(){
            cutsceneId=24, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=8,
            dialogue=new Dialogue(){npcId=9, dialogueText=new string[]{"You caught me! Flying is tricky.","Glad you're safe—stay grounded."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stick closer to land."},
                        new Choice(){text="Onwards!"}
                    }}
                }
            }
        },

        // LEVEL 9 - ENCHANTED
        new Cutscene(){
            cutsceneId=25, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=9,
            dialogue=new Dialogue(){npcId=9, dialogueText=new string[]{"Everything here is magical—and dangerous!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Magic is my specialty."},
                        new Choice(){text="I'll stay alert."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=26, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=9, dialogue=null},
        new Cutscene(){
            cutsceneId=27, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=9,
            dialogue=new Dialogue(){npcId=10, dialogueText=new string[]{"You broke the curse—thank you!","All in a day's work!"},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Glad to help."},
                        new Choice(){text="Ready for what's next?"}
                    }}
                }
            }
        },

        // LEVEL 10 - CORRUPTION
        new Cutscene(){
            cutsceneId=28, cutsceneType=CutsceneType.Start, firstEncounter=false, campaignLevel=10,
            dialogue=new Dialogue(){npcId=10, dialogueText=new string[]{"This place is deeply corrupted—tread carefully."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Corruption ends here."},
                        new Choice(){text="I'll keep my guard up."}
                    }}
                }
            }
        },
        new Cutscene(){cutsceneId=29, cutsceneType=CutsceneType.Boss, firstEncounter=false, campaignLevel=10, dialogue=null},
        new Cutscene(){
            cutsceneId=30, cutsceneType=CutsceneType.Saved, firstEncounter=false, campaignLevel=10,
            dialogue=new Dialogue(){npcId=0, dialogueText=new string[]{"I'm free from corruption—thanks to you!","Welcome back to normal."},
                choiceSets=new ChoiceSet[]{
                    new ChoiceSet(){ choiceIdx=0, choices=new Choice[]{
                        new Choice(){text="Stay pure!"},
                        new Choice(){text="Let’s finish this."}
                    }}
                }
            }
        },
    };
}

[System.Serializable]
public class Cutscene
{
    public int cutsceneId;
    public CutsceneType cutsceneType;
    public bool firstEncounter;
    public int campaignLevel;
    public Dialogue dialogue;
}

[System.Serializable]
public class Dialogue
{
    public int npcId;
    public string[] dialogueText;

    public ChoiceSet[] choiceSets;

}

[System.Serializable]
public class ChoiceSet
{
    public int choiceIdx;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string text;
    public Sprite icon;
}