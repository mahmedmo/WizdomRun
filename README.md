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

# Project: WizdomRun

# 📁 Collection: User

undefined

## End-point: Create User

### Method: POST

> ```
> http://127.0.0.1:5000/auth/signup
> ```

### Body (**raw**)

```json
{
  "email": "email@email.com",
  "password": "password",
  "screenName": "magemaster"
}
```

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get User

### Method: GET

> ```
> http://127.0.0.1:5000/users/JoDqF0Ii36e9kqSJwRgNKCmTrTf1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete User

### Method: DELETE

> ```
> http://127.0.0.1:5000/users/JoDqF0Ii36e9kqSJwRgNKCmTrTf1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Update User

### Method: PUT

> ```
> http://127.0.0.1:5000/users/update/JoDqF0Ii36e9kqSJwRgNKCmTrTf1
> ```

### Body (**raw**)

```json
{
  "screenName": "MageMasterUpdated"
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get All Users

### Method: GET

> ```
> http://127.0.0.1:5000/users
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Character

undefined

## End-point: Create Character

### Method: POST

> ```
> http://127.0.0.1:5000/characters/create
> ```

### Body (**raw**)

```json
{
  "userID": "JoDqF0Ii36e9kqSJwRgNKCmTrTf1",
  "modelID": 2,
  "hairID": 3,
  "robeID": 1,
  "bootID": 2
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get Characters

### Method: GET

> ```
> http://127.0.0.1:5000/characters/JoDqF0Ii36e9kqSJwRgNKCmTrTf1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Update Character

### Method: PUT

> ```
> http://127.0.0.1:5000/characters/update/1
> ```

### Body (**raw**)

```json
{
  "hairID": 4,
  "robeID": 2
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete Character

### Method: DELETE

> ```
> http://127.0.0.1:5000/characters/delete/4
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Campaign

undefined

## End-point: Create Campaign

### Method: POST

> ```
> http://127.0.0.1:5000/campaigns/create
> ```

### Body (**raw**)

```json
{
  "userID": "JoDqF0Ii36e9kqSJwRgNKCmTrTf1",
  "title": "Battle for Knowledge",
  "campaignLength": "saga",
  "currLevel": 1
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get Campaigns

### Method: GET

> ```
> http://127.0.0.1:5000/campaigns/JoDqF0Ii36e9kqSJwRgNKCmTrTf1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Update Campaign

### Method: PUT

> ```
> http://127.0.0.1:5000/campaigns/update/1
> ```

### Body (**raw**)

```json
{
  "currLevel": 2,
  "remainingTries": 1
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete Campaign

### Method: DELETE

> ```
> http://127.0.0.1:5000/campaigns/delete/3
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get a Campaign

### Method: GET

> ```
> http://127.0.0.1:5000/campaigns/single/1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Restart Camaign

### Method: PATCH

> ```
> http://127.0.0.1:5000/campaigns/1/restart
> ```

### Body (**raw**)

```json
{
  "remainingTries": 2,
  "currLevel": 1
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Questions

undefined

## End-point: Get Questions

### Method: GET

> ```
> http://127.0.0.1:5000/questions/1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Answer Question

### Method: PUT

> ```
> http://127.0.0.1:5000/questions/answer/2
> ```

### Body (**raw**)

```json
{
  "gotCorrect": true
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Create Questions

### Method: POST

> ```
> http://127.0.0.1:5000/questions/batch_create
> ```

### Body (**raw**)

```json
[
  {
    "campaignID": 1,
    "difficulty": "medium",
    "questionStr": "What is a binary tree?",
    "answers": [
      {
        "answerStr": "A data structure",
        "isCorrect": true
      },
      {
        "answerStr": "A sorting algorithm",
        "isCorrect": false
      },
      {
        "answerStr": "A compiler",
        "isCorrect": false
      },
      {
        "answerStr": "A programming language",
        "isCorrect": false
      }
    ]
  },
  {
    "campaignID": 1,
    "difficulty": "hard",
    "questionStr": "What is the time complexity of quicksort on average?",
    "answers": [
      {
        "answerStr": "O(n log n)",
        "isCorrect": true
      },
      {
        "answerStr": "O(n^2)",
        "isCorrect": false
      },
      {
        "answerStr": "O(log n)",
        "isCorrect": false
      },
      {
        "answerStr": "O(n)",
        "isCorrect": false
      }
    ]
  }
]
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get a Question

### Method: GET

> ```
> http://127.0.0.1:5000/questions/question/2
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete Question

### Method: DELETE

> ```
> http://127.0.0.1:5000/questions/delete/2
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get Answers

### Method: GET

> ```
> http://127.0.0.1:5000/questions/answers/2
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Wrong Attempt

### Method: PUT

> ```
> http://127.0.0.1:5000/questions/wrong_attempt/2
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Player Stats

undefined

## End-point: Get Stats

### Method: GET

> ```
> http://127.0.0.1:5000/stats/1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Update Stats

### Method: PUT

> ```
> http://127.0.0.1:5000/stats//update/1
> ```

### Body (**raw**)

```json
{
  "attack": 5.0,
  "hp": 85,
  "mana": 60
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Create Stats

### Method: POST

> ```
> http://127.0.0.1:5000/stats/create
> ```

### Body (**raw**)

```json
{
  "campaignID": 1,
  "attack": 3.0,
  "hp": 100,
  "mana": 50,
  "affinity": "fire"
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Replenish Mana

### Method: PATCH

> ```
> http://127.0.0.1:5000/stats/replenish_mana/1
> ```

### Body (**raw**)

```json
{
  "manaAmount": 20
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Achievements

undefined

## End-point: Unlock Achievement

### Method: POST

> ```
> http://127.0.0.1:5000/achievements/unlock
> ```

### Body (**raw**)

```json
{
  "campaignID": 1,
  "title": "First Blood",
  "description": "Defeated the first enemy"
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get Achievments

### Method: GET

> ```
> http://127.0.0.1:5000/achievements/1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete Achivement

### Method: DELETE

> ```
> http://127.0.0.1:5000/achievements/4
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Player Spells

undefined

## End-point: Unlock Spell

### Method: POST

> ```
> http://127.0.0.1:5000/stats/assign_spell
> ```

### Body (**raw**)

```json
{
  "campaignID": 1,
  "spellID": 1
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Get Player Spells

### Method: GET

> ```
> http://127.0.0.1:5000/stats/player_spells/1
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Delete Player Spell

### Method: DELETE

> ```
> http://127.0.0.1:5000/stats/player_spells/4
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

# 📁 Collection: Spells

undefined

## End-point: Get Spells

### Method: GET

> ```
> http://127.0.0.1:5000/stats/spells
> ```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |

⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃ ⁃

## End-point: Create Spell

### Method: POST

> ```
> http://127.0.0.1:5000/stats/spells/create
> ```

### Body (**raw**)

```json
{
  "spellName": "Lightning Strike",
  "description": "A powerful lightning-based attack",
  "spellElement": "air"
}
```

### 🔑 Authentication bearer

| Param | value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Type   |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------ |
| token | eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB*N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT*-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q | string |
