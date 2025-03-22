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
- [Requirements, Design, Testing](#requirements-design-testing)
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

- **Character Creation:** After account creation, users choose and customize their wizard through different spell classes (CH).
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

### Requirements, Design, Testing

All these documents are located in the documentation folder in the root directory. 

---

## Tech Stack

### 📺 Frontend (3x Team Members)

- **React Native + Unity Integration (MH)**
  - **Deployment:** Cross-platform (iOS, Android, Web).
  - **Division of Labor:** Unity manages core gameplay; React Native handles UI (splash, authentication, note imports).
- **Splash Screen & Auth (MH):** Powered by Firebase.
- **Local Note Import (MH):** Uses file system access for offline uploads, minimizing database load and ensuring privacy.

### ⚙️ LLM Integration (2x Team Members)

- **LLM Communication (MH):** Integrates OpenAI's GPT-4o-mini to generate multiple-choice questions (MCQs) from research documents.
  - **Document Loading:** Uses PyPDFLoader to extract content from PDFs.
  - **Question Generation:** GPT-4o-mini analyzes the extracted content and generates diverse MCQs with randomized answer placements.
  - **Difficulty Scaling:** Questions are generated in Easy, Medium, and Hard difficulty levels to ensure comprehensive coverage.
- **Storage:** The generated questions and answers are structured into a JSON format for database storage, maintaining campaign-specific IDs.

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
