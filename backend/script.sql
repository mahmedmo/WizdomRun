create table users (
    userID SERIAL PRIMARY KEY,
    screenName Varchar(31) UNIQUE NOT NULL,
    createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
create table player_character (
    characterID SERIAL PRIMARY KEY,
    userID INT NOT NULL,
    modelID INT NOT NULL,
    hairID INT,
    robeID INT,
    bootID INT,
    FOREIGN KEY (userID) REFERENCES users(userID) ON DELETE CASCADE
);
create TYPE length_type AS ENUM ('quest', 'odyssey', 'saga');
create table campaign (
    campaignID SERIAL PRIMARY KEY,
    lastUpdated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    userID INT NOT NULL,
    title varchar(127) UNIQUE NOT NULL,
    campaignLength length_type NOT NULL,
    currLevel INT NOT NULL,
    remainingTries INT NOT NULL DEFAULT 2,
    FOREIGN KEY (userID) REFERENCES users(userID) ON DELETE CASCADE
);
create TYPE question_difficulty AS ENUM ('easy', 'medium', 'hard');
create table questions (
    questionID SERIAL PRIMARY KEY,
    campaignID INT NOT NULL,
    difficulty question_difficulty NOT NULL,
    questionStr TEXT NOT NULL,
    gotCorrect BOOLEAN NOT NULL DEFAULT FALSE,
    wrongAttempts INT NOT NULL DEFAULT 0,
    FOREIGN KEY (campaignID) REFERENCES campaign(campaignID) ON DELETE CASCADE
);
create table answers (
    answerID SERIAL PRIMARY KEY,
    questionID INT NOT NULL,
    answerStr TEXT NOT NULL,
    isCorrect BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (questionID) REFERENCES questions(questionID) ON DELETE CASCADE
);
create TYPE playerClass AS ENUM ('fire', 'earth', 'water', 'air');
create table player_stats (
    campaignID INT PRIMARY KEY,
    attack NUMERIC(2, 1) NOT NULL CHECK (
        attack >= 1
        AND attack <= 5
    ),
    hp INT NOT NULL DEFAULT 100,
    mana int NOT NULL DEFAULT 0,
    affinity playerClass,
    FOREIGN KEY (campaignID) REFERENCES campaign(campaignID) ON DELETE CASCADE
);
create table spells (
    spellID SERIAL PRIMARY KEY,
    spellName varchar(31) NOT NULL,
    description TEXT,
    spellElement playerClass NOT NULL
);
create table player_spells (
    playerSpellID SERIAL PRIMARY KEY,
    playerID INT NOT NULL,
    spellID INT NOT NULL,
    FOREIGN KEY (playerID) REFERENCES player_stats(campaignID),
    FOREIGN KEY (spellID) REFERENCES spells(spellID)
);
create table achievements (
    achievementID SERIAL PRIMARY KEY,
    campaignID INT NOT NULL,
    title varchar(63) NOT NULL,
    description TEXT,
    FOREIGN KEY (campaignID) REFERENCES campaign(campaignID) ON DELETE CASCADE
);
CREATE OR REPLACE FUNCTION enforce_one_correct_answer() RETURNS TRIGGER AS $$ BEGIN IF NEW."isCorrect" = TRUE THEN IF EXISTS (
        SELECT 1
        FROM answers
        WHERE "questionID" = NEW."questionID"
            AND "isCorrect" = TRUE
            AND "answerID" <> NEW."answerID"
    ) THEN RAISE EXCEPTION 'A question can only have one correct answer.';
END IF;
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER check_correct_answer BEFORE
INSERT
    OR
UPDATE ON answers FOR EACH ROW EXECUTE FUNCTION enforce_one_correct_answer();
CREATE OR REPLACE FUNCTION enforce_answer_count() RETURNS TRIGGER AS $$
DECLARE answer_count INT;
BEGIN -- Count existing answers for the question
SELECT COUNT(*) INTO answer_count
FROM answers
WHERE "questionID" = NEW."questionID";
-- Ensure the count is either 0, 2, or 4
IF NOT (
    answer_count = 0
    OR answer_count = 2
    OR answer_count = 4
) THEN RAISE EXCEPTION 'Each question must have exactly 2 or 4 answers.';
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER check_answer_count
AFTER
INSERT
    OR DELETE ON answers FOR EACH ROW EXECUTE FUNCTION enforce_answer_count();