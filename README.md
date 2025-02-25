# 👾 Wizdom Run 👾

## Table of Contents

- [Project Summary](#project-summary)
- [Project Overview](#project-overview)
  - [👤 Splash Screen & Authentication (MH)](#-splash-screen--authentication-mh)
  - [🎨 Character Creation & Tutorial (MH)](#-character-creation--tutorial-mh)
  - [✏️ Campaign Setup & Notes Import](#-campaign-setup--notes-import)
  - [🎭 Campaign Structure](#-campaign-structure)
  - [⌛️ Gameplay Mechanics](#-gameplay-mechanics)
  - [🗡️ Boss Battles: Turn-Based Combat](#-boss-battles-turn-based-combat)
  - [🏆 Winning, Losing, and Achievements](#-winning-losing-and-achievements)
- [Requirements](#requirements)
- [Design](#design)
- [Testing](#testing)
- [Tech Stack](#tech-stack)
- [Team Roles](#team-roles-seng-401---group-17)

---

## Project Summary

Learning complex terminology often feels like a tedious chore, draining motivation and focus. Our solution transforms this challenge into an engaging adventure—empowering learners to master new concepts through an immersive game-based experience. Study sessions become captivating quests for growth!

---

## Project Overview

### 👤 Splash Screen & Authentication (MH)

- **Splash Screen:** On launch, users are greeted with a dynamic screen displaying the game’s title and environment.
- **Authentication:** Users can log in or sign up.

### 🎨 Character Creation & Tutorial (MH)

- **Character Creation:** After account creation, users choose and customize their wizard (CH).
- **Tutorial:** Explains game mechanics:
  - How to import notes for study.
  - How to navigate and play the game.

### ✏️ Campaign Setup & Notes Import

- **Campaign Options:** Start a new campaign or continue an existing one.
- **Notes Import (CH):** Users import study notes.
- **LLM Processing:** The LLM extracts key terminology/concepts and generates three question sets: **easy**, **medium**, and **hard** (CH).

### 🎭 Campaign Structure

- **Campaign Length:** Users choose from:
  - **Quest (Short)**
  - **Odyssey (Medium)**
  - **Saga (Long)**
- **Difficulty Progression:** Questions progress from easy (early levels) to hard (later levels).

### ⌛️ Gameplay Mechanics

- **Auto-Run:** The player character runs automatically through diverse environments.
- **Combat:** Enemies appear; players cast spells to defeat them.
- **Mana System:**
  - **Spell Cost:** Casting spells consumes mana.
  - **Mana Replenishment:** Tapping a button prompts a question:
    - **Correct Answer:** Replenishes mana.
    - **Incorrect Answer:** No mana gain, limiting spell usage.
- **Special Events:** Correct responses during events grant additional spells/abilities. _(SH)_

### 🗡️ Boss Battles: Turn-Based Combat

- **Boss Encounter:** At the end of each level, a boss battle begins.
- **Turn-Based Mechanics:**
  - Players receive a hand of cards with unique buffs or bonuses.
  - Playing a card triggers a question:
    - **Correct Answer:** Grants bonuses or extra spells.
    - **Incorrect Answer:** Causes the player to skip a turn.
- **Progression:** Defeating the boss advances the player, carrying over any gained abilities.

### 🏆 Winning, Losing, and Achievements

- **Failure Limit:** Three losses in a level result in campaign failure and restart.
- **Achievements:** Successful campaign completions are celebrated and saved on the **Achievements Page** (CH) in the main menu.

---

## Requirements

_Details to be determined._

---

## Design

_Details to be determined._

---

## Testing

_Details to be determined._

---

## Tech Stack

### 📺 Frontend (3x Team Members)

- **React Native + Unity Integration (MH)**
  - **Deployment:** Cross-platform (iOS, Android, Web).
  - **Division of Labor:** Unity manages core gameplay; React Native handles UI (splash, authentication, note imports).
- **Splash Screen & Auth (MH):** Powered by Firebase.
- **Local Note Import (MH):** Uses file system access for offline uploads, minimizing database load and ensuring privacy.

### ⚙️ LLM Integration (2x Team Members)

- **LLM Communication (MH):** Integrates Deepseek with two specialized models:
  - **deepseek-reasoner:** For initial note analysis and knowledge extraction.
  - **deepseek-chat:** Processes the output to generate final Q&A formats.
- **Storage:** Engineered responses are stored in the database.

### 🗄️ Backend (1x Team Member)

- **Python Framework (MH):** API endpoints built with Flask.
- **Database (MH):** PostgreSQL hosted on cloud providers (e.g., Neon, Tembo, AWS) to store player progress, generated questions, and achievements.

---

## Team Roles (SENG 401 - Group 17)

- **Muhammad Ahmed:** Project Manager, Frontend/Unity Engineer.
- **Matthew Roxas:** Frontend/Unity Engineer.
- **Wilson Zheng:** Frontend/Unity Engineer.
- **Sukriti Badhwar:** AI Engineer.
- **Sahib Thethi:** AI Engineer.
- **Charlie Lang:** Backend Engineer.

## Endpoints

This API allows users to interact with the game-based learning system. It supports user management, campaign tracking, question answering, player stats, spells, and achievements.

---

## **User Endpoints**

### **POST /users/create**

Create a new user.

**Sample Request Body:**

```json
{
  "screenName": "MageMaster"
}
```

**Sample Response Status Code:**

```
Created 201
```

**Sample Response Body:**

```json
{
  "userID": 1,
  "screenName": "MageMaster",
  "createdAt": "2025-02-20T12:00:00Z"
}
```

### **GET /users/{userID}**

Retrieve user details by user ID.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "userID": 1,
  "screenName": "MageMaster",
  "createdAt": "2025-02-20T12:00:00Z"
}
```

### **DELETE /users/{userID}**

Delete a user by user ID.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "User deleted successfully"
}
```

---

## **Character Endpoints**

### **POST /characters/create**

Create a new character for a user.

**Sample Request Body:**

```json
{
  "userID": 1,
  "modelID": 2,
  "hairID": 3,
  "robeID": 1,
  "bootID": 2
}
```

**Sample Response Status Code:**

```
Created 201
```

**Sample Response Body:**

```json
{
  "characterID": 1,
  "userID": 1,
  "modelID": 2,
  "hairID": 3,
  "robeID": 1,
  "bootID": 2
}
```

### **GET /characters/{userID}**

Retrieve a user's character details.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "characterID": 1,
  "userID": 1,
  "modelID": 2,
  "hairID": 3,
  "robeID": 1,
  "bootID": 2
}
```

### **PUT /characters/update/{characterID}**

Update a character's appearance.

**Sample Request Body:**

```json
{
  "hairID": 4,
  "robeID": 2
}
```

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "Character updated successfully"
}
```

---

## **Campaign Endpoints**

### **POST /campaigns/create**

Create a new campaign.

**Sample Request Body:**

```json
{
  "userID": 1,
  "title": "Battle for Knowledge",
  "campaignLength": "saga",
  "currLevel": 1
}
```

**Sample Response Status Code:**

```
Created 201
```

**Sample Response Body:**

```json
{
  "campaignID": 10,
  "userID": 1,
  "title": "Battle for Knowledge",
  "campaignLength": "saga",
  "currLevel": 1,
  "remainingTries": 2
}
```

### **GET /campaigns/{userID}**

Retrieve all campaigns for a user.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
[
  {
    "campaignID": 10,
    "title": "Battle for Knowledge",
    "campaignLength": "saga",
    "currLevel": 1,
    "remainingTries": 2
  }
]
```

### **PUT /campaigns/update/{campaignID}**

Update campaign progress.

**Sample Request Body:**

```json
{
  "currLevel": 2,
  "remainingTries": 1
}
```

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "Campaign updated successfully"
}
```

### **DELETE /campaigns/delete/{campaignID}**

Delete a campaign.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "Campaign deleted successfully"
}
```

## **Question Endpoints**

### **GET /questions/{campaignID}**

Retrieve all questions for a campaign.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
[
  {
    "questionID": 5,
    "difficulty": "medium",
    "question": "What is a binary tree?",
    "options": ["A data structure", "A sorting algorithm", "A compiler"],
    "gotCorrect": false,
    "wrongAttempts": 1
  }
]
```

### **PUT /questions/answer/{questionID}**

Submit an answer to a question.

**Sample Request Body:**

```json
{
  "isCorrect": true
}
```

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "Answer recorded successfully"
}
```

## **Player Stats Endpoints**

### **GET /stats/{campaignID}**

Retrieve player stats for a campaign.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "campaignID": 10,
  "attack": 4.5,
  "hp": 90,
  "mana": 50,
  "affinity": "fire"
}
```

### **PUT /stats/update/{campaignID}**

Update player stats.

**Sample Request Body:**

```json
{
  "attack": 5.0,
  "hp": 85,
  "mana": 60
}
```

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
{
  "message": "Player stats updated successfully"
}
```

### **GET /stats/player_spells/{campaignID}**

Retrieve all spells assigned to a player.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
[
  {
    "playerSpellID": 1,
    "spellID": 3,
    "spellName": "Fireball",
    "spellElement": "fire"
  }
]
```

## **Player Spells Endpoints**

### **POST /stats/assign_spell**

Assign a spell to a player in a specific campaign.

**Request Body:**

```json
{
  "campaignID": 1,
  "spellID": 3
}
```

**Sample Response Status Code:**

```
Created 201
```

**Sample Response Body:**

```json
{
  "message": "Spell assigned successfully"
}
```

---

### **GET /stats/player_spells/{campaignID}**

Retrieve all spells assigned to a player.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
[
  {
    "playerSpellID": 1,
    "spellID": 3,
    "spellName": "Fireball",
    "spellElement": "fire"
  }
]
```

---

## **Achievements Endpoints**

### **POST /achievements/unlock**

Unlock a new achievement for a player in a campaign.

**Request Body:**

```json
{
  "campaignID": 1,
  "title": "First Blood",
  "description": "Defeated the first enemy"
}
```

**Sample Response Status Code:**

```
Created 201
```

**Sample Response Body:**

```json
{
  "message": "Achievement unlocked"
}
```

---

### **GET /achievements/{campaignID}**

Retrieve all achievements for a specific campaign.

**Sample Response Status Code:**

```
Ok 200
```

**Sample Response Body:**

```json
[
  {
    "achievementID": 1,
    "title": "First Blood",
    "description": "Defeated the first enemy"
  }
]
```
